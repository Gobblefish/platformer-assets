/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

namespace Platformer.Physics {

    ///<summary>
    /// A set of functions that define simple collision checks.
    ///<summary>
    [System.Serializable]
    public class CollisionCheck {

        // Checks whether anything within the circle is touching something on the given layer.
        public bool Touching(Vector3 center, float radius, Vector3 direction, LayerMask layer) {
            Vector3 normal = Quaternion.Euler(0f, 0f, 90f) * direction;
            for (int i = -1; i <= 1; i++) {
                Vector3 offset = direction * radius + i * normal * radius / 1.5f;
                Collider2D temp = Physics2D.OverlapCircle(center + offset, PhysicsManager.Settings.collisionPrecision, layer);
                if (temp != null) {
                    return true;
                }
            }
            return false;
        }

        // Finds all object of the given class within a specified radius (null if none exist).
        public bool Touching(Vector3 position, float radius, LayerMask layers) {
            Collider2D[] colliders = UnityEngine.Physics2D.OverlapCircleAll(position, radius, layers);
            return colliders.Length != 0;
        }

        // Finds the closest object of the given class within a specified radius (null if none exist).
        public TMonoBehaviour Closest<TMonoBehaviour>(Vector3 position, float radius, LayerMask layers) where TMonoBehaviour : MonoBehaviour {
            float minDistance = Mathf.Infinity;
            TMonoBehaviour closest = null;
            Collider2D[] colliders = UnityEngine.Physics2D.OverlapCircleAll(position, radius, layers);
            for (int i = 0; i < colliders.Length; i++) {
                TMonoBehaviour temp = colliders[i].GetComponent<TMonoBehaviour>();
                if (temp != null) {
                    float distance = (temp.transform.position - position).magnitude;
                    if (distance < minDistance) {
                        closest = temp;
                    }
                }
            }
            return closest;
        }

        // Finds all object of the given class within a specified radius (null if none exist).
        public List<TMonoBehaviour> All<TMonoBehaviour>(Vector3 position, float radius, LayerMask layers) where TMonoBehaviour : MonoBehaviour {
            List<TMonoBehaviour> list = new List<TMonoBehaviour>();
            Collider2D[] colliders = UnityEngine.Physics2D.OverlapCircleAll(position, radius, layers);
            for (int i = 0; i < colliders.Length; i++) {
                TMonoBehaviour behaviour = colliders[i].GetComponent<TMonoBehaviour>();
                if (behaviour != null) {
                    list.Add(behaviour);
                }
            }
            return list;
        }

        // Finds all object of the given class interseting the line (null if none exist).
        public TMonoBehaviour LineOfSight<TMonoBehaviour>(Vector3 position, Vector2 direction, LayerMask layers, float distance = -1f) where TMonoBehaviour : MonoBehaviour {
            distance = distance == -1f ? Mathf.Infinity : distance;
            RaycastHit2D hit = UnityEngine.Physics2D.Raycast(position + (Vector3)direction * PhysicsManager.Settings.collisionPrecision, direction, distance, layers);
            TMonoBehaviour behaviour = hit.collider?.GetComponent<TMonoBehaviour>();
            if (behaviour != null) {
                return behaviour;
            }
            return null;
        }

        // Finds all object of the given class interseting the line (null if none exist).
        public GameObject ILineOfSight<I>(Vector3 position, Vector2 direction, LayerMask layers, float distance = -1f) {
            distance = distance == -1f ? Mathf.Infinity : distance;
            RaycastHit2D hit = UnityEngine.Physics2D.Raycast(position + (Vector3)direction * PhysicsManager.Settings.collisionPrecision, direction, distance, layers);
            Debug.Log(hit.collider);
            bool hasInterface = hit.collider != null && hit.collider.GetComponent<I>() != null;
            if (hasInterface) {
                return hit.collider.gameObject;
            }
            return null;
        }

        //
        public float DistanceToFirst(Vector3 position, Vector2 direction, LayerMask layers, float distance = -1f) {
            distance = distance == -1f ? Mathf.Infinity : distance;

            RaycastHit2D hit = UnityEngine.Physics2D.Raycast(position + (Vector3)direction * PhysicsManager.Settings.collisionPrecision, direction, distance, layers);
            
            if (hit.collider != null) {
                return hit.distance;
            }
            else {
                return distance;
            }

        }

    }

}