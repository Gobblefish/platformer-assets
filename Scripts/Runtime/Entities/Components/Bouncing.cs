/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    // TODO: Add the Jelly.
    // TODO: Add the Springs.
    [RequireComponent(typeof(Entity))]
    public class Bouncing : MonoBehaviour {

        #region Enumerations.

        public enum BounceState {
            None,
            Tensing,
            Releasing,
        }

        #endregion

        #region Variables

        /* --- Constants --- */

        // The default bounce speed.
        private const float BOUNCE_SPEED = 26f;

        // The jump speed for missed bounces.
        private const float MISSED_BOUNCE_SPEED = 18f;

        /* --- Members --- */

        // The threshold between which 
        private Entity m_Entity;

        // Whether this is bouncing or releasing.
        [SerializeField] 
        private BounceState m_BounceState = BounceState.None;
        
        // The speed with which this moves.
        [SerializeField] 
        private float m_SinkSpeed = 2f;
        
        // The speed with which this moves.
        [SerializeField] 
        private float m_RiseSpeed = 6f;
        
        // The max tension before releasing.
        [SerializeField] 
        private float m_MaxTension = 0.7f;
        private Vector3 MaxTensionPosition => m_Entity.Origin + Vector3.down * m_MaxTension;

        // The sound that plays when this bounces.
        // [SerializeField] 
        // private AudioClip m_BounceSound = null;

        #endregion
        
        void Awake() {
            m_Entity = GetComponent<Entity>();
        }

        void FixedUpdate() {

            // What to do for each state.
            switch (m_BounceState) {
                case BounceState.Tensing:
                    WhileTensing(Time.fixedDeltaTime);
                    break;
                case BounceState.Releasing:
                    WhileReleasing(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        public void ClampMainPlayerJump() {
            Platformer.Character.CharacterController character = Platformer.PlayerManager.Character;
            // character.Default.ClampJump(true);
        }

        public void OnStartTensing() {
            if (m_BounceState != BounceState.Tensing) {
                m_BounceState = BounceState.Tensing;
            }
        }

        private void WhileTensing(float dt) {
            transform.Move(MaxTensionPosition, m_SinkSpeed, dt, m_Entity.CollisionContainer);
            
            float distance = (transform.localPosition - MaxTensionPosition).magnitude;
            
            // float distance = (m_Origin - MaxTensionPosition).magnitude / m_SinkSpeed;
            if (distance < (m_Entity.Origin - MaxTensionPosition).magnitude / 2f) {
                PreemptiveClamp();
            }

            if (distance < PhysicsManager.Settings.collisionPrecision) {
                CheckPreemptiveBounce();
                // Game.Audio.Sounds.PlaySound(m_BounceSound, 0.2f);
                m_BounceState = BounceState.Releasing;
            }
        }

        private void WhileReleasing(float dt) {
            transform.Move(m_Entity.Origin, m_RiseSpeed, dt, m_Entity.CollisionContainer);

            // Bounce a character that did not pre-emptively bounce if it
            // PRESSES jump while the platform is releasing.
            CheckBounce();

            float distance = (transform.localPosition - m_Entity.Origin).magnitude;
            if (distance < PhysicsManager.Settings.collisionPrecision) {
                m_BounceState = BounceState.None;
                MissedBounce();
            }

        }

        // Clamps the characters jump after the platform has gone down a certain distance.
        private void PreemptiveClamp() {
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    character.Default.ClampJump(true);
                    if (character.Input.Actions[0].Released) {
                        character.Default.OnExternalJump(character, character.Default.JumpSpeed);
                        character.Default.ClampJump(false);
                    }
                }
            }
        }

        // If the character has pressed the jump key/ is holding the jump key while the
        // bouncy platform has preemptively clamped the character.
        private void CheckPreemptiveBounce() {
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    if (true || character.Input.Actions[0].Held) {
                        print("pre-emptive bounce");
                        character.Default.OnExternalJump(character, BOUNCE_SPEED);
                        character.Default.ClampJump(false);
                    }
                    
                }
            }
        }

        // Bounce a character that did not pre-emptively bounce if it
        // PRESSES jump while the platform is releasing.
        private void CheckBounce() {
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    character.Default.OnExternalJump(character, character.Default.JumpSpeed + BOUNCE_SPEED);
                    character.Default.ClampJump(false);
                }
            }
        }

        private void OldCheckBounce() {
            print(m_Entity.CollisionContainer.Count);
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    if (true || character.Input.Actions[0].Held) {
                        print("releasing bounce");
                        character.Default.OnExternalJump(character, BOUNCE_SPEED);
                        character.Default.ClampJump(false);
                    }
                }
            }
        }

        // For characters that missed a bounce.
        private void MissedBounce() {
            for (int i = 0; i < m_Entity.CollisionContainer.Count; i++) {
                CharacterController character = m_Entity.CollisionContainer[i].GetComponent<CharacterController>();
                if (character != null) {
                    print("missed bounce");
                    character.Default.OnExternalJump(character, MISSED_BOUNCE_SPEED);
                    character.Default.ClampJump(false);
                }
            }
            m_Entity.ForceClearContainer();
        }

    }
}
