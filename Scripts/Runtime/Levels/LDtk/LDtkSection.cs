/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// LDtk.
using LDtkUnity;

namespace Platformer.Levels.LDtk {

    public class LDtkSection : MonoBehaviour {

        public enum State {
            Loaded,
            Unloaded
        }

        // Whether this level is currently loaded.
        [SerializeField]
        private State state = State.Unloaded;  

        // The id of this level.
        [SerializeField]
        private int m_ID = 0;
        private LDtkUnity.Level m_LDtkLevel = null;
        public LDtkUnity.Level ldtkLevel => m_LDtkLevel;
        
        // The dimensions of the level.
        [SerializeField] 
        private Vector2Int m_Dimensions;
        public int Height => m_Dimensions.y;
        public int Width => m_Dimensions.x;

        // The position of the bottom left corner of the level in the world.
        [SerializeField]
        private Vector2Int m_WorldPosition;
        public Vector2Int WorldPosition => m_WorldPosition;
        public Vector2 WorldCenter => GetCenter(this.Width, this.Height, this.m_WorldPosition);

        // The entities currently loaded into the level.
        [SerializeField]
        private List<LDtkEntity> m_Entities = new List<LDtkEntity>();
        public List<LDtkEntity> Entities => m_Entities;

        // Creates a new level section camera.
        public static LDtkSection New(int jsonID, LDtkUnity.LdtkJson json) {
            LDtkSection section = new GameObject(json.Levels[jsonID].Identifier, typeof(LDtkSection)).GetComponent<LDtkSection>();
            section.Set(jsonID, json);
            return section;
        }

        public void Set(int jsonID, LDtkUnity.LdtkJson json) {
            transform.localPosition = Vector3.zero;

            m_ID = jsonID;
            m_LDtkLevel = json.Levels[jsonID];
            m_Dimensions.y = (int)(m_LDtkLevel.PxHei / json.DefaultGridSize);
            m_Dimensions.x = (int)(m_LDtkLevel.PxWid / json.DefaultGridSize);
            m_WorldPosition.y = (int)(m_LDtkLevel.WorldY / json.DefaultGridSize);
            m_WorldPosition.x = (int)(m_LDtkLevel.WorldX / json.DefaultGridSize);
            
        }

        public void GenerateEntities(LDtk.LDtkEntityManager entityManager, LDtkLayers ldtkLayers) {
            m_Entities.RemoveAll(entity => entity == null);
            m_Entities = entityManager.Generate(this, ldtkLayers);
            Debug.Log(m_Entities.Count);
            m_Entities = m_Entities.FindAll(entity => entity != null);
        }

        public void DestroyEntities() {
            if (Application.isPlaying) {
                // m_Entities = LDtkEntity.Destroy(m_Entities);
                for (int i = 0; i < m_Entities.Count; i++) {
                    Destroy(m_Entities[i].gameObject);
                }
                m_Entities.RemoveAll(entity => entity == null);
            }
            else {
                for (int i = 0; i < m_Entities.Count; i++) {
                    DestroyImmediate(m_Entities[i].gameObject);
                }
                m_Entities.RemoveAll(entity => entity == null);
            }
        }

        public static Vector2 GetCenter(int width, int height, Vector2Int gridOrigin) {
            Vector2Int origin = new Vector2Int(width / 2, height / 2);
            Vector2 offset = new Vector2( width % 2 == 0 ? 0.5f : 0f, height % 2 == 1 ? 0f : -0.5f);
            // Assuming a grid size of 16.
            return (Vector2)GridToWorldPosition(origin, gridOrigin, 16) - offset;
        }

        public Vector3 GridToWorldPosition(Vector2Int gridPosition, int gridSize) {
            return GridToWorldPosition(gridPosition, this.m_WorldPosition, gridSize);
        }
        
        public static Vector3 GridToWorldPosition(Vector2Int gridPosition, Vector2Int gridOrigin, int gridSize) {
            float ratio = (float)gridSize / (float)LDtkReader.GRID_SIZE;
            return new Vector3((ratio * gridPosition.x + gridOrigin.x) + 0.5f, - (ratio * gridPosition.y + gridOrigin.y) + 0.5f, 0f);
        }

        public Vector3Int GridToTilePosition(Vector2Int gridPosition) {
            return GridToTilePosition(gridPosition, this.m_WorldPosition);
        }

        public static Vector3Int GridToTilePosition(Vector2Int gridPosition, Vector2Int gridOrigin) {
            return new Vector3Int(gridPosition.x + gridOrigin.x, -(gridPosition.y + gridOrigin.y), 0);
        }

    }

}
