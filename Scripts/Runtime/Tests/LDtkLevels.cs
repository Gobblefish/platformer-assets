// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using LDtkUnity;

namespace Platformer.Tests {

    [CreateAssetMenu(fileName="LDtkLevels", menuName="LDtkLevels")]
    public class LDtkLevels : ScriptableObject {

        public List<LDtkComponentProject> levels;

        public LDtkComponentProject Get(string name) {
            return Find(name);
        }

        public LDtkComponentProject Find(string name) {
            for (int i = 0; i < levels.Count; i++) {
                LdtkJson json = levels[i].FromJson();
                Debug.Log(json.LevelNamePattern);
                if (json != null && name.ToLower() == json.LevelNamePattern.ToLower()) {
                    return levels[i];
                }
            }
            return null;
        }

    }

}