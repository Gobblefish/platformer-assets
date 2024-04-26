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
    public class AttachOriginToSpriteShapePoint : MonoBehaviour {

        [SerializeField]
        private SpriteShapeController m_SpriteShapeController;

        [SerializeField]
        private float m_Ratio;

        [SerializeField]
        private Transform m_Scale;

        void Update() {

            int index = (int)Mathf.Floor(m_Ratio * m_SpriteShapeController.spline.GetPointCount());
            if (index >= m_SpriteShapeController.spline.GetPointCount()) {
                return;
            }

            Vector3 v =  m_SpriteShapeController.spline.GetPosition(index);

            if (m_Scale != null) {
                v.x *= m_Scale.localScale.x;
                v.y *= m_Scale.localScale.y;
            }

            transform.position = m_SpriteShapeController.transform.position + (Quaternion.Euler(0f, 0f, m_SpriteShapeController.transform.eulerAngles.z) * v);

        }

    }

}