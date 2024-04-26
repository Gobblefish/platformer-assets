/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// Platformer.
using Platformer.Physics;
using Gobblefish.Animation;
using Platformer.Levels;

namespace Platformer.Graphics {

    public class Starmap : MonoBehaviour {

        public const int texWidth = 256;
        public const int texHeight = 256;

        public float fX;
        public float fY;

        Vector2 min = new Vector2(1e9f, 1e9f);
        Vector2 max = new Vector2(-1e9f, -1e9f);

        public Renderer _renderer;
        public Texture2D tex;

        // Runs once before the first frame.
        void Start() {

            transform.SetParent(null);
            // tex = new Texture2D(texWidth, texHeight);

            /* --- */

            LevelManager levelManager = LevelManager.Instance;
            if (levelManager != null) {

                min = new Vector2(1e9f, 1e9f);
                max = new Vector2(-1e9f, -1e9f);

                foreach (LevelSection section in levelManager.Sections) {

                    print(section);
                    
                    // float x = section.WorldCenter.x - (float)section.Width / 2f;
                    // float y = section.WorldCenter.y - (float)section.Height / 2f;
                    BoxCollider2D box = section.GetComponent<BoxCollider2D>();
                    float x = box.offset.x - box.size.x;
                    float y = box.offset.y - box.size.y;

                    if (x < min.x) {
                        min.x = x;
                    }
                    if (y < min.y) {
                        min.y = y;
                    }

                    x = box.offset.x + box.size.x;
                    y = box.offset.y + box.size.y;

                    if (x > max.x) {
                        max.x = x;
                    }
                    if (y > max.y) {
                        max.y = y;
                    }
                
                }

                print(min);
                print(max);

            }

            /* --- */

            fX = (float)texWidth / (max.x - min.x);
            fY = (float)texHeight / (max.y - min.y);
            positions = new List<Vector2Int>();

            // _renderer.material.mainTexture = tex;

        }

        public List<Vector2Int> positions = new List<Vector2Int>();
        public void AddPoint(Vector3 position) {
            positions.Add(new Vector2Int((int)(fX * (position.x - min.x)), (int)(fY * (position.y - min.y))));
            SetTex();
        }

        public void SetTex() {
            // Texture2D tex = (Texture2D)_renderer.material.mainTexture;

            Color blank = new Color(0f, 0f, 0f, 0f);

            for (int y = 0; y < tex.height; y++) {
                for (int x = 0; x < tex.width; x++) {

                    Color color = positions.Contains(new Vector2Int(x, y)) ? Color.white : blank;
                    tex.SetPixel(x, y, color);

                }
            }

            tex.Apply();

        }

        void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position, new Vector3(texWidth, texHeight, 1));
            Gizmos.DrawWireCube(new Vector3(max.x + min.x, max.y + min.y, 1f) / 2f, new Vector3(max.x - min.x, max.y - min.y, 1));

            for (int i = 0; i < positions.Count; i++) { 
                Gizmos.DrawWireCube(transform.position + (Vector3)(Vector2)positions[i] - new Vector3(texWidth / 2f, texHeight / 2f, 1), new Vector3(10f, 10f, 1f));
            }

        }

    }

}
