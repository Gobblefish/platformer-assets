/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Levels;
using Platformer.Entities.Components;
using Platformer.Entities.Utility;

namespace Platformer.Levels.LDtk {

    public struct LDtkPath {

        public Vector2Int origin;
        public List<Vector3> nodes;
        public int speedLevel;

        public void Add(Vector2Int node) {
            Vector3 v = (Vector3)(Vector2)(node - origin);
            v.y *= -1f;
            nodes.Add(v);
        }

        public void Add(Vector3 node) {
            nodes.Add(node);
        }

        public LDtkPath(Vector2Int origin) {
            this.origin = origin;
            this.nodes = new List<Vector3>();
            this.speedLevel = 0;
            Add(origin);
        }
    } 

    public static class LDtkPathing {

        // public const float[] SPEED = new float[] { 2f, 4f }
        // public const float[] PAUSE new float[] { 0.5f, 0.25f }

        // The stop node value for pathing.
        public const int STOP_NODE = 4;
        public static Vector2Int CATCH_ALL_STOP_NODE = new Vector2Int(1, 0);

        public static void SetPath(this LDtkEntity entity, List<LDtkTileData> pathData) {
            LDtkTileData pathTile = pathData.Find(pathTile => pathTile.gridPosition == entity.GridPosition);
            if (pathTile != null) {
                Pathing pathing = entity.AddPath();
                LDtkPath lDtkPath = pathTile.GetPath(pathData);
                SetPathFromLDtk(pathing, lDtkPath);
            }
        }

        public static void SetPathFromLDtk(Pathing pathing, LDtkPath ldtkPath) {
            List<PathingNode> pathingNodes = new List<PathingNode>();
            for (int i = 0; i < ldtkPath.nodes.Count; i++) {
                pathingNodes.Add(PathingNode.Create(pathing.transform, pathing.transform.position + ldtkPath.nodes[i]));
            }
            pathing.SetPath(pathingNodes, ldtkPath.speedLevel, 4f);
        }

        public static Pathing AddPath(this LDtkEntity entity) {
            Pathing newPath = new GameObject(entity.gameObject.name + " Path", typeof(Pathing)).GetComponent<Pathing>();
            // Attach the entity under the path.
            newPath.transform.SetParent(entity.transform.parent);
            newPath.transform.localPosition = entity.transform.localPosition;
            entity.transform.SetParent(newPath.transform);
            newPath.SetEntity(entity.GetComponent<Platformer.Entities.Entity>());
            return newPath;
        }

        // The logic of turning the ldtk data into a path.
        public static LDtkPath GetPath(this LDtkTileData pathTile, List<LDtkTileData> pathData) {
            LDtkPath ldtkPath = new LDtkPath(pathTile.gridPosition);

            // Cache the direction of the path.
            // int speed = 0;
            int xOffset = 0;
            int speedLevel = 0;
            if (pathTile.vectorID.x == 4) { 
                return ldtkPath; 
            }
            
            if (pathTile.vectorID.x >= 5) {
                xOffset = 5 * (int)Mathf.Floor((float)pathTile.vectorID.x / 5f);
                // Debug.Log("X Offset " + xOffset.ToString());
                speedLevel = (int)Mathf.Floor((float)pathTile.vectorID.x / 5f);
            }

            int controlColor = pathTile.vectorID.y;
            Vector2Int gridDirection = GetGridDirection(pathTile, xOffset);

            // Cache all the stop nodes in the scene.
            Vector2Int colouredStopNode = new Vector2Int(STOP_NODE, controlColor);

            // Itterate in the direction of the path until reaching a stop node.
            Vector2Int currentOffset = Vector2Int.zero;
            Vector2Int currentPosition = pathTile.gridPosition;
            int distance = 0; 
            bool continueSearch = true;
            while (continueSearch && distance < 50) {

                distance += 1;

                // if (xOffset > 4) { Debug.Log("X Offset " + distance.ToString()); }

                currentOffset += gridDirection;
                currentPosition = pathTile.gridPosition + currentOffset;
                if (currentPosition == pathTile.gridPosition) {
                    continueSearch = false;
                    ldtkPath.Add(currentPosition);
                    break;
                }


                LDtkTileData control = pathData.Find(control => control.gridPosition == currentPosition);
                if (control != null) {
                    if (control.vectorID.y == controlColor || control.vectorID == CATCH_ALL_STOP_NODE) {

                        if (control.vectorID == colouredStopNode || control.vectorID == CATCH_ALL_STOP_NODE) {
                            ldtkPath.Add(currentPosition);

                            if (ldtkPath.nodes.Count > 2) {
                                for (int i = ldtkPath.nodes.Count - 2; i >= 0; i--) {
                                    ldtkPath.Add(ldtkPath.nodes[i]);
                                }
                            }

                            continueSearch = false;
                            break;
                        }
                        else {
                            gridDirection = GetGridDirection(control, xOffset);
                            ldtkPath.Add(currentPosition);
                        }

                    }
                }
                
            }

            ldtkPath.speedLevel = speedLevel;

            return ldtkPath;

        }

        public static Vector2Int GetGridDirection(LDtkTileData pathTile, int xOffset) {
            Vector2 worldDirection = Quaternion.Euler(0f, 0f, -90f * (pathTile.vectorID.x - xOffset)) * Vector2.up;
            Vector2Int gridDirection = new Vector2Int((int)Mathf.Round(worldDirection.x), (int)Mathf.Round(worldDirection.y));
            gridDirection.y *= -1;
            // if (xOffset > 4) { Debug.Log(gridDirection); }
            return gridDirection;
        }
        
    }
}