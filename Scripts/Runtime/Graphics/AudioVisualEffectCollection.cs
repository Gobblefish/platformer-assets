// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
// Gobblefish.
using Gobblefish;
using Gobblefish.Graphics;
using Gobblefish.Audio;

namespace Platformer {

    [System.Serializable]
    public class AudioVisualEffect {
        public VisualEffect visualEffect;
        public AudioSnippet audioSnippet;
    }

    [CreateAssetMenu(fileName="AudioVisualEffectCollection", menuName="Collections/AudioVisualEffect")]
    public class AudioVisualEffectCollection : Gobblefish.CollectionAsset<AudioVisualEffect> {

        public void Play(string name) {
            AudioVisualEffect av = Get(name);
            // if (av.visualEffect != null) {
            //     VisualEffect newVfx = Duplicate(av.visualEffect, transform);
            // }
            if (av.audioSnippet != null) {
                av.audioSnippet.Play();
            }

        }

        public VisualEffect Duplicate(VisualEffect visualEffect, Transform transform) {
            VisualEffect duplicatedEffect = Instantiate(visualEffect, transform).GetComponent<VisualEffect>();
            duplicatedEffect.transform.localPosition = Vector3.zero;
            duplicatedEffect.gameObject.SetActive(true);
            return duplicatedEffect;
        }

    }


}
