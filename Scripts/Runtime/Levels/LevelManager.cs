/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
using LDtkUnity;
// 
using Gobblefish.Animation;

namespace Platformer.Levels {

    /// <summary>
    /// Loads all the levels in the world from the LDtk file.
    /// </summary>
    public class LevelManager : Gobblefish.Manager<LevelManager, LevelSettings> {

        // A reference to all the created levels.
        [SerializeField] 
        public List<LevelSection> m_Sections = new List<LevelSection>();
        public List<LevelSection> Sections => m_Sections;

        [SerializeField]
        private Grid m_Grid;
        public static Grid Grid => Instance.m_Grid;

        //
        [SerializeField]
        private Tilemap m_DecorationMap;

        [SerializeField]
        private Tilemap m_CollisionMap;

        // The current section.
        private LevelSection m_CurrentSection = null;
        public static LevelSection CurrentSection => Instance.m_CurrentSection;

        protected override void Awake() {
            m_Settings = new LevelSettings();
            base.Awake();
            foreach (LevelSection section in m_Sections) {
                section.EnableEntities(false);
            }
        }

        public void SetSections(List<LevelSection> sections) {
            for (int i = 0; i < m_Sections.Count; i++) {
                if (m_Sections[i] != null && m_Sections[i].gameObject != null) {
                    DestroyImmediate(m_Sections[i].gameObject);
                }
            }
            m_Sections = sections;
        }

        public static void AddDeath() {
            Settings.deaths += 1;
        }

        public static void AddPoint() {
            Settings.points += 1;
        }

        public static void SetCurrentSection(LevelSection levelSection) {
            Instance.m_CurrentSection = levelSection;
        }

    }

}
