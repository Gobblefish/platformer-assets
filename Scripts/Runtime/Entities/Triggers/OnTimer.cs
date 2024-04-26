/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */

namespace Platformer.Entities.Triggers {

    [DefaultExecutionOrder(1000)]
    public class OnTimer : MonoBehaviour {

        [SerializeField]
        private float m_Interval = 2f;

        // The timer with which this spits things.
        [HideInInspector]
        private Timer m_Timer = new Timer(0f, 0f);

        [SerializeField]
        private UnityEvent m_TimedEvent;
        
         // Runs once before the first frame.
        void Awake() {
            m_Timer.Start(m_Interval);
        }

        private void FixedUpdate() {
            bool finished = m_Timer.TickDown(Time.fixedDeltaTime);
            if (finished) {
                m_TimedEvent.Invoke();
                m_Timer.Start(m_Interval);
            }
        }

    }

}