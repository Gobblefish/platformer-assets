// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Levels {

    [ExecuteInEditMode]
    public class DecorationSection : MonoBehaviour {

        [SerializeField]
        private LevelSection m_LevelSection;
        public LevelSection levelSection => m_LevelSection;

        [SerializeField]
        private List<DecorationLayer> m_Layers = new List<DecorationLayer>();
        public List<DecorationLayer> layers => m_Layers;

        public static DecorationSection New(string name) {
            return new GameObject(name, typeof(DecorationSection)).GetComponent<DecorationSection>();
        }

        public void SetSection(LevelSection levelSection) {
            m_LevelSection = levelSection;
        }

        public void GetLayers() {
            m_Layers = new List<DecorationLayer>();
            DecorationLayer[] _layers = transform.GetComponentsInChildren<DecorationLayer>();
            for (int i = 0; i < _layers.Length; i++) {
                m_Layers.Add(_layers[i]);
            }

            // for (int i = 0; i < )

        }

        public void ParentToSection(Transform transform) {
            
            SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                DecorationLayer l = m_Layers.Find(_l => _l.sortingLayer == spriteRenderer.sortingLayerName);
                if (l != null) {
                    transform.SetParent(l.transform);
                }
            }

        }

    }

}
