// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Gobblefish.Input {

    ///<summary>
    /// Collects the inputs from a keyboard.
    ///<summary>
    [CreateAssetMenu(fileName="NPCInputChain", menuName="NPCInputChain")]
    public class NPCInputChain : ScriptableObject {

        [System.Serializable]
        public class NPCInputBlock {
            public string name;
            public float horizontalDirection;
            public float verticalDirection;
            public bool[] actionInput;
            public float duration;
            public Vector2 direction => new Vector2(horizontalDirection, verticalDirection);
        }

        public Vector3 origin;
        public NPCInputBlock[] chain;

        public bool turnOffOnEnd;
        public bool snapToPosOnEnd;
        public Vector3 positionOnEnd;

        public void OnEnd(NPCInputSystem npc) {

            if (turnOffOnEnd) {
                npc.gameObject.SetActive(false);
            }
            else if (snapToPosOnEnd) {
                npc.transform.position = positionOnEnd;
            }
                        
        }

    }

}