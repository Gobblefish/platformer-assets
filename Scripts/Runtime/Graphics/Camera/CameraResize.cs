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

        void Start() {

            if (Platformer.Levels.LevelManager.Instance != null) {
                
                if (Platformer.Levels.LevelManager.Instance.Sections.Count > 0) {
                    Camera cam = Camera.main;
                    Vector2 camSize = cam.GetOrthographicDimensions();

                    float factor = 1f;
                    if (m_Width > camSize.x) {
                        factor = m_Width / camSize.x;
                    }
                    if (m_Height > camSize.y) {
                        factor = Mathf.Max(m_Height / camSize.y, factor);
                    }

                    cam.orthographicSize *= factor;

                }

            }
            
        }

    }

}