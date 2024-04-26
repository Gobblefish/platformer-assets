/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    [CreateAssetMenu(fileName="HangingRope", menuName="Ropes/Hanging")]
    public class HangingRopeConstraint : RopeConstraint {

        [SerializeField] public float positionDamping = 0.975f;
        [SerializeField] public float velocityDamping = 0.65f;

        [SerializeField] public Vector2 ropeWidth;

        public override void Initialize(LineRenderer lineRenderer) {
            // edgeCollider.isTrigger = true;
            lineRenderer.startWidth = ropeWidth.x;
            lineRenderer.endWidth = ropeWidth.y;
        }

        public override void Simulate(Vector3[] ropeSegments, Vector3[] prevRopeSegments, Vector3[] velocities, Vector3 bodyVelocity, int segmentCount, float dt) {
            for (int i = 0; i < segmentCount; i++) {
                Vector3 velocity = ropeSegments[i] - prevRopeSegments[i];
                prevRopeSegments[i] = ropeSegments[i];
                ropeSegments[i] += velocity * positionDamping;
                ropeSegments[i] += (gravity - bodyVelocity) * dt;
                ropeSegments[i] += velocities[i] * dt;
            }

            for (int i = 0; i < velocities.Length; i++) {
                velocities[i] *= velocityDamping;
            }

        }

        public override void Constrain(Vector3 origin, Vector3[] ropeSegments, int segmentCount) {
            for (int n = 0; n < constraintDepth; n++) {
                ropeSegments[0] = origin;
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
}