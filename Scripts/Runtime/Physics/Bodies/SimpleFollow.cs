/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Physics;
using Gobblefish.Animation;

namespace Platformer.Graphics {

    ///<summary>
    ///
    ///<summary>
    public class SimpleFollow : MonoBehaviour {

        private Transform m_Parent;

        private Vector3 m_Offset = Vector3.zero;

        // // The duration this crumbles for before falling.
        // [SerializeField]
        // private float m_FollowDelay = 0.5f;

        [SerializeField]
        private float m_FollowStrength = 5f;

        private float m_Scale;
        private Vector3 m_BaseParentScale;

        public bool flipX;

        public bool onlyHorizontal;

        void Start() {
            m_Offset = transform.localPosition;
            m_Parent = transform.parent;
            transform.SetParent(null);

            Matrix4x4 parentMatrix = m_Parent.localToWorldMatrix;

            m_BaseParentScale = parentMatrix.ExtractScale();
            m_Scale = transform.localToWorldMatrix.ExtractScale().x;
        }


        // Runs once every fixed interval.
        void FixedUpdate() {
            WhileFollowing();
        }

        private void WhileFollowing() {
            Vector3 offset = Quaternion.Euler(0f, m_Parent.eulerAngles.y, 0f) * m_Offset;
            // if (flipX) {
            //     offset.x *= -1f;
            // }

            transform.eulerAngles = m_Parent.eulerAngles;
            transform.localScale = m_Scale * (new Vector3(1f, 1f, 1f) + (m_Parent.localScale - m_BaseParentScale));

            Vector3 followPosition = m_Parent.position + offset; // (m_Follow.position - transform.position).normalized * m_Index;
            float mag = (followPosition - transform.position).magnitude;
            transform.Move(followPosition, mag * m_FollowStrength, Time.fixedDeltaTime);

            // if (onlyHorizontal) {
            //     transform.position = new Vector3(followPosition.x, transform.position.y, transform.position.z);
            // }
        }

    }

}
