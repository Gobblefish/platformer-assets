/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

namespace Platformer.Entities.Components {

    public class Elongatable : MonoBehaviour {

        public enum SearchDirection {
            Horizontal,
            Vertical
        }

        [System.Serializable]
        public class OverrideLength {
            public int length;
            public GameObject gameObject;
        }

        // Overrides.
        [SerializeField]
        private OverrideLength[] m_Overrides;

        // The box collider.
        private BoxCollider2D m_BoxCollider;
        
        // The sprite shape.
        [SerializeField]
        private SpriteShapeController m_SpriteShapeController;
        public Spline spline => m_SpriteShapeController.spline;

        [SerializeField]
        private Light2D m_Light;

        [SerializeField]
        private Transform[] m_TileAlongPath;
        
        [SerializeField]
        private SpriteShapeController[] m_SubShapes;

        // Height.
        [SerializeField]
        private float m_ColliderHeight;

        // Offset.
        [SerializeField]
        private float m_ColliderVerticalOffset;

        // Offset.
        [SerializeField]
        private float m_ColliderHorizontalInset;

        [SerializeField]
        private int m_LengthUnits = 1;
        public int LengthUnits => m_LengthUnits;

        //
        [SerializeField]
        private SearchDirection m_SearchDirection;
        public SearchDirection searchDirection => m_SearchDirection; 

        void Start() {
            m_BoxCollider = GetComponent<BoxCollider2D>();
        }

        public void SetLength(int length) {
            m_LengthUnits = length;

            if (length < 0) {
                if (!Application.isPlaying) { DestroyImmediate(gameObject); }
                else { Destroy(gameObject); }
                return;
            }
            SetSpriteshapePoints(length);
            SetHitbox((float)length);
            SetLightShape((float)length);
            TileObjects(length);
        }

        public bool SetSpriteshapePoints(int length) {
            Spline spline = m_SpriteShapeController.spline;

            // In the special case that the length of this is 0 or less.
            // if (length <= 2) { return false; }

            m_SpriteShapeController.gameObject.SetActive(false);
            for (int i = 0; i < m_Overrides.Length; i++) {
                if (m_Overrides[i].gameObject != null) {
                    m_Overrides[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < m_Overrides.Length; i++) {
                if (length == m_Overrides[i].length) {
                    if (m_Overrides[i].gameObject != null) {
                        m_Overrides[i].gameObject.SetActive(true);
                    }
                    return false;
                }
            }

            length -= 2;
            spline.Clear();

            Quaternion q = transform.localRotation;

            spline.InsertPointAt(0, q * (0.5f * Vector3.right));
            spline.InsertPointAt(1, q * ((length + 0.5f) * Vector3.right));
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetTangentMode(1, ShapeTangentMode.Continuous);
            m_SpriteShapeController.gameObject.SetActive(true);

            if (m_SubShapes != null && m_SubShapes.Length > 0) {

                for (int i = 0; i < m_SubShapes.Length; i++) {

                    Spline _spline = m_SubShapes[i].spline;

                    _spline.Clear();
                    _spline.InsertPointAt(0, q * (0.5f * Vector3.right));
                    _spline.InsertPointAt(1, q * ((length + 0.5f) * Vector3.right));
                    _spline.SetTangentMode(0, ShapeTangentMode.Continuous);
                    _spline.SetTangentMode(1, ShapeTangentMode.Continuous);
                    
                }

            }

            return true;

        }

        // Edits the hitbox of an obstacle
        public void SetHitbox(float length) {
            m_BoxCollider = GetComponent<BoxCollider2D>();
            if (m_BoxCollider == null) { return; }

            Quaternion q = transform.localRotation;

            Vector2 size = new Vector2(length - m_ColliderHorizontalInset, m_ColliderHeight);
            Vector2 offset = new Vector2((length - 1f) / 2f, m_ColliderVerticalOffset);

            if (transform.eulerAngles.z == 180f) {
                offset.x *= -1f;
            }

            if (m_SearchDirection == SearchDirection.Vertical) {
                size = new Vector2(size.y, size.x);
                offset = new Vector2(offset.y, offset.x);
            }

            m_BoxCollider.size = size;
            m_BoxCollider.offset = offset;
            
        }

        public void SetLightShape(float length) {
            if (m_Light == null) { return; }
            // m_Light.
            print(m_Light.shapePath);

            List<Vector3> shapePath = new List<Vector3>(); //  new Vector3[4 + (int)length * 2]
            int i = 0;
            while (i < (int)length) {
                shapePath.Add(new Vector3(i, -m_ColliderHeight / 2f, 0f));
                i+=1;
            }
            while (i >= 0) {
                shapePath.Add(new Vector3(i, m_ColliderHeight / 2f, 0f));
                i-=1;
            }

            // m_Light.SetShapePath(shapePath);
            SetShapePath(m_Light, shapePath.ToArray());
            // m_Light.BroadcastMessage("UpdateMesh");
        
        }

        void SetFieldValue<T>(object obj, string name, T val) {
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, val);
        }
        
        void SetShapePath(Light2D light, Vector3[] path) {
            SetFieldValue<Vector3[]>(light, "m_ShapePath",  path);
        }

        public void TileObjects(int length) {
            // for (int i = 0; i < m_TileAlongPath.Length; i++) {
            //     for (int j = 0; j < length; j++) {
            //         Transform newTile = Instantiate(m_TileAlongPath[i].gameObject).transform;
            //         newTile.transform.SetParent(m_TileAlongPath[i].parent); 
            //         newTile.localPosition = m_TileAlongPath[i].localPosition + Vector3.right * j; 
            //     }
            // }
        }

    }

}