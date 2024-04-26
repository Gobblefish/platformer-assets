// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Gobblefish.Graphics {

    public class CameraTarget : MonoBehaviour {

        public virtual Vector2 GetPosition(Transform defaultTarget) {
            return (Vector2)transform.position;
        }

    }

}