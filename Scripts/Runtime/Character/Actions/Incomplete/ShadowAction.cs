// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// using UnityExtensions;
// // Platformer.
// using Platformer.Character;
// using Platformer.Character;

// /* --- Definitions --- */
// // using ShadowBlock = Platformer.Objects.Blocks.ShadowBlock;

// namespace Platformer.Character {

//     ///<summary>
//     /// An ability that near-instantly moves the character.
//     ///<summary>
//     [System.Serializable]
//     public class ShadowAction : CharacterAction {

//         #region Variables.

//         /* --- Constants --- */

//         // The leniency on killing the character when they come out of shadow mode.
//         public const float COLLISION_LENIENCY = 0.1f;

//         // The amount of time it takes to start shadow travelling.
//         public const float SHADOW_TRAVEL_START_DURATION = 0.32f;

//         // The amount of time in between shadow travelling.
//         public const float SHADOW_TRAVEL_DURATION = 0.14f;

//         // The distance travelled when shadow travelling.
//         public const float SHADOW_TRAVEL_DISTANCE = 0.6f;

//         // The amount of time to check for the next shadow block.
//         public const float POST_SHADOW_TRAVEL_DURATION = 0.06f;

//         /* --- Members --- */

//         // The time it takes to fully charge.
//         [SerializeField]
//         private float m_ShadowModeDuration = 0.5f;

//         // The timer that tracks how much has been charged.
//         [SerializeField]
//         private Timer m_ShadowTimer = new Timer(0f, 0f);
//         public bool ShadowModeActive => m_ShadowTimer.Active;

//         // The animation that plays while in shadow mode.
//         [SerializeField]
//         private Sprite[] m_ShadowModeAnimation = null;

//         // The sound that plays when starting shadow travel.
//         [SerializeField]
//         private AudioClip m_StartShadowTravelSound = null;

//         // The sound that plays when shadow travelling.
//         [SerializeField]
//         private AudioClip m_NextShadowTravelSound = null;

//         #endregion

//         // When enabling/disabling this ability.
//         public override void Enable(CharacterController character, bool enable = true) {
//             base.Enable(character, enable);
//             if (m_ShadowTimer.Active) {
//                 OnEndShadowMode(character);
//                 OnEndShadowTravel(character);
//             }
//             m_ActionPhase = ActionPhase.None;
//             m_ShadowTimer.Stop();

//             if (!enable) {
//                 character.Animator.Remove(m_ShadowModeAnimation);
//             }

//         }

//         // When this ability is activated.
//         public override void InputUpdate(CharacterController character) {
//             if (!m_Enabled) { return; }

//             // Dashing.
//             if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
//                 // The character should start dashing.
//                 OnStartShadowMode(character);

//                 // Release the input and reset the refresh.
//                 character.Input.Actions[1].ClearPressBuffer();
//                 m_Refreshed = false;
//             }

//             // Dashing.
//             if (character.Input.Actions[1].Released && m_ActionPhase == ActionPhase.PreAction) {
//                 // The character should start dashing.
//                 OnEndShadowMode(character);

//                 // Release the input and reset the refresh.
//                 character.Input.Actions[1].ClearReleaseBuffer();
//                 m_Refreshed = false;
//             }
            
//         }
        
//         // Refreshes the settings for this ability every interval.
//         public override void PhysicsUpdate(CharacterController character, float dt){
//             if (!m_Enabled) { return; }

//             // Whether the power has been reset by touching ground after using it.
//             m_Refreshed = character.OnGround && !m_ShadowTimer.Active ? true : m_Refreshed;
            
//             // Tick down the shadow mode.
//             bool finished = m_ShadowTimer.TickDown(dt);

//             // If swapping states.
//             if (finished) { 

//                 switch (m_ActionPhase) {
//                     case ActionPhase.PreAction:
//                         OnEndShadowMode(character);
//                         break;
//                     case ActionPhase.MidAction:
//                         OnNextShadowTravel(character);
//                         break;
//                     case ActionPhase.PostAction:
//                         OnEndShadowTravel(character);
//                         break;
//                     default:
//                         break;
//                 }

//             }
                
//             switch (m_ActionPhase) {
//                 case ActionPhase.PreAction:
//                     WhileInShadowMode(character, dt);
//                     break;
//                 case ActionPhase.MidAction:
//                     WhileShadowTravelling(character, dt);
//                     break;
//                 default:
//                     break;
//             }

//         }

//         // Starts the shadow mode that allows for shadow travelling.
//         private void OnStartShadowMode(CharacterController character) {
//             // Start the shadow mode timer.
//             m_ShadowTimer.Start(m_ShadowModeDuration);
//             m_ActionPhase = ActionPhase.PreAction;
//             // Animate the shadow mode.
//             character.Animator.Push(m_ShadowModeAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
//         }

//         // End the shadow mode so that they can no longer shadow travel.
//         private void OnEndShadowMode(CharacterController character) {
//             // Stop the shadow timer.
//             m_ShadowTimer.Stop();
//             m_ActionPhase = ActionPhase.None;
//             // Remove the shadow mode animation.
//             character.Animator.Remove(m_ShadowModeAnimation);
//         }

//         // Try starting the shadow travel when touching a block.
//         public void TryStartShadowTravel(CharacterController character, ShadowBlock block) {
//             if (m_ActionPhase == ActionPhase.PreAction) {
//                 OnStartShadowTravel(character, block, SHADOW_TRAVEL_START_DURATION);
//             }
//             else if (m_ActionPhase == ActionPhase.PostAction) {
//                 OnStartShadowTravel(character, block, SHADOW_TRAVEL_DURATION);
//             }
//         }

//         // Start shadow travelling through blocks.
//         private void OnStartShadowTravel(CharacterController character, ShadowBlock block, float travelDuration) {
//             // Disable the default movement.
//             character.Default.Enable(character, false);

//             // Start the timer for moving through blocks.
//             m_ShadowTimer.Start(travelDuration);
//             m_ActionPhase = ActionPhase.MidAction;

//             // Lock the character.
//             character.transform.position = block.transform.position;
//             character.Body.Freeze();
            
//             // Play the sound.
//             Game.Audio.Sounds.PlaySound(m_StartShadowTravelSound, 0.15f);
//         }

//         // Move to the next shadow block.
//         private void OnNextShadowTravel(CharacterController character) {
//             // Set the timer to look for the next block.
//             m_ShadowTimer.Start(POST_SHADOW_TRAVEL_DURATION);
//             m_ActionPhase = ActionPhase.PostAction;

//             // Move the character.
//             character.transform.position += SHADOW_TRAVEL_DISTANCE * (Vector3)character.Input.Direction.MostRecent;

//             // Play the sound.
//             Game.Audio.Sounds.PlaySound(m_NextShadowTravelSound, 0.15f);
//         }

//         // End the shadow travelling if a block wasn't found in the post shadow travel duration.
//         private void OnEndShadowTravel(CharacterController character) {
//             // Re-enable the default movement.
//             character.Default.Enable(character, true);
//             m_ActionPhase = ActionPhase.None;
            
//             // Reset the body.
//             character.Body.ReleaseXY();
//             character.Body.SetVelocity(character.Default.Speed * character.Input.Direction.Normal);

//             // Check to see if the character should die.
//             float radius = character.Collider.radius - COLLISION_LENIENCY;
//             bool touching = PhysicsManager.Collisions.Touching(character.Body.position, radius, PhysicsManager.CollisionLayers.Ground);
//             bool onScreen = Game.Visuals.Camera.IsWithinBounds(character.Body.position);

//             if (touching || !onScreen) {

//                 // Move the character.
//                 character.Body.position += (character.Collider.radius * 2f) * character.Input.Direction.MostRecent;

//                 touching = PhysicsManager.Collisions.Touching(character.Body.position, radius, PhysicsManager.CollisionLayers.Ground);
//                 onScreen = Game.Visuals.Camera.IsWithinBounds(character.Body.position);

//                 if (touching || !onScreen) {
//                     character.Reset();
//                 }
                
//             }
            
//             // Remove the animations.
//             character.Animator.Remove(m_ShadowModeAnimation);
//         }

//         private void WhileInShadowMode(CharacterController character, float dt) {
            
//         }

//         private void WhileShadowTravelling(CharacterController character, float dt) {
            
//         }

//     }
// }