/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;

namespace Platformer.Levels.LDtk {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-10000)]
    public class LDtkReader : MonoBehaviour {

        // Grid Size
        public const int GRID_SIZE = 16;

        // Handles all the tilemap functionality.
        [SerializeField] 
        private LDtkEntityManager m_LDtkEntityManager;

        // Handles all the tilemap functionality.
        [SerializeField] 
        private LDtkTilemapManager m_LDtkTilemapManager;

        // The JSON data corresponding to the given ldtk data.
        [SerializeField]
        private LDtkLayers m_LDtkLayers = new LDtkLayers();

        [SerializeField]
        private List<LDtkSection> m_Sections = new List<LDtkSection>();
        public List<LDtkSection> Sections => m_Sections;

        // The given LDtk file.
        [SerializeField] 
        private LDtkComponentProject m_LDtkData;
        public static LDtkComponentProject setData = null;

        [Header("Controls")]
        public bool dont = false;
        public bool m_Reload;
        public bool playerStarted = false;

        // The JSON data corresponding to the given ldtk data.
        private LdtkJson m_JSON;

        void OnEnable() {
            m_Reload = false;
            if (!dont) {
                if (!Application.isPlaying) {
                    OnReload();
                }
            }
            
            m_LDtkData = setData == null ? m_LDtkData : setData;
            print(m_LDtkData == null);

            if (dont && setData == null) { return; }

            OnReload();
        }

        void Update() {
            if (m_Reload && !Application.isPlaying) {
                OnReload();
                m_Reload = false;
            }
        }

        public void OnReload() {
            m_JSON = m_LDtkData.FromJson();
            m_LDtkEntityManager.CollectReferences();
            m_LDtkEntityManager.staticAlternator.Refresh();

            m_Sections = CollectSections(m_JSON);
            Debug.Log("Number of sections: " + m_Sections.Count.ToString());
            Debug.Log("Number of entity refs: " + m_LDtkEntityManager.All.Count);

            m_LDtkTilemapManager.Refresh(m_Sections, m_LDtkLayers.Ground);    
        }

        // Collects all the levels from the LDtk file.
        private List<LDtkSection> CollectSections(LdtkJson json) {
            List<LDtkSection> sections = new List<LDtkSection>();
            for (int i = 0; i < json.Levels.Length; i++) {
                LDtkSection section = LDtkSection.New(i, json);
                section.transform.parent = transform;
                section.DestroyEntities();
                section.GenerateEntities(m_LDtkEntityManager, m_LDtkLayers);
                sections.Add(section);
            }
            return sections;
        }

        public static List<LDtkTileData> GetLayerData(LDtkUnity.Level ldtkLevel, string layerName) {
            List<LDtkTileData> layerData = new List<LDtkTileData>();

            LDtkUnity.LayerInstance layer = GetLayerInstance(ldtkLevel, layerName);
            if (layer != null) { 
                for (int index = 0; index < layer.GridTiles.Length; index++) {
                    LDtkUnity.TileInstance tile = layer.GridTiles[index];
                    LDtkTileData tileData = new LDtkTileData(GetVectorID(tile), GetGridPosition(tile, (int)layer.GridSize), index, (int)layer.GridSize);
                    layerData.Add(tileData);
                }
            }
            return layerData;
        }

        public static LDtkUnity.LayerInstance GetLayerInstance(LDtkUnity.Level ldtkLevel, string layerName) {
            for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
                LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
                if (layer.IsTilesLayer && layer.Identifier == layerName) {
                    return layer;
                }
            }
            return null;
        }

        private static Vector2Int GetVectorID(LDtkUnity.TileInstance tile) {
            return new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / GRID_SIZE;
        }

        private static Vector2Int GetGridPosition(LDtkUnity.TileInstance tile, int gridSize) {
            return tile.UnityPx / gridSize;
        }

        protected static Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
            LDtkTileData tileData = data.Find(tileData => tileData != null && tileData.gridPosition == gridPosition);
            return (Vector2Int?)tileData?.vectorID;
        }

    } 

}
    