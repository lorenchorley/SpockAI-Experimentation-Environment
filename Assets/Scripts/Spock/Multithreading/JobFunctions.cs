//#define USE_PAUSES

using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Spock {

    public class JobFunctions {

        [Conditional("USE_PAUSES")]
        public static void WaitThreadAMoment() {
            try {
                Thread.Sleep(500);
            } catch (ThreadInterruptedException) { /* Continue if interrupted */ }
        }

        public static void ThreadHandler(object o) {
            ThreadController.AssertNotMainThread();
            ThreadController.RaiseEventThreadStarted();

            try {

                Assert.True("NewThreadHandler parameters is an array", o is object[]);
                object[] O = o as object[];
                Assert.True("NewThreadHandler parameters are valid", O.Length > 1 && O[0] is WaitCallback);

                (O[0] as WaitCallback).Invoke(O[1]);

            } catch (Exception e) {
                UnityEngine.Debug.LogError(e.Message + " (ProcessEverythingOnNode thread)\n" + e.StackTrace);
            } finally {
                ThreadController.RaiseEventThreadFinished();
            }
        }
        
        // ProcessEverythingOnObject(Node)
        public static void ProcessEverythingOnNode(object o) {
            ThreadController.AssertNotMainThread();
            SelectiveDebug.LogThread("ProcessEverythingOnNode");

            Node node;
            CheckEverythingOnNodeArguments(o, out node);
            
            while (node.OP.IterationRequired()) {
                WaitThreadAMoment();
                node.OP.OutputIteration(OutputProcess.START_NEW_THREAD);
            }
                
        }
        
        // ProcessAcceptSignal(ISignalSending, Signal, ISignalAccepting[, delayInMilliseconds])
        // ISignalSending [sends] Signal [to] ISignalAccepting
        public static void ProcessAcceptSignal(object o) {
            ThreadController.AssertNotMainThread();
            SelectiveDebug.LogThread("ProcessAcceptSignal");

            ISignalSending from;
            Signal signal;
            ISignalAccepting to;
            int? delayInMilliseconds;
            CheckAcceptSignalArguments(o, out from, out signal, out to, out delayInMilliseconds);

            if (delayInMilliseconds.HasValue) {
                try {
                    Thread.Sleep(delayInMilliseconds.Value); // TODO Would prefer that this does not block here and that unity handles the delayed callback
                } catch (ThreadInterruptedException) { /* Continue if interrupted */ }
            }
                    
            to.AcceptSignal(signal, from);
                    
        }
        
        // ProcessDuplication(Node/Connection/Signal)
        public static void ProcessDuplication(object o) {
            ThreadController.AssertNotMainThread();
            SelectiveDebug.LogThread("ProcessDuplication");

            Node node;
            Connection connection;
            Signal signal;
            Action callback;
            CheckDuplicationArguments(o, out node, out connection, out signal, out callback);

            if (node != null) {

                // Duplicate the components of the node and place the new components into the new node
                // Currently node is a new shell of a node that contains all the components of the previous node
                node.ForEachComponentType((type, component) => node.Set(type, component.Duplicate(type, node)));
                
                // Enqueue effects for new connections 
                node.SP.ForEachInput(
                    input => node.N.S.Get<EffectController>().EnqueueEffect_TS(node.N.S.EffectFactory.NewNodeInputEffect(node, input))
                    );
                node.SP.ForEachOutput(
                    output => node.N.S.Get<EffectController>().EnqueueEffect_TS(node.N.S.EffectFactory.NewNodeOutputEffect(node, output))
                    );

                // Invoke completion callback if it was given
                if (callback != null)
                    callback.Invoke();

                // Start an output iteration in this thread in case there is need
                ProcessEverythingOnNode(node);
                    
            } else if (connection != null) {
                throw new NotImplementedException();
            } else if (signal != null) {

                // Duplicate the components of the signal and place the new components into the new signal
                // Currently signal is a new shell of a signal that contains all the components of the previous signal
                signal.CR = signal.CR.Duplicate<ComponentRegistration>(signal);
                signal.SC = signal.SC.Duplicate<SignalContent>(signal);

                // Start a new signal effect
                signal.N.S.Get<EffectController>().EnqueueEffect_TS(signal.N.S.EffectFactory.NewSignalEffect(signal));

                // Invoke completion callback if it was given
                if (callback != null)
                    callback.Invoke();

            }

        }

        // ProcessWakeAndInvoke()
        public static void ProcessWakeAndInvoke(object o) {
            ThreadController.AssertNotMainThread();
            SelectiveDebug.LogThread("ProcessWakeAndInvoke");

            Action callback;
            Func<int> GetNextSleepDuration;
            CheckWakeAndInvokeArguments(o, out callback, out GetNextSleepDuration);

            int sleepDuration = GetNextSleepDuration();
            while (sleepDuration >= 0) {

                // Sleep the thread for the given period of time
                try {
                    Thread.Sleep(sleepDuration);
                } catch (ThreadInterruptedException) { /* Continue if interrupted */ }

                // Exit if the application is closing
                if (ThreadController.I.stopThreads)
                    return;

                // Invoke the callback and get the next duration
                callback.Invoke();
                sleepDuration = GetNextSleepDuration();

            }
                    
        }


        private static void CheckEverythingOnNodeArguments(object o, out Node n) {

            if (!(o is Node))
                throw new ArgumentException();

            n = o as Node;

        }

        private static void CheckAcceptSignalArguments(object o, out ISignalSending from, out Signal signal, out ISignalAccepting to, out int? d) {
            
            if (!(o is object[]))
                throw new ArgumentException();

            object[] O = o as object[];

            if (!(O.Length > 2 && (O[0] is ISignalSending || O[0] == null) && O[1] is Signal && O[2] is ISignalAccepting))
                throw new ArgumentException();

            from = O[0] as ISignalSending;
            signal = O[1] as Signal;
            to = O[2] as ISignalAccepting;

            if (O.Length > 3 && O[3] is int)
                d = (int) O[3];
            else
                d = null;

        }

        private static void CheckDuplicationArguments(object o, out Node n, out Connection c, out Signal s, out Action a) {

            n = null;
            c = null;
            s = null;

            if (!(o is object[]))
                throw new ArgumentException();

            object[] O = o as object[];

            if (!(O.Length > 0 && O[0] is INetworkComponent))
                throw new ArgumentException();

            if (O[0] is Node)
                n = O[0] as Node;
            else if (O[0] is Connection)
                c = O[0] as Connection;
            else if (O[0] is Signal)
                s = O[0] as Signal;

            if (O.Length > 1 && O[1] is Action)
                a = O[1] as Action;
            else
                a = null;

        }

        private static void CheckWakeAndInvokeArguments(object o, out Action a, out Func<int> f) {

            if (!(o is object[]))
                throw new ArgumentException();
            
            object[] O = o as object[];

            if (!(O.Length > 1 && O[0] is Action && O[1] is Func<int>))
                throw new ArgumentException();

            a = O[0] as Action;
            f = O[1] as Func<int>;
            
        }

    }

}
