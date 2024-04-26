/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
// Platformer.
using Platformer.Entities;

namespace Platformer.Entities.Utility {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Entity))]
    public class SpriteShapeCollider : MonoBehaviour {

        [HideInInspector]
        private Entity m_Entity;

        [HideInInspector]
        private SpriteShapeController m_SpriteShapeController;

        // The height of a platform.
        [SerializeField]
        public float m_OffsetHeight = 0.1f;

        // In order to properly center the platform.
        [SerializeField]
        protected float m_Inset = 2f/16f;

        // The height of a platform.
        [SerializeField]
        public float m_EdgeRadius = 0.15f;

        // The height of a platform.
        [SerializeField]
        public float m_ExtrusionFactor = 1.2f;

        // The subdivided curve.
        private List<Vector2> m_SubdividedCurve = new List<Vector2>();
        
        // The subdivision depth.
        [SerializeField]
        private int m_Depth = 10;
        
        // The number of points in the sprite shape.
        private List<Vector3> m_Points = new List<Vector3>();

        // The right tangents of each point.
        private List<Vector3> m_RightTangents = new List<Vector3>();
        
        // The left tangents of each point.
        private List<Vector3> m_LeftTangents = new List<Vector3>();

        void Awake() {
            // Cache relevant components.
            m_Entity = GetComponent<Entity>();
            if (m_Entity.Renderer.GetComponent<SpriteShapeController>() != null) {
                m_SpriteShapeController = m_Entity.Renderer.GetComponent<SpriteShapeController>();
            }

            // Create the collider.
            CalculateCollider();
            CreateCollider();
        }

        void CalculateCollider() {

            m_Points = new List<Vector3>();
            m_RightTangents = new List<Vector3>();
            m_LeftTangents = new List<Vector3>();

            int pointCount = m_SpriteShapeController.spline.GetPointCount();
            Vector2 offset = new Vector2(0f, m_OffsetHeight);

            for (int i = 0; i < pointCount; i++) {
                m_SpriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                m_Points.Add(m_SpriteShapeController.spline.GetPosition(i)+(Vector3)offset);
                m_RightTangents.Add(m_Points[i]+m_SpriteShapeController.spline.GetRightTangent(i));
                m_LeftTangents.Add(m_Points[i]+m_SpriteShapeController.spline.GetLeftTangent(i));
            }

            m_SubdividedCurve = new List<Vector2>();

            for (int i = 1; i < m_Points.Count; i++) {
                
                m_SubdividedCurve.Add(m_Points[i-1]);
                for (int j = 1; j < m_Depth; j++) {
                    Vector3 p = FourPointBezierCurve(i, j);
                    m_SubdividedCurve.Add(p);
                }
                
            }

            m_SubdividedCurve.Add(m_Points[m_Points.Count - 1]);

            float outset = (1f - m_Inset) - 2f * m_EdgeRadius;
            Vector3 startCap= -outset * (m_SubdividedCurve[1] - m_SubdividedCurve[0]).normalized + m_SubdividedCurve[0];  
            m_SubdividedCurve.Insert(0, startCap);

            Vector3 endCap = -outset * (m_SubdividedCurve[m_SubdividedCurve.Count-2] - m_SubdividedCurve[m_SubdividedCurve.Count-1]).normalized + m_SubdividedCurve[m_SubdividedCurve.Count-1];  
            m_SubdividedCurve.Add(endCap);

        }

        private Vector3 FourPointBezierCurve(int i, int j) {
            
            float t = (float)j / (float)m_Depth;

            Vector3 p1 = (1-t)*(1-t)*(1-t)*m_Points[i-1]; 
            Vector3 p2 = 3 * (1-t)*(1-t)*t*m_RightTangents[i-1];
            Vector3 p3 = 3 * (1-t)*t*t*m_LeftTangents[i];
            Vector3 p4 = t*t*t*m_Points[i];

            return p1 + p2 + p3 + p4;
            
        }

        private void CreateCollider() {
            // Edge Collider.
            EdgeCollider2D collider = gameObject.AddComponent<EdgeCollider2D>();
            collider.edgeRadius = m_EdgeRadius;
            collider.points = m_SubdividedCurve.ToArray();

            // Box Collider Caps.
            BoxCollider2D boxA = AddCap(0, 1);
            BoxCollider2D boxB = AddCap(m_SubdividedCurve.Count - 1, m_SubdividedCurve.Count - 2);

            // Pass them to the obstacle.
            m_Entity.AddCollider(collider);
            m_Entity.AddCollider(boxA);
            m_Entity.AddCollider(boxB);

        }

        private BoxCollider2D AddCap(int index0, int index1) {
            BoxCollider2D box = new GameObject("box cap", typeof(BoxCollider2D), typeof(SpriteShapeCap)).GetComponent<BoxCollider2D>();
            box.transform.parent = transform;

            box.size = 2f * m_EdgeRadius * new Vector2(m_ExtrusionFactor, 0.5f);
            box.transform.localPosition = m_SubdividedCurve[index0]; // offset = partitions[0];
            box.transform.localPosition += new Vector3(0f, box.size.y / 2f, 0f);
            
            float angle = Vector2.SignedAngle(Vector2.right, m_SubdividedCurve[index0] - m_SubdividedCurve[index1]);
            box.transform.eulerAngles = new Vector3(0f, 0f, angle);
            box.GetComponent<SpriteShapeCap>().entity = m_Entity;
            box.gameObject.layer = gameObject.layer;
            return box;
        }

        public bool debugCollider = false;
        void OnDrawGizmos() {
            if (!debugCollider || Application.isPlaying) { return; }
            
            Vector2 origin = (Vector2)transform.position;
            CalculateCollider();

            Gizmos.color = Color.yellow;
            for (int i = 0; i < m_RightTangents.Count; i++) {
                Gizmos.DrawWireSphere(origin + (Vector2)m_RightTangents[i], 0.25f);
            }

            Gizmos.color = Color.red;
            for (int i = 1; i < m_SubdividedCurve.Count; i++) {
                Gizmos.DrawLine(origin + m_SubdividedCurve[i-1], origin + m_SubdividedCurve[i]);
                Gizmos.color = Gizmos.color == Color.red ? Color.blue : Color.red; 
            }

        }

    }

}