// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    ///<summary>
    /// Defines the general physics settings for the game.
    ///<summary>
    public sealed class PhysicsSettings : Gobblefish.Settings<PhysicsSettings> {

        public float movementPrecision = 1f/32f;

        public float collisionPrecision = 1f/48f;

        public float gravityScale = 1f;

    }
    
}