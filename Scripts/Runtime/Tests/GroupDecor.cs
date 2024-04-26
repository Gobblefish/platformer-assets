// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEditor;
using LDtkUnity;

namespace Platformer.Tests {

    [ExecuteInEditMode]
    public class GroupDecor : MonoBehaviour {

        public bool reparent;

        private Transform parent;

        public Transform[] transforms;

        public void Update() {

            transforms = UnityEditor.Selection.transforms;

            if (reparent) {
                reparent = false;
                if (transforms.Length < 1) {
                    return;
                }

                GameObject newObject = new GameObject("untitled");
                newObject.transform.SetParent(this.transform);

                Vector3 pos = Vector3.zero;
                foreach (Transform t in transforms) {
                    pos += t.position;
                }
                pos /= transforms.Length;

                newObject.transform.position = pos;
                foreach (Transform t in transforms) {
                    t.SetParent(newObject.transform);
                }

                newObject.SetActive(false);

            }

        }

    }

}