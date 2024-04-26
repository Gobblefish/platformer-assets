// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// using UnityEngine.Rendering;
// // Gobblefish.
// using Gobblefish;
// using Gobblefish.Graphics;
// // Platformer.
// using Platformer.Levels;
// using Platformer.Physics;

// namespace Platformer.Character {

//     ///<summary>
//     /// An ability that near-instantly moves the character.
//     ///<summary>
//     [System.Serializable]
//     public class GhostAction : CharacterAction {

//         #region Variables.

//         /* --- Constants --- */

//         // The base speed with which the corpse rotates at.
//         private const float BASE_CORPSE_ROTATE_SPEED = 180f;

//         // The maximum distance before applying pressure between the corpse and anchor.
//         private const float TETHER_DISTANCE = 12f;

//         // The strength of the tether.
//         private const float BASE_TETHER_STRENGTH = 0.8f;

//         // The return of the ghost 's speed
//         private const float GHOST_RETURN_SPEED = 25f;

//         // The friction the ghost hand feels.
//         private const float FRICTION = 0.2f;

//         /* --- Members --- */

//         // The time it takes to fully charge.
//         [SerializeField]
//         private float m_GhostModeDuration = 4f;

//         // The speed the ghost hand moves at.
//         [SerializeField]
//         private float m_GhostAcceleration = 60f;

//         // The acceleration with which the ghost hand moves at.
//         [SerializeField]
//         private float m_GhostSpeed = 8f;

//         // The timer that tracks how much has been charged.
//         [SerializeField]
//         private Timer m_GhostTimer = new Timer(0f, 0f);
//         public bool GhostModeActive => m_ActionPhase == ActionPhase.MidAction && m_GhostTimer.Active;

//         // The ghost anchor.
//         [SerializeField]
//         private Rigidbody2D m_Corpse;

//         // The animation that plays when the ghost is active.
//         [SerializeField]
//         private Sprite[] m_GhostModeAnimation = null;

//         //
//         private Quaternion m_CachedRotation;

//         [SerializeField]
//         private VolumeProfile m_GhostVolumeProfile;

//         #endregion

//         // When enabling/disabling this ability.
//         public override void Enable(CharacterController character, bool enable = true) {
//             base.Enable(character, enable);

//             m_CachedRotation = Quaternion.identity;

//             if (!enable) {
//                 if (m_GhostTimer.Active) {
//                     OnGhostReachedAnchor(character);
//                 }
//                 m_ActionPhase = ActionPhase.None;
//                 m_GhostTimer.Stop();
//                 character.Animator.Remove(m_GhostModeAnimation);
//             }

//         }

//         // // When this ability is activated.
//         public override void InputUpdate(CharacterController character) {
//             if (!m_Enabled) { return; }

//             // Ghost.
//             if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None && m_Refreshed) {
//                 OnStartGhostMode(character);
//                 character.Input.Actions[1].ClearPressBuffer();
//                 m_Refreshed = false;
//             }

//             // Ghost.
//             if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.MidAction) {
//                 OnEndGhostMode(character);
//                 character.Input.Actions[1].ClearReleaseBuffer();
//                 m_Refreshed = false;
//             }

//         }
        
//         // // Refreshes the settings for this ability every interval.
//         public override void PhysicsUpdate(CharacterController character, float dt){
//             if (!m_Enabled) { return; }

//             // Whether the power has been reset by touching ground after using it.
//             m_Refreshed = character.OnGround && !m_GhostTimer.Active ? true : m_Refreshed;
            
//             // Tick down the shadow mode.
//             bool finished = m_GhostTimer.TickDown(dt);

//             // If swapping states.
//             if (finished) { 

//                 switch (m_ActionPhase) {
//                     case ActionPhase.MidAction:
//                         OnEndGhostMode(character);
//                         break;
//                     default:
//                         break;
//                 }

//             }

//             switch (m_ActionPhase) {
//                 case ActionPhase.MidAction:
//                     WhileInGhostMode(character, dt);
//                     break;
//                 case ActionPhase.PostAction:
//                     WhileEndingGhostMode(character, dt);
//                     break;
//                 default:
//                     break;
//             }
            
//         }

//         private List<Rigidbody2D> m_Bodies = new List<Rigidbody2D>();

//         private void OnStartGhostMode(CharacterController character) {
//             character.Default.Enable(character, false);

//             // Start the shadow mode timer.
//             m_GhostTimer.Start(m_GhostModeDuration);
//             m_ActionPhase = ActionPhase.MidAction;

//             // m_CachedRotation = character.transform.localRotation;
//             BoundsInt bounds = GraphicsManager.MainCamera.GetBoundsInt(LevelManager.Grid);
//             List<Rigidbody2D> bodies = LevelManager.Instance.ConvertToBlocks(bounds.Pad(2));

//             Platformer.Levels.LevelSection currentSection = LevelManager.CurrentSection;
//             for (int i = 0; i < currentSection.Entities.Count; i++) {
//                 Rigidbody2D body = currentSection.Entities[i].gameObject.GetComponent<Rigidbody2D>();
//                 if (body == null) {
//                     body = currentSection.Entities[i].gameObject.AddComponent<Rigidbody2D>();
//                     body.gravityScale = 0f;
//                 }
//                 m_Bodies.Add(body);
//             }


//             for (int i = 0; i < bodies.Count; i++) {
//                 m_Bodies.Add(bodies[i]);
//             }

//             for (int i = 0; i < m_Bodies.Count; i++) {
//                 m_Bodies[i].isKinematic = false;
//             }

//             Debug.Log(m_Bodies.Count);
//             // for (int i = 0; i < bodies.Count; i++) {
//             //     bodies[i].velocity = 500f * Vector3.up; // Random.insideUnitCircle; // AddForce(
//             // }
//             // m_GhostedDict.Add(Game.Level.CurrentSection, bodies)

//             // GraphicsManager.PostProcessor.SetVolumeProfile(m_GhostVolumeProfile);
//             m_Corpse.GetComponent<Collider2D>().isTrigger = false;

//             character.Body.Stop();

//             if (!m_Corpse.gameObject.activeSelf) {
//                 m_Corpse.gameObject.SetActive(true);
//             }

//             character.Animator.Push(m_GhostModeAnimation, CharacterAnimator.AnimationPriority.ActionActive);
        
//         }

//         private void OnEndGhostMode(CharacterController character) {
//             // Stop the shadow timer.
//             m_GhostTimer.Stop();
//             m_ActionPhase = ActionPhase.PostAction;

//             m_Corpse.GetComponent<Collider2D>().isTrigger = true;

//             Vector3 directionToAnchor = (character.transform.position - m_Corpse.transform.position).normalized;            
//             m_Corpse.velocity = directionToAnchor * GHOST_RETURN_SPEED;

//             for (int i = 0; i < m_Bodies.Count; i++) {
//                 m_Bodies[i].velocity = Vector2.zero;
//                 m_Bodies[i].angularVelocity = 0f;
//                 m_Bodies[i].isKinematic = true;
//             }

//             // GraphicsManager.PostProcessor.RemoveVolumeProfile(m_GhostVolumeProfile);
            
//             // character.transform.localRotation = m_CachedRotation;
//             // Game.Visuals.Effects.StopEffect(m_CircleEffectIndex);

//         }

//         private void OnGhostReachedAnchor(CharacterController character) {
//             // character.transform.localRotation = m_CachedRotation;

//             character.Default.Enable(character, true);
//             character.Body.Stop();
            
//             m_ActionPhase = ActionPhase.None;

//             if (m_Corpse != null) {
//                 m_Corpse.gameObject.SetActive(false);
//             }
            

//             character.Animator.Remove(m_GhostModeAnimation);
            
//         }

//         private void WhileInGhostMode(CharacterController character, float dt) {
//             Vector2 direction = character.Input.Direction.Normal;
//             m_Corpse.AddVelocity(m_GhostAcceleration * direction * dt);

//             if (m_Corpse.velocity.magnitude > m_GhostSpeed) {
//                 m_Corpse.SetVelocity(m_GhostSpeed * m_Corpse.velocity.normalized);
//             }

//             // float rotationFactor = m_Corpse.velocity.magnitude / m_GhostSpeed;
//             // character.transform.Rotate(BASE_CORPSE_ROTATE_SPEED * rotationFactor, dt);
//             character.Body.velocity *= 0.9f;

//             OutOfBounds(m_Corpse.transform);

//         }

//         private void OutOfBounds(Transform characterTransform) {
//             // BoundsInt bounds = GraphicsManager.MainCamera.GetBoundsInt(LevelManager.Grid);
//             // m_Bodies = LevelManager.Instance.ConvertToBlocks(bounds.Pad(2));

//             Vector3 position = characterTransform.position;
//             Vector2 cameraPosition = (Vector2)GraphicsManager.MainCamera.transform.position;
//             Vector2 screenSize = GraphicsManager.MainCamera.GetOrthographicDimensions();

//             float precision = 0.02f;

//             if (position.x < cameraPosition.x - screenSize.x / 2f - 0.5f - precision) {
//                 position.x += screenSize.x;
//             }
//             if (position.x > cameraPosition.x + screenSize.x / 2f + 0.5f + precision) {
//                 position.x -= screenSize.x;
//             }

//             if (position.y < cameraPosition.y - screenSize.y / 2f - 0.5f - precision) {
//                 position.y += screenSize.y;
//             }
//             if (position.y > cameraPosition.y + screenSize.y / 2f + 0.5f + precision) {
//                 position.y -= screenSize.y;
//             }

//             if (characterTransform.position != position) {
//                 characterTransform.position = position;
//             }

//         }


        
//         private void WhileEndingGhostMode(CharacterController character, float dt) {

//             // character.transform.localRotation = m_CachedRotation;

//             Vector3 directionToAnchor = (character.transform.position - m_Corpse.transform.position).normalized;            
//             m_Corpse.velocity = directionToAnchor * GHOST_RETURN_SPEED;
            
//             float distanceToAnchor = (character.transform.position - m_Corpse.transform.position).magnitude;   
//             if (distanceToAnchor < 0.5f) {
//                 OnGhostReachedAnchor(character);
//             }

//         }

//     }
// }