// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using LDtkUnity;

namespace Platformer.Tests {

    public class PlayTest : MonoBehaviour {

        public TMP_InputField inputField;

        public LDtkLevels levels;

        public void OnPlay() {
            LDtkComponentProject project = levels.Get(inputField.text);

            if (project == null) {
                Debug.Log("couldn't find level");
            }
            else {
                Platformer.Levels.LDtk.LDtkReader.setData = project;
                SceneManager.LoadScene("Game (Test)");
            }
            
        }

    }

}