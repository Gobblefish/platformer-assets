/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

namespace Platformer.Entities.Utility {

    ///<summary>
    ///
    ///<summary>
    public class PathingNode : MonoBehaviour {

        private Vector3 m_Position;
        public Vector3 Position => m_Position;

        void Awake() {
            m_Position = transform.position;
        }

        public static PathingNode Create(Transform parent, Vector3 position) {
            PathingNode pathingNode = new GameObject("Pathing Node", typeof(PathingNode)).GetComponent<PathingNode>();
            pathingNode.transform.position = position;
            pathingNode.transform.SetParent(parent);
            pathingNode.Awake();
            return pathingNode;
        }

    }

}
