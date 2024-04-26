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

        [SerializeField]
        private LevelDecorationCollection m_LevelDecorations;

        // A reference to all the created levels.
        [SerializeField] 
        public List<LevelSection> m_Sections = new List<LevelSection>();
        public List<LevelSection> Sections => m_Sections;

        // The current section.
        private LevelSection m_CurrentSection = null;
        public static LevelSection CurrentSection => Instance.m_CurrentSection;

        public bool dont = false;

        protected override void Awake() {
            m_Settings = new LevelSettings();
            base.Awake();
            foreach (LevelSection section in m_Sections) {
                section.EnableEntities(false);
            }
            // Decoration decor = m_LevelDecorations.GetNew("Demo");
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

        [SerializeField]
        private Grid m_Grid;
        public static Grid Grid => Instance.m_Grid;

        //
        [SerializeField]
        private Tilemap m_DecorationMap;

        [SerializeField]
        private Tilemap m_CollisionMap;

        [SerializeField]
        private GameObject m_CollisionBlock;

        public List<Rigidbody2D> ConvertToBlocks(BoundsInt bounds) {

            int sortingOrder = m_DecorationMap.GetComponent<TilemapRenderer>().sortingOrder;
            string sortingLayerName = m_DecorationMap.GetComponent<TilemapRenderer>().sortingLayerName;

            List<Rigidbody2D> bodies = new List<Rigidbody2D>();
            for (int i = bounds.yMin; i <= bounds.yMax; i++) {
                for (int j = bounds.xMin; j <= bounds.xMax; j++) {
                    Vector3Int position = new Vector3Int(j, i, 0);
                    Sprite sprite = m_DecorationMap.GetSprite(position);
                    if (sprite != null) {
                        Rigidbody2D body = CreateBlock(sprite, position, sortingLayerName, sortingOrder);
                        print(body.gameObject.name);
                        bodies.Add(body);
                    }
                }
            }

            for (int i = bounds.yMin; i <= bounds.yMax; i++) {
                for (int j = bounds.xMin; j <= bounds.xMax; j++) {
                    Vector3Int pos = new Vector3Int(j, i, 0);
                    m_DecorationMap.SetTile(pos, null);
                    m_CollisionMap.SetTile(pos, null);
                }
            }

            return bodies;
        }

        public Rigidbody2D CreateBlock(Sprite sprite, Vector3Int position, string sortingLayerName, int sortingOrder) {
            // Create the block.
            GameObject newObject = Instantiate(m_CollisionBlock);
            newObject.name = "Block " + position.ToString();
            newObject.transform.position = m_Grid.GetCellCenterWorld(position);
            newObject.GetComponent<SpriteRenderer>().sprite = sprite;
            newObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
            newObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            // Set the transform.
            Matrix4x4 matrix = m_DecorationMap.GetTransformMatrix(position);
            newObject.transform.FromMatrix(matrix);
            // Set the block active.
            newObject.SetActive(true);
            return newObject.GetComponent<Rigidbody2D>();
        }


    }

}
