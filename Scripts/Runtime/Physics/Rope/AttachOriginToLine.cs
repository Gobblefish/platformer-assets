/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using Platformer;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    public class AttachOriginToLine : MonoBehaviour {

        [SerializeField]
        private LineRenderer m_LineRenderer;

        [SerializeField]
        private float m_Ratio;

        [SerializeField]
        private Transform m_Scale;

        void Update() {

            int index = (int)Mathf.Floor(m_Ratio * m_LineRenderer.positionCount);
            if (index >= m_LineRenderer.positionCount) {
                return;
            }

            Vector3 v =  m_LineRenderer.GetPosition(index);

            float angle = 0f;
            if (index > 0) {
                angle = Vector2.SignedAngle(Vector2.right, m_LineRenderer.GetPosition(index) - m_LineRenderer.GetPosition(index - 1));
            }

            if (m_Scale != null) {
                v.x *= m_Scale.localScale.x;
                v.y *= m_Scale.localScale.y;
            }

            transform.position = v;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        }

    }

}