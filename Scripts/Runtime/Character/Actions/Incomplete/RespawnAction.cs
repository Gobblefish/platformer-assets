// // System.
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// // Platformer.
// using Platformer.Physics;

// namespace Platformer.Character {

//     ///<summary>
//     /// An ability that near-instantly moves the character.
//     ///<summary>
//     [System.Serializable]
//     public class RespawnAction : CharacterAction {

//         #region Variables

//         private bool m_Dying = false;

//         // The timer that tracks how much has been charged.
//         [SerializeField]
//         private Timer m_RespawnTimer = new Timer(0f, 0f);

//         #endregion

//         // When enabling/disabling this ability.
//         public override void Enable(CharacterController character, bool enable = true) {
//             base.Enable(character, enable);
//             if (!enable) {
//                 character.Animator.Remove(m_DeathAnimation);
//             }
//         }

//         public void Set(Respawn respawn) {
//             if (m_Respawn != null) {
//                 m_Respawn.Deactivate();
//             }
//             m_Respawn = respawn;
//             m_Respawn.Activate();
//         }
        
//         // Refreshes the settings for this ability every interval.
//         public override void PhysicsUpdate(CharacterController character, float dt){
//             if (!m_Enabled) { return; }

//             // Tick down the climb timer.
//             bool finished = m_RespawnTimer.TickDown(dt);

//             // If swapping states.
//             if (finished) { 

//                 switch (m_ActionPhase) {
//                     case ActionPhase.PreAction:
//                         OnStartRespawn(character);
//                         break;
//                     case ActionPhase.PostAction:
//                         OnEndRespawn(character);
//                         break;
//                     default:
//                         break;
//                 }

//             }

//             // If in a phase.
//             switch (m_ActionPhase) {
//                 case ActionPhase.PreAction:
//                     WhileClimbing(character, dt);
//                     break;
//                 case ActionPhase.MidAction:
//                     WhileWallJumping(character, dt);
//                     break;
//                 default:
//                     break;
//             }
            
//         }

//         public void OnStartPreDeath(CharacterController character) {
//             if (m_Respawn == null && tag == "Player") {
//                 SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//                 return;
//             }

//             // Resetting the character.
//             m_Respawn.CreateCorpse(this);

//             character.Disable(Respawn.RESPAWN_DELAY * 2f);
//             character.DisableAllAbilityActions();
//             character.Default.Enable(character, false);

//             character.Body.SetWeight(0f);
//             character.Body.Stop();
//             m_Dying = true;

//             m_ActionPhase = ActionPhase.PreAction;
//             m_ClimbTimer.Start(m_PredeathDuration);

//             character.Animator.Push(m_DeathAnimation, CharacterAnimator.AnimationPriority.ActionPreActive);
            
//         }

//         public void Reset() {
//             if (m_Dying) {
//                 return;
//             }

//             // GraphicsManager.
//             // The visual feedback played when dying.
//             // PhysicsManager.Time.RunHitStop(16);
//             // GraphicsManager.Effects.PlayImpactEffect(m_OnDeathParticle,30, 5f, transform, Vector3.zero);
//             // AudioManager.Sounds.PlaySound(m_OnDeathSound, 0.15f);

//             // Noting the death in the stats.
//             // LevelManager.AddDeath();
//             // LevelManager.Reset();

            

//             // transform.position = m_Respawn.RespawnPosition;
//             StartCoroutine(IERespawn(Respawn.RESPAWN_DELAY));

//         }

//         private void OnStartDeath() {
//             m_Respawn.CreateNewShell(this);

//             // yield return new WaitForSeconds(delay);
//             m_Dying = false;
//             m_DefaultAction.Enable(this, true);
//             // Game.Audio.Sounds.PlaySound(m_OnRespawnSound, 0.15f);
//         }

//         private void OnEndDeath()

//     }
// }