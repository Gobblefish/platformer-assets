// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
// Platformer.
using Platformer.Levels;
using Platformer.Entities.Components;
using Platformer.Entities.Utility;
using SearchDirection = Platformer.Entities.Components.Elongatable.SearchDirection;

namespace Platformer.Levels.LDtk {

    public static class LDtkElongate {

        public static void SetLength(this LDtkEntity entity, List<LDtkTileData> entityData) {
            Elongatable elongatable = entity.GetComponent<Elongatable>();
            if (elongatable != null) {
                int length = GetLength(elongatable.searchDirection, entity, entityData);
                elongatable.SetLength(length);
            }
        }

        public static int GetLength(SearchDirection searchDirection, LDtkEntity entity, List<LDtkTileData> entityData) {

            Vector2Int search = Vector2Int.right;
            if (searchDirection == SearchDirection.Vertical) {
                search = Vector2Int.down;
            }

            LDtkTileData opposite = entityData.Find(tile => tile.gridPosition == entity.GridPosition - search && tile.vectorID == entity.VectorID);
            if (opposite != null) {
                return -1;
            }

            int length = 0;
            while (length < 50) {
                length += 1;
                LDtkTileData found = entityData.Find(tile => tile.gridPosition == entity.GridPosition + length * search && tile.vectorID == entity.VectorID);
                if (found == null) {
                    break;
                }
            }

            return length;

        }

    }

}