/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.LevelEditing;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    public class HangingRope : Rope {

        [SerializeField, Range(0f, 100f)] protected float m_RopeLengthVariationPercent = 30f;

        protected override void OnAwake() {
            m_EdgeCollider.isTrigger = true;
        }

        protected override void Constraints() {
            ropeSegments[0] = Vector3.zero;
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

                newDist = (ropeSegments[i - 1] - ropeSegments[i]).magnitude;
                if (newDist < segmentLength / 4f) {
                    ropeSegments[i] = ropeSegments[i-1] + (ropeSegments[i] - ropeSegments[i-1]).normalized * segmentLength / 4f;
                }

            }
        }

    }
}
