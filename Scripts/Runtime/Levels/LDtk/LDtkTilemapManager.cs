/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;
// Platformer.
using Platformer.Levels;
using Gobblefish;

namespace Platformer.Levels.LDtk {

    /// <summary>
    /// A tile entity used to match tiles to the ldtk file.
    /// </summary>
    [System.Serializable]
    public class LDtkTileEntity {
        public TileBase tile;
        public Vector2Int vectorID;
        public int order = 0;

        public Tilemap map;
    }

    // [System.Serializable]
    // public class DecorationMapParams {
    //     public string name;
    //     public Color color;
    // }

    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    public class LDtkTilemapManager : MonoBehaviour {

        [SerializeField]
        private Grid m_Grid;

        [SerializeField]
        private Tilemap m_DecorationMap;

        [SerializeField]
        private Tilemap m_CollisionMap;

        [SerializeField] 
        private List<LDtkTileEntity> m_DecorationTiles = new List<LDtkTileEntity>();

        [SerializeField]
        private TileBase m_CollisionTile;

        // [SerializeField]
        // private List<DecorationMapParams> m_DecorMapParams = new List<DecorationMapParams>();
        
        // Loads the map layouts for all the given levels.
        public void Refresh(List<LevelSection> sections, string layerName) {

            m_DecorationMap.ClearAllTiles();

            for (int i = 0; i < m_DecorationTiles.Count; i++) {
                if (m_DecorationTiles[i].map != null && m_DecorationTiles[i].map.gameObject != null) {
                    GameObject.DestroyImmediate(m_DecorationTiles[i].map.gameObject);
                }
            }
            
            for (int i = 0; i < m_DecorationTiles.Count; i++) {

                // Create a new map.
                Tilemap decorMap = Instantiate(m_DecorationMap.gameObject).GetComponent<Tilemap>();
                decorMap.GetComponent<TilemapRenderer>().sortingOrder += m_DecorationTiles[i].order;
                decorMap.gameObject.SetActive(true);
                decorMap.transform.SetParent(m_DecorationMap.transform.parent);
                decorMap.transform.localPosition = m_DecorationMap.transform.localPosition;

                m_DecorationTiles[i].map = decorMap;
            }

            for (int i = 0; i < sections.Count; i++) {
                GenerateDecorationSection(sections[i], layerName);
            }
            

            // super inefficient but whatever.
            // Vector3Int max = m_DecorationMap.cellBounds.max;
            // VectorInt min = m_DecorationMap.cellBounds.min;

            // //
            // TileBase tile = m_DecorationTiles[0];
            // for (int x = min.x; x < max.x; x++) {
            // for (int y = min.y; y < max.y; y++) {
            //     m_DecorationMap.SetTile(new Vector3Int(x, y, 0), tile);
            // }
            // }

            // for (int i = 0; i < sections.Count; i++) {
            //     GenerateDecorationSection(sections[i], layerName);
            // }

            RefreshCollisionMap();
        }

        public void RefreshCollisionMap() {
            if (m_CollisionMap == null) { return; }

            m_CollisionMap.ClearAllTiles();
            for (int i = 0; i < m_DecorationTiles.Count; i++) {
                if (m_DecorationTiles[i].map != null) {
                    foreach (Vector3Int pos in m_DecorationTiles[i].map.cellBounds.allPositionsWithin) {
                        if (m_DecorationTiles[i].map.HasTile(pos)) {
                            m_CollisionMap.SetTile(pos, m_CollisionTile);
                        }
                    }
                }
            }
            
        }

        public void GenerateDecorationSection(LevelSection section, string layerName) {
            List<LDtkTileData> tileData = LDtkReader.GetLayerData(section.ldtkLevel, layerName);

            for (int i = 0; i < tileData.Count; i++) {
                LDtkTileEntity tileEntity = m_DecorationTiles.Find(tileEnt => tileEnt.vectorID == tileData[i].vectorID);
                if (tileEntity != null && tileEntity.map != null) {
                    Vector3Int tilePosition = section.GridToTilePosition(tileData[i].gridPosition);
                    tileEntity.map.SetTile(tilePosition, tileEntity.tile);
                }
            }

            for (int i = 0; i < m_DecorationTiles.Count; i++) {
                if (m_DecorationTiles[i].map != null) {
                    m_DecorationTiles[i].map.RefreshAllTiles();
                }
            }

        }
        
    }
    
}