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
    [CreateAssetMenu(fileName="ThrowAction", menuName ="Actions/Throw")]
    public class ThrowAction : ChargeAction {

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_ThrowSpeed = 12f;

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_ChargeDuration = 0.5f;

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_PostbounceWeight = 1.5f;

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_ThrowDuration = 2f;

        // The speed of the actual bounce.
        [SerializeField] 
        private float m_PostthrowDuration = 0.2f;

        [SerializeField]
        private PhysicsMaterial2D m_BouncyMaterial;

        [SerializeField]
        private PhysicsMaterial2D m_DefaultMaterial;

        private Timer m_ThrowTimer = new Timer(0f, 0f);

        protected Vector2 m_CachedDirection = new Vector2(0f, 0f);

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            OnEndAction(character);
        }

        protected override void OnStartAction(CharacterController character) {
            base.OnStartAction(character);
            
            character.Body.SetWeight(0f);
            character.Body.velocity = character.Input.Direction.Normal * m_ThrowSpeed;
            if (character.Input.Direction.Normal == Vector2.zero) {
                character.Body.velocity = Vector2.right * character.FacingDirection * m_ThrowSpeed;
            }

            m_ThrowTimer.Start(m_ThrowDuration * ChargeValue);
            character.Collider.sharedMaterial = m_BouncyMaterial;

        }

        protected override void OnStartPostaction(CharacterController character) {
            base.OnStartPostaction(character);
            m_ThrowTimer.Start(m_ThrowTimer.MaxValue); // m_PostthrowDuration);
            character.Body.SetWeight(m_PostbounceWeight);
        }

        // End the bounce.
        protected override void OnEndAction(CharacterController character) {
            base.OnEndAction(character);
            m_ThrowTimer.Stop();
            character.Collider.sharedMaterial = m_DefaultMaterial;
        }

        protected override void WhileAction(CharacterController character, float dt) {
            character.Body.ClampSpeed(m_ThrowSpeed);
        }

        protected override void WhilePostaction(CharacterController character, float dt) {
            character.Body.ClampFallSpeed(DefaultAction.MAX_FALL_SPEED);

        }

    }
}