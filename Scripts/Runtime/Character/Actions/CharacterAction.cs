/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Platformer.
using Gobblefish.Input;
using Platformer.Physics;

namespace Platformer.Character {

    ///<summary>
    /// An abstract class defining the functionality of a
    /// character's action. 
    ///<summary>
    [System.Serializable]
    public abstract class CharacterAction : ScriptableObject {

        #region Enumerations

        public enum ActionPhase {
            None,
            PreAction,
            MidAction,
            PostAction
        }

        #endregion

        #region Fields
        
        /* --- Member Variables --- */

        // Checks whether the activation conditions have been fulfilled.
        [SerializeField, ReadOnly] 
        protected ActionPhase m_ActionPhase = ActionPhase.None;
        
        // Checks whether the activation conditions have been fulfilled.
        [SerializeField, ReadOnly] 
        protected bool m_Refreshed = false;

        // Checks whether this ability is enabled.
        [SerializeField] 
        protected bool m_Enabled = false;
        public bool Enabled => m_Enabled;

        // Runs through the phases of the dash cycle.
        protected Timer m_ActionTimer = new Timer(0f, 0f);

        #endregion

        #region Methods
    
        // When this ability is activated.
        public abstract void InputUpdate(CharacterController character);
        
        // Refreshes the settings for this ability every interval.
        public virtual void PhysicsUpdate(CharacterController character, float dt) {
            // Tick down the dash timer.
            bool finished = m_ActionTimer.TickDown(dt);

            // If swapping states.
            if (finished) { 

                switch (m_ActionPhase) {
                    case ActionPhase.MidAction:
                        OnStartPostaction(character);
                        break;
                    case ActionPhase.PostAction:
                        OnEndAction(character);
                        break;
                    default:
                        break;
                }

            }

            // If in a phase.
            switch (m_ActionPhase) {
                case ActionPhase.PreAction:
                    WhilePreaction(character, dt);
                    break;
                case ActionPhase.MidAction:
                    WhileAction(character, dt);
                    break;
                case ActionPhase.PostAction:
                    WhilePostaction(character, dt);
                    break;
                default:
                    break;
            }

        }

        // Enables the action.
        public virtual void Enable(CharacterController character, bool enable = true) {
            m_Enabled = enable;
            m_Refreshed = enable;
            OnEndAction(character);
        }

        protected virtual void OnStartPreaction(CharacterController character) {
            m_ActionPhase = ActionPhase.PreAction;
        }

        protected virtual void OnStartAction(CharacterController character) {
            m_ActionPhase = ActionPhase.MidAction;
            OnEndPreaction(character);
        }

        protected virtual void OnStartPostaction(CharacterController character) {
            m_ActionPhase = ActionPhase.PostAction;
            character.Default.Enable(character, true);

            //
            character.Animator.SetPowerIndicator(null);
        }

        protected virtual void OnEndPreaction(CharacterController character) {
            m_ActionPhase = ActionPhase.None;
            character.Default.Enable(character, true);
        }

        protected virtual void OnEndAction(CharacterController character) {
            m_ActionPhase = ActionPhase.None;
            if (m_ActionTimer.Active) { m_ActionTimer.Stop(); }
            character.Default.Enable(character, true);
        }

        protected virtual void WhilePreaction(CharacterController character, float dt) {
            
        }

        protected virtual void WhileAction(CharacterController character, float dt) {
            
        }

        protected virtual void WhilePostaction(CharacterController character, float dt) {

        }

        #endregion

    }

}

