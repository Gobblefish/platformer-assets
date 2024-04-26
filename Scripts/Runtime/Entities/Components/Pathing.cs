/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;
// Platformer.
using Platformer.Physics;

/* --- Definitions --- */
using PathingNode = Platformer.Entities.Utility.PathingNode;
using Reset = Platformer.Entities.Utility.Reset;

namespace Platformer.Entities.Components {

    ///<summary>
    ///
    ///<summary>
    public class Pathing : MonoBehaviour {

        public Entity m_Entity;

        // The path that this platform follows.
        [SerializeField]
        private PathingNode[] m_Nodes;
        
        // The current position in the path that the path is following.
        [SerializeField, ReadOnly] 
        protected int m_PathIndex;

        // The pause timer.
        [SerializeField]
        protected Timer m_PauseTimer = new Timer(0f, 0f);

        // The amount of time the platform pauses
        // [SerializeField]
        // protected float PauseDuration = 0.5f;
        protected float PauseDuration => 1f / m_Speed;

        // The speed with which the platform moves at.
        [SerializeField] 
        protected float m_Speed = 3f;

        // The sound that plays when the platform starts moving.
        [SerializeField]
        private AudioClip m_StartMovingSound = null;

        // The sound that plays when the platform stops moving.
        [SerializeField]
        private AudioClip m_StopMovingSound = null;

        // Incase this has an elongatable length.
        public Elongatable m_Elongatable;

        private Reset reset;

        [HideInInspector] public int speedLevel;

        // Used to cache references.
        void Awake() {
            // if (m_Entity == null) {
            //     m_Entity = GetComponent<Entity>();
            // }
        }

        void Start() {

            foreach (Transform child in transform) {
                if (child.GetComponent<Power>() != null) {
                    m_Speed /= 2f;
                }
                // if (child.GetComponent<Reset>() != null) {
                //     reset = child.GetComponent<Reset>();
                // }
            }

            for (int i = 0; i < m_Nodes.Length; i++) {
                m_Nodes[i].transform.parent = transform.parent;
            }

            Entity entity = GetComponentInChildren<Entity>();
            if (entity != null) {
                m_Elongatable = entity.GetComponent<Elongatable>();
            }

        }

        // Runs once every fixed interval.
        private void FixedUpdate() {
            transform.Move(currentTargetPos, m_Speed, Time.fixedDeltaTime, m_Entity.CollisionContainer);
            SetTarget(Time.fixedDeltaTime);
        }

        Vector3 currentTargetPos => GetTargetPosition();
        private Vector3 GetTargetPosition() {
            // if (m_Elongatable != null && m_Elongatable.LengthUnits > 1) {
            //     if (m_Elongatable.searchDirection == Elongatable.SearchDirection.Horizontal) {
            //         Vector3 direction = (m_Nodes[m_PathIndex].Position - transform.position).normalized;
            //         if (direction.x > 0f && m_PathIndex != 0) {
            //             return m_Nodes[m_PathIndex].Position - Mathf.Sign(direction.x) * Vector3.right * m_Elongatable.spline.GetPosition(1).x;
            //         }
            //     }
            // }
            return m_Nodes[m_PathIndex].Position;
        }

        // Sets the target for this platform.
        private void SetTarget(float dt) {
            // Take a step.
            float distance = ((Vector2)currentTargetPos - (Vector2)transform.position).magnitude;
            if (distance == 0f && m_PauseTimer.Value == PauseDuration) {
                // Game.Audio.Sounds.PlaySound(m_StopMovingSound);
            }

            // At an end point.
            bool finished = m_PauseTimer.TickDownIf(dt, distance < 0.01f);
            bool neverStarted = distance == 0f && m_PauseTimer.MaxValue == 0f;
            if (finished || neverStarted) {
                // Game.Audio.Sounds.PlaySound(m_StartMovingSound);
                m_PathIndex = (m_PathIndex + 1) % m_Nodes.Length;
                m_PauseTimer.Start(PauseDuration);
                if (reset != null) {
                    reset.HardReset();
                }
            }

        }

        public bool debugPath = true;
        void OnDrawGizmos() {
            if (!debugPath) { return; }
            for (int i = 0; i < m_Nodes.Length; i++) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(m_Nodes[i].transform.position, 0.2f);
                if (i > 0) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(m_Nodes[i-1].transform.position, m_Nodes[i].transform.position);
                }
            }
            Gizmos.DrawLine(m_Nodes[m_Nodes.Length - 1].transform.position, m_Nodes[0].transform.position);

        }

        public void SetEntity(Entity entity) {
            m_Entity = entity;
        }

        public void SetPath(List<PathingNode> path, int speedLevel, float speed = 3f) {
            m_Nodes = path.ToArray();
            m_Speed = speed + (2f / 3f * speed * speedLevel);
            // speedLevel = path.speedLevel;
        }

    }

}
