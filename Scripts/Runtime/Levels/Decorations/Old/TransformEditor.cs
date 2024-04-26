/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Gobblefish;

namespace Platformer.Levels {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class TransformEditor : MonoBehaviour {


        [SerializeField]
        private Transform m_SelectedTransform;

        [Header("Flip")]
        public bool m_FlipHorizontally = false;
        public bool m_FlipVertically = false;
        [Header("Rotate")]
        // public bool m_Rotate90 = false;
        // public bool m_RotateBack90 = false;
        public bool m_Rotate45 = false;
        public bool m_RotateBack45 = false;
        public bool m_Rotate5 = false;
        public bool m_RotateBack5 = false;

        void Update() {
            if (!Application.isPlaying) {
                // if (UnityEditor.Selection.activeTransform != null) {
                //     ControlLastAddedTransform(UnityEditor.Selection.activeTransform);
                // }
            }
            
        }

        void ControlLastAddedTransform(Transform transform) {
            if (m_FlipHorizontally) {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
                m_FlipHorizontally = false;
            }

            if (m_FlipVertically) {
                Vector3 localScale = transform.localScale;
                localScale.y *= -1f;
                transform.localScale = localScale;
                m_FlipVertically = false;
            }

            // if (m_Rotate90) { Rotate(-90f, ref m_Rotate90); }
            // if (m_RotateBack90) { Rotate(90f, ref m_RotateBack90); }
            if (m_Rotate45) { Rotate(transform, -45f, ref m_Rotate45); }
            if (m_RotateBack45) { Rotate(transform, 45f, ref m_RotateBack45); }
            if (m_Rotate5) { Rotate(transform, -5f, ref m_Rotate5); }
            if (m_RotateBack5) { Rotate(transform, 5f, ref m_RotateBack5); }
        }

        public void Rotate(Transform transform, float angle, ref bool p) {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z += angle;
            transform.eulerAngles = eulerAngles;
            p = false;
        }

    }

}