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
    /// A useful data structure for reading and matching LDtk data to game data.
    ///<summary>
    public class LDtkTileData {

        public Vector2Int vectorID {  get; private set; } = new Vector2Int(0, 0);
        public Vector2Int gridPosition {  get; private set; } = new Vector2Int(0, 0);
        public int index { get; private set; } = 0;
        public int gridSize { get; private set; } = 0;

        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index, int gridSize) {
            this.vectorID = vectorID;
            this.gridPosition = gridPosition;
            this.index = index;
            this.gridSize = gridSize;
        }

    }

}
    