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

    public class QuitTest : MonoBehaviour {

        public void OnQuit() {
            SceneManager.LoadScene("Menu (Test)");
        }

    }

}