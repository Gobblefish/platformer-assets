// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish.Audio;
// Platformer.
using Platformer.Physics;

namespace Platformer.Character {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [CreateAssetMenu(fileName="GravityAction", menuName ="Actions/Gravity")]
    public class GravityAction : CharacterAction {

        [SerializeField]
        private float m_Strength = 50f;

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            if (m_Enabled && !enable) {
                character.Body.velocity = Vector2.zero;
            }

            base.Enable(character, enable);
            m_ActionPhase = ActionPhase.None;

            if (enable) {
                // SetGravity(Physics2D.gravity.normalized, m_Strength);
                character.Body.SetWeight(0f);
                character.Body.velocity = Vector2.zero;
                character.Default.Enable(character, false);
                started = false;
                m_Count = m_MaxCount;
                m_Refreshed = true;
            }
            else {
                SetGravity(Vector3.down, 9.81f);
                // character.Body.velocity = Vector2.zero;
                character.Default.Enable(character, true);
            }

        }

        public bool dontCare;
        public bool dontCount;

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            if (character.Input.Direction.Normal != Vector2.zero && (dontCount || m_Count > 0)) {
                if (dontCare || !started || character.Input.Direction.MostRecent.normalized != Physics2D.gravity.normalized) {
                    m_Count -= 1;
                    started = true;
                    SetGravity(character.Input.Direction.MostRecent, m_Strength);
                    character.Body.velocity = Vector2.zero;
                }
                character.Input.Direction.Clear();
            }

            if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None && m_Count >= 1) {
                // The character should start dashing.

                character.Body.SetWeight(0f);
                character.Body.velocity = Vector2.zero;
                started = false;

                SetGravity(Vector3.zero, 0f);
                Rigidbody2D[] bodies = (Rigidbody2D[])GameObject.FindObjectsOfType<Rigidbody2D>();
                for (int i = 0; i < bodies.Length; i++) {
                    if (bodies[i].gameObject.activeSelf) { bodies[i].velocity = Vector2.zero; }
                }

                // Release the input and reset the refresh.
                character.Input.Actions[1].ClearPressBuffer();
                m_Refreshed = false;
            }
            
            // Bouncing.
            // if (character.Input.Actions[1].Released && m_ActionPhase != ActionPhase.None) {
            //     // The character should start bouncing.
            //     OnEndBounce(character);

            //     // Release the input and reset the refresh.
            //     character.Input.Actions[1].ClearReleaseBuffer();
            //     m_Refreshed = false;
            // }

        }

        private bool started = false;
        public int m_Count = 0;
        public int m_MaxCount = 3;
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            character.Default.Enable(character, false);

            bool m_OnGravGround = PhysicsManager.Collisions.Touching(character.Body.position + character.Collider.offset, character.Collider.radius, (Vector3)(Physics2D.gravity.normalized), PhysicsManager.CollisionLayers.Ground);
            if (m_OnGravGround) {
                character.Body.velocity = Vector2.zero;
            }

            // m_Refreshed = m_OnGravGround;
            m_Count = m_OnGravGround ? m_MaxCount : m_Count;
            m_Refreshed = m_OnGravGround ? true : m_Refreshed;

            if (started) { character.Body.SetWeight(1f); }
            character.Body.ClampSpeed(25f);

            if (character.Body.velocity.normalized != Physics2D.gravity.normalized) {
                character.Body.velocity = Physics2D.gravity.normalized * character.Body.velocity.magnitude;
            }

            // if (!started) {
            //     Rigidbody2D[] bodies = (Rigidbody2D[])GameObject.FindObjectsOfType<Rigidbody2D>();
            //     for (int i = 0; i < bodies.Length; i++) {
            //         if (bodies[i].gameObject.activeSelf) { bodies[i].velocity = Vector2.zero; }
            //     }
            // }


        }

        public void SetGravity(Vector2 direction, float strength) {
            float angle = Vector2.SignedAngle(Vector2.down, direction);
            Physics2D.gravity = Quaternion.Euler(0f, 0f, angle) * (strength * Vector2.down); // new Vector2(0, -9.81f);
        }

    }
}