/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.LevelEditing;

namespace Platformer.LevelEditing {

    [RequireComponent(typeof(EdgeCollider2D))]
    public class BridgeRope : Rope {

        [SerializeField] public Transform endpoint;

        public void Set(float length, Vector3 startpoint, Vector3 endpoint) {
            this.ropeLength = length;
            this.startpoint.position = startpoint;
            this.endpoint.position = endpoint;
        }

        // Runs once on initialization.
        protected override void OnAwake() {
            // Cache these references.
            m_EdgeCollider.edgeRadius = ropeWidth / 2f;
            m_EdgeCollider.isTrigger = true;
        }

        protected override void Constraints() {
            ropeSegments[0] = Vector2.zero;
            ropeSegments[segmentCount - 1] = endpoint.position - startpoint.position;

            for (int i = 1; i < segmentCount; i++) {
                // Get the distance and direction between the segments.
                float newDist = (ropeSegments[i - 1] - ropeSegments[i]).magnitude;
                Vector3 direction = (ropeSegments[i - 1] - ropeSegments[i]).normalized;

                // Get the error term.
                float error = newDist - segmentLength;
                Vector3 errorVector = direction * error;

                // Adjust the segments by the error term.
                if (i != 1) {
                    ropeSegments[i - 1] -= errorVector * 0.5f;
                }
                ropeSegments[i] += errorVector * 0.5f;
            }
        }
    
    }

}
