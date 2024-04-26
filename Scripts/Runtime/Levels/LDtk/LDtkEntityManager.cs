/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
//
using Platformer.Entities;
using Platformer.Entities.Utility;

namespace Platformer.Levels.LDtk {

    [ExecuteInEditMode]
    public class LDtkEntityManager : MonoBehaviour {

        [System.Serializable]
        public class LDtkEntitySet {
            public string name;
            public List<LDtkEntity> collection;
        }

        // A list of all the reference entities.
        [SerializeField] 
        private List<LDtkEntitySet> m_LDtkEntities;
        private List<LDtkEntity> m_References = new List<LDtkEntity>();

        // A list of all the entities.
        public List<LDtkEntity> All => m_References;

        //
        public StaticAlternator staticAlternator;

        public void CollectReferences() {
            m_References = new List<LDtkEntity>();
            for (int i = 0; i < m_LDtkEntities.Count; i++) {
                foreach (LDtkEntity ldtkEnt in m_LDtkEntities[i].collection) {
                    m_References.Add(ldtkEnt);
                }
            }
        }

        public LDtkEntity GetEntityByVectorID(Vector2Int vectorID) {
            for (int i = 0; i < m_References.Count; i++) {
                if (m_References[i].VectorID == vectorID) {
                    return m_References[i];
                }
                else {
                    if (m_References[i].Rotations != null && m_References[i].Rotations.Count > 0) {
                        for (int j = 0; j < m_References[i].Rotations.Count; j++) {
                            if (m_References[i].Rotations[j].VectorID == vectorID) {
                                m_References[i].SetCurrentVectorID(vectorID);
                                return m_References[i];
                            }
                        }
                    }
                }
            }
            return null;
        }

        // Generate a entities from a list.
        public List<LDtkEntity> Generate(LevelSection section, LDtkLayers ldtkLayers) {

            // entities, entityData, controlData, entityReferences, transform, worldPosition

            List<LDtkTileData> entityData = LDtkReader.GetLayerData(section.ldtkLevel, ldtkLayers.Entity);
            List<LDtkTileData> pathData = LDtkReader.GetLayerData(section.ldtkLevel, ldtkLayers.Path);
            List<LDtkTileData> altData = LDtkReader.GetLayerData(section.ldtkLevel, ldtkLayers.Alternate);

            List<LDtkEntity> entities = new List<LDtkEntity>();

            for (int i = 0; i < entityData.Count; i++) {

                // Check whether this exact entity has already been loaded.
                // bool preloaded = section.entities.Find(ent => ent.VectorID == entityData[i].vectorID && ent.GridPosition == entityData[i].gridPosition) != null;
                bool preloaded = false;

                // Get the reference entity.
                LDtkEntity entity = GetEntityByVectorID(entityData[i].vectorID);
                if (entity != null && !preloaded) {
                    // Swap the reference entity for a duplicated entity.
                    entity = entity.Duplicate(section.transform);
                    entity.SetGridValues(entityData[i].gridPosition, entityData[i].gridSize);

                    //
                    entity.SetRotation();
                    entity.SetPosition(section.WorldPosition);
                    entity.SetPath(pathData);
                    entity.SetLength(entityData); // Can possible destroy this entity.
                    
                    entity.SetAlternate(altData, staticAlternator); // Can possible destroy this entity.

                    //
                    if (entity != null && entity.gameObject != null) {
                        entities.Add(entity);
                        entity.gameObject.SetActive(true);
                        if (entity.GetComponent<Platformer.Tests.GetRoomName>() != null) {
                            entity.GetComponent<Platformer.Tests.GetRoomName>().SetRoomName(section.name);
                        }
                    }

                }
            }

            Debug.Log(entities.Count);
   
            return entities;

        }

        // Destroy and clean through a list of entities.
        public static List<LDtkEntity> Destroy(List<LDtkEntity> entities) {
            if (entities == null || entities.Count == 0) { return entities; }
            
            for (int i = 0; i < entities.Count; i++) {
                bool destroy = entities[i] != null && entities[i].Unloadable;
                if (destroy) {
                    Destroy(entities[i].gameObject);
                }
            }
            return entities;
        }

    }
    
}