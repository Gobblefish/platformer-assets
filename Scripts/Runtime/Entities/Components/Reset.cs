/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
// Platformer.
using Platformer.Entities;

/* --- Definitions --- */
using IReset = Platformer.Entities.Utility.IReset;
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Utility {

    ///<summary>
    /// 
    ///<summary>
    [RequireComponent(typeof(Entity))]
    public class Reset : MonoBehaviour {

        #region Variables.

        /* --- Constants --- */

        // The amount of time before it starts blinking.
        private const float RESET_BLINK_DELAY = 0.7f;
        
        // The amount of times the orb blinks before it reappears.
        protected const float RESET_BLINK_COUNT = 3;

        // [SerializeField]
        // private VisualEffect m_BurstParticle;

        // [SerializeField]
        // private VisualEffect m_EmissionParticle;

        /* --- Parameters --- */

        [HideInInspector]
        private Entity m_Entity;

        // The amount of time before the orb naturally respawns.
        [SerializeField]
        private float m_ResetDelay = 1.5f;

        // The sound that plays when this orb is reset.
        [SerializeField]
        private AudioClip m_OnResetSound;

        // The sound that plays when this orb blinks.
        [SerializeField]
        protected AudioClip m_OnBlinkSound;

        [SerializeField]
        private UnityEvent m_OnResetEvent = new UnityEvent();

        #endregion

        // Runs once before the first frame.
        void Awake() {
            m_Entity = GetComponent<Entity>();
            // if (m_EmissionParticle != null) {
            //     m_EmissionParticle.Play();
            // }
        }

        public void Refresh() {
            if (m_ResetDelay > 0f) {
                // Disable the orb for a bit.
                // if (m_EmissionParticle != null) {
                //     m_EmissionParticle.Stop();
                // }

                m_Entity.EnableColliders(false);
                m_Entity.Renderer.enabled = false;
                StartCoroutine(IEReset());
            }

        }

        // A coroutine to eventually reset this orb object.
        protected IEnumerator IEReset() {
            // Wait a little until this orb starts blinking back into existence.
            IReset[] resets = gameObject.GetComponents<IReset>();
            for (int i = 0; i < resets.Length; i++) {
                resets[i].OnStartResetting();
            }

            yield return new WaitForSeconds(GetPreBlinkTime());
            m_Entity.ResetPosition();

            // Break if the orb has been prematurely reset.
            // if (m_Entity.CollisionEnabled) {
            //     m_Entity.Renderer.enabled = true;
            //     yield break;
            // }

            // Blink the orb a couple of times.
            for (int i = 0; i < 2 * RESET_BLINK_COUNT; i++) {
                m_Entity.Renderer.enabled = !m_Entity.Renderer.enabled;
                // Game.Audio.Sounds.PlaySound(m_OnBlinkSound, 0.05f);

                if (m_Entity.CollisionEnabled) {
                    m_Entity.Renderer.enabled = true;
                    yield break;
                }

                yield return new WaitForSeconds(GetBlinkTime());
            }

            // Wait one more blink just because it feels more correct.
            yield return new WaitForSeconds(GetPostBlinkTime());
            HardReset();

        }

        // Resets the object to its default state.
        public void HardReset() {
            // Give the player feedback that this object has been reset.
            // Game.Audio.Sounds.PlaySound(m_OnResetSound, 0.15f);

            // if (m_EmissionParticle != null) {
            //     m_EmissionParticle.Play();
            // }
            
            IReset[] resets = gameObject.GetComponents<IReset>();
            for (int i = 0; i < resets.Length; i++) {
                resets[i].OnFinishResetting();
            }

            // Reset the hitbox and rendering components of this object.
            m_Entity.Renderer.enabled = true;
            m_Entity.EnableColliders(true);

            m_OnResetEvent.Invoke();
        }
        
        // Returns the time that this object takes per blink.
        protected virtual float GetBlinkTime() { return (m_ResetDelay - RESET_BLINK_DELAY) / (float)(2 * RESET_BLINK_COUNT); }

        // Returns the time in between blinking and resetting fully.
        protected virtual float GetPostBlinkTime() { return (m_ResetDelay - RESET_BLINK_DELAY) / (float)RESET_BLINK_COUNT; }

        // Returns the time before the object starts blinking back.
        protected virtual float GetPreBlinkTime() { return RESET_BLINK_DELAY; }

    }

}