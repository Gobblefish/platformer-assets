/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

namespace Platformer.Physics {

    /// <summary>
    ///
    /// <summary>
    [RequireComponent(typeof(EdgeCollider2D))]
    public class RopeCollider : MonoBehaviour {

        protected EdgeCollider2D edgeCollider;

        [SerializeField] protected Vector3[] jiggle;

        [SerializeField] private float jiggleDamp;
        [SerializeField] private float edgeWidth;

        public void Initialize(int segmentCount) {
            edgeCollider = GetComponent<EdgeCollider2D>();
            edgeCollider.isTrigger = true;
            edgeCollider.edgeRadius = edgeWidth;

            jiggle = new Vector3[segmentCount];
        }

        // Adds a jiggle whenever a body collides with this.
        public void NoticeCollision(Rigidbody2D body, Vector3[] ropeSegments, int segmentCount) {
            // Get the segment closest to the collider.
            Vector3 pos = (Vector3)body.position;
            int index = 1; // because the first position can't jiggle?
            float minDist = 1e9f;
            for (int i = 1; i < segmentCount; i++) {
                // Vector3 segPos = transform.position + ropeSegments[i];
                float dist = (pos - ropeSegments[i]).magnitude;
                if (dist < minDist) {
                    index = i;
                    minDist = dist;
                }
            }

            // Add a jiggle to this segment.
            if (index > 1 && index < segmentCount) {
                jiggle[index] = new Vector2(body.velocity.x, 0f);
            }

        }

        public void ProcessCollision(Vector3[] ropeSegments, int segmentCount, float dt) {
            for (int i = 0; i < segmentCount; i++) {
                ropeSegments[i] += jiggle[i] * dt;
                jiggle[i] *= jiggleDamp;
            }

            Vector2[] points = new Vector2[segmentCount];
            for (int i = 0; i < segmentCount; i++) {
                points[i] = (Vector2)(ropeSegments[i] - ropeSegments[0]);
            }
            edgeCollider.points = points;
            edgeCollider.edgeRadius = edgeWidth;
        }

    }

}
