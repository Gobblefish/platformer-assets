/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Platformer.LevelEditing;

namespace Platformer.LevelEditing {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class CollectByTag : MonoBehaviour {

        [SerializeField]
        private string m_SearchTag;

        void Update() {
            if (!Application.isPlaying) {
                
                GameObject[] allObjects = GameObject.FindGameObjectsWithTag(m_SearchTag);
                for (int i = 0; i < allObjects.Length; i++) {
                    if (allObjects[i].transform.parent == null) {
                        allObjects[i].transform.SetParent(transform);
                    }
                }
            }
        }

    }

}