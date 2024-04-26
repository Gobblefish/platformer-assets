/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish.Audio;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */
using Projectile = Platformer.Entities.Utility.Projectile;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    public class Spitting: MonoBehaviour {

        public enum SpitState {
            None,
            Tensing,
            Releasing,
        }

        // Whether this is bouncing or releasing.
        [SerializeField] 
        private SpitState m_SpitState = SpitState.None;        

        // The direction that this object spits in.
        [SerializeField]
        private float m_SpitAngleOffset = 0;
        private float SpitAngle => transform.eulerAngles.z + m_SpitAngleOffset;

        // Tracks whether        
        [SerializeField] 
        private Timer m_SpitTimer = new Timer(0f, 0f);

        [SerializeField]
        private float m_SpitDelay = 0.5f;

        // The speed with which this spits the projectile.
        [SerializeField] 
        private float m_SpitSpeed = 10f;

        [SerializeField]
        private float m_SpitTorque = 0f;

        // The projectile that this thing fires.
        [SerializeField]
        private Projectile m_SpitProjectile = null;

        // The effect that plays when this spike shatters.
        [SerializeField] 
        private VisualEffect m_SpitEffect;
        
        // The effect that plays when the spike shatters.
        [SerializeField] 
        private AudioSnippet m_SpitSound;
        
        void FixedUpdate() {
            bool finished = m_SpitTimer.TickDown(Time.fixedDeltaTime);

            // Whenever the crumble timer hits 0.
            if (finished) {

                switch (m_SpitState) {
                    case SpitState.Tensing:
                        OnSpit();
                        break;
                    default:
                        break;
                }
            }

        }

        public void OnStartSpitting() {
            if (m_SpitState != SpitState.None) { return; }

            m_SpitTimer.Start(m_SpitDelay);
            m_SpitState = SpitState.Tensing;

        }

        private void OnSpit() {
            Projectile projectile = m_SpitProjectile.CreateInstance();
            projectile.Fire(m_SpitSpeed, Quaternion.Euler(0f, 0f, SpitAngle) * Vector2.right, m_SpitTorque);
            
            if (m_SpitSound != null) {
                m_SpitSound.Play(); // Game.Audio.Sounds.PlaySound(m_SpitSound, 0.15f);
            }

            m_SpitState = SpitState.None;
            // // Game.Visuals.Effects.PlayEffect(m_SpitEffect)
        }

        public bool debugSpitter = true;
        void OnDrawGizmos() {
            if (!debugSpitter) { return; }

            Gizmos.color = Color.red;
            Vector2 v = m_SpitSpeed / 10f * (Quaternion.Euler(0f, 0f, SpitAngle) * Vector2.right);
            Gizmos.DrawLine(m_SpitProjectile.transform.position, m_SpitProjectile.transform.position + (Vector3)v); 

        }

    }

}
