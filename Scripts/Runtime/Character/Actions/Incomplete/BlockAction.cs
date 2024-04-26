// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// using UnityExtensions;
// // Platformer.
// using Gobblefish.Input;
// using Platformer.Character;
// using Platformer.Character;

// /* --- Definitions --- */
// 
// namespace Platformer.Character {

//     ///<summary>
//     /// An ability that near-instantly moves the character.
//     ///<summary>
//     [System.Serializable]
//     public class BlockAction : CharacterAction {

//         #region Variables.

//         // The duration between pressing and moving, gives a little anticapatory feel.
//         [SerializeField] 
//         private float m_PredashDuration = 0.16f;

//         // The duration under which the character is actually dashing.
//         [SerializeField] 
//         private float m_DashDuration = 0.14f;
        
//         // A little cooldown after the dash to avoid spam pressing it.
//         [SerializeField] 
//         private float m_PostdashDuration = 0.16f;

//         // Runs through the phases of the dash cycle.
//         [HideInInspector] 
//         private Timer m_ShadowTimer = new Timer(0f, 0f);

//         // The distance covered by a dash.
//         [SerializeField] 
//         private float m_DashDistance = 5f;

//         // The speed of the actual dash.
//         private float DashSpeed => m_DashDistance / m_DashDuration;

//         // The direction the player was facing before the dash started.
//         [HideInInspector]
//         private Vector2 m_CachedDirection = new Vector2(0f, 0f);

//         // The sprites this is currently animating through.
//         [SerializeField]
//         private Sprite[] m_PredashAnimation = null;

//         // The sprites this is currently animating through.
//         [SerializeField]
//         private Sprite[] m_DashAnimation = null;

//         // The sprites this is currently animating through.
//         [SerializeField]
//         private Sprite[] m_PostdashAnimation = null;

//         // The sounds that plays when dashing.
//         [SerializeField]
//         private AudioClip m_DashSound = null;

//         // The effect that plays when dashing.
//         // [SerializeField]
//         // private ParticleSystem m_DashEffect;

//         #endregion

//         // When enabling/disabling this ability.
//         public override void Enable(CharacterController character, bool enable = true) {
//             base.Enable(character, enable);
//             if (m_ShadowTimer.Active) {
//                 OnStartPostdash(character);
//             }
//             m_ActionPhase = ActionPhase.None;
//             m_ShadowTimer.Stop();

//             if (!enable) {
//                 character.Animator.Remove(m_PredashAnimation);
//                 character.Animator.Remove(m_PostdashAnimation);
//                 character.Animator.Remove(m_DashAnimation);
//             }

//         }

//         // When this ability is activated.
//         public override void InputUpdate(CharacterController character) {
//             if (!m_Enabled) { return; }

//             // Dashing.
//             if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
//                 // The character should start dashing.
//                 OnStartPredash(character);

//                 // Release the input and reset the refresh.
//                 character.Input.Actions[1].ClearPressBuffer();
//                 m_Refreshed = false;
//             }

//         }
        
//         // Refreshes the settings for this ability every interval.
//         public override void PhysicsUpdate(CharacterController character, float dt){
//             if (!m_Enabled) { return; }

//             // Whether the power has been reset by touching ground after using it.
//             m_Refreshed = character.OnGround && !m_ShadowTimer.Active ? true : m_Refreshed;

//             // Tick down the dash timer.
//             bool finished = m_ShadowTimer.TickDown(dt);

//             // If swapping states.
//             if (finished) { 

//                 switch (m_ActionPhase) {
//                     case ActionPhase.PreAction:
//                         OnStartDash(character);
//                         break;
//                     case ActionPhase.MidAction:
//                         OnStartPostdash(character);
//                         break;
//                     case ActionPhase.PostAction:
//                         OnEndDash(character);
//                         break;
//                     default:
//                         break;
//                 }

//             }
            
//             // If in a phase.
//             switch (m_ActionPhase) {
//                 case ActionPhase.PreAction:
//                     WhilePredashing(character, dt);
//                     break;
//                 case ActionPhase.MidAction:
//                     WhileDashing(character, dt);
//                     break;
//                 case ActionPhase.PostAction:
//                     WhilePostdashing(character, dt);
//                     break;
//                 default:
//                     break;
//             }

//         }

//         private void OnStartPredash(CharacterController character) {
//             // Disable other inputs.
//             character.Disable(m_PredashDuration + m_DashDuration);
//             character.Default.Enable(character, false);

//             // Stop the body.
//             character.Body.Stop();

//             // Start the dash timer.
//             m_ShadowTimer.Start(m_PredashDuration);
//             m_ActionPhase = ActionPhase.PreAction;

//             character.Animator.Push(m_PredashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
//             Game.Audio.Sounds.PlaySound(m_DashSound, 0.15f);

//         }

//         private void OnStartDash(CharacterController character) {
//             m_CachedDirection = new Vector2(character.FacingDirection, 0f);
//             character.Body.SetVelocity(m_CachedDirection * DashSpeed);
//             // PlayerManager.Character.ExplodeDust.Activate();

//             m_ShadowTimer.Start(m_DashDuration);
//             m_ActionPhase = ActionPhase.MidAction;

//             character.Animator.Remove(m_PredashAnimation);
//             character.Animator.Push(m_DashAnimation, CharacterAnimator.AnimationPriority.ActionActive);
//             // Game.Visuals.Effects.PlayImpactEffect(character.OnActionParticle, 16, 1.6f, character.transform, Vector3.zero);

//         }

//         private void OnStartPostdash(CharacterController character) {
//             if (character.Input.Direction.Horizontal == Mathf.Sign(m_CachedDirection.x)) {
//                 character.Body.SetVelocity(m_CachedDirection * character.Default.Speed);
//                 character.Animator.Push(m_PostdashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
//             }
//             else {
//                 character.Body.SetVelocity(Vector2.zero);
//             }

//             character.Default.Enable(character, true);

//             character.Animator.Remove(m_DashAnimation);

//             m_ShadowTimer.Start(m_PostdashDuration);
//             m_ActionPhase = ActionPhase.PostAction;
//         }

//         private void OnEndDash(CharacterController character) {
//             character.Animator.Remove(m_PostdashAnimation);
//             m_ActionPhase = ActionPhase.None;
//         }

//         private void WhilePredashing(CharacterController character, float dt) {

//         }

//         private void WhileDashing(CharacterController character, float dt) {
//             if (Mathf.Abs(character.Body.velocity.x) < DashSpeed / 2f || Mathf.Abs(character.Body.velocity.y) > 0.2f) {
//                 OnStartPostdash(character);
//             }
//         }

//         private void WhilePostdashing(CharacterController character, float dt) {

//         }

//     }
// }