/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Platformer.Tests {

    public class GetRoomName : MonoBehaviour {

        public TextMeshProUGUI textMesh;

        public void SetRoomName(string name) {
            textMesh.text = name;
        }

    }

}