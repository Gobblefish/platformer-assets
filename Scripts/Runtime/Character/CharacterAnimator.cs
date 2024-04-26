// TODO: Clean
/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish;
using Gobblefish.Audio;

namespace Platformer.Character {

    ///<summary>
    /// Animates the character.
    ///<summary>
    // [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterAnimator : MonoBehaviour {

        public enum AnimationPriority {
            // Default.
            DefaultIdle,
            DefaultMoving,
            DefaultJumpRising,
            DefaultJumpFalling,
            // Action Passive.
            ActionPassiveIdle,
            ActionPassiveMoving,
            ActionPassiveRising,
            ActionPassiveFalling,
            // Action Active
            ActionPreActive,
            ActionActive,
            ActionPostActive,
            Count,
        }

        protected CharacterController m_Character = null;

        // [SerializeField]
        // private AudioVisualEffectCollection m_AudioVisualEffectCollection;

        [SerializeField]
        private AudioSnippetCollection m_AudioSnippets;

        // Runs once before the first frame.
        protected virtual void Start() {
            m_Character = transform.parent.GetComponent<CharacterController>();
        }

        // Runs once every frame.
        void Update() {
            Animate(Time.deltaTime);
            Rotate();
        }

        // Animates the flipbook by setting the animation, frame, and playing any effects.
        protected virtual void Animate(float dt) {
            
        }

        private void Rotate() {
            float currentAngle = transform.eulerAngles.y;
            float angle = m_Character.FacingDirection < 0f ? 180f : m_Character.FacingDirection > 0f ? 0f : currentAngle;
            if (transform.eulerAngles.y != angle) {
                transform.eulerAngles = angle * Vector3.up;
            }
        }

        public virtual void PlayAnimation(string name, AnimationPriority priority) {
            
        }

        public virtual void StopAnimation(AnimationPriority priority) {
            
        }

        public virtual void PlayAnimation(string name, float speed) {
            
        }

        public virtual void PlayAnimation(string name) {
            
        }

        public virtual void StopAnimation(string name) {
            
        }

        public void PlayEffect(string name) {

        }

        public void StopEffect(string name) {

        }

        public virtual void SetPowerIndicator(Platformer.Entities.Components.Power power) {

        }

        public void PlayAudioVisualEffect(string name) {
            m_AudioSnippets.Play(name);
        }

        public void StopAudioVisualEffect(string name) {
            m_AudioSnippets.Stop(name);
        }

        public void PlayAudioVisualEffect(VisualEffect visualEffect, AudioSnippet audioSnippet) {
            if (visualEffect != null) {
                visualEffect.Play();
            }
            if (audioSnippet != null && audioSnippet.clip != null) {
                audioSnippet.Play();
            }
        }

    }

}


