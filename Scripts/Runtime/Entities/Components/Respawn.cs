/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
// Platformer.
using Platformer.Physics;
using Platformer.Character;

/* --- Definitions --- */
using Entity = Platformer.Entities.Entity;
using Projectile = Platformer.Entities.Utility.Projectile;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    public class Respawn : MonoBehaviour {

        // The amount above the respawn block that this respawns people at.
        public const float RESPAWN_HEIGHT = 1.5f;

        // The amount of time before something is respawned.
        public const float RESPAWN_DELAY = 0.8f;

        // The position at which things are respawned for this block.
        public Vector3 RespawnPosition => transform.position + Vector3.up * RESPAWN_HEIGHT;

        [SerializeField, ReadOnly] 
        private float m_FirstActivationTime = Mathf.Infinity;
        public float FirstActivationTime => m_FirstActivationTime;

        [SerializeField] 
        private float m_TotalActivatedTime = 0f;
        public float TotalTimeActive => m_TotalActivatedTime;

        [SerializeField]
        private VisualEffect m_EmissionParticle;

        [SerializeField, ReadOnly]
        private bool m_Active = false;

        [SerializeField]
        private Projectile m_CorpseProjectile;

        [SerializeField]
        private UnityEvent m_RespawnEvent;

        [SerializeField]
        private UnityEvent m_ActivateEvent;

        [SerializeField]
        private UnityEvent m_DeactivateEvent;

        // Runs once before the first frame.
        void Start() {
            if (PlayerManager.Character.CurrentRespawn == this) {
                Activate();
            }
            else {
                Deactivate();
            }
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            if (m_Active) {
                m_TotalActivatedTime += Time.fixedDeltaTime;
            }
        }

        public void SetRespawnPoint(CharacterController character) {
            if (character == PlayerManager.Character) {
                character.SetRespawn(this);
            }
        }

        public void CreateCorpse(CharacterController character) {

            Projectile projectile = m_CorpseProjectile.CreateInstance();
            projectile.transform.position = character.transform.position;
            projectile.Fire(character.Body.velocity.magnitude, -character.Body.velocity.normalized, 50f);
            // projectile.Fire(5, Quaternion.Euler(0f, 0f, transform.eulerAngles.z + m_SpitAngle) * Vector2.right);

        }

        public void CreateNewShell(CharacterController character) {
            character.transform.position = transform.position + Vector3.up * 1.5f;
            character.Body.SetVelocity(8f * (transform.localRotation * Vector3.up));
            // character.Animator.Push(character.Default.FallingFastAnim, CharacterAnimator.AnimationPriority.ActionPassiveFalling);

            m_RespawnEvent.Invoke();
        }

        public void Activate() {
            if (!m_Active) {
                m_ActivateEvent.Invoke();
            }
            
            m_Active = true;
            if (m_EmissionParticle != null) {
                m_EmissionParticle.Play();
            }
            if (m_TotalActivatedTime == 0f) {
                m_FirstActivationTime = PhysicsManager.Time.Ticks;

                Gobblefish.Graphics.GraphicsManager.Starmap.AddPoint(transform.position);

            }
            
        }

        public void Deactivate() {
            if (m_Active) {
                m_DeactivateEvent.Invoke();
            }

            m_Active = false;
            if (m_EmissionParticle != null) {
                m_EmissionParticle.Stop();
            }        
        }
        
    }

}