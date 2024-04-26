/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    ///<summary>
    /// Controls the time of the game.
    ///<summary>
    public class TimeController : MonoBehaviour {

        #region Fields.

        /* --- Constants --- */

        // The threshold above which a ramp stop starts ramping up.
        public const float RAMP_THRESHOLD = 0.5f;

        // The default time scale.
        public const float DEFAULT_TIMESCALE = 1f;

        // The paused time scale.
        public const float PAUSED_TIMESCALE = 0f;

        /* --- Static --- */

        // The timescale of the current game.
        [SerializeField, ReadOnly]
        private float m_TimeScale = 1f;

        /* --- Members --- */

        // The amount of time the game has been running for.
        [SerializeField, ReadOnly]
        private float m_Ticks = 0f;
        public float Ticks => m_Ticks;

        // Whether a ramp stop is currently being run.
        [HideInInspector] 
        private Timer m_RampStopTimer = new Timer(0f, 0f);

        // Whether a hit stop is currently being run.
        [HideInInspector] 
        private Timer m_HitStopTimer = new Timer(0f, 0f);

        //
        public bool TimePaused => m_TimeScale == PAUSED_TIMESCALE;
        public bool TimePlaying => !TimePaused;
        
        #endregion

        // Runs once per frame.
        void Update() {
            // if (!Game.Playing) {
            //     m_TimeScale = PAUSED_TIMESCALE;
            // }

            if (m_RampStopTimer.Active) {
                UpdateRampStop();
            }
            else if (m_HitStopTimer.Active) {
                UpdateHitStop();
            }
            else {
                m_TimeScale = DEFAULT_TIMESCALE;
            }

            Time.timeScale = m_TimeScale;
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            m_Ticks += Time.fixedDeltaTime;
        }

        // Run a hit stop.
        public void RunHitStop(int frames = 16) {
            m_TimeScale = PAUSED_TIMESCALE;
            m_HitStopTimer.Start(frames);
        }

        private void UpdateHitStop() {
            m_HitStopTimer.TickDown(1f);
        }

        // Run a ramp stop.
        public void RunRampStop(int frames = 128) {
            m_TimeScale = PAUSED_TIMESCALE;
            m_RampStopTimer.Start(frames);
        }

        private void UpdateRampStop() {
            m_RampStopTimer.TickDown(1f);
            if (m_RampStopTimer.InverseRatio > RAMP_THRESHOLD) {
                m_TimeScale = m_RampStopTimer.InverseRatio;
            }
        }

        public void Play() {
            m_TimeScale = DEFAULT_TIMESCALE;
            m_RampStopTimer.Stop();
            m_HitStopTimer.Stop();
        }

        public void Pause() {
            m_TimeScale = PAUSED_TIMESCALE;
            m_RampStopTimer.Stop();
            m_HitStopTimer.Stop();
        }

    }

}