/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Levels.LDtk {

    [System.Serializable]
    public class RotationID {
        public float Rotation;
        public Vector2Int VectorID;
    }

    /// <summary>
    /// An entity object readable by the level loader.
    /// <summary>
    public class LDtkEntity : MonoBehaviour {

        // Whether this entity should be duplicated or taken directly.       
        [SerializeField] 
        private bool m_Singular = false;
        public bool Singular => m_Singular;

        // Whether this entity can be unloaded.
        [SerializeField] 
        private bool m_Unloadable = true;
        public bool Unloadable => m_Unloadable;

        // The mapping from LDtk to this entity.
        [SerializeField] 
        protected Vector2Int m_VectorID = new Vector2Int(0, 0);
        public Vector2Int VectorID => m_VectorID;

        // The grid space coordinates this entity should be loaded at.
        [SerializeField, ReadOnly] 
        private Vector2Int m_GridPosition = new Vector2Int(0, 0);
        public Vector2Int GridPosition => m_GridPosition;

        [SerializeField]
        private int m_GridSize = 16;
        public int GridSize => m_GridSize;

        [SerializeField]
        private List<RotationID> m_Rotations = new List<RotationID>();
        public List<RotationID> Rotations => m_Rotations;

        public void SetGridValues(Vector2Int gridPosition, int gridSize) {
            m_GridPosition = gridPosition;
            m_GridSize = gridSize;
        }

        public void SetCurrentVectorID(Vector2Int vectorID) {
            m_VectorID = vectorID;
        }

        public void SetPosition(Vector2Int roomOrigin) {
            Vector3 worldPosition = LevelSection.GridToWorldPosition(m_GridPosition, roomOrigin, m_GridSize);      
            transform.position = worldPosition;
        }

        public void SetRotation() {
            RotationID rotationID = m_Rotations.Find(rotationID => rotationID.VectorID == m_VectorID);
            float rotation = 0f;
            if (rotationID != null) {
                rotation = rotationID.Rotation;
            }

            // if (rotation == 180f) {
            //     transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            // }

            transform.eulerAngles = Vector3.forward * rotation;
        }

        // Duplicate an entity.
        public LDtkEntity Duplicate(Transform parent) {
            LDtkEntity entity = Instantiate(gameObject, Vector3.zero, transform.localRotation, parent).GetComponent<LDtkEntity>();
            if (Singular) {
                Destroy(gameObject);
            }
            return entity;
        }

    }

}
