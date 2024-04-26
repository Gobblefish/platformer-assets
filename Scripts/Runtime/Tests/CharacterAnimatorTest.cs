// // TODO: Clean
// /* --- Libraries --- */
// // System.
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// // Unity.
// using UnityEngine;
// using UnityEngine.VFX;
// // Platformer.
// using Platformer.Character;

// /* --- Definitions --- */
// 
// namespace Platformer.Tests {

//     ///<summary>
//     /// Animates the character.
//     ///<summary>
//     [RequireComponent(typeof(SpriteRenderer))]
//     public class CharacterAnimatorTest : MonoBehaviour {

//         #region Data Structures

//         public enum AnimationPriority {
//             // Default.
//             DefaultIdle,
//             DefaultMoving,
//             DefaultJumpRising,
//             DefaultJumpFalling,
//             // Action Passive.
//             ActionPassiveIdle,
//             ActionPassiveMoving,
//             ActionPassiveRising,
//             ActionPassiveFalling,
//             // Action Active
//             ActionPreActive,
//             ActionActive,
//             ActionPostActive,
//             Count,
//         }

//         #endregion

//         #region Variables

//         /* --- Constants --- */

//         // The frame rate that the animation plays at.
//         public const float FRAME_RATE = 12;

//         // The factor by which this stretches.
//         public const float STRETCH_FACTOR = 2.5f/2f;

//         // The sprite renderer attached to this object.
//         private SpriteRenderer m_SpriteRenderer = null;        

//         // Okay. Lets see if this works.
//         [HideInInspector]
//         private Dictionary<AnimationPriority, Sprite[]> m_AnimationSheet = new Dictionary<AnimationPriority, Sprite[]>();
//         // private Sprite[][] m_AnimationSheet;
//         // The sprites this is currently animating through.
//         [SerializeField]
//         private Sprite[] m_CurrentAnimation = null;

//         // The sprite we are currently on.
//         [SerializeField, ReadOnly]
//         private int m_CurrentFrame = 0;

//         // The ticks until the next frame.
//         [SerializeField, ReadOnly]
//         private float m_Ticks = 0f;

//         // The amount this character was stretched last frame.
//         [SerializeField, ReadOnly]
//         private Vector2 m_CachedStretch = new Vector2(0f, 0f);

//         [SerializeField]
//         private VisualEffect m_SimpleBurst;
//         public VisualEffect SimpleBurst => m_SimpleBurst;

//         [SerializeField]
//         private VisualEffect m_ColoredBurst;
//         public void ColoredBurst(Color color) {
//             m_ColoredBurst.SetVector4("_Color", color);
//             m_ColoredBurst.Play();
//         }

//         #endregion

//         #region Methods.

//         // Runs once before the first frame.
//         void Start() {
//             m_Character = transform.parent.GetComponent<CharacterController>();
//             m_SpriteRenderer = GetComponent<SpriteRenderer>();
//             // m_AnimationSheet = new Sprite[(int)AnimationPriority.Count][];
//         }

//         void Update() {
//             Animate(Time.deltaTime);
//             Scale(Time.deltaTime);
//             Rotate();
//             Warp();
//         }

//         // Animates the flipbook by setting the animation, frame, and playing any effects.
//         private void Animate(float dt) {
//             // Guard clause to protect from animating with no sprites.
//             Sprite[] previousAnimation = m_CurrentAnimation;
//             m_CurrentAnimation = GetHighestPriorityAnimation();
//             if (!Game.Validate<Sprite>(m_CurrentAnimation)) { return; }

//             m_Ticks = previousAnimation == m_CurrentAnimation ? m_Ticks + dt : 0f;
//             m_CurrentFrame = (int)Mathf.Floor(m_Ticks * FRAME_RATE) % m_CurrentAnimation.Length;
//             m_SpriteRenderer.sprite = m_CurrentAnimation[m_CurrentFrame];

//         }

//         public Sprite[] GetHighestPriorityAnimation() {
//             // for (int i = m_AnimationSheet.Length - 1; i >= 0; i--) {
//             //     if (m_AnimationSheet[i] != null) {
//             //         return m_AnimationSheet[i];
//             //     }
//             // }
//             // return null;
//             return m_AnimationSheet[m_AnimationSheet.Keys.Max()];
//         }

//         public void Push(Sprite[] animation, AnimationPriority priority) {
//             if (animation == null || animation.Length == 0) { return; }

//             // m_AnimationSheet[(int)priority] = animation;

//             if (m_AnimationSheet.ContainsKey(priority)) {
//                 m_AnimationSheet[priority] = animation;
//             }
//             else {
//                 m_AnimationSheet.Add(priority, animation);
//             }
//         }

//         public void Remove(Sprite[] animation) {
//             if (animation == null) { return; }

//             // for (int i = 0; i < m_AnimationSheet.Length; i++) {
//             //     if (m_AnimationSheet[i] == animation) {
//             //         print("Remmoving");
//             //         m_AnimationSheet[i] = null;
//             //     }
//             // }

//             List<KeyValuePair<AnimationPriority, Sprite[]>> kvp = m_AnimationSheet.Where(kv => kv.Value == animation).ToList();
//             foreach(KeyValuePair<AnimationPriority, Sprite[]> kv in kvp) {
//                 m_AnimationSheet.Remove(kv.Key);
//             }
//         }

//         void Rotate() {
//             float currentAngle = transform.eulerAngles.y;
//             float angle = m_Character.FacingDirection < 0f ? 180f : m_Character.FacingDirection > 0f ? 0f : currentAngle;
//             if (transform.eulerAngles.y != angle) {
//                 transform.eulerAngles = angle * Vector3.up;
//             }
//         }

//         void Scale(float dt) {
//             transform.localScale = new Vector3(1f, 1f, 1f);
//             Vector2 stretch = Vector2.zero;
//             if (m_Character.Rising || m_Character.Falling) {
//                 float x = Mathf.Abs(m_Character.Body.velocity.x) * STRETCH_FACTOR * dt;
//                 float y = Mathf.Abs(m_Character.Body.velocity.y) * STRETCH_FACTOR * dt;
//                 stretch = new Vector2((x - y) / 2f, y - x);
//                 transform.localScale += (Vector3)(stretch + m_CachedStretch);
//             }
//             m_CachedStretch = stretch;
//         }

//         void Warp() {
//             if (m_Character.OnGround) {
//                 m_SpriteRenderer.material.SetFloat("_WarpIntensity", 0f); 
//                 return;
//             }

//             float warpBase = m_Character.Body.velocity.y;
//             if (warpBase < 0.5) {
//                 warpBase /= Platformer.Character.DefaultAction.MAX_FALL_SPEED;
//                 warpBase *= 4f;
//             }
//             else if (warpBase > 0.5) {
//                 warpBase /= m_Character.Default.JumpSpeed * 1.5f;
//                 warpBase *= 2f;
//             }
//             m_SpriteRenderer.material.SetFloat("_WarpIntensity", warpBase);            

//         }

//         #endregion

//     }

// }


