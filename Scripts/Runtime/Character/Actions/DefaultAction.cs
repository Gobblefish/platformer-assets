/* --- Libraries --- */
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
    /// The default ability for controlling the character.
    ///<summary>
    [CreateAssetMenu(fileName="DefaultAction", menuName ="Actions/Default")]
    public class DefaultAction : CharacterAction {

        #region Variables

        /* --- Constant Variables --- */

        // The factor that slows down rising if not holding the input.
        public const float RISE_FRICTION = 0.1f;

        // The friction applied to a body that is still under coyote time.
        public const float COYOTE_FRICTION = 0.5f;

        // The maximum speed with which this character can fall.
        public const float FAST_FALL_SPEED_THRESHOLD = 7f;

        // The maximum speed with which this character can fall.
        public const float FAST_FALL_DIST_THRESHOLD = 3f;

        // The maximum speed with which this character can fall.
        public const float MAX_FALL_SPEED = 25f;

        // The maximum speed with which this character can fall.
        public const float REST_IDLE = 1.5f;

        // The maximum speed with which this character can fall.
        public const float SITTING_IDLE = 4f;

        /* --- Member Variables --- */

        [SerializeField, ReadOnly]
        private bool m_Interacting = false;

        // Whether the character is using the default movement.
        [SerializeField, ReadOnly] 
        public bool m_MoveEnabled = true;

        // Whether the character is using the default falling.
        [SerializeField, ReadOnly] 
        public bool m_FallEnabled = true;

        // Whether the character is using the default falling.
        [SerializeField, ReadOnly] 
        public bool m_DuckEnabled = true;

        // Forcefully clamp internal jumps, usually for prepping external jumps.
        [SerializeField, ReadOnly] 
        public bool m_ClampJump = false;

        // The default speed the character moves at.
        [SerializeField] 
        private float m_Speed = 7.5f;
        public float Speed => m_Speed;

        // The default acceleration of the character.
        [SerializeField] 
        private float m_Acceleration = 75f;

        // The default jump height of the character.
        [SerializeField] 
        private float m_Height = 3.5f;

        // The default time taken to reach the m_HangTimer of the jump.
        [SerializeField] 
        private float m_RisingTime = 0.45f;

        // The default time taken to fall from the m_HangTimer of the jump.
        [SerializeField] 
        private float m_FallingTime = 0.4f;

        // The amount of time before noticing we're falling.
        [SerializeField] 
        protected float m_CoyoteBuffer = 0.12f;

        // The amount of hang time.
        [SerializeField] 
        protected float m_HangBuffer = 0.04f;

        // Holds the player at the apex of their jump for a brief moment longer.
        [SerializeField] 
        protected float m_HangFactor = 0.24f;

        // The calculated jump speed based on the height, rising and falling time.
        [SerializeField, ReadOnly] 
        private float m_JumpSpeed = 0f;
        public float JumpSpeed => m_JumpSpeed;

        // The calculated m_Weight based on the height, rising and falling time.
        [SerializeField, ReadOnly] 
        private float m_Weight = 0f;

        // The calculated sink factor based on the height, rising and falling time.
        // The sink is the factor applied to the m_Weight when falling.
        [SerializeField, ReadOnly] 
        private float m_Sink = 0f;

        // Tracks how long the character has not been on the ground.
        [HideInInspector] 
        private Timer m_CoyoteTimer = new Timer(0f, 0f);

        // Tracks how long its been since the character reached the m_HangTimer.
        [HideInInspector] 
        private Timer m_HangTimer = new Timer(0f, 0f);

        // Tracks how long its been since the character reached the m_HangTimer.
        [HideInInspector] 
        private float m_IdleTicks = 0f;

        // Tracks how long its been since the character reached the m_HangTimer.
        [HideInInspector] 
        private float m_FootstepTicks = 0f;
        public float FootstepInterval = 0.3f;

        //
        [SerializeField]
        private Vector2 m_Direction = new Vector2(0f, 0f);

        #endregion

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            m_Enabled = enable;
            m_Refreshed = false;
            m_HangTimer = new Timer(0f, m_HangBuffer);
            m_CoyoteTimer = new Timer(0f, m_CoyoteBuffer);
            RefreshJumpSettings(ref m_JumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            m_MoveEnabled = true;
            m_FallEnabled = true;
            m_Interacting = false;
            m_ClampJump = false;
            m_Refreshed = false;

            character.Animator.PlayAnimation("Idle");
            character.Animator.StopAnimation("Moving");
            character.Animator.StopAnimation("Rising");
            character.Animator.StopAnimation("Falling");
        }

        // When enabling/disabling this ability by movement and falling seperately.
        public void Enable(CharacterController character, bool move, bool fall) {
            Enable(character, true);

            m_MoveEnabled = move;
            m_FallEnabled = fall;
        
        }

        // Runs once every frame to check the inputs for this ability.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) {  return;  }

            // Current.
            if (character.CurrentInteractable == null) {
                m_Interacting = false;
            }

            m_Direction = new Vector2(character.Input.Direction.Horizontal, character.Input.Direction.Vertical);

            // Jumping.
            if (character.Input.Actions[0].Pressed && m_Refreshed) {
                // If interacting with an NPC.
                if (character.CurrentInteractable != null) {
                    if (!m_Interacting) {
                        OnInteract(character);
                    }
                    else {
                        WhileInteracting(character);
                    }
                    character.Input.Actions[0].ClearPressBuffer();
                    return;
                }

                // The character should jump.
                OnJump(character);

                // Release the input and reset the refresh.
                character.Input.Actions[0].ClearPressBuffer();
                m_CoyoteTimer.Stop();
                m_Refreshed = false;
            }
            
        }

        // 
        public override void PhysicsUpdate(CharacterController character, float dt) {
            // Landing.
            if (character.OnGround && !m_Refreshed) {
                OnLand(character);
            }

            // Refreshing.
            m_Refreshed = character.OnGround || m_CoyoteTimer.Value > 0f;

            GetDefaultState(character, dt);
            if (!m_Enabled) { return; }

            // Tick the m_CoyoteTimer timer.
            m_CoyoteTimer.TickDownIfElseReset(dt, !character.OnGround);
            if (m_MoveEnabled) { 
                WhileMoving(character, dt); 
            }
            if (m_FallEnabled) { 
                WhileFalling(character, dt); 
            }
            if (m_DuckEnabled) {
                WhileDucking(character, dt);
            }
            
        }

        private void GetDefaultState(CharacterController character, float dt) {

            if (!character.OnGround) {
                if (character.Rising) {
                    character.Animator.PlayAnimation("Rising");
                }
                else {

                    Vector2 footPosition = character.Body.position + character.Collider.offset + Vector2.down * (character.Collider.radius + 0.1f);
                    float dist = PhysicsManager.Collisions.DistanceToFirst(footPosition, Vector3.down, PhysicsManager.CollisionLayers.Solid);

                    if (m_HangTimer.Active) {
                        character.Animator.PlayAnimation("Hanging");
                    }
                    else if (Mathf.Abs(character.Body.velocity.y) > FAST_FALL_SPEED_THRESHOLD && dist > FAST_FALL_DIST_THRESHOLD) {
                        character.Animator.PlayAnimation("FallingFast");
                    }
                    else {
                        character.Animator.PlayAnimation("Falling");
                    }

                }
                return;
            }

            character.Animator.StopAnimation("Rising");
            character.Animator.StopAnimation("Falling");
            character.Animator.StopAnimation("Hanging");
            character.Animator.StopAnimation("FallingFast");

            if (character.Input.Direction.Horizontal != 0f && !character.Disabled) {
                character.Animator.PlayAnimation("Moving");

                if (m_FootstepTicks == -1f) {
                    character.Animator.PlayAudioVisualEffect("Footstep"+Random.Range(1,4).ToString());
                    m_FootstepTicks = 0f;
                }

                m_FootstepTicks += dt;
                if (m_FootstepTicks > FootstepInterval) {
                    character.Animator.PlayAudioVisualEffect("Footstep"+Random.Range(1,4).ToString());
                    m_FootstepTicks -= FootstepInterval;
                }
                m_IdleTicks = 0f;
            }
            else {
                character.Animator.StopAnimation("Moving");

                m_IdleTicks += character.Body.velocity.sqrMagnitude < 0.1f ? dt : 0f;
                m_FootstepTicks = -1f;
                if (m_IdleTicks > SITTING_IDLE) {
                    character.Animator.PlayAnimation("Idle Sit");
                }
                else if (m_IdleTicks > REST_IDLE) {
                    character.Animator.PlayAnimation("Idle Rest");
                }
                else {
                    character.Animator.PlayAnimation("Idle");
                }

            }
        }

        //
        private void OnInteract(CharacterController character) {
            m_Interacting = character.CurrentInteractable.StartInteraction();
        }

        //
        private void WhileInteracting(CharacterController character) {
            m_Interacting = character.CurrentInteractable.ContinueInteraction();
        }

        private void OnJump(CharacterController character) {
            // Refresh the jump settings.
            if (m_ClampJump) { return; }

            RefreshJumpSettings(ref m_JumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            // Jumping.
            character.Body.Move(Vector2.up * 2f * PhysicsManager.Settings.collisionPrecision);
            // These two lines are the key!!!
            character.Body.ClampFallSpeed(0f); 
            character.Body.AddVelocity(Vector2.up * m_JumpSpeed);
            
            // The effect.
            character.Animator.PlayAnimation("Jump");
            character.Animator.PlayAudioVisualEffect("OnJump");
            character.Animator.PlayAudioVisualEffect("WhileJumping");

        }

        public void ClampJump(bool clamp) {
            m_ClampJump = clamp;
        }

        public void OnExternalJump(CharacterController character, float jumpSpeed) {
            // Refresh the jump settings.
            // RefreshJumpSettings(ref jumpSpeed, ref m_Weight, ref m_Sink, m_Height, m_RisingTime, m_FallingTime);

            // Jumping.
            character.Body.Move(Vector2.up * 2f * PhysicsManager.Settings.collisionPrecision);
            // These two lines are the key!!!
            character.Body.ClampFallSpeed(0f); 
            character.Body.SetVelocity(Vector2.up * jumpSpeed);

            character.Input.Actions[0].ClearPressBuffer();
            m_CoyoteTimer.Stop();
            m_Refreshed = false;

        }

        private void OnLand(CharacterController character) {
            m_ClampJump = false;

            character.Animator.PlayAnimation("OnLand");
            character.Animator.PlayAudioVisualEffect("OnLand");

            character.Animator.StopAnimation("Rising");
            character.Animator.StopAnimation("Hanging");
            character.Animator.StopAnimation("Falling");
            character.Animator.StopAnimation("FallingFast");
            // if (character.Trail != null) { character.Trail.Stop(); }
        }

        // Process the physics of this action.
        private void WhileMoving(CharacterController character, float dt) {
            // Cache the target and current velocities.
            float targetSpeed = m_Direction.x * m_Speed;
            float currentSpeed = character.Body.velocity.x;

            // Calculate the change in velocity this frame.
            float unitSpeed = Mathf.Sign(targetSpeed - currentSpeed);
            float deltaSpeed = unitSpeed * dt * m_Acceleration;

            // Calculate the precision of the change.
            Vector2 velocity = new Vector2(currentSpeed + deltaSpeed, character.Body.velocity.y);
            if (Mathf.Abs(targetSpeed - currentSpeed) < Mathf.Abs(deltaSpeed)) {
                velocity = new Vector2(targetSpeed, character.Body.velocity.y);
            }

            // Set the velocity.
            character.Body.SetVelocity(velocity);
            // m_Direction = Vector2.zero;
            
        }

        public void SetIdleTicks(float ticks) {
            m_IdleTicks = ticks;
        }

        // Not really falling, but rather "while default grav acting on this body"
        private void WhileFalling(CharacterController character, float dt) {
            // Set the m_Weight to the default.
            float weight = m_Weight;

            // If the body is not on the ground.
            if (!character.OnGround) { 
                // And it is rising.
                if (character.Rising) {

                    // Multiply it by its m_Weight.
                    weight = m_Weight;
                    m_HangTimer.Start();

                    // If not holding jump, then rapidly slow the rising body.
                    if (!character.Input.Actions[0].Held) {
                        character.Body.SetVelocity(new Vector2(character.Body.velocity.x, character.Body.velocity.y * (1f - RISE_FRICTION)));
                        character.Animator.StopAudioVisualEffect("WhileJumping");
                    }
                    
                }
                else {
                    // If it is falling, also multiply the sink m_Weight.
                    weight = (m_Weight * m_Sink);

                    // If it is still at its m_HangTimer, factor this in.
                    if (m_HangTimer.Active) {
                        weight *= m_HangFactor;
                        m_HangTimer.TickDown(dt);
                    }

                    // If the m_CoyoteTimer timer is still ticking, fall slower.
                    if (m_CoyoteTimer.Active) {
                        weight *= COYOTE_FRICTION;
                    }

                }

            }
            
            // Set the m_Weight.
            character.Body.SetWeight(weight);

            // Clamp the fall speed at a given value.
            character.Body.ClampFallSpeed(MAX_FALL_SPEED);
        }

        // While ducking.
        private void WhileDucking(CharacterController character, float dt) {

            int characterLayer = LayerMask.NameToLayer("Characters");
            if (character.Input.Direction.Vertical == -1f && character.gameObject.layer == characterLayer) {
                character.gameObject.layer = LayerMask.NameToLayer("Ignore Character");
            }
            else if (character.Input.Direction.Vertical != -1f && character.gameObject.layer != characterLayer) {
                character.gameObject.layer = characterLayer;
            }

        }

        // Calculates the speed and m_Weight of the jump.
        public static void RefreshJumpSettings(ref float v, ref float w, ref float s, float h, float t_r, float t_f) {
            v = 2f * h / t_r;
            w = 2f * h / (t_r * t_r) / Mathf.Abs(UnityEngine.Physics2D.gravity.y);
            s = (t_f * t_f) * w * Mathf.Abs(UnityEngine.Physics2D.gravity.y) / (2f * h);
            s = 1f / s;
        }

    }

}

