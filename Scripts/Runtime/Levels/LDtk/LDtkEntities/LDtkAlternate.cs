// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
// Platformer.
using Platformer.Levels;
using Platformer.Entities;
using Platformer.Entities.Components;
using Platformer.Entities.Utility;
using AlternatingType = Platformer.Entities.Utility.StaticAlternator.AlternatingType;

namespace Platformer.Levels.LDtk {

    public static class LDtkAlternate {

        public static Vector2Int A_NODE = new Vector2Int(0, 0);
        public static Vector2Int B_NODE = new Vector2Int(1, 0);

        public static void SetAlternate(this LDtkEntity entity, List<LDtkTileData> altData, StaticAlternator alternator) {
            LDtkTileData altTile = altData.Find(altTile => altTile.gridPosition == entity.GridPosition);
            if (altTile != null) {
                alternator.Add(entity.gameObject.GetComponent<Entity>(), altTile.vectorID.x, altTile.vectorID.y);
            }
        }

    }

}