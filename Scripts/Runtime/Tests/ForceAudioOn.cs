/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gobblefish.Audio;
using Platformer.Character;

namespace Platformer.Tests {

    public class ForceAudioOn : MonoBehaviour {

        // Master Volume.
        public float masterVolume;
        public bool masterMuted;
        
        // Music Volume.
        public float musicVolume;
        public bool musicMuted;
        
        // Ambience Volume.
        public float ambienceVolume;
        public bool ambienceMuted;
        
        // Sound Volume.
        public float soundVolume;
        public bool soundMuted;

        void Start() {
            if (AudioManager.Instance != null) {

                AudioManager.Settings.masterVolume = masterVolume;
                AudioManager.Settings.masterMuted = masterMuted;

                AudioManager.Settings.musicVolume = musicVolume;
                AudioManager.Settings.ambienceVolume = ambienceVolume;
                AudioManager.Settings.soundVolume = soundVolume;
                
                AudioManager.Settings.musicMuted = musicMuted;
                AudioManager.Settings.ambienceMuted = ambienceMuted;
                AudioManager.Settings.soundMuted = soundMuted;

            }
        }


    }

}