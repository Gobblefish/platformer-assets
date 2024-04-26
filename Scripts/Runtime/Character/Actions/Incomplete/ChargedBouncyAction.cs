// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// // Gobblefish.
// using Gobblefish.Audio;
// // Platformer.
// using Platformer.Physics;

// namespace Platformer.Character {

//     ///<summary>
//     /// An ability that near-instantly moves the character.
//     ///<summary>
//     [CreateAssetMenu(fileName="ChargedBouncyAction", menuName ="Actions/ChargedBouncy")]
//     public class ChargedBouncyAction : CharacterAction {

//         #region Variables.
        
//         // The duration between pressing and moving, gives a little anticapatory feel.
//         [SerializeField] 
//         protected float m_PrebounceDuration = 0.16f;

//         // The duration under which the character is actually bouncing.
//         [SerializeField] 
//         protected float m_BounceDuration = 1f;
        
//         // A little cooldown after the bounce to avoid spam pressing it.
//         [SerializeField] 
//         protected float m_PostbounceDuration = 0.16f;

//         // Runs through the phases of the bounce cycle.
//         [HideInInspector] 
//         protected Timer m_BounceTimer = new Timer(0f, 0f);

//         // The speed of the actual bounce.
//         [SerializeField] 
//         private float m_BounceSpeed = 24f;

//         // The direction the player was facing before the bounce started.
//         [HideInInspector]
//         protected Vector2 m_CachedDirection = new Vector2(0f, 0f);

//         // The sprites this is currently animating through.
//         [SerializeField]
//         protected Sprite[] m_PrebounceAnimation = null;

//         // The sprites this is currently animating through.
//         [SerializeField]
//         protected Sprite[] m_BounceAnimation = null;

//         // The sprites this is currently animating through.
//         [SerializeField]
//         protected Sprite[] m_PostbounceAnimation = null;

//         // The visual effect that plays at the start of the bounce.
//         [SerializeField]
//         protected VisualEffect m_StartBounceEffect;

//         // The visual effect that plays at the start of the bounce.
//         [SerializeField]
//         protected AudioSnippet m_StartBounceSound;

//         // The visual effect that plays at the start of the bounce.
//         [SerializeField]
//         protected VisualEffect m_EndBounceEffect;

//         // The time it takes to fully charge.
//         [SerializeField]
//         private float m_ChargeDuration = 0.5f;

//         // The timer that tracks how much has been charged.
//         [SerializeField]
//         private Timer m_ChargeTimer = new Timer(0f, 0f);

//         // The tracks the increments of charge.
//         [SerializeField]
//         private Timer m_ChargeIncrementTimer = new Timer(0f, 0f);

//         // The sounds that plays when charging the hop.
//         [SerializeField]
//         private VisualEffect m_ChargeBounceEffect = null;

//         // The sounds that plays when charging the hop.
//         [SerializeField]
//         private AudioSnippet m_ChargeBounceSound = null;

//         [SerializeField]
//         private PhysicsMaterial2D m_BouncyMaterial;

//         [SerializeField]
//         private PhysicsMaterial2D m_DefaultMaterial;

//         #endregion

//         // When enabling/disabling this ability.
//         public override void Enable(CharacterController character, bool enable = true) {
//             base.Enable(character, enable);
//             if (m_BounceTimer.Active) {
//                 OnStartPostbounce(character);
//             }
//             m_ActionPhase = ActionPhase.None;
//             m_BounceTimer.Stop();

//             if (!enable) {
//                 character.Animator.Remove(m_PrebounceAnimation);
//                 character.Animator.Remove(m_PostbounceAnimation);
//                 character.Animator.Remove(m_BounceAnimation);
//             }

//         }

//         // When this ability is activated.
//         public override void InputUpdate(CharacterController character) {
//             if (!m_Enabled) { return; }

//             // Bouncing.
//             if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
//                 // The character should start bouncing.
//                 OnStartCharge(character);

//                 // Release the input and reset the refresh.
//                 character.Input.Actions[1].ClearPressBuffer();
//                 m_Refreshed = false;
//             }

//             // Bouncing.
//             if (character.Input.Actions[1].Released && m_ActionPhase == ActionPhase.PreAction) {
//                 // The character should start bouncing.
//                 OnStartPrebounce(character);

//                 // Release the input and reset the refresh.
//                 character.Input.Actions[1].ClearReleaseBuffer();
//                 m_Refreshed = false;
//             }

//         }
        
//         // Refreshes the settings for this ability every interval.
//         public override void PhysicsUpdate(CharacterController character, float dt){
//             if (!m_Enabled) { return; }

//             // Whether the power has been reset by touching ground after using it.
//             m_Refreshed = character.OnGround && !m_BounceTimer.Active && !m_ChargeTimer.Active ? true : m_Refreshed;

//             // Tick down the bounce timer.
//             bool finished = m_BounceTimer.TickDown(dt);

//             // If swapping states.
//             if (finished) { 

//                 switch (m_ActionPhase) {
//                     case ActionPhase.MidAction:
//                         OnStartPostbounce(character);
//                         break;
//                     default:
//                         break;
//                 }

//             }

//             // Charge the hop.
//             m_ChargeTimer.TickDown(dt);
            
//             // If in a phase.
//             switch (m_ActionPhase) {
//                 case ActionPhase.PreAction:
//                     WhileCharging(character, dt);
//                     break;
//                 case ActionPhase.MidAction:
//                     WhileBouncing(character, dt);
//                     break;
//                 case ActionPhase.PostAction:
//                     WhilePostbouncing(character, dt);
//                     break;
//                 default:
//                     break;
//             }

//         }

//         private void OnStartCharge(CharacterController character) {
//             // Disable other inputs.
//             // character.Disable(duration); // lol, why did i do it like this?
//             character.Default.Enable(character, false);
//             changedDir = false;

//             // Stop the body.
//             character.Body.Stop();
//             character.Body.SetWeight(0.05f);

//             // Start the bounce timer.
//             m_ChargeTimer.Start(m_ChargeDuration);
//             m_ActionPhase = ActionPhase.PreAction;

//             character.Animator.Push(m_PrebounceAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
//             character.Animator.PlayAudioVisualEffect(m_ChargeBounceEffect, m_ChargeBounceSound);
//             m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);

//         }

       
//         private Vector2 cachedDirection = Vector2.zero;
//         protected virtual void OnStartPrebounce(CharacterController character) {
//             // Get the direction the character is facing.
//             // Vector2 direction = new Vector2(character.FacingDirection, character.Input.Direction.Vertical);
//             // if (direction.y == 0f) {
//             //     direction = new Vector2(character.FacingDirection, -1f);
//             // }

//             Vector2 direction = new Vector2(character.FacingDirection, -1f);
//             direction = direction.normalized;
            
//             character.Body.SetVelocity(direction * m_BounceSpeed * Mathf.Sqrt(m_ChargeTimer.InverseRatio));

//             character.Collider.sharedMaterial = m_BouncyMaterial;

//             // Start the bounce timer.
//             // m_BounceTimer.Start(m_BounceDuration * Mathf.Sqrt(m_ChargeTimer.InverseRatio));
//             m_ActionPhase = ActionPhase.MidAction;

//             // Replace the animation.
//             character.Animator.Remove(m_PrebounceAnimation);
//             character.Animator.Push(m_BounceAnimation, CharacterAnimator.AnimationPriority.ActionActive);
//             character.Animator.PlayAudioVisualEffect(m_StartBounceEffect, m_StartBounceSound);
//             if (character.Default.Trail != null) { character.Default.Trail.Play(); }

//             //
//             m_ChargeBounceSound.Stop();
//             m_ChargeTimer.Stop();
            
//         }

//         protected virtual void OnStartPostbounce(CharacterController character) {

//             // Re-enable control over the character.
//             // character.Default.Enable(character, true);
//             character.Body.SetWeight(3f);
//             character.Collider.sharedMaterial = m_DefaultMaterial;

//             // Replace the animation.
//             character.Animator.Remove(m_BounceAnimation);
//             character.Animator.Push(m_PostbounceAnimation, CharacterAnimator.AnimationPriority.ActionPostActive);
//             character.Animator.PlayAudioVisualEffect(m_EndBounceEffect, null);

//             // Start the post-bounce (bounce cooldown) timer.
//             // m_BounceTimer.Start(m_PostbounceDuration);
//             m_ActionPhase = ActionPhase.PostAction;
//         }

//         // End the bounce.
//         protected virtual void OnEndBounce(CharacterController character) {
//             character.Default.Enable(character, true);
//             character.Animator.Remove(m_PostbounceAnimation);
//             m_ActionPhase = ActionPhase.None;
//             if (character.Default.Trail != null) { character.Default.Trail.Stop(); }
//         }

//         private void WhileCharging(CharacterController character, float dt) {
//             character.Body.ClampRiseSpeed(0f);

//             bool chargeIncremented = m_ChargeIncrementTimer.TickDown(dt);
//             if (chargeIncremented && m_ChargeTimer.InverseRatio < 1f) {
//                 character.Animator.PlayAudioVisualEffect(m_ChargeBounceEffect, m_ChargeBounceSound);
//                 m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);
//             }

//         }

//         private void WhilePrebouncing(CharacterController character, float dt) {
            
//         }

//         private bool changedDir = false;
//         private void WhileBouncing(CharacterController character, float dt) {
//             if (cachedDirection != character.Body.velocity.normalized && !changedDir) {
//                 m_BounceTimer.Start(m_BounceDuration);
//                 cachedDirection = character.Body.velocity.normalized;
//                 changedDir = true;
//                 character.Body.SetWeight(1f);
//             }
//         }

//         private void WhilePostbouncing(CharacterController character, float dt) {
//             if (character.OnGround) {
//                 OnEndBounce(character);
//             }
//         }

//     }
// }