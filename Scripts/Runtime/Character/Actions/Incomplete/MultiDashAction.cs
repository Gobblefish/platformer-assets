// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// // Gobblefish.
// using Gobblefish.Input;
// // Platformer.
// using Platformer.Physics;
// using Platformer.Character;
// using Platformer.Character;

// /* --- Definitions --- */

// namespace Platformer.Character {

//     ///<summary>
//     /// An ability that near-instantly moves the character.
//     ///<summary>
//     [System.Serializable]
//     public class MultiDashAction : DashAction {

//         [SerializeField]
//         private int m_MaxDashCount = 3;
//         private int m_DashCount = 0;

//         protected override void OnStartDash(CharacterController character) {
//             m_CachedDirection = new Vector2(character.FacingDirection, 0f);
//             m_CachedDirection = character.Input.Direction.Normal;

//             character.Body.SetVelocity(m_CachedDirection * DashSpeed);

//             m_DashTimer.Start(m_DashDuration);
//             m_ActionPhase = ActionPhase.MidAction;

//             character.Animator.Remove(m_PredashAnimation);
//             character.Animator.Push(m_DashAnimation, CharacterAnimator.AnimationPriority.ActionActive);

//             float angle = Vector2.SignedAngle(Vector2.right, m_CachedDirection);
//             m_StartDashEffect.transform.eulerAngles = Vector3.forward * angle;
//             m_StartDashEffect.Play();
//             m_StartDashSound.Play();

//         }

//         protected override void OnStartPostdash(CharacterController character) {
//             if (character.Input.Direction.Horizontal == Mathf.Sign(m_CachedDirection.x)) {
//                 character.Body.SetVelocity(m_CachedDirection * character.Default.Speed);
//                 character.Animator.Push(m_PostdashAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
//             }
//             else {
//                 character.Body.SetVelocity(Vector2.zero);
//             }

//             character.Default.Enable(character, true);

//             character.Animator.Remove(m_DashAnimation);
//             m_EndDashEffect.Play();

//             m_DashTimer.Start(m_PostdashDuration);
//             m_ActionPhase = ActionPhase.PostAction;
//         }

//         protected override void OnEndDash(CharacterController character) {
//             character.Animator.Remove(m_PostdashAnimation);
            
//             m_ActionPhase = ActionPhase.None;

//             if (m_DashCount > 0 && character.Input.Actions[1].Held && character.Input.Direction.Normal != Vector2.zero) {
//                 OnStartPredash(character);
//                 m_DashCount -= 1;
//             }

//         }

//         private void WhilePredashing(CharacterController character, float dt) {

//         }

//         private void WhileDashing(CharacterController character, float dt) {
//             // if (Mathf.Abs(character.Body.velocity.x) < DashSpeed / 2f || Mathf.Abs(character.Body.velocity.y) > 0.2f) {
//             //     OnStartPostdash(character);
//             // }
        
//             if (character.Body.velocity.sqrMagnitude < DashSpeed * DashSpeed / 4f) {
//                 OnStartPostdash(character);
//             }

//         }

//         private void WhilePostdashing(CharacterController character, float dt) {

//         }

//     }
// }