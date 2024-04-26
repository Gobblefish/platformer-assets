/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
using TMPro;
// Platformer.
using Platformer.Entities;

/* --- Definitions --- */
using CharacterController = Platformer.Character.CharacterController;

namespace Platformer.Entities.Components {

    [System.Serializable]
    public class Dialogue {
            
            // The interval between printing letters.
            public static float PRINT_INTERVAL = 0.035f;

            [SerializeField]
            private TextMeshProUGUI m_TextField;

            [SerializeField]
            private GameObject m_DialogueBox;
            public GameObject DialogueBox => m_DialogueBox;

            // The lines this prompt has.
            [SerializeField]
            private string[] m_Lines;
            public string[] Lines => m_Lines;

            [SerializeField, ReadOnly]
            private string m_CurrentText = "";

            // The current line index;
            [SerializeField]
            private int m_LineIndex;

            [SerializeField]
            private int m_CharacterIndex;

            [SerializeField]
            private bool m_LineComplete = false;

            [SerializeField, ReadOnly]
            private float m_Ticks = 0f;

            [SerializeField, ReadOnly]
            private bool m_Active = false;

            public bool Start() {
                m_DialogueBox.SetActive(true);
                if (m_Lines.Length > 0) {
                    m_LineIndex = 0;
                    m_Ticks = 0f;
                    m_Active = true;
                    m_LineComplete = false;
                    return true;
                }
                return false;
            }

            public bool Reset() {
                m_DialogueBox.SetActive(false);
                m_Ticks = 0f;
                m_LineIndex = 0;
                m_CharacterIndex = 0;
                m_CurrentText = "";
                m_Active = false;
                m_LineComplete = false;
                return false;
            }

            public void Update(float dt) {
                if (!m_Active) { return; }
                m_TextField.text = m_CurrentText;
                m_Ticks -= dt;
                
                if (m_Ticks <= 0f && !m_LineComplete) {

                    m_CharacterIndex += 1;
                    if (m_CharacterIndex <= m_Lines[m_LineIndex].Length) {
                        m_CurrentText = m_Lines[m_LineIndex].Substring(0, m_CharacterIndex);
                        m_LineComplete = false;
                    }
                    else {
                        m_LineComplete = true;
                    }                         
                    m_Ticks = PRINT_INTERVAL;

                }

            }

            public bool Next() {
                if (!m_LineComplete) {
                    m_CurrentText = m_Lines[m_LineIndex];
                    m_LineComplete = true;
                    return true;
                }
                else if (m_LineIndex < m_Lines.Length - 1) {
                    m_Ticks = PRINT_INTERVAL;
                    m_LineIndex += 1;
                    m_CharacterIndex = 0;
                    m_CurrentText = "";
                    m_LineComplete = false;
                    return true;
                }
                m_Active = false;
                Reset();
                return false;
            }

        }

    ///<summary>
    ///
    ///<summary>
    [RequireComponent(typeof(Entity))]
    public class Interactable : MonoBehaviour {

        [HideInInspector]
        private Entity m_Entity;

        [SerializeField]
        private Dialogue m_Dialogue;

        [SerializeField]
        private bool m_FacePlayer = false;

        [SerializeField, ReadOnly]
        private float m_BaseRadius;

        // The buffer radius.
        public const float BUFFER_RADIUS = 0.7f;

        void Awake() {
            m_Dialogue.DialogueBox.SetActive(false);
            m_Dialogue.DialogueBox.transform.SetParent(null);
            m_Entity = GetComponent<Entity>();
            if (m_Entity.CircleCollider != null) {
                m_BaseRadius = m_Entity.CircleCollider.radius;         
            }
        }

        void Update() {
            if (m_FacePlayer) {
                FacePlayer();
            }

            if (PlayerManager.Character.CurrentInteractable == this) {
                m_Dialogue.Update(Time.deltaTime);
                m_Entity.Renderer.GetComponent<SpriteRenderer>().color = Color.blue; // transform.localScale = new Vector3(1f, 1f, 1f) * 2f;
            }
            else {
                m_Entity.Renderer.GetComponent<SpriteRenderer>().color = Color.white; 
            }
        }

        private void FacePlayer() {
            if (m_FacePlayer) {
                // Make this NPC face towards the player.
                float direction = (PlayerManager.Character.transform.position.x - transform.position.x);
                float angle = 0f;
                if (direction < 0f) {
                    angle = 180f;
                } 
                transform.eulerAngles = angle * Vector3.up;
            }
        }

        public void SetInteractable() {
            CharacterController character = PlayerManager.Character;
            character.SetInteractable(this);
            m_Dialogue.Reset();
            if (m_Entity.CircleCollider != null) {
                m_Entity.CircleCollider.radius = m_BaseRadius + BUFFER_RADIUS;        
            }
        }

        public void EndInteractable() {
            CharacterController character = PlayerManager.Character;
            character.SetInteractable(null);
            m_Dialogue.Reset();
            if (m_Entity.CircleCollider != null) {
                m_Entity.CircleCollider.radius = m_BaseRadius;        
            }        
        }
        
        public bool StartInteraction() {
            return m_Dialogue.Start();
        }

        public bool ContinueInteraction() {
            return m_Dialogue.Next();
        }

    }

}
