//#define SIGNAL_PROGRESS
//#define THREADS
//#define EFFECTS

//#define TIMERS

using System.Diagnostics;

namespace Spock {
     
	public class SelectiveDebug {

		[Conditional("TIMERS")] 
		public static void StartTimer(string id) {
			Timer.StartTimer(id);
		} 
		 
		[Conditional("TIMERS")] 
		public static void StopTimer(string id) { 
			Timer.StopTimer(id, true);
		}
        
        [Conditional("SIGNAL_PROGRESS")] 
		public static void LogSignalProgress(string message) { 
			UnityEngine.Debug.Log("SIGNAL_PROGRESS: " + message);
		}

        [Conditional("THREADS")]
        public static void LogThread(string message) {
            UnityEngine.Debug.Log("THREADS: " + message);
        }

        [Conditional("EFFECTS")]
        public static void LogEffect(string message) {
            UnityEngine.Debug.Log("EFFECTS: " + message);
        }

    }

}