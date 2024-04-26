/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

/* --- Definitions --- */
using IReset = Platformer.Entities.Utility.IReset;
using CharacterController = Platformer.Character.CharacterController;
// using TrailAnimator = Gobblefish.Animation.TrailAnimator;

namespace Platformer.Entities.Components {

    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteRenderer))]
    public class Floating : MonoBehaviour {

        //
        private BoxCollider2D m_BoxCollider;
        
        //
        private Rigidbody2D m_Body;

        void Start() {
            m_BoxCollider = GetComponent<BoxCollider2D>();
            m_Body = GetComponent<Rigidbody2D>();
            m_Body.gravityScale = 0f;
            m_Body.angularDrag = 0.05f;
            Freeze();
        }

        void Update() {
            Release();
            m_Body.velocity *= 0.99f;
        }

        protected virtual void Release() {
            m_Body.constraints = RigidbodyConstraints2D.None;
            m_Body.gravityScale = 0f;
        }

        protected virtual void Freeze() {
            m_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            m_Body.gravityScale = 0f;
        }
        
    }

}

        // public List<Rigidbody2D> ConvertToBlocks(BoundsInt bounds) {

        //     List<Rigidbody2D> bodies = new List<Rigidbody2D>();
        //     for (int i = bounds.yMin; i <= bounds.yMax; i++) {
        //         for (int j = bounds.xMin; j <= bounds.xMax; j++) {
        //             Vector3Int position = new Vector3Int(j, i, 0);
        //             Sprite sprite = m_LevelMap.GetSprite(position);
        //             if (sprite != null) {
        //                 Rigidbody2D body = CreateBlock(sprite, position);
        //                 print(body.gameObject.name);
        //                 bodies.Add(body);
        //             }
        //         }
        //     }

        //     for (int i = bounds.yMin; i <= bounds.yMax; i++) {
        //         for (int j = bounds.xMin; j <= bounds.xMax; j++) {
        //             Vector3Int pos = new Vector3Int(j, i, 0);
        //             m_LevelMap.SetTile(pos, null);
        //             m_CollisionMap.SetTile(pos, null);
        //         }
        //     }

        //     return bodies;
        // }

        // public Rigidbody2D CreateBlock(Sprite sprite, Vector3Int position) {
        //     // Create the block.
        //     GameObject newObject = Instantiate(m_CollisionBlock);
        //     newObject.name = "Block " + position.ToString();
        //     newObject.transform.position = m_Grid.GetCellCenterWorld(position);
        //     newObject.GetComponent<SpriteRenderer>().sprite = sprite;
        //     // Set the transform.
        //     Matrix4x4 matrix = m_LevelMap.GetTransformMatrix(position);
        //     newObject.transform.FromMatrix(matrix);
        //     // Set the block active.
        //     newObject.SetActive(true);
        //     return newObject.GetComponent<Rigidbody2D>();
        // }