using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    [RequireComponent(typeof(Rigidbody))]
    public class ChargedParticle : MonoBehaviour {

        public float MaxAcceleration = 200f;
        public float MaxInfluence = 5f;
        public float PreferedDistance = 6f;
        public float forceModifier = 100f;
        public List<ChargedParticle> LinkedParticles;

        private Rigidbody rb;
        private float sqrMaxAcceleration;

        public ChargedParticle() {
            LinkedParticles = new List<ChargedParticle>();
            sqrMaxAcceleration = MaxAcceleration * MaxAcceleration;
        }

        void Start() {
            rb = GetComponent<Rigidbody>();

            // make sure that each charge particle relationship is mutual
            foreach (ChargedParticle cp in LinkedParticles) {
                if (!cp.LinkedParticles.Contains(this))
                    cp.LinkedParticles.Add(this);
            }

        }

        public void LinkChargedParticle(ChargedParticle cp) {
            if (!LinkedParticles.Contains(cp))
                LinkedParticles.Add(cp);
            if (!cp.LinkedParticles.Contains(this))
                cp.LinkedParticles.Add(this);
        }

        public void UnlinkChargedParticle(ChargedParticle cp) {
            if (LinkedParticles.Contains(cp))
                LinkedParticles.Remove(cp);
            if (cp != null && cp.LinkedParticles.Contains(this))
                cp.LinkedParticles.Remove(this);
        }

        void Update() {

            Vector3 resultantForce = Vector3.zero;

            foreach (ChargedParticle cp in LinkedParticles) {
                resultantForce += CalculateForceCausedByParticle(cp);
            }

            foreach (Collider c in Physics.OverlapSphere(transform.position, MaxInfluence)) {
                ChargedParticle cp = c.gameObject.GetComponent<ChargedParticle>();
                if (cp != null) {
                    resultantForce += CalculateForceCausedByParticle(cp);
                }
            }

            if (resultantForce.sqrMagnitude > sqrMaxAcceleration) {
                resultantForce = resultantForce.normalized * MaxAcceleration;
            }

            rb.AddForce(resultantForce);
            
        }
        
        private Vector3 CalculateForceCausedByParticle(ChargedParticle cp) {
            Vector3 thisToOther = cp.transform.position - transform.position;
            float thisToOtherDistance = thisToOther.magnitude;
            float differenceFromPreferredDistance = thisToOtherDistance - PreferedDistance;
            Vector3 thisToOtherNormalised = thisToOther.normalized;
            return forceModifier * differenceFromPreferredDistance * cp.GetComponent<Rigidbody>().mass * thisToOtherNormalised;
        }

    }

}
