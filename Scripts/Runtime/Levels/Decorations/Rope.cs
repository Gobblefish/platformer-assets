/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

namespace Platformer.LevelEditing {

    /// <summary>
    ///
    /// <summary>
    [RequireComponent(typeof(EdgeCollider2D))]
    public class Rope : MonoBehaviour {

        //
        public SpriteShapeController m_SpriteShape;
        
        //
        protected EdgeCollider2D m_EdgeCollider;

        //

        [HideInInspector] protected int segmentCount; // The number of segments.
        [SerializeField] public Transform startpoint; // The width of the rope.
        [SerializeField] protected float weight = 1.5f;
        [SerializeField] protected int stiffness = 5;
        public float jiggleDamp;
        public float velocityDamp; 
        // public float posDamp; 
        public float segmentLength  = 6f/16f;

        [SerializeField] public float ropeLength; // The width of the rope.
        [SerializeField] public float ropeWidth; // The width of the rope.
        [SerializeField] protected Vector3[] ropeSegments; // The current positions of the segments.
        [SerializeField] protected Vector3[] prevRopeSegments; // The previous positions of the segments.
        [SerializeField] protected Vector3[] jiggle; // The previous positions of the segments.

        /* --- Unity --- */
        // Runs once on initialization.
        void Awake() {
            // Cache these references.
            m_EdgeCollider = GetComponent<EdgeCollider2D>();
            m_EdgeCollider.isTrigger = false;
            m_EdgeCollider.edgeRadius = ropeWidth;
            OnAwake();
            RopeSegments();
        }

        // Runs once every set time interval.
        void FixedUpdate() {
            Simulation();
        }

        // Runs if this trigger is activated.
        void OnTriggerStay2D(Collider2D collider) {
            if (collider.GetComponent<Rigidbody2D>()) {
                Jiggle(collider);
            }
        }

        /* --- Methods --- */
        // Initalizes the rope segments.
        void RopeSegments() {
            // Get the number of segments for a rope of this length.
            segmentCount = (int)Mathf.Ceil(ropeLength / segmentLength);

            // Initialize the rope segments.
            ropeSegments = new Vector3[segmentCount];
            prevRopeSegments = new Vector3[segmentCount];
            jiggle = new Vector3[segmentCount];
            
            ropeSegments[0] = Vector3.zero;
            prevRopeSegments[0] = ropeSegments[0];
            jiggle[0] = Vector2.zero;

            m_SpriteShape.spline.Clear();
            m_SpriteShape.spline.InsertPointAt(0, ropeSegments[0]);
            m_SpriteShape.spline.SetTangentMode(0, ShapeTangentMode.Continuous);

            m_EdgeCollider.points = new Vector2[segmentCount];

            for (int i = 1; i < segmentCount; i++) {
                Vector2 offset = segmentLength * Random.insideUnitCircle.normalized;
                offset.y = -Mathf.Abs(offset.y);

                //
                ropeSegments[i] = ropeSegments[i - 1] + (Vector3)offset;
                prevRopeSegments[i] = ropeSegments[i];
                jiggle[i] = new Vector2(0f, 0f);

                //
                m_SpriteShape.spline.InsertPointAt(i, ropeSegments[i]);
                m_SpriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);

            }

        }

        // Adds a jiggle whenever a body collides with this.
        void Jiggle(Collider2D collider) {
            Rigidbody2D body = collider.GetComponent<Rigidbody2D>();
            // Get the segment closest to the collider.
            Vector3 pos = collider.transform.position;
            int index = 1;
            float minDist = 1e9f;
            for (int i = 1; i < segmentCount; i++) {

                Vector3 segPos = transform.position + ropeSegments[i];

                float dist = (pos - segPos).magnitude;
                if (dist < minDist) {
                    index = i;
                    minDist = dist;
                }

            }
            // Add a jiggle to this segment.
            jiggle[index] = body.velocity; // body.gravityScale /  
        }

        void Simulation() {
            Vector3 forceGravity = new Vector3(0f, -weight, 0f);
            for (int i = 0; i < segmentCount; i++) {
                Vector3 velocity = ropeSegments[i] - prevRopeSegments[i];
                prevRopeSegments[i] = ropeSegments[i];
                ropeSegments[i] += velocity * velocityDamp;
                ropeSegments[i] += forceGravity * Time.fixedDeltaTime;
                ropeSegments[i] += jiggle[i] * Time.fixedDeltaTime;

                // float angle = Vector2.SignedAngle(ropeSegments[i] - ropeSegments[i-1], (Vector2)forceGravity); 
                // if (angle > 45f) {}

            }

            for (int i = 0; i < jiggle.Length; i++) {
                jiggle[i] *= jiggleDamp;
            }

            for (int i = 0; i < stiffness; i++) {
                Constraints();
            }

            Vector2[] points = new Vector2[segmentCount];
            for (int i = 0; i < segmentCount; i++) {
                m_SpriteShape.spline.SetPosition(i, ropeSegments[i]);
                points[i] = (Vector2)ropeSegments[i];
            }

            m_EdgeCollider.points = points;
        }

        protected virtual void OnAwake() {

        }

        protected virtual void Constraints() {
            //
        }

    }

}
