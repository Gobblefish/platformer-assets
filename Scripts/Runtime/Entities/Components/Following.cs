/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */
using IReset = Platformer.Entities.Utility.IReset;
using CharacterController = Platformer.Character.CharacterController;
// using TrailAnimator = Gobblefish.Animation.TrailAnimator;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Entity))]
    public class Following : MonoBehaviour, IReset {

        #region Enumerations.

        public enum FollowState {
            Still, 
            Following, 
        }

        #endregion

        #region Variables.

        /* --- Constants --- */

        // The weight with which gravity pulls this.
        // private static float WEIGHT = 1f;

        /* --- Components --- */
        
        // The body attached to this gameObject 
        private Entity m_Entity;

        /* --- Members --- */

        // The current fall state of this falling spike.
        [SerializeField]
        private FollowState m_FollowState = FollowState.Still;

        // Whether this transform is following something.
        [SerializeField, ReadOnly] 
        private Transform m_FollowTransform = null;
        public Transform FollowTransform => m_FollowTransform;

        private Transform m_Parent;

        // // The duration this crumbles for before falling.
        // [SerializeField] 
        // private float m_FollowDelay = 0.5f;
        
        // // Tracks how long this is crumbling for
        // [SerializeField] 
        // private Timer m_FallDelayTimer = new Timer(0f, 0f);

        // The strength with which this shakes while crumbling
        // [SerializeField] 
        // private float m_ShakeStrength = 0.12f;
        // private float Strength => m_ShakeStrength * m_FallDelayTimer.InverseRatio;

        #endregion

        void Awake() {
            m_Parent = transform.parent;
            m_Entity = GetComponent<Entity>();
        }

        // Initialize the spike.
        void Start() {
            m_FollowState = FollowState.Still;
        }

        // Runs once every fixed interval.
        void FixedUpdate() {

            switch (m_FollowState) {
                case FollowState.Following:
                    WhileFollowing();
                    break;
                default:
                    break;
            }

        }

        public void StartFollowing() {
            if (m_Entity.CollisionEnabled && m_FollowState == FollowState.Still) {
                m_FollowState = FollowState.Following;
                OnFollow();
            }
        }

        private void OnFollow() {
            // Index.
            Following[] followingArray = (Following[])GameObject.FindObjectsOfType(typeof(Following));
            Following following = FindFollowing(followingArray, PlayerManager.Character.transform);
            if (following == null) {
                m_FollowTransform = PlayerManager.Character.transform;
            }
            else {
                Following cachedFollowing = following;
                while (following != null) {
                    cachedFollowing = following;
                    following = FindFollowing(followingArray, following.transform);
                }
                m_FollowTransform = cachedFollowing.transform;
            }

            transform.SetParent(m_FollowTransform.parent);
        }

        public static Following FindFollowing(Following[] following, Transform transform) {
            for (int i = 0; i < following.Length; i++) {
                if (following[i].FollowTransform == transform) { 
                    return following[i];
                }
            }
            return null;
        }

        private void WhileFollowing() { 
            Vector3 direction = -(m_FollowTransform.position - transform.position).normalized;
            Vector3 followPosition = m_FollowTransform.position + direction; // (m_Follow.position - transform.position).normalized * m_Index;
            float mag = (followPosition - transform.position).magnitude;
            transform.Move(followPosition, mag * 5f, Time.fixedDeltaTime);
        }

        public void OnStartResetting() {
            transform.SetParent(m_Parent);
            print("is this being called");
        }

        public void OnFinishResetting() {
            m_FollowState = FollowState.Still;
        }

    }

}
