// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
//
using Gobblefish.Animation;
// Platformer.
using Platformer.Physics;

namespace Platformer.Character {

    ///<summary>
    /// An ability that near-instantly moves the character.
    ///<summary>
    [CreateAssetMenu(fileName="StickyAction", menuName ="Actions/Sticky")]
    public class StickyAction : CharacterAction {

        // The threshold speed for ending a wall jump.
        private const float WALLJUMP_SPEED_THRESHOLD = 0.8f;

        // The speed with which this moves while climbing.
        [SerializeField]
        private float m_ClimbSpeed = 6f;

        // The speed with which this accelerates while climbing.
        [SerializeField]
        private float m_ClimbAcceleration = 30f;

        // The speed at which this shoots of walls.
        [SerializeField]
        private float m_WallJumpSpeed = 12f;

        // The speed at which you can wiggle while wall jumping.
        [SerializeField]
        private float m_WallJumpWiggleSpeed = 5f;

        // Whether the character can currently climb a wall.
        [SerializeField, ReadOnly]
        private bool m_CanClimb = false;

        // Tracks how long the player has been climbing for.
        [SerializeField]
        private Timer m_ClimbTimer = new Timer(0f, 0f);

        // The duration for which the character can climb for.
        [SerializeField]
        private float m_ClimbDuration = 4f;

        // The duration after ending climb to re-adjust.
        [SerializeField]
        private float m_EndClimbBuffer = 0.08f;

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            // Game.Visuals.Effects.StopEffect(m_CircleEffectIndex);

            if (m_ClimbTimer.Active || m_ActionPhase != ActionPhase.None) {
                OnEndPostClimb(character);
            }

        }

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            if (character.Input.Actions[1].Held && m_ActionPhase == ActionPhase.None && m_CanClimb) {
                // The character should start dashing.
                OnStartClimb(character);

                // Release the input and reset the refresh.
                m_Refreshed = false;
            }

            // Jumping.
            if (character.Input.Actions[0].Pressed && (m_ActionPhase == ActionPhase.PreAction || m_ActionPhase == ActionPhase.PostAction)) {
                // The character should jump.
                OnWallJump(character);

                // Release the input and reset the refresh.
                character.Input.Actions[0].ClearPressBuffer();
            }
        }
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && !m_ClimbTimer.Active ? true : m_Refreshed;
            m_CanClimb = character.FacingWall && m_Refreshed;

            // Tick down the climb timer.
            bool finished = m_ClimbTimer.TickDown(dt);

            // If swapping states.
            if (finished) { 

                switch (m_ActionPhase) {
                    case ActionPhase.PreAction:
                        OnEndClimb(character);
                        break;
                    case ActionPhase.PostAction:
                        OnEndPostClimb(character);
                        break;
                    default:
                        break;
                }

            }

            // If in a phase.
            switch (m_ActionPhase) {
                case ActionPhase.PreAction:
                    WhileClimbing(character, dt);
                    break;
                case ActionPhase.MidAction:
                    WhileWallJumping(character, dt);
                    break;
                default:
                    break;
            }
            
        }

        private void OnStartClimb(CharacterController character) {
            // If coming from a wall jump.
            character.LockDirection(false);

            // Set the action phase and timer.
            // Pre action is climbing, actual action is wall jumping.
            m_ActionPhase = ActionPhase.PreAction;
            m_ClimbTimer.Start(m_ClimbDuration);

            // Disable the body and adjust it.
            character.Default.Enable(character, false);
            character.Body.SetWeight(0f);
            character.Body.SetVelocity(Vector2.zero);

        }

        private void OnEndClimb(CharacterController character) {
            // Set the action phase.
            // Give a lil post action time before defaulting back to normal movement.
            // This time can be used to activate a wall jump.
            m_ActionPhase = ActionPhase.PostAction;
            m_ClimbTimer.Start(m_EndClimbBuffer);

        }

        private void OnEndPostClimb(CharacterController character) {
            // Reset back to the default.
            m_ActionPhase = ActionPhase.None;
            character.Default.Enable(character, true);
            m_ClimbTimer.Stop();

            // If coming from a wall jump.
            character.LockDirection(false);
        }

        // Shoots the player off the wall.
        private void OnWallJump(CharacterController character) {
            // Adjust the bodies velocity.
            float direction = character.FacingWall ? -character.FacingDirection : character.FacingDirection;
            character.Body.velocity = m_WallJumpSpeed * direction * Vector2.right;
            character.LockDirection(false, direction);

            // Set the action phase and pause the climb timer. 
            m_ActionPhase = ActionPhase.MidAction;
            m_ClimbTimer.Stop();

        }

        // When ending a wall jump.
        private void OnEndWallJump(CharacterController character) {
            OnEndPostClimb(character);
        }

        // Process the physics of this action.
        private void WhileClimbing(CharacterController character, float dt) {
            // End the climb early if the character is not facing the wall.
            // Should the player have to hold the key to keep wall climbing.
            if (!character.FacingWall || !character.Input.Actions[1].Held) {
                OnEndClimb(character);
                return;
            }

            // Cache the target and current velocities.
            float targetSpeed = character.Input.Direction.Vertical * m_ClimbSpeed;
            float currentSpeed = character.Body.velocity.y;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * m_ClimbAcceleration;

            // Calculate the precision of the change.
            Vector2 velocity = new Vector2(0f, currentSpeed + deltaSpeed);
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                velocity = new Vector2(0f, targetSpeed);
            }

            // Set the velocity.
            character.Body.SetVelocity(velocity);

        }

        private void WhileWallJumping(CharacterController character, float dt) {
            // Check if we hit something we can restick to.
            if (character.FacingWall) {
                OnStartClimb(character);
            }
            // Otherwise check something potentially stopping us.
            else {
                bool somethingSlowedUsDown = Mathf.Abs(character.Body.velocity.x) < WALLJUMP_SPEED_THRESHOLD * m_WallJumpSpeed;
                bool turnedTheOtherDirection = character.FacingDirection != Mathf.Sign(character.Body.velocity.x);
                // bool letGoJump = !character.Input.Actions[0].Held;
                bool letGoAction = !character.Input.Actions[1].Held;
                if (somethingSlowedUsDown || letGoAction || turnedTheOtherDirection) {
                    OnEndWallJump(character);
                }
            }

            // Cache the target and current velocities.
            float targetSpeed = character.Input.Direction.Vertical * m_WallJumpWiggleSpeed;
            float currentSpeed = character.Body.velocity.y;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * 100f;

            // Calculate the precision of the change.
            Vector2 velocity = new Vector2(character.Body.velocity.x, currentSpeed + deltaSpeed);
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                velocity = new Vector2(character.Body.velocity.x, targetSpeed);
            }

            // Set the velocity.
            character.Body.SetVelocity(velocity);

        }

    }
}