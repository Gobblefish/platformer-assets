/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

namespace Platformer.Entities.Utility {

    ///<summary>
    ///
    ///<summary>
    public class SpriteShapeCap : MonoBehaviour {

        [SerializeField]
        private Entity m_Entity;
        public Entity entity {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        // Runs when something collides with this platform.
        private void OnCollisionEnter2D(Collision2D collision) {
            m_Entity.OnCollisionEnter2D(collision);
        }

        // Runs when something exit this platform.
        private void OnCollisionExit2D(Collision2D collision) {
            m_Entity.OnCollisionExit2D(collision);            
        }

    }

}