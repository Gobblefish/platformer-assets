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
    [RequireComponent(typeof(Entity)), RequireComponent(typeof(Rigidbody2D))]
    public class Falling : MonoBehaviour, IReset {

        #region Enumerations.

        public enum FallState {
            Stable, 
            Crumbling, 
            Falling, 
            Reforming
        }

        #endregion

        #region Variables.

        /* --- Constants --- */

        // The weight with which gravity pulls this.
        private static float WEIGHT = 1f;

        /* --- Components --- */
        
        // The body attached to this gameObject 
        private Entity m_Entity;

        // The body attached to this gameObject 
        private Rigidbody2D m_Body;

        /* --- Members --- */

        // The current fall state of this falling spike.
        [SerializeField]
        private FallState m_FallState = FallState.Stable;

        // The duration this crumbles for before falling.
        [SerializeField] 
        private float m_FallDelay = 0.5f;
        
        // Tracks how long this is crumbling for
        [SerializeField] 
        private Timer m_FallDelayTimer = new Timer(0f, 0f);

        // The strength with which this shakes while crumbling
        [SerializeField] 
        private float m_ShakeStrength = 0.12f;
        private float Strength => m_ShakeStrength * m_FallDelayTimer.InverseRatio;

        [SerializeField]
        private Transform m_FallRenderer;

        #endregion

        void Awake() {
            m_Entity = GetComponent<Entity>();
            m_Body = GetComponent<Rigidbody2D>();
        }

        // Initialize the spike.
        void Start() {
            // m_Origin = transform.position;
            m_Body.Stop();
            m_Body.Freeze();
            m_FallState = FallState.Stable;
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            bool finished = m_FallDelayTimer.TickDown(Time.fixedDeltaTime);

            // When we're finished crumbling.
            if (finished) {

                switch (m_FallState) {
                    case FallState.Crumbling:
                        OnFall();
                        break;
                    default:
                        break;
                }

            }

            switch (m_FallState) {
                case FallState.Crumbling:
                    WhileCrumbling();
                    break;
                case FallState.Falling:
                    WhileFalling();
                    break;
                default:
                    break;
            }

        }

        public void StartFall() {
            if (m_Entity.CollisionEnabled && m_FallState == FallState.Stable) {
                m_FallState = FallState.Crumbling;
                m_FallDelayTimer.Start(m_FallDelay);
            }
        }

        private void OnFall() {
            // m_TrailSparkle.enabled = true;
            // transform.position = m_Origin + PhysicsManager.Settings.collisionPrecision * Vector3.down;
            if (!m_Entity.CollisionEnabled) {
                return;
            }

            m_Body.ReleaseXY();
            m_Body.SetWeight(WEIGHT);
            m_FallState = FallState.Falling;

        }

        private void WhileCrumbling() {
            m_FallRenderer.localPosition = Strength * (Vector3)Random.insideUnitCircle.normalized;
        }

        private void WhileFalling() { 

        }

        public void OnStartResetting() {
            print("is this being called");
            m_Body.Stop();
            m_Body.Freeze();
        }

        public void OnFinishResetting() {
            m_FallState = FallState.Stable;
        }

    }

}
