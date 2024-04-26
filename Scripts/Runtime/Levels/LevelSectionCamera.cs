// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Gobblefish.
using Gobblefish.Graphics;

namespace Platformer.Levels {

    /// <summary>
    ///
    /// <summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class LevelSectionCamera : MonoBehaviour {

        // The slight shave off the boundary box for entering/exiting.
        private const float BOUNDARYBOX_SHAVE = 1.99f * 0.25f; // 0.1f; // 0.775f;

        // The trigger box for this camera node.
        [SerializeField]
        private BoxCollider2D m_Box;
        public BoxCollider2D Box => m_Box;

        // The center position of this node.
        private Vector2 m_Position;
        public Vector2 Position => m_Position;

        // Creates a new level section camera.
        public static LevelSectionCamera New(LevelSection section) {
            LevelSectionCamera sectionCamera = new GameObject(section.gameObject.name + " Camera Node", typeof(LevelSectionCamera)).GetComponent<LevelSectionCamera>();
            sectionCamera.Set(section);
            return sectionCamera;
        }

        // Sets the parameters of a level section camera.
        public void Set(LevelSection section) {
            transform.SetParent(section.transform);

            // Edit the hitbox.
            m_Box = gameObject.GetComponent<BoxCollider2D>();
            m_Box.size = new Vector2((float)(section.Width - BOUNDARYBOX_SHAVE), (float)(section.Height - BOUNDARYBOX_SHAVE));
            m_Box.isTrigger = true;

            // Set the position.
            transform.position = section.WorldCenter;
            m_Position = (Vector2)section.WorldCenter;
        }

        public void Snap() {
            Vector3 targetPosition = (Vector3)transform.position + Vector3.forward * GraphicsManager.CamMovement.CameraPlaneDistance;
            Camera.main.transform.position = targetPosition;
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if (collider == PlayerManager.Character.Collider) {
                Debug.Log(gameObject.name);
                GraphicsManager.CamMovement.AddTarget(this);
                // cameraMovement.SetDefaultTarget(transform);
                // cameraMovement.RemoveAll()
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (collider == PlayerManager.Character.Collider) {
                GraphicsManager.CamMovement.RemoveTarget(this);
            }
        }

        public Vector2 GetPosition(Transform track) {
            return CheckBounds(transform.position, track.position);
        }

        private Vector2 CheckBounds(Vector2 centerPosition, Vector2 trackPosition) {

            if (m_Box == null) {
                m_Box = gameObject.GetComponent<BoxCollider2D>();
                Debug.Log("Box was null");
            }

            Vector2 size = m_Box.size / 2f;
            Vector2 dim = GraphicsManager.MainCamera.GetOrthographicDimensions() / 2f;

            float y = CheckDim(trackPosition.y, dim.y, centerPosition.y, size.y);
            float x = CheckDim(trackPosition.x, dim.x, centerPosition.x, size.x);
            
            return new Vector2(x, y); 
            
        }

        private float CheckDim(float track, float dim, float center, float size) {
            float a = track;
            if (size < dim) {
                a = center;
            }
            else {
                // If the top of the camera is higher than the box.
                if (track + dim > center + size) {
                    // print("tracked position ")
                    a = center + size - dim; 
                }
                // If the bottom of the camera is less than the box.
                if (track - dim < center - size) {
                    // Move the bottom of the camera up to the bottom of the level.
                    a = center - size + dim;
                }
            }
            return a;
        }

        public bool debug = true;
        void OnDrawGizmos() {
            if (!debug) { return; }
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            Camera camera = Camera.main;
            float halfHeight = camera.orthographicSize;
            float halfWidth = camera.aspect * halfHeight;
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawCube(transform.position, new Vector3(halfWidth * 2f, halfHeight * 2f, 0f));
        }

    }

}
