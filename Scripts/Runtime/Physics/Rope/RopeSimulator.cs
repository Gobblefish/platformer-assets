/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    [RequireComponent(typeof(LineRenderer))]
    public class RopeSimulator : MonoBehaviour {

        protected LineRenderer lineRenderer;
        // protected EdgeCollider2D edgeCollider;

        public RopeConstraint ropeConstraint;

        public Rigidbody2D trackBody;
        public float bodyFactor;

        [SerializeField] protected RopeCollider ropeCollider;

        /* --- Variables --- */
        [SerializeField] private int segmentCount; // The number of segments.

        [SerializeField] private Vector3[] ropeSegments; // The current positions of the segments.
        [SerializeField] private Vector3[] prevRopeSegments; // The previous positions of the segments.
        [SerializeField] private Vector3[] velocities; // The previous positions of the segments.

        /* --- Unity --- */

        // Runs once on initialization.
        void Awake() {
            // Cache these references.
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            ropeConstraint.Initialize(lineRenderer);
            ropeCollider = GetComponent<RopeCollider>();
            // Initialize.
            InitalizeRopeSegments();
        }

        // Runs once every set time interval.
        void FixedUpdate() {
            Vector3 bodyVel = trackBody != null ? trackBody.velocity * bodyFactor : Vector3.zero;
            ropeConstraint.Simulate(ropeSegments, prevRopeSegments, velocities, bodyVel, segmentCount, Time.fixedDeltaTime);
            ropeConstraint.Constrain(transform.position, ropeSegments, segmentCount);

            if (ropeCollider != null) {
                ropeCollider.ProcessCollision(ropeSegments, segmentCount, Time.fixedDeltaTime);
            }

            lineRenderer.positionCount = segmentCount;
            lineRenderer.SetPositions(ropeSegments);

        }

        // Runs if this trigger is activated.
        void OnTriggerStay2D(Collider2D collider) {
            if (ropeCollider == null) { return; }
            Rigidbody2D body = collider.GetComponent<Rigidbody2D>();
            if (body != null) {
                ropeCollider.NoticeCollision(body, ropeSegments, segmentCount);
            }
        }

        // Initalizes the rope segments.
        void InitalizeRopeSegments() {
            // Get the number of segments for a rope of this length.
            segmentCount = (int)Mathf.Ceil(ropeConstraint.ropeLength / ropeConstraint.segmentLength);

            // Initialize the rope segments.
            ropeSegments = new Vector3[segmentCount];
            prevRopeSegments = new Vector3[segmentCount];
            velocities = new Vector3[segmentCount];
            
            ropeSegments[0] = transform.position; // Vector3.zero;
            prevRopeSegments[0] = ropeSegments[0];
            velocities[0] = Vector2.zero;

            for (int i = 1; i < segmentCount; i++) {
                Vector2 offset = ropeConstraint.segmentLength * Random.insideUnitCircle.normalized;
                offset.y = -Mathf.Abs(offset.y);
                ropeSegments[i] = ropeSegments[i - 1] + (Vector3)offset;
                prevRopeSegments[i] = ropeSegments[i];
                velocities[i] = new Vector2(0f, 0f);
            }

            if (ropeCollider != null) {
                ropeCollider.Initialize(segmentCount);
            }
        }

        public void SetWeight(Vector3 gravity) {
            ropeConstraint.gravity = gravity;
        }

    }

}