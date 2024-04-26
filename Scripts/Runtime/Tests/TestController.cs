/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Character;

namespace Platformer.Tests {

    ///<summary>
    ///
    ///<summary>
    public class TestController : MonoBehaviour {

        public float speed;
        public Rigidbody2D rb;

        void Update() {
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.velocity = rb.velocity.normalized * speed;
        }


    }

}