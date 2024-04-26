// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer {

    /// <summary>
    /// Loads all the levels in the world from the LDtk file.
    /// </summary>
    public class PlayerManager : Gobblefish.Manager<PlayerManager, PlayerSettings> {

        [SerializeField]
        private Platformer.Character.CharacterController m_Character;
        public static Platformer.Character.CharacterController Character => Instance.m_Character;

        // Resets the current room.
        public void Reset() {
            // Platformer.Objects.Blocks.BlockObject.ResetAll();
            // Platformer.Objects.Orbs.OrbObject.ResetAll();
        }

        protected override void Awake() {
            m_Settings = new PlayerSettings();
            base.Awake();
        }

    }

}
