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
    public class ChargeAction : CharacterAction {

        // The increment with which to notify charge.
        public const float CHARGE_INCREMENT = 0.1f;

        //
        public const float CHARGE_WEIGHT = 0.05f;

        //
        public const float MIN_CHARGE_VALUE = 0.2f;

        // The time it takes to fully charge.
        [SerializeField]
        private float m_ChargeDuration = 0.5f;

        // The timer that tracks how much has been charged.
        [SerializeField]
        private Timer m_ChargeTimer = new Timer(0f, 0f);

        // The tracks the increments of charge.
        [SerializeField]
        private Timer m_ChargeIncrementTimer = new Timer(0f, 0f);

        public float ChargeValue => Mathf.Max(MIN_CHARGE_VALUE, Mathf.Sqrt(m_ChargeTimer.InverseRatio));

        [SerializeField]
        private AudioSnippet m_ChargeSound;

        [SerializeField]
        private VisualEffect m_ChargeEffect;

        // When this ability is activated.
        public override void InputUpdate(CharacterController character) {
            if (!m_Enabled) { return; }

            // The character should start charging.
            if ((character.Input.Actions[1].Held || character.Input.Actions[1].Pressed) && m_ActionPhase == ActionPhase.None && m_Refreshed) {
                OnStartPreaction(character);
                character.Input.Actions[1].ClearPressBuffer();
                m_Refreshed = false;
            }

            // The character should start the action.
            if (character.Input.Actions[1].Released && m_ActionPhase == ActionPhase.PreAction) {
                OnStartAction(character);
                character.Input.Actions[1].ClearReleaseBuffer();
                m_Refreshed = false;
            }

        }
        
        // Start the character charging.
        protected override void OnStartPreaction(CharacterController character) {
            // Disable default inputs.
            character.Default.Enable(character, false);
            character.Body.Stop();
            character.Body.SetWeight(CHARGE_WEIGHT);

            // Start the timers.
            m_ChargeTimer.Start(m_ChargeDuration);
            m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);
            
            // Set the action phase.
            m_ActionPhase = ActionPhase.PreAction;
            character.Animator.PlayAnimation("OnStartCharge");
            character.Animator.PlayAudioVisualEffect("WhileCharging");
        }

        protected override void OnEndPreaction(CharacterController character) {
            // Stop the charge timers.
            // m_ChargeTimer.Stop();
            m_ChargeIncrementTimer.Stop();
            character.Animator.StopAnimation("OnStartCharge");
            character.Animator.StopAudioVisualEffect("WhileCharging");
        }

        protected override void WhilePreaction(CharacterController character, float dt) {
            character.Body.ClampRiseSpeed(0f);
            bool chargeIncremented = m_ChargeIncrementTimer.TickDown(dt);

            m_ChargeTimer.TickDown(dt);
            character.Animator.PlayAnimation("OnStartCharge", 1f + 5f * ChargeValue);

            if (chargeIncremented && m_ChargeTimer.InverseRatio < 1f) {
                character.Animator.PlayAudioVisualEffect("WhileCharging");
                m_ChargeIncrementTimer.Start(CHARGE_INCREMENT);
            }
        }

    }
}