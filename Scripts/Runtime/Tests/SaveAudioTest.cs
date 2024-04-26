// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
//
using Gobblefish.Audio;

namespace Platformer.Tests {

    public class SaveAudioTest : MonoBehaviour {

        // public Material mat;
        public bool m_Save;

        void Update() {
            if (m_Save) {
                Save();
            }
        }

        public void Save() {
            if (AudioManager.Instance != null) {
                AudioManager.Settings.Save();
            }
        }

    }

}