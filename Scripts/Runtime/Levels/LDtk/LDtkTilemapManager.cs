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
    }

    /// <summary>
    /// Stores specific data on how to generate the level.
    /// </summary>
    public class LDtkTilemapManager : MonoBehaviour {

        public class TilemapSection {
            public Tilemap decor;
            public Tilemap collision;
            public LDtkSection lDtkSection;
            public TilemapSection(LDtkSection section, Tilemap decor, Tilemap collision) {
                this.lDtkSection = section;
                this.decor = decor;
                this.collision = collision;
            }
            public void Destroy() {
                if (decor.gameObject != null) { decor.gameObject.DestroyAppropriately(); }
                if (collision.gameObject != null) { collision.gameObject.DestroyAppropriately(); }
            }
        }

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

        [SerializeField]
        private List<TilemapSection> m_MapSections = new List<TilemapSection>();

        [SerializeField]
        private bool m_GenerateSeperately;

        // [SerializeField]
        // private List<DecorationMapParams> m_DecorMapParams = new List<DecorationMapParams>();
        
        // Loads the map layouts for all the given levels.
        public void Refresh(List<LDtkSection> sections, string layerName) {

            m_DecorationMap.ClearAllTiles();
            m_CollisionMap.ClearAllTiles();

            for (int i = 0; i < m_MapSections.Count; i++) {
                m_MapSections[i].Destroy();
            }
            m_MapSections = new List<TilemapSection>();

            if (m_GenerateSeperately) {
                for (int i = 0; i < sections.Count; i++) {
                    m_MapSections.Add(Generate(sections[i], layerName, null, null));
                }
            }
            else {
                for (int i = 0; i < sections.Count; i++) {
                    Generate(sections[i], layerName, m_DecorationMap, m_CollisionMap);
                }
            }
        }

        public TilemapSection Generate(LDtkSection section, string layerName, Tilemap decorMap = null, Tilemap collisionMap = null) {

            // Create a new map.
            if (decorMap == null) {
                decorMap = Instantiate(m_DecorationMap.gameObject).GetComponent<Tilemap>();
                decorMap.gameObject.SetActive(true);
            }
            
            if (collisionMap == null) {
                collisionMap = Instantiate(m_CollisionMap.gameObject).GetComponent<Tilemap>();
                collisionMap.gameObject.SetActive(true);
            }
            
            List<LDtkTileData> tileData = LDtkReader.GetLayerData(section.ldtkLevel, layerName);
            for (int i = 0; i < tileData.Count; i++) {
                LDtkTileEntity tileEntity = m_DecorationTiles.Find(tileEnt => tileEnt.vectorID == tileData[i].vectorID);
                if (tileEntity != null) {
                    Vector3Int tilePosition = section.GridToTilePosition(tileData[i].gridPosition);
                    decorMap.SetTile(tilePosition, tileEntity.tile);
                    collisionMap.SetTile(tilePosition, m_CollisionTile);
                }
            }

            decorMap.RefreshAllTiles();
            collisionMap.RefreshAllTiles();

            return new TilemapSection(section, decorMap, collisionMap);

        }

    }
    
}