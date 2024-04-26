/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Character;

namespace Platformer.Tests {

    ///<summary>
    ///
    ///<summary>
    public class ObjectTester : MonoBehaviour {

        public int count; 

        public GameObject _object;

        void Start() {

            for (int i = 0; i < count; i++) {
                
                GameObject __object = Instantiate(_object);
                __object.transform.position = (Vector3)Random.insideUnitCircle * 10f;
                __object.SetActive(true);

            }


        }

        void Update() {


        }


    }

}