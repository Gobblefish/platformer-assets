/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Platformer.
using Platformer.Physics;
using Platformer.Entities;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Utility {

    [DefaultExecutionOrder(1000), RequireComponent(typeof(Entity))]
    public class OnSight : MonoBehaviour {

        [HideInInspector]
        private Entity m_Entity;

        [SerializeField]
        private UnityEvent m_SightEvent;

        [SerializeField]
        private Vector3 m_Direction = new Vector3(0f, -1f, 0f);
        
        // Runs once before the first frame.
        void Awake() {
            m_Entity = GetComponent<Entity>();
        }

        // Runs once every frame.
        private void Update() {
            CharacterController character = PhysicsManager.Collisions.LineOfSight<CharacterController>(transform.position + m_Direction, m_Direction, PhysicsManager.CollisionLayers.Solid);
            if (character != null) {
                m_SightEvent.Invoke();
            }
        }

    }

}
