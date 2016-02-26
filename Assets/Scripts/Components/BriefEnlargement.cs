using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class BriefEnlargement : MonoBehaviour {

        public static void ApplyBriefEnlargement(GameObject obj, float factor, Action callback = null, long duration = 500) {
            BriefEnlargement be = obj.GetComponent<BriefEnlargement>();
            if (be == null) 
                be = obj.AddComponent<BriefEnlargement>();

            //if (obj.GetComponent<ConnectionRepresentation>() != null)
            if (obj.GetComponent<LineRenderer>() != null)
                be.Set(0.1f, 0.3f);
            else
                be.Set(0.1f);

            be.callback = callback;

        }

        public Action callback;

        private bool isLineRenderer;
        private bool set = false;
        private bool started = false;
        private float frames = 50;

        private Vector3 originalScale;
        private Vector3 targetScale;

        //private List<LineRenderer> lines;
        private LineRenderer line;
        private float originalThickness;
        private float targetThickness;

        public void Set(float originalThickness, float targetThickness) {
            this.originalThickness = originalThickness;
            this.targetThickness = targetThickness;
            //lines = new List<LineRenderer>(GetComponentsInChildren<LineRenderer>());
            line = GetComponent<LineRenderer>();
            isLineRenderer = true;
            set = true;

            if (!started)
                StartCoroutine("RestoreOriginal");

            started = true;

        }

        public void Set(float factor) {
            if (!started) {
                this.originalScale = transform.localScale;
                this.targetScale = transform.localScale * (1f + factor);
            }
            isLineRenderer = false;
            set = true;

            if (!started)
                StartCoroutine("RestoreOriginal");

            started = true;

        }

        IEnumerator RestoreOriginal() {
            set = false;
            float i;

            for (i = 1; i <= frames; i++) {
                if (isLineRenderer) {
                    float width = Mathf.Lerp(targetThickness, originalThickness, i / frames);
                    //foreach (LineRenderer line in lines) {
                    //    line.SetWidth(width, width);
                    //}
                    line.SetWidth(width, width);
                } else {
                    transform.localScale = Vector3.Lerp(targetScale, originalScale, i / frames);
                }
                yield return null;

                if (set) {
                    i = 1;
                    set = false;
                }
            } 

            if (i >= frames) {
                if (isLineRenderer) {
                    //foreach (LineRenderer line in lines) {
                    //    line.SetWidth(originalThickness, originalThickness);
                    //}
                    line.SetWidth(originalThickness, originalThickness);
                } else {
                    transform.localScale = originalScale;
                }

                if (callback != null)
                    callback.Invoke();

                Destroy(this);
            }

        }

    }

}
