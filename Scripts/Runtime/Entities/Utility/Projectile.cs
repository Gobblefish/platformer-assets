/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.U2D;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Utility {

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour {

        protected Rigidbody2D m_Body;

        protected Vector3 m_Scale;

        [SerializeField]
        protected bool m_PointInDirection;

        public Projectile CreateInstance() {
            Projectile projectile = Instantiate(gameObject).GetComponent<Projectile>();
            projectile.transform.position = transform.position;
            return projectile;
        }

        void Awake() {
            m_Body = GetComponent<Rigidbody2D>();
            m_Scale = transform.localScale;
        }

        void FixedUpdate() {
            if (m_PointInDirection) {
                float signedAngle = Vector2.SignedAngle(Vector2.right, m_Body.velocity);
                if (signedAngle < -90f || signedAngle > 90f) {
                    signedAngle += 180f;
                    signedAngle = signedAngle % 360f;
                    transform.localScale = new Vector3(-m_Scale.x, m_Scale.y, m_Scale.z);
                }
                Quaternion direction = Quaternion.Euler(0f, 0f, signedAngle);
                transform.localRotation = direction;
            }
        }

        public virtual void Fire(float speed, Vector2 direction) {
            gameObject.SetActive(true);
            m_Body.Move(direction.normalized * 0.5f);
            m_Body.SetVelocity(speed * direction);
        }

        public void Fire(float speed, Vector2 direction, float torque) {
            Fire(speed, direction);
            if (!m_PointInDirection) {
                m_Body.AddTorque(torque);
            }
        }

        public void DeleteSelf() {
            Destroy(gameObject);
        }

        public static void DeleteAll() {
            Projectile[] projectiles = (Projectile[])GameObject.FindObjectsOfType(typeof(Projectile));
            for (int i = 0; i < projectiles.Length; i++) {
                Destroy(projectiles[i].gameObject);
            }
        }

    }

}
