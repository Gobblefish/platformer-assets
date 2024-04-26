// /* --- Libraries --- */
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Platformer.Character;

// namespace Platformer.Tests {

//     ///<summary>
//     ///
//     ///<summary>
//     public class CharacterAnimationTests : MonoBehaviour {

//         public int count; 

//         public CharacterAnimator animator;

//         public Sprite[] _animation0;
//         public Sprite[] _animation1;
//         public Sprite[] _animation2;
//         public Sprite[] _animation3;

//         public List<CharacterAnimator> animators = new List<CharacterAnimator>();

//         void Start() {

//             for (int i = 0; i < count; i++) {
                
//                 CharacterAnimator _animator = Instantiate(animator).GetComponent<CharacterAnimator>();
//                 _animator.transform.position = (Vector3)Random.insideUnitCircle * 10f;
//                 _animator.gameObject.SetActive(true);
//                 // _animator.SetArray();

                
//                 _animator.Push(_animation0, CharacterAnimator.AnimationPriority.DefaultIdle);
//                 animators.Add(_animator);

//             }


//         }

//         void Update() {


//         }


//     }

// }