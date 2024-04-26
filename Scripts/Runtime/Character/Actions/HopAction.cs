/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish.Audio;
using Gobblefish.Animation;
// Platformer.
using Platformer.Physics;

namespace Platformer.Character {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [CreateAssetMenu(fileName="HopAction", menuName ="Actions/Hop")]
    public class HopAction : ChargeAction {

        // The height to be covered by a fully charged jump.
        [SerializeField] 
        private float m_Height = 8f;
        
        // The time the jump will last.
        [SerializeField] 
        private float m_RisingTime = 0.4f;
        
        // The speed calculated for the jump.
        [SerializeField, ReadOnly] 
        private float m_Speed = 0f;
        
        // The weight calculated for the hop.
        [SerializeField, ReadOnly] 
        private float m_Weight = 0f;

        [SerializeField]
        private AudioSnippet m_OnStartHopSound;

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            if (!enable && m_ActionPhase == ActionPhase.MidAction) {
                character.Body.ClampRiseSpeed(Mathf.Min(character.Body.velocity.y, character.Default.JumpSpeed));
            }
            RefreshHopSettings(ref m_Speed, ref m_Weight, m_Height, m_RisingTime);
        }

        protected override void OnStartAction(CharacterController character) {
            base.OnStartAction(character);
            character.Animator.PlayAnimation("OnStartHop");
            character.Animator.PlayAudioVisualEffect("OnStartHop");

            character.Default.Enable(character, true, false);
            character.Body.Move(Vector2.up * 2f * PhysicsManager.Settings.collisionPrecision);
            character.Body.SetVelocity(m_Speed * ChargeValue * Vector2.up);
            character.Body.SetWeight(m_Weight);

        }

        protected override void OnStartPostaction(CharacterController character) {
            base.OnStartPostaction(character);
            character.Animator.PlayAnimation("OnStartPosthop");
            character.Animator.PlayAudioVisualEffect("OnStartPosthop");
        }

        protected override void WhileAction(CharacterController character, float dt) {
            if (!character.Rising) {
                OnStartPostaction(character);
            }
        }

        protected override void WhilePostaction(CharacterController character, float dt) {
            Debug.Log("While post hop action");

            if (character.OnGround) {
                OnEndAction(character);
            }
        }

        protected override void OnEndAction(CharacterController character) {
            base.OnEndAction(character);
            character.Animator.StopAnimation("OnStartHop");
            character.Animator.StopAnimation("OnStartPosthop");
        }

        // Calculates the speed and weight of the jump.
        public static void RefreshHopSettings(ref float v, ref float w, float h, float t_r) {
            v = 2f * h / t_r;
            w = 2f * h / (t_r * t_r) / Mathf.Abs(UnityEngine.Physics2D.gravity.y);
        }

    }

}