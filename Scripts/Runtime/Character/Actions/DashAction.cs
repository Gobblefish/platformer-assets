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
    [CreateAssetMenu(fileName="DashAction", menuName ="Actions/Dash")]
    public class DashAction : ChargeAction {

        // The duration under which the character is actually dashing.
        [SerializeField] 
        protected float m_DashDuration = 0.14f;
        
        // A little cooldown after the dash to avoid spam pressing it.
        [SerializeField] 
        protected float m_PostdashDuration = 0.16f;

        // The distance covered by a dash.
        [SerializeField] 
        protected float m_DashDistance = 5f;

        // The direction the player was facing before the dash started.
        protected Vector2 m_CachedDirection = new Vector2(0f, 0f);

        // The speed of the actual dash.
        protected float DashSpeed => m_DashDistance / m_DashDuration;
        
        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            
            if (!enable || m_ActionTimer.Active) {
                OnStartPostaction(character);
                OnEndAction(character);
            }

        }

        protected override void OnStartAction(CharacterController character) {
            base.OnStartAction(character);
            character.Animator.PlayAnimation("OnStartDash");
            
            // character.Animator.PlayAudioVisualEffect(m_StartDashEffect, m_StartDashSound);

            Debug.Log(ChargeValue);
            Debug.Log(DashSpeed);

            m_ActionTimer.Start(m_DashDuration * ChargeValue);
            m_CachedDirection = new Vector2(character.FacingDirection, 0f);
            character.Body.SetVelocity(m_CachedDirection * DashSpeed); // m_CachedDirection * DashSpeed
            
        }

        protected override void OnStartPostaction(CharacterController character) {
            base.OnStartPostaction(character);
            character.Animator.PlayAnimation("OnStartPostdash");
            // character.Animator.PlayAudioVisualEffect(m_EndDashEffect, null);

            // Handle the momentum coming out of the dash.
            if (character.Input.Direction.Horizontal == Mathf.Sign(m_CachedDirection.x)) {
                character.Body.SetVelocity(m_CachedDirection * character.Default.Speed);
            }
            else {
                character.Body.SetVelocity(Vector2.zero);
            }

            character.Default.Enable(character, true);

            // Start the post-dash (dash cooldown) timer.
            m_ActionTimer.Start(m_PostdashDuration);
        }

        protected override void OnEndAction(CharacterController character) {
            base.OnEndAction(character);
            character.Animator.StopAnimation("OnStartDash");
            character.Animator.StopAnimation("OnStartPostdash");
        }

        protected override void WhileAction(CharacterController character, float dt) {
            if (Mathf.Abs(character.Body.velocity.x) < DashSpeed / 2f || Mathf.Abs(character.Body.velocity.y) > 0.2f) {
                OnStartPostaction(character);
            }
        }

    }
}