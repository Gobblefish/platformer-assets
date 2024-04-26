/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Physics {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class Timer {

        #region Fields

        // The current value of the timer.
        [SerializeField] 
        private float m_Value = 0f;
        public float Value => m_Value;

        // The default reset value of the timer.
        [SerializeField] 
        private float m_MaxValue = 0f;
        public float MaxValue => m_MaxValue;

        // Whether this timer is currently not zero.
        public bool Active => m_Value > 0f;

        // The ratio of the current value of this timer.
        public float Ratio => m_MaxValue > 0f ? m_Value / m_MaxValue : 0f;

        // The inverse ratio of the current value of this timer.
        public float InverseRatio => m_MaxValue > 0f ? 1f - (m_Value / m_MaxValue) : 1f;

        #endregion

        #region Methods

        public Timer(float initialValue, float maxValue) {
            m_Value = initialValue;
            m_MaxValue = maxValue;
        }

        // Sets the value of the timer.
        public void Set(float value, bool loop = false, float maxValue = -1f) {
            m_MaxValue = maxValue == -1f ? m_MaxValue : maxValue;
            m_Value = value;
            if (m_Value > m_MaxValue && loop) {
                m_Value = m_Value % m_MaxValue;
            }
            else if (m_Value > m_MaxValue && !loop) {
                m_Value = m_MaxValue;
            }
        }

        // Starts the timer to its max value.
        public void Start(float startValue = -1f) {
            m_MaxValue = startValue == -1f ? m_MaxValue : startValue;
            m_Value = m_MaxValue;
        }

        // Starts the timer to its max value if the predicate is fulfilled.
        public void StartIf(bool predicate, float startValue = -1f) {
            if (predicate) { Start(startValue); }
        }

        // Stops the timer (resets it to 0).
        public void Stop() {
            m_Value = 0;
        }

        // Ticks the timer down by the given interval.
        public bool TickDown(float dt) {
            bool wasnotzero = m_Value > 0f;
            bool isnowzero = false;
            m_Value -= dt;
            if (m_Value <= 0f) {
                m_Value = 0f;
                isnowzero = true;
            }
            return wasnotzero && isnowzero;
        }

        // Ticks the timer down by the given interval if the predicate is fulfilled.
        public bool TickDownIf(float dt, bool p) {
            bool wasnotzero = m_Value > 0f;
            bool isnowzero = false;
            if (p) {  m_Value -= dt; }
            if (m_Value <= 0f) {
                m_Value = 0f;
                isnowzero = true;
            }
            return wasnotzero && isnowzero;
        }

        // Ticks the timer down by the given interval if the predicate is fulfilled.
        public bool TickDownIfElseReset(float dt, bool p) {
            bool wasnotzero = m_Value > 0f;
            bool isnowzero = false;
            if (p) {  m_Value -= dt; }
            else { m_Value = m_MaxValue; }
            if (m_Value <= 0f) {
                m_Value = 0f;
                isnowzero = true;
            }
            return wasnotzero && isnowzero;
        }

        // Rename this to triangle ticks
        public bool TriangleTickDownIf(float dt, bool p) {
            bool wasnotzero = m_Value != 0f;
            if (p) {  m_Value += dt; }
            else { m_Value -= dt; }
            if (m_Value >= m_MaxValue) { m_Value = m_MaxValue; }
            if (m_Value < 0f) { m_Value = 0f; }
            bool isnowzero = m_Value == 0f;
            return wasnotzero && isnowzero;
        }

        // Ticks the timer up to a specified maximum by the given interval.
        public bool TickUp(float dt) {
            bool wasnotmax = m_Value < m_MaxValue;
            bool isnowmax = false;
            m_Value += dt;
            if (m_Value >= m_MaxValue) {
                m_Value = m_MaxValue;
                isnowmax = true;
            }
            return wasnotmax && isnowmax;
        }

        // Ticks the timer up by the given interval  a specified maximum if the predicate is fulfilled.
        public bool TickUpIf(float dt, bool p) {
            bool wasnotmax = m_Value < m_MaxValue;
            bool isnowmax = false;
            if (p) { m_Value += dt; }
            if (m_Value >= m_MaxValue) {
                m_Value = m_MaxValue;
                isnowmax = true;
            }
            return wasnotmax && isnowmax;
        }

        public bool Cycle(float dt) {
            m_Value += dt;
            if (m_Value > 2f * m_MaxValue) {
                m_Value -= 2f * m_MaxValue;
            }
            return m_Value > m_MaxValue;
        }

        public bool Loop(float dt) {
            m_Value += dt;
            if (m_Value > m_MaxValue) {
                m_Value -= m_MaxValue;
            }
            return m_Value > m_MaxValue;
        }

        #endregion

    }

}



