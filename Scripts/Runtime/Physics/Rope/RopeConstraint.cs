/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Platformer.Physics {

    public abstract class RopeConstraint : ScriptableObject {

        [SerializeField] public float ropeLength = 1f;
        [SerializeField] public float segmentLength = 4f / 16f;
        [SerializeField] protected int constraintDepth = 5;
        [SerializeField] public Vector3 gravity = new Vector3(0f, -1.5f, 0f);

        public abstract void Initialize(LineRenderer lineRenderer);

        public abstract void Simulate(Vector3[] ropeSegments, Vector3[] prevRopeSegments, Vector3[] velocities, Vector3 bodyVelocity, int segmentCount, float dt);

        public abstract void Constrain(Vector3 origin, Vector3[] ropeSegments, int segmentCount);

    }
}