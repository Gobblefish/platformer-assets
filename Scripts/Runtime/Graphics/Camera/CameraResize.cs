/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Gobblefish.Graphics {

    ///<summary>
    /// Controls the size of the camera.
    ///<summary>
    [RequireComponent(typeof(Camera))]
    public class CameraResize : MonoBehaviour {

        [SerializeField]
        private int m_Width = 19;

        [SerializeField]
        private int m_Height = 11;

        [SerializeField]
        private bool m_ResizeOnLoad = true;

        void Start() {
            if (m_ResizeOnLoad) {
                Resize(GraphicsManager.MainCamera, m_Width, m_Height);
            }
        }

        public void Resize(Camera cam, int width, int height) {
            Vector2 camSize = cam.GetOrthographicDimensions();

            float factor = 1f;
            if (width > camSize.x) {
                factor = width / camSize.x;
            }
            if (height > camSize.y) {
                factor = Mathf.Max(height / camSize.y, factor);
            }

            cam.orthographicSize *= factor;
        }

    }

}