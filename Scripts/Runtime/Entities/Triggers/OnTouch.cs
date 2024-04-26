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
using Platformer.Entities;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Triggers {

    [DefaultExecutionOrder(1000), RequireComponent(typeof(Entity))]
    public class OnTouch : MonoBehaviour {

        [HideInInspector]
        private Entity m_Entity;

        // The number of hit stop frames when touched
        [SerializeField]
        private int m_HitStopFrames = 8;

        [SerializeField]
        private UnityEvent<Platformer.Character.CharacterController> m_TouchEvent;

        [SerializeField]
        private UnityEvent m_TouchExitEvent;

        [SerializeField]
        private UnityEvent m_TouchGroundEvent;

        [SerializeField]
        private bool m_DisableGroundTouching = true;

        // Runs once before the first frame.
        void Awake() {
            m_Entity = GetComponent<Entity>();
            m_Entity.SetAsTrigger(true);
        }

        // Runs everytime something enters this trigger area.
        void OnTriggerEnter2D(Collider2D collider) {
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                PhysicsManager.Time.RunHitStop(m_HitStopFrames);
                m_TouchEvent.Invoke(character);
            }
            else if (!m_DisableGroundTouching) {
                bool hitGround = collider.gameObject.layer == PhysicsManager.CollisionLayers.PlatformLayer;
                if (hitGround) {
                    m_TouchGroundEvent.Invoke();
                }
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            CharacterController character = collider.GetComponent<CharacterController>();
            if (character != null) {
                m_TouchExitEvent.Invoke();
            }
        }

    }

}