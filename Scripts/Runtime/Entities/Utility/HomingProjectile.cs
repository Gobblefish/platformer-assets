/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish;
// Platformer.
using Platformer.Physics;

namespace Platformer.Entities.Utility {

    ///<summary>
    ///
    ///<summary>
    public class HomingProjectile : Projectile {

        [SerializeField]
        private float m_Acceleration = 7f;

        // The thing that this targets.
        private Transform HomingTarget => PlayerManager.Character.transform;

        public override void Fire(float speed, Vector2 direction) {
            gameObject.SetActive(true);
            direction = (HomingTarget.position - transform.position).normalized;
            transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
            m_Body.Move(direction.normalized * 0.5f);
            m_Body.SetVelocity(speed * direction);
        }

        // void FixedUpdate() {
        //     transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, m_Body.velocity);

        //     Vector2 deltaVelocity = m_Acceleration * Time.fixedDeltaTime * (HomingTarget.position - transform.position).normalized;
        //     m_Body.AddVelocity(deltaVelocity);
        // }

    }

}
