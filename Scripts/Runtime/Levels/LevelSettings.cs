// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Levels {

    [System.Serializable]
    public class LevelSettings : Gobblefish.Settings<LevelSettings> {
        
        public float totalTime = 0f;
        public float[] times = new float[0];

        public float xPos = 0f;
        public float yPos = 0f;

        public int deaths = 0;
        public int points = 0;
        
    }

}