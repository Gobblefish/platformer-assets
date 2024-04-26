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
using Gobblefish.Animation;

using UnityAnimator = UnityEngine.Animator;
using UnityAnimation = UnityEngine.AnimationClip;

namespace Platformer.Character {

    ///<summary>
    /// Animates the character.
    ///<summary>
    // [RequireComponent(typeof(SpriteRenderer))]
    public class SplineAnimator : CharacterAnimator {

        [System.Serializable]
        protected class AnimationItem {
            public string name;
            public AnimationPriority priority;
            public UnityAnimation animation;
            public float playbackSpeed = 1f;
            [HideInInspector] public float modifiedSpeed = 1f;
            public Vector2 playbackRange = new Vector2(0f, 1f);
            public bool loop = true;
            public bool removeOnEnd = false;

            public float speed => modifiedSpeed * playbackSpeed;
            public float minTicks => animation == null ? 0f : playbackRange.x * animation.length;
            public float maxTicks => animation == null ? 0f : playbackRange.y * animation.length;

            // public bool pauseAtEnd;
            // public VisualEffect visualEffect;
            // public AudioSnippet audioSnippet;
        }

        private UnityAnimator m_Animator = null;

        [SerializeField]
        private SpriteRenderer m_Shell;

        [SerializeField]
        private UnityAnimation m_ResetAnimation;

        [SerializeField]
        private List<AnimationItem> m_AnimationCollection = new List<AnimationItem>();

        private AnimationItem[] m_AnimationSheet;

        float ticks = 0f;
        AnimationItem prevAnim;

        // Runs once before the first frame.
        protected override void Start() {
            m_Animator = GetComponent<UnityAnimator>();
            m_Character = transform.parent.GetComponent<CharacterController>();
            m_AnimationSheet = new AnimationItem[(int)AnimationPriority.Count];
            defaultColor = m_Shell.color;
        }
        
        // Animates the flipbook by setting the animation, frame, and playing any effects.
        protected override void Animate(float dt) {
            // Animator.
            AnimationItem anim = GetHighestPriorityAnimation();
            // print(anim.name);

            if (anim == null) {
                return;
            }

            if (anim != prevAnim) {
                ticks = anim.minTicks;
                prevAnim = anim;
                m_ResetAnimation.SampleAnimation(gameObject, ticks);
            }

            ticks += anim.speed * dt;
            if (ticks >= anim.maxTicks) {
                if (anim.loop) {
                    ticks -= anim.maxTicks;
                }
                else {
                    if (anim.removeOnEnd) {
                        StopAnimation(anim.name);
                        return;
                    }
                    ticks = anim.maxTicks;
                }
            }

            anim.animation.SampleAnimation(gameObject, ticks);

            // m_Animator.playbackTime	= m_Ticks; // 
            // m_Animator.PlayInFixedTime(_cacheHash, _cacheLayer, m_Ticks);
            // animatePhysics	
            // print(m_Animator.GetCurrentAnimatorStateInfo(_cacheLayer).loop);
            
        }

        private Color defaultColor = new Color(0f, 0f, 0f, 0f);
        public override void SetPowerIndicator(Platformer.Entities.Components.Power power) {
            // m_Shell.sprite = power.spriteRenderer.sprite;
            if (power == null) {
                m_Shell.color = defaultColor;
            }
            else {
                m_Shell.color = power.spriteRenderer.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color; // .sprite;
            }
            // m_Shell.transform.localScale *=  4f;
        }

        public override void PlayAnimation(string name, AnimationPriority priority) {
            AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            if (anim != null) {
                m_AnimationSheet[(int)priority] = anim;
                anim.modifiedSpeed = 1f;
            }
        }

        public override void PlayAnimation(string name, float speed) {
            AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            if (anim != null) {
                m_AnimationSheet[(int)anim.priority] = anim;
                anim.modifiedSpeed = speed;
            }
        }

        public override void PlayAnimation(string name) {
            AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            if (anim != null) {
                m_AnimationSheet[(int)anim.priority] = anim;
                anim.modifiedSpeed = 1f;
            }
        }

        public override void StopAnimation(string name) {
            AnimationItem anim = m_AnimationCollection.Find(anim => anim.name == name);
            if (anim != null && m_AnimationSheet[(int)anim.priority] == anim) {
                m_AnimationSheet[(int)anim.priority] = null;
            }
        }

        private AnimationItem GetHighestPriorityAnimation() {
            for (int i = m_AnimationSheet.Length - 1; i >= 0; i--) {
                if (m_AnimationSheet[i] != null) {
                    return m_AnimationSheet[i];
                }
            }
            return m_AnimationSheet[0];
        }

    }

}


