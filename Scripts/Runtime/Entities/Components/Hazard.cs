/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Platformer.
using Platformer;

// Definitions.
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    [DefaultExecutionOrder(1000)]
    public class Hazard : MonoBehaviour {

        // Collects this orb.
        public void Kill(CharacterController character) {
            if (character == PlayerManager.Character) { 
                character.Reset();
                Debug.Log("Player killed"); 
            }
            // CharacterController character = PlayerManager.Character;
        }
        
    }
    
}