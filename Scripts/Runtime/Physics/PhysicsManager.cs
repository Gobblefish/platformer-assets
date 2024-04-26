// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    ///<summary>
    /// Ties the physics functionality to the rest of the game.
    ///<summary>
    public sealed class PhysicsManager : Gobblefish.Manager<PhysicsManager, PhysicsSettings> {

        // Controls the flow of time in the game.
        [SerializeField]
        private TimeController m_TimeController;
        public static TimeController Time => Instance.m_TimeController;

        // Exposes simple collision functionality.
        [SerializeField]
        private CollisionCheck m_CollisionCheck = new CollisionCheck();
        public static CollisionCheck Collisions => Instance.m_CollisionCheck;

        // Collision.
        [SerializeField]
        private CollisionLayers m_CollisionLayers = new CollisionLayers();
        public static CollisionLayers CollisionLayers => Instance.m_CollisionLayers;

        protected override void Awake() {
            m_Settings = new PhysicsSettings();
            base.Awake();
        }

    }

}