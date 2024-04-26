// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using Platformer.Levels;

namespace Platformer.Levels {

    [ExecuteInEditMode]
    public class DecorationEditor : MonoBehaviour {

        // The singleton.
        [SerializeField]
        private static DecorationEditor Instance = null;

        // The level manager.
        [SerializeField]
        private LevelManager m_LevelManager;

        [SerializeField]
        private List<DecorationSection> m_DecorationSection = new List<DecorationSection>();

        [Header("Controls")]
        public bool m_UpdateSections = false;
        public bool m_Reposition = false;
        public bool m_Reorganize = false;

        [SerializeField]
        private DecorationLayer m_CurrentLayer = null;
        public static DecorationLayer CurrentLayer => Instance != null ? Instance.m_CurrentLayer : null;

        void Update() {
            if (!Application.isPlaying) {
                if (Instance == null) {
                    Instance = this;
                }
            }

            if (m_Reposition && !Application.isPlaying) {
                // Reposition();
                m_Reposition = false;
            }

            if (m_Reorganize && !Application.isPlaying) {
                // Reorganize2();
                m_Reorganize = false;
            }

            // if (m_Reorganize && !Application.isPlaying) {
            //     m_Reorganize = false;
            // }

            if (m_UpdateSections) {
                UpdateDecorationSections();
            }
            
        }

        private void Reposition() {
            if (m_LevelManager == null) { return; }

            foreach (DecorationSection section in m_DecorationSection) {
                if (section.levelSection != null) {
                    section.transform.position = section.levelSection.camHandles.transform.position;
                }
            } 

        }

        // public enum LayerName {
        //     Background,
        //     Midground,
        //     Foreground,
        // }

        // public LayerName currentEditingLayer;

        private void Reorganize2() {
            // SpriteRenderer[] allObjects = (SpriteRenderer[])GameObject.FindObjectsOfType<SpriteRenderer>();
            // for (int i = 0; i < allObjects.Length; i++) {
                
            //     if (allObjects[i].transform.parent == null) {
            //         foreach (DecorationSection section in m_DecorationSection) {
            //             if (section.levelSection != null) {
            //                 if (section.levelSection.camHandles.Box.bounds.Contains(child.position)) {
            //                     section.ParentToSection(child, currentEditingLayer.ToString());
            //                     break;
            //                 }
            //             }
            //         }
            //     }
            // }
        }

        private void UpdateDecorationSections() {
            foreach (DecorationSection section in m_DecorationSection) {
                section.GetLayers();
                if (section.levelSection == null) {
                    section.SetSection(FindLevelSection(section.gameObject.name));
                }
            }

            for (int i = 0; i < m_LevelManager.Sections.Count; i++) {
                DecorationSection section = m_DecorationSection.Find(section => section.levelSection == m_LevelManager.Sections[i]);
                if (section == null) {
                    section = DecorationSection.New(m_LevelManager.Sections[i].gameObject.name + " Decorations");
                    m_DecorationSection.Add(section);
                    section.transform.SetParent(transform);
                    section.SetSection(m_LevelManager.Sections[i]);
                }
            }
        }

        private LevelSection FindLevelSection(string name) {
            string sectionName = name.Split(" ")[0];
            print(sectionName);

            foreach (LevelSection section in m_LevelManager.Sections) {
                if (section.gameObject.name == sectionName) {
                    print("found section");
                    return section;
                }
            }

            return null;

        }

    }

}
