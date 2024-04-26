/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;

namespace Platformer.Levels.LDtk {

    /// <summary>
    /// Stores a reference to the ldtk layers for easy reference.
    /// <summary>
    [System.Serializable]
    public class LDtkLayers {

        [SerializeField]
        private string m_Alternate = "ALTERNATE";
        public string Alternate => m_Alternate;

        // Layer Names
        [SerializeField]
        private string m_Path = "PATH";
        public string Path => m_Path;

        [SerializeField]
        private string m_Entity = "ENTITIES";
        public string Entity => m_Entity;

        [SerializeField]
        private string m_Ground = "GROUND";
        public string Ground => m_Ground;
        
    }

}
    