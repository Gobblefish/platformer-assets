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

        // Handles all the tilemap functionality.
        [SerializeField] 
        private LevelManager m_LevelManager;

        // The JSON data corresponding to the given ldtk data.
        [SerializeField]
        private LDtkLayers m_LDtkLayers = new LDtkLayers();

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

            // if (setData != null) {
            //     PlayerManager playerManager = 
            // }
            if (dont && setData == null) { return; }


            OnReload();
        }

        // void Awake() {
        //     print("hi");
        //     m_LDtkData = setData == null ? m_LDtkData : setData;
        //     OnReload();
        // }

        void Update() {
            if (m_Reload && !Application.isPlaying) {
                OnReload();
                m_Reload = false;
            }

            if (setData != null && !playerStarted) {
                print("trying");
                LDtkEntity entity = m_LevelManager.Sections.Find(section => section.gameObject.name == "START").Entities.Find(entity => entity.GetComponent<Platformer.Entities.Components.Respawn>() != null);
                if (entity != null) {
                    Platformer.PlayerManager.Character.transform.position = entity.transform.position + Vector3.up * 0.5f;
                    playerStarted = true;
                }
            }
        }

        public void OnReload() {
            m_JSON = m_LDtkData.FromJson();
            m_LDtkEntityManager.CollectReferences();

            List<LevelSection> sections = CollectSections(m_JSON);
            Debug.Log("Number of sections: " + sections.Count.ToString());
            Debug.Log("Number of entity refs: " + m_LDtkEntityManager.All.Count);

            m_LDtkTilemapManager.Refresh(sections, m_LDtkLayers.Ground);    

            m_LDtkEntityManager.staticAlternator.Refresh();
            for (int i = 0; i < sections.Count; i++) {
                sections[i].transform.parent = transform;
                sections[i].DestroyEntities();
                sections[i].GenerateEntities(m_LDtkEntityManager, m_LDtkLayers);
            }

            m_LevelManager.SetSections(sections);            
        }

        // Collects all the levels from the LDtk file.
        private static List<LevelSection> CollectSections(LdtkJson json) {
            List<LevelSection> sections = new List<LevelSection>();
            for (int i = 0; i < json.Levels.Length; i++) {
                LevelSection section = LevelSection.New(i, json);
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
    