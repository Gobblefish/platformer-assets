/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Gobblefish;

namespace Platformer.Levels {

    ///<summary>
    ///
    ///<summary>
    [ExecuteInEditMode]
    public class DecorationLayer : MonoBehaviour {

        [SerializeField]
        private Material m_Material;

        [SerializeField]
        private string m_SortingLayer;
        public string sortingLayer => m_SortingLayer;

        [SerializeField]
        private Color m_Color = new Color(1f, 1f, 1f, 1f);
        
        [SerializeField]
        private int m_SortingOrderOffset;
        private const int ORDER_OFFSET_PER_CHILD = -10;

        [SerializeField]
        private Decoration[] m_Decorations;

        [SerializeField]
        private SpriteRenderer[] m_SpriteRenderers;

        void Update() {
            if (!Application.isPlaying) {
                SetName();
                if (DecorationEditor.CurrentLayer == this) { 
                    ParentAllDecorations(transform);
                    ParentAllSpriteRenderers(transform);
                }

                // m_Decorations = transform.GetComponentsInChildren<Decoration>();
                m_SpriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();

                int i = m_SortingOrderOffset;
                foreach (SpriteRenderer spriteRenderer in m_SpriteRenderers) {
                    if (spriteRenderer != null) {
                        i += ORDER_OFFSET_PER_CHILD;
                        EditSpriteRenderer(spriteRenderer, m_Material, m_SortingLayer, i, m_Color);
                    }
                }

            }
            
        }

        void SetName() {
            string matName = m_Material != null ? m_Material.name : "default";
            string roomName = transform.parent != null ? transform.parent.gameObject.name : "N/A";
            gameObject.name = roomName + " " + m_SortingLayer + " " + m_SortingOrderOffset.ToString(); // " (" + matName + ")";
        }

        public void ParentAllDecorations(Transform parent) {
            Decoration[] allObjects = (Decoration[])GameObject.FindObjectsOfType<Decoration>();
            for (int i = 0; i < allObjects.Length; i++) {
                if (allObjects[i].transform.parent == null) {
                    allObjects[i].transform.SetParent(transform);
                }
            }

        }

        public void ParentAllSpriteRenderers(Transform parent) {
            SpriteRenderer[] allObjects = (SpriteRenderer[])GameObject.FindObjectsOfType<SpriteRenderer>();
            for (int i = 0; i < allObjects.Length; i++) {
                if (allObjects[i].transform.parent == null) {
                    allObjects[i].transform.SetParent(transform);
                }
            }
        }

        void EditSpriteRenderer(SpriteRenderer spriteRenderer, Material mat, string sortingLayer, int sortingOrder, Color color) {
            if (spriteRenderer != null) {
                // spriteRenderer.sharedMaterial = DecorationOverhead.GetMaterial(spriteRenderer, mat);
                spriteRenderer.sortingLayerName = sortingLayer;
                spriteRenderer.sortingOrder = sortingOrder;
                spriteRenderer.color = color;
            }
        }

        void OnDrawGizmos() {
            for (int i = 0; i < m_SpriteRenderers.Length; i++) {
                Bounds bounds = m_SpriteRenderers[i].bounds;
                Gizmos.DrawWireCube(bounds.center, bounds.extents * 2f);
            }
        }

    }

}