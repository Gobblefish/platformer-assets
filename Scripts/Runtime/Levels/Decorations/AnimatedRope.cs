/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.U2D;

namespace Platformer.LevelEditing {

    [RequireComponent(typeof(SpriteShapeController))]
    public class AnimatedRope : MonoBehaviour {

        // The length of each segment
        private const float SEGMENT_LENGTH  = 2f/16f;
        private const int MIN_SEGMENTS = 8;

        #region Variables.

        [SerializeField] 
        public float m_Length;

        [SerializeField] 
        public float m_LengthVariation;

        [SerializeField] 
        public float m_AnimationScaleAtOrigin;

        [SerializeField] 
        public float m_AnimationScaleAtTip; // As a percent of scale.

        [SerializeField]
        private AnimationCurve m_AnimationCurve;

        [SerializeField]
        private float m_Duration;
        public float GetDuration() { return m_Duration; }
        public void SetDuration(float value) { m_Duration = value; }

        // The spriteshape controller attached to this object.
        private SpriteShapeController m_SpriteShape;
        
        [SerializeField, ReadOnly]
        protected int m_SegmentCount;

        [SerializeField, ReadOnly]
        private float m_Ticks; 

        [SerializeField]
        private bool m_RandomStartPoint = true;

        public float m_WaveLength = 1f;
        
        #endregion

        /* --- Unity --- */
        // Runs once on initialization.
        void Awake() {
            m_Ticks = m_RandomStartPoint ? Random.Range(0f, m_Duration) : 0f;
            m_Length = m_Length + Random.Range(-m_LengthVariation, m_LengthVariation);

            // Get the number of segments for a rope of this length.
            m_SpriteShape = GetComponent<SpriteShapeController>();
            m_SegmentCount = (int)Mathf.Ceil(m_Length / SEGMENT_LENGTH);
            // m_SegmentCount = (int)Mathf.Max(MIN_SEGMENTS, m_SegmentCount);

            // Clear the spriteshape.
            m_SpriteShape.spline.Clear();
            m_SpriteShape.spline.InsertPointAt(0, Vector3.zero);
            m_SpriteShape.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            

            // Insert all the points.
            float yValue;
            float xValue;
            Vector3 position;
            for (int i = 1; i < m_SegmentCount; i++) {
                yValue = ((float)i/m_SegmentCount);
                xValue = m_AnimationCurve.Evaluate(yValue);
                position = GetAnimationScale(i) * Vector3.right * xValue + m_Length * Vector3.up * yValue;
                m_SpriteShape.spline.InsertPointAt(i, position);
                m_SpriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            }


        }

        // Runs once every set time interval.
        void FixedUpdate() {
            m_Ticks += Time.fixedDeltaTime;
            
            float tValue;
            float yValue;
            float xValue;
            Vector3 position;
            for (int i = 0; i < m_SegmentCount; i++) {
                yValue = ((float)i/m_SegmentCount);
                tValue = (m_Ticks/m_Duration + m_WaveLength * yValue) % 1;
                xValue = m_AnimationCurve.Evaluate(tValue);
                position = GetAnimationScale(i) * Vector3.right * xValue + m_Length * Vector3.up * yValue;
                m_SpriteShape.spline.SetPosition(i, position);
                m_SpriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            }

        }

            
        public float GetAnimationScale(int i) {
            float t = (float)i / m_SegmentCount;
            return m_Length * ((1f-t) * m_AnimationScaleAtOrigin + t * m_AnimationScaleAtTip);
        }

    }


}
