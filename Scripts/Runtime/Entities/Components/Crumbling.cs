/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Events;
// Platformer.
using Platformer.Physics;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Entity))]
    public class Crumbling : MonoBehaviour {

        public const float THRESHOLD = 0.8f;

        #region Enumerations.

        // A macro overview of the state.
        public enum CrumbleState {
            None,
            Crumbling,
            Reforming
        }

        #endregion

        #region Variables.

        // The threshold between which 
        private Entity m_Entity;

        // Whether this platform is crumbling.
        [SerializeField] 
        private CrumbleState m_CrumbleState = CrumbleState.None;

        // The duration this is crumbling for.
        [SerializeField] 
        private float m_CrumbleDuration = 1f;

        // The duration this is crumbling for.
        [SerializeField] 
        private float m_CrumbleScale = 5f;

        [SerializeField]
        private AnimationCurve m_CrumbleCurve;

        // The time it takes for this to reform.
        [SerializeField] 
        private float m_ReformDuration = 2f;

        [SerializeField]
        private AnimationCurve m_ReformCurve;

        // Tracks whether        
        [SerializeField] 
        private Timer m_CrumbleTimer = new Timer(0f, 0f);

        // Tracks whether        
        [SerializeField] 
        private Timer m_InitialCrumbleTimer = new Timer(0f, 0f);

        public UnityEvent m_CrumbleEvent = new UnityEvent();

        public UnityEvent m_CrumbleSecondPhaseEvent = new UnityEvent();
        private bool m_CrumbleSecondPhaseStarted = false;
        
        public UnityEvent m_ReformEvent = new UnityEvent();

        // The sound this plays on crumbling.
        // [SerializeField] 
        // private AudioClip m_OnCrumbleSound = null;

        // The sound this plays on reforming.
        // [SerializeField] 
        // private AudioClip m_OnReformSound = null;

        // The strength with which this shakes while crumbling
        [SerializeField] 
        private float m_ShakeStrength = 0.12f;

        public Transform m_Renderer;

        private Vector3 m_RendererOrigin;

        #endregion

        #region Methods.

        void Awake() {
            m_Entity = GetComponent<Entity>();
            m_RendererOrigin = m_Renderer.transform.localPosition;
        }

        void Start() {
            // m_Entity.SetMaterial(Game.Visuals.RenderingLayers.CrumblyMat);
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            bool finished = m_CrumbleTimer.TickDown(Time.fixedDeltaTime);

            // Whenever the crumble timer hits 0.
            if (finished) {

                switch (m_CrumbleState) {
                    case CrumbleState.Crumbling:
                        OnCrumble();
                        break;
                    case CrumbleState.Reforming:
                        OnReform();
                        break;
                    default:
                        break;
                }
            }

            // What to do for each state.
            switch (m_CrumbleState) {
                case CrumbleState.Crumbling:
                    WhileCrumbling(Time.fixedDeltaTime);
                    break;
                case CrumbleState.Reforming:
                    WhileReforming(Time.fixedDeltaTime);
                    break;
                default:
                    break;
            }

        }

        public void OnStartCrumble() {
            if (m_CrumbleState != CrumbleState.None) { return; }

            m_CrumbleTimer.Start(m_CrumbleDuration);
            m_InitialCrumbleTimer.Start(m_CrumbleDuration - 0.3f);
            
            m_CrumbleState = CrumbleState.Crumbling;
            m_CrumbleSecondPhaseStarted = false;

        }

        private void OnCrumble() {
            m_CrumbleState = CrumbleState.Reforming;
            m_Entity.EnableColliders(false);
            m_CrumbleTimer.Start(m_ReformDuration);
            // Game.Audio.Sounds.PlaySound(m_OnCrumbleSound, 0.15f);

            m_CrumbleEvent.Invoke();

        }

        private void OnReform() {
            m_CrumbleState = CrumbleState.None;
            m_Entity.EnableColliders(true);

            m_ReformEvent.Invoke();

            // Game.Audio.Sounds.PlaySound(m_OnReformSound, 0.15f);
            // m_Platform.entity.SetMaterialValue("_DissolveAmount", val);
        }

        private void WhileCrumbling(float dt) {
            m_InitialCrumbleTimer.TickDown(dt);
            if (m_InitialCrumbleTimer.Active) {
                m_Renderer.localPosition = m_RendererOrigin + m_ShakeStrength * (Vector3)Random.insideUnitCircle.normalized;
            }
            else {
                m_Renderer.localPosition = m_RendererOrigin;
                if (!m_CrumbleSecondPhaseStarted) {
                    m_CrumbleSecondPhaseEvent.Invoke();
                    m_CrumbleSecondPhaseStarted = true;
                }
            }

            // if (m_CrumbleTimer.InverseRatio < THRESHOLD) { return; }

            // // Game.Audio.Sounds.PlaySound(m_WhileCrumblingSound, Mathf.Sqrt(m_CrumbleTimer.InverseRatio) * 1f);
            // float x = (m_CrumbleTimer.InverseRatio - THRESHOLD) / (1f - THRESHOLD);
            // float val = m_CrumbleScale * m_CrumbleCurve.Evaluate(x);
            // m_Entity.SetMaterialValue("_DissolveAmount", val);
        }

        private void WhileReforming(float dt) {
            if (m_CrumbleTimer.InverseRatio < THRESHOLD) { return; }
            
            float x = (m_CrumbleTimer.InverseRatio - THRESHOLD) / (1f - THRESHOLD);
            float val = m_CrumbleScale * m_ReformCurve.Evaluate(x);
            m_Entity.SetMaterialValue("_DissolveAmount", val);
        }

        #endregion

    }

}
