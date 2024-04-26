using UnityEngine;
using System.Collections;
using Gobblefish.Graphics;

namespace Gobblefish.Graphics {

    // For usage apply the script directly to the element you wish to apply parallaxing
    // Based on Brackeys 2D parallaxing script http://brackeys.com/
    public class Parallax : MonoBehaviour {

        Transform cam; // Camera reference (of its transform)
        Vector3 previousCamPos;

        public float distanceX = 0f; // Distance of the item (z-index based)
        public float distanceY = 0f;

        public float smoothingX = 1f; // Smoothing factor of parrallax effect
        public float smoothingY = 1f;

        public bool tileX;

        Transform mostRight;
        Transform mostLeft;

        void Awake () {
            cam = Camera.main.transform;
            mostRight = transform;
            mostLeft = transform;
        }
        
        void Update () {
            if (distanceX != 0f) {
                float parallaxX = (previousCamPos.x - cam.position.x) * distanceX;
                Vector3 backgroundTargetPosX = transform.position + Vector3.right * parallaxX; 
                transform.position = Vector3.Lerp(transform.position, backgroundTargetPosX, smoothingX * Time.deltaTime);
            }

            if (distanceY != 0f) {
                float parallaxY = (previousCamPos.y - cam.position.y) * distanceY;
                Vector3 backgroundTargetPosY = transform.position + Vector3.up * parallaxY; 
                transform.position = Vector3.Lerp(transform.position, backgroundTargetPosY, smoothingY * Time.deltaTime);
            }

            previousCamPos = cam.position;	

            if (mostLeft != transform || mostRight != transform) {
                return;
            }

            if (tileX) {

                (Vector2, Vector2) corners = GraphicsManager.MainCamera.GetCorners();
                Vector2 minCorner = corners.Item1;
                Vector2 maxCorner = corners.Item2;

                float maxX = mostRight.GetComponent<SpriteRenderer>().bounds.max.x;
                float minX = mostLeft.GetComponent<SpriteRenderer>().bounds.min.x;

                if (maxX < maxCorner.x) {
                    CreateNewRightSprite();
                    print("maxX is less than camera");
                }
                if (minX > minCorner.x) {
                    CreateNewLeftSprite();
                    print("minX is greater than camera");
                }

            }
        }

        public void CreateNewRightSprite() {
            GameObject newObject = NewObject(mostRight);
            newObject.transform.localPosition = Vector3.right * mostRight.GetComponent<SpriteRenderer>().bounds.size.x / mostRight.localScale.x;
            mostRight = newObject.transform;
        }

        public void CreateNewLeftSprite() {
            GameObject newObject = NewObject(mostLeft);
            newObject.transform.localPosition = Vector3.left * mostRight.GetComponent<SpriteRenderer>().bounds.size.x / mostLeft.localScale.x;
            mostLeft = newObject.transform;
        }

        public GameObject NewObject(Transform parent) {
            // GameObject newObject = new GameObject("", typeof(SpriteRenderer));
            // newObject.transform.SetParent(transform); 
            // newObject.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            GameObject newObject = Instantiate(gameObject, parent);
            newObject.GetComponent<Parallax>().enabled = false;
            newObject.transform.localScale = new Vector3(1f, 1f, 1f);
            return newObject;
        }

    }

}

