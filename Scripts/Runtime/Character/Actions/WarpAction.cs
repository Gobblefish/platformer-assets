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
    [CreateAssetMenu(fileName="WarpAction", menuName ="Actions/Warp")]
    public class WarpAction : CharacterAction {

        /* --- */
        public const float CHARGE_INCREMENT = 0.1f;
        public const float CHARGE_WEIGHT = 0.05f;
        public const float MIN_CHARGE_VALUE = 0.2f;
        public float m_ChargeDuration = 0.5f;
        private Timer m_ChargeTimer = new Timer(0f, 0f);
        private Timer m_ChargeIncrementTimer = new Timer(0f, 0f);
        /* --- */

        // The speed of the actual warp.
        [SerializeField] 
        private float m_WarpDistance = 2f;

        // The direction the player was facing before the warp started.
        [HideInInspector]
        protected Vector2 m_CachedDirection = new Vector2(0f, 0f);

        [SerializeField]
        private Sprite m_WarpIndicatorSprite = null;

        private Transform m_WarpIndicator = null;

        public float CYCLE_DURATION = 2f;

        public float m_PostwarpWeight;

        public float m_PostwarpDuration;

        public float m_PostwarpSpeed;

        private Timer m_WarpTimer = new Timer(0f, 0f);

        public bool option0;
        public bool option1;
        public bool option2;

        Transform moving;
        Transform stationary;
        float offset = 0f;

        // When enabling/disabling this ability.
        public override void Enable(CharacterController character, bool enable = true) {
            base.Enable(character, enable);
            m_ActionPhase = ActionPhase.None;
            OnEndAction(character);

            if (!enable) {
                character.Default.Enable(character, true);
            }

            if (enable && m_WarpIndicator == null) {
                m_WarpIndicator = new GameObject("warp indicator", typeof(SpriteRenderer)).transform;
                m_WarpIndicator.GetComponent<SpriteRenderer>().sprite = m_WarpIndicatorSprite;
                m_WarpIndicator.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
                m_WarpIndicator.GetComponent<SpriteRenderer>().sortingOrder = 1;
                // m_WarpIndicator.transform.SetParent(character.transform);

                m_WarpIndicator.position = character.transform.position + Vector3.up * m_WarpDistance;
                m_WarpIndicator.localScale *= 0.4f;
                rotationDir = (m_WarpIndicator.position - character.transform.position).normalized;
                
            }

            if (enable) {
                m_WarpIndicator.gameObject.SetActive(true);

                moving = m_WarpIndicator;
                stationary = character.transform;
                
                if (option1) {
                    character.Default.Enable(character, false);
                    character.Body.SetWeight(0f);
                    character.Body.SetVelocity(Vector2.zero);
                }
            }

        }

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            if (character.Input.Actions[1].Pressed && m_ActionPhase == ActionPhase.None) {
                if (option0) { 
                    OnStartAction(character); 
                }
                else {
                    if (moving == m_WarpIndicator) { 
                        if (option2) {
                            character.Default.Enable(character, false);
                            character.Body.velocity *= 0f;
                            character.Body.isKinematic = true;
                            character.Body.SetWeight(0f);
                        }
                        Swap(); 
                    }
                    else {
                        if (option1) { 
                            Swap(); 
                        }
                        else if (option2) { 
                            Throw(character); 
                        }
                    }
                }

                character.Input.Actions[1].ClearPressBuffer();
                m_Refreshed = false;
            }

        }
        
        // Refreshes the settings for this ability every interval.
        public override void PhysicsUpdate(CharacterController character, float dt){
            if (!m_Enabled) { return; }

            // Whether the power has been reset by touching ground after using it.
            m_Refreshed = character.OnGround && m_ActionPhase == ActionPhase.None ? true : m_Refreshed;

            //
            bool finished = m_WarpTimer.TickDown(dt) || !m_WarpTimer.Active;

            if (finished) {
                switch (m_ActionPhase) {
                    case ActionPhase.PostAction:
                        OnEndAction(character);
                        break;
                    default:
                        break;
                }
            }

            // Charge the hop.
            bool cycleFin = m_ChargeTimer.TickDown(dt) || !m_ChargeTimer.Active;
            
            // For constant cycling.
            if (cycleFin) {
                m_ChargeTimer.Start(CYCLE_DURATION);
            }

            Rotate(dt, character);

            if (option0) {
                m_WarpIndicator.position = character.transform.position + (m_WarpDistance * rotationDir);
            }
            else if (option1) {
                moving.position = stationary.position + m_WarpDistance * rotationDir;
            }
            else if (option2) {
                if (m_ActionPhase == ActionPhase.None) {
                    moving.position = stationary.position + m_WarpDistance * rotationDir;
                }
                
            }

            if (moving == m_WarpIndicator) {
                m_WarpIndicator.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, rotationDir);
            }
            else {
                m_WarpIndicator.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, -rotationDir);
            }

        }

        protected override void OnStartAction(CharacterController character) {
            base.OnStartAction(character);

            Vector3 dir = (m_WarpIndicator.position - character.transform.position).normalized;

            character.transform.position += (m_WarpDistance * dir);

            character.Body.velocity = (m_PostwarpSpeed * rotationDir);
            character.Body.SetWeight(m_PostwarpWeight);
            character.Default.Enable(character, false);

            m_WarpTimer.Start(m_PostwarpDuration);
            
            OnStartPostaction(character);
        }

        protected override void OnStartPostaction(CharacterController character) {
            base.OnStartPostaction(character);
        }

        protected override void OnEndAction(CharacterController character) {
            base.OnEndAction(character);
            character.Animator.StopAnimation("OnPostwarp");
            character.Body.isKinematic = false;
            m_WarpIndicator.gameObject.SetActive(false);
        }

        /* ---- */


        private Vector3 rotationDir; 
        public void Rotate(float dt, CharacterController character) {
            float dir = -1f;
            if (option2 && bigThrow) {
                dir = -character.FacingDirection;
            }

            rotationDir = Quaternion.Euler(0f, 0f, dir * dt / CYCLE_DURATION * 360f) * rotationDir;
            rotationDir = rotationDir.normalized;
        }

        
        public void Swap() {

            Transform tmp = moving;
            moving = stationary;
            stationary = tmp;

            rotationDir = (moving.position - stationary.position).normalized;

            offset = Vector2.SignedAngle(Vector2.right, moving.position - stationary.position);

        }

        public bool bigThrow = false;

        public float bigThrowDuration = 10f;
        public float bigThrowSpeed = 10f;
        public float bigThrowWeight = 5f;

        public void Throw(CharacterController character) {

            character.Body.isKinematic = false;
            
            
            character.Body.velocity = Quaternion.Euler(0f, 0f, -90f) * (m_PostwarpSpeed * rotationDir);
            character.Body.SetWeight(0f);

            if (bigThrow) {

                float dir = -character.FacingDirection;

                character.Body.velocity = Quaternion.Euler(0f, 0f, dir * 90f) * (bigThrowSpeed * rotationDir);
                character.Body.SetWeight(bigThrowWeight);
                m_WarpTimer.Start(bigThrowDuration);
            }

            character.Default.Enable(character, false);

            m_WarpTimer.Start(m_PostwarpDuration);
            m_ActionPhase = ActionPhase.PostAction;
            Swap();

        }

        

    }
}