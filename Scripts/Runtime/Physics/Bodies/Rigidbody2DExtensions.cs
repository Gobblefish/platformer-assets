// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    ///<summary>
    /// Defines useful and easy to use extensions for the rigidbody.
    ///<summary>
    public static class Rigidbody2DExtensions {

        // Moves the rigidbodies position.
        public static void Move(this Rigidbody2D rb, Vector2 dx) {
            rb.position += dx;
        }

        // Stops this body.
        public static void Stop(this Rigidbody2D rb) {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        // Freezes this body on the plane and its axis.
        public static void Freeze(this Rigidbody2D rb) {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Releases this body to be free on the 2d plane.
        public static void ReleaseXY(this Rigidbody2D rb) {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Releases this body to be completely free.
        public static void ReleaseAll(this Rigidbody2D rb) {
            rb.constraints = RigidbodyConstraints2D.None;
        }

        // Adds to the rigidbodies velocity.
        public static void AddVelocity(this Rigidbody2D rb, Vector2 dv) {
            rb.velocity += dv;
        }

        // Sets the rigidbodies velocity.
        public static void SetVelocity(this Rigidbody2D rb, Vector2 v) {
            rb.velocity = v;
        }

        // Sets the rigidbodies angular drag.
        public static void SetAngularDrag(this Rigidbody2D rb, float o) {
            rb.angularDrag = o;
        }

        // Slows this body by the given factor.
        public static void Slowdown(this Rigidbody2D rb, float c) {
            rb.velocity *= c;
        }

        // Clamps the rigidbodies fall speed.
        public static void ClampSpeed(this Rigidbody2D rb, float s) {
            rb.velocity = rb.velocity.sqrMagnitude > s ? rb.velocity.normalized * s : rb.velocity;
        }

        // Clamps the rigidbodies fall speed.
        public static void ClampFallSpeed(this Rigidbody2D rb, float s) {
            rb.velocity = rb.velocity.y < -s ? new Vector2(rb.velocity.x, -s) : rb.velocity;
        }

        // Clamps the rigidbodies rising speed.
        public static void ClampRiseSpeed(this Rigidbody2D rb, float s) {
            rb.velocity = rb.velocity.y > s ? new Vector2(rb.velocity.x, s) : rb.velocity;
        }

        // Clamps the rigidbodies gravity scale.
        public static void SetWeight(this Rigidbody2D rb, float g = 1f, float m = 1f) {
            rb.gravityScale = PhysicsManager.Settings.gravityScale * g;
            rb.mass = m;
        }

        // Checks if this body is rising.
        public static bool Rising(this Rigidbody2D rb) {
            return rb.velocity.y > PhysicsManager.Settings.movementPrecision;
        }

        // Checks if this body is rising.
        public static bool Falling(this Rigidbody2D rb) {
            return rb.velocity.y < -PhysicsManager.Settings.movementPrecision;
        }

    }
    
}