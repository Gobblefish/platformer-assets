// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using LDtkUnity;

namespace Platformer.Tests {

    public class RippleTest : MonoBehaviour {

        // public Material mat;

        public UnityEngine.U2D.SpriteShapeRenderer spriteShapeRenderer;
        public bool resetTime;
        public bool resetPos;

        public void SetPositionToTransform(Transform transform) {
            spriteShapeRenderer.materials[1].SetVector("_RipplePos", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0f));
            
            float time = Shader.GetGlobalVector("_Time")[1];
            spriteShapeRenderer.materials[1].SetFloat("_StartTime", time);
        }

        public void SetPositionToMainPlayer() {
            if (Platformer.PlayerManager.Instance != null) {
                SetPositionToTransform(Platformer.PlayerManager.Character.transform);
            }
        }

    }

}