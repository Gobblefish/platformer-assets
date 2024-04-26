/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Physics {

    ///<summary>
    ///
    ///<summary>
    public static class TransformExtensions {

        public static void Reset(this Transform transform) {
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(1f, 1f, 1f);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f); 
        }

        // Moves an obstacle towards a target.
        public static void Move(this Transform transform, Vector2 destination, float speed, float deltaTime, List<Transform> transforms = null) {
            Vector2 pos = (Vector2)transform.localPosition;
            if (destination == pos) {
                return;
            }

            Vector2 displacement = destination - pos;
            Vector3 deltaPosition = displacement.normalized * speed * deltaTime;
            if (displacement.magnitude < deltaPosition.magnitude) {
                deltaPosition = displacement;
                transform.localPosition = destination;
            }
            else {
                transform.localPosition += deltaPosition;
            }

            if (transforms != null) {
                transforms.Drag(deltaPosition);
            }
            
        }

        // Moves an obstacle towards a target.
        public static void Rotate(this Transform transform, float speed, float deltaTime) {
            transform.eulerAngles += Vector3.forward * speed * deltaTime;
        }

        // Moves an obstacle towards a target.
        public static void RotateTowards(this Transform transform, float angle, float speed, float deltaTime) {
            float amount = Mathf.Abs(angle - transform.eulerAngles.z);
            float direction = Mathf.Sign(angle - transform.eulerAngles.z);
            float deltaAngle = speed * deltaTime;

            if (amount < deltaAngle) {
                transform.eulerAngles += Vector3.forward * angle;
            }
            else {
                transform.eulerAngles += Vector3.forward * direction * deltaAngle;
            }
        }

        public static void RotateAround(this Transform transform, Vector3 origin, float dphi, float radius) {

            Vector3 v = transform.position - origin;
            transform.position = origin + Quaternion.Euler(0f, 0f, dphi) * (v.normalized * radius);

        }

        // Drags a collection of transforms with the obstacle.
        public static void Drag(this List<Transform> transforms, Vector3 deltaPosition) {
            for (int i = 0; i < transforms.Count; i++) {
                transforms[i].localPosition += deltaPosition;
            }
        }

        // Cycles an object around a given origin.
        public static void Cycle(this Transform transform, float ticks, float period, Vector3 origin, Vector2 ellipse) {
            Vector3 horizontal = Vector3.right * ellipse.x * Mathf.Cos(2f * Mathf.PI * ticks / period);
            Vector3 vertical = Vector3.up * ellipse.y * Mathf.Sin(2f * Mathf.PI * ticks / period);
            transform.position = origin + horizontal + vertical;
        }

        // Shakes the obstacle a little bit.
        public static void Shake(this Transform transform, Vector3 origin, float strength) {
            transform.position = origin;
            transform.position += (Vector3)Random.insideUnitCircle * strength;
        }

        // Shakes the obstacle along the horizontal axis.
        public static void HorizontalShake(this Transform transform, Vector3 origin, float strength) {
            transform.position = origin;
            transform.position += (Vector3)Random.insideUnitCircle * strength;
            transform.position = new Vector3(transform.position.x, origin.y, origin.z);
        }

    }
    
}