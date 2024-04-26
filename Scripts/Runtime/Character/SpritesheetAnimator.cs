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

namespace Platformer.Character {

    ///<summary>
    /// Animates the character.
    ///<summary>
    // [RequireComponent(typeof(SpriteRenderer))]
    public class SpritesheetAnimator : CharacterAnimator {

        const float BASE_TIREDNESS = 3f;
        const float MAX_TIREDNESS = 2.8f;
        const float MIN_MOVING_FACTOR = 1f;
        const float PERCENT_TIREDNESS_GAINED_IN_A_SECOND = 0.15f;
        const float PERCENT_TIREDNESS_LOST_IN_A_SECOND = 0.5f;

        [System.Serializable]
        protected class AnimationItem {
            public string name;
            public AnimationPriority priority;
            public SpriteAnimation animation;
            public bool loop = true;
            public bool removeOnEnd = false;

            [HideInInspector] 
            public float modifiedSpeed = 1f;
        }

        // The frame rate that the animation plays at.
        public const float FRAME_RATE = 12;

        // The factor by which this stretches.
        public const float STRETCH_FACTOR = 2.5f/2f;

        // The sprite renderer attached to this object.
        [SerializeField]
        private SpriteRenderer m_SpriteRenderer = null;

        [SerializeField]
        private SpriteRenderer m_Foreleg = null;

        [SerializeField]
        private SpriteRenderer m_Backleg = null;

        [SerializeField]
        private SpriteRenderer m_Body;

        [SerializeField]
        private List<AnimationItem> m_AnimationCollection = new List<AnimationItem>();

        // Okay. Lets see if this works.
        [HideInInspector]
        private AnimationItem[] m_AnimationSheet;

        // The sprites this is currently animating through.
        [SerializeField]
        private SpriteAnimation m_CurrentAnimation = null;

        [SerializeField]
        private float m_Tiredness = 0f;

        // The amount this character was stretched last frame.
        [SerializeField, ReadOnly]
        private Vector2 m_CachedStretch = new Vector2(0f, 0f);

        [SerializeField]
        private Gobblefish.Animation.TransformAnimator m_TirednessAnimator;

        // Runs once before the first frame.
        protected override void Start() {
            m_Character = transform.parent.GetComponent<CharacterController>();
            m_AnimationSheet = new AnimationItem[(int)AnimationPriority.Count];
        }

        // Animates the flipbook by setting the animation, frame, and playing any effects.
        protected override void Animate(float dt) {
            Scale(dt);
            SetTiredness(dt);

            m_CurrentAnimation = GetHighestPriorityAnimation().animation;
            m_CurrentAnimation.Tick(dt);

            int halfNextFrame = m_CurrentAnimation.currentFrame;
            if (m_CurrentAnimation.sprites.Length > 0) {
                halfNextFrame += (int)Mathf.Ceil(m_CurrentAnimation.sprites.Length / 2f);
                halfNextFrame = halfNextFrame % m_CurrentAnimation.sprites.Length;
            }
            else {
                return;
            }

            m_Foreleg.sprite =  m_CurrentAnimation.GetFrame();
            m_Backleg.sprite = m_CurrentAnimation.sprites[halfNextFrame];
            
        }

        // public void SetBody(Sprite sprite) {
        //     m_Body.sprite = sprite;
        // }

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

        private void Scale(float dt) {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Vector2 stretch = Vector2.zero;
            if (m_Character.Rising || m_Character.Falling) {
                float x = Mathf.Abs(m_Character.Body.velocity.x) * STRETCH_FACTOR * dt;
                float y = Mathf.Abs(m_Character.Body.velocity.y) * STRETCH_FACTOR * dt;
                stretch = new Vector2((x - y) / 2f, y - x);
                transform.localScale += (Vector3)(stretch + m_CachedStretch);
            }
            m_CachedStretch = stretch;
        }

        
        private void SetTiredness(float dt) {
            if (m_Character.Body.velocity.sqrMagnitude > 0.2f) {
                if (m_Tiredness < MIN_MOVING_FACTOR) {
                    m_Tiredness = MIN_MOVING_FACTOR;
                }
                m_Tiredness += PERCENT_TIREDNESS_GAINED_IN_A_SECOND * dt;
            }
            else {
                m_Tiredness -= PERCENT_TIREDNESS_LOST_IN_A_SECOND * dt;
            }
            m_Tiredness = m_Tiredness < 0f ? 0f : m_Tiredness > 1f ? 1f : m_Tiredness;
            m_TirednessAnimator.SetDuration(BASE_TIREDNESS - MAX_TIREDNESS * m_Tiredness);
        }


    }

}


