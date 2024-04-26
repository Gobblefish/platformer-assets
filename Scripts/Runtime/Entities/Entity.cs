/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
// Platformer.
using Platformer.Entities;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities {

    ///<summary>
    ///
    ///<summary>
    public class Entity : MonoBehaviour {

        [SerializeField, ReadOnly]
        private Vector3 m_Origin;
        public Vector3 Origin => m_Origin;

        private void Start() {
            m_Origin = transform.localPosition;
        }

        public void ResetPosition() {
            transform.localPosition = m_Origin;
        }

        #region Collision

        [SerializeField]
        private List<Collider2D> m_Colliders = new List<Collider2D>();
        public List<Collider2D> Colliders => m_Colliders;
        public Collider2D Collider => m_Colliders.Count > 0 ? m_Colliders[0] : null;
        public CircleCollider2D CircleCollider => m_Colliders.Count > 0 ? m_Colliders[0].GetComponent<CircleCollider2D>() : null;

        // The objects that are attached to the platform.
        protected List<Transform> m_CollisionContainer = new List<Transform>();
        public List<Transform> CollisionContainer => m_CollisionContainer;

        // Add a collider to the list of colliders this entity contains.
        public void AddCollider(Collider2D collider2D) { 
            if (!m_Colliders.Contains(collider2D)) { 
                m_Colliders.Add(collider2D);
            } 
        }

        // Remove a collider from the list of colliders.
        public void RemoveCollider(Collider2D collider2D) { 
            if (m_Colliders.Contains(collider2D)) { 
                m_Colliders.Remove(collider2D); 
            }
        }
        
        // Enable the colliders on this entity.
        public void EnableColliders(bool enable) {
            for (int i = 0; i < m_Colliders.Count; i++) {
                m_Colliders[i].enabled = enable;
            }
        }

        // Enable the colliders on this entity.
        public void EnableRenderers(bool enable) {

            SpriteRenderer r1 = null;
            SpriteShapeRenderer r2 = null;

            foreach (Transform child in transform) {
                
                r1 = child.GetComponent<SpriteRenderer>();
                if (r1 != null) {
                    r1.enabled = enable;
                }

                r2 = child.GetComponent<SpriteShapeRenderer>();
                if (r2 != null) {
                    r2.enabled = enable;
                }

            }

            for (int i = 0; i < m_Colliders.Count; i++) {
                m_Colliders[i].enabled = enable;
            }
        }

        
        
        // Check the state of the collision on this entity. 
        public bool CollisionEnabled => AllCollidersEnabled(true); 
        public bool AllCollidersEnabled(bool enabled) {
            for (int i = 0; i < m_Colliders.Count; i++) {
                if (m_Colliders[i].enabled != enabled) {
                    return false;
                }
            }
            return true;
        }

        // Runs when something collides with this platform.
        public void OnCollisionEnter2D(Collision2D collision) {
            // Check if there is a character.
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            if (!m_CollisionContainer.Contains(character.transform)) {
                m_CollisionContainer.Add(character.transform);
            }
            
        }

        // Runs when something exit this platform.
        public void OnCollisionExit2D(Collision2D collision) {
            CharacterController character = collision.collider.GetComponent<CharacterController>();
            if (character == null) { return; }

            // Edit the collision container as appropriate.
            if (m_CollisionContainer.Contains(character.transform)) {
                m_CollisionContainer.Remove(character.transform);
            }
        }

        public void SetAsTrigger(bool trigger) {
            for (int i = 0; i < m_Colliders.Count; i++) {
                m_Colliders[i].isTrigger = trigger;
            }
        }

        public void ForceClearContainer() {
            m_CollisionContainer = new List<Transform>();
        }

        #endregion
        
        [SerializeField]
        private Renderer m_Renderer;
        public Renderer Renderer => m_Renderer;

        public void SetMaterial(Material material) {
            // Should this be more careful.
            m_Renderer.material = material;
            for (int i = 0; i < m_Renderer.materials.Length; i++) {
                m_Renderer.materials[i] = material;
            }
        }

        public void SetMaterialValue(string name, float value) {
            // Should this be more careful.
            for (int i = 0; i < m_Renderer.materials.Length; i++) {
                m_Renderer.materials[i].SetFloat(name, value);
            }
        }

    }

}