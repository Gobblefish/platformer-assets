// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace Gobblefish.Input {

    ///<summary>
    /// Collects the inputs from a keyboard.
    ///<summary>
    public class NPCInputSystem : InputSystem {

        public NPCInputChain m_InputChain;

        public float m_Ticks = 0f;

        public int m_Index = 0;

        public bool m_Loop = false;

        public bool reset = false;
        Vector3 _origin;

        public bool useChainPosition = false;
        public bool setChainPosition = false;

        protected override void Awake() {
            _origin = transform.position;
            CreateInputs();
            Reset();
        }

        public void SetChain(NPCInputChain chain) {
            m_InputChain = chain;
            useChainPosition = true;
            Reset();
        }

        protected override void CreateInputs() {
            int maxInput = 0;
            foreach (NPCInputChain.NPCInputBlock block in m_InputChain.chain) {
                if (block.actionInput.Length > maxInput) { maxInput = block.actionInput.Length; }
            }

            m_Actions = new ActionInput[maxInput];
            for (int i = 0; i < m_Actions.Length; i++) {
                m_Actions[i] = new ActionInput();
            }
        }

        protected override void Think(float dt) {
        
        }

        void FixedUpdate() {
            FixedThink(Time.fixedDeltaTime);

            if (setChainPosition) {
                m_InputChain.origin = _origin;
            }

            if (reset) {
                Reset();
            }
        }

        void Reset() {
            if (m_InputChain != null && useChainPosition) {
                transform.position = m_InputChain.origin;
            }
            else if (!useChainPosition) {
                transform.position = _origin;
            }
            m_Ticks = 0f;
            m_Index = 0;
            reset = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        // Updates the inputs.
        protected void FixedThink(float dt) {
            if (m_InputChain == null) {
                return;
            }

            if (m_Index >= m_InputChain.chain.Length) {
                return;
            }

            m_Ticks += dt;
            if (m_Ticks >= m_InputChain.chain[m_Index].duration) {
                m_Ticks -= m_InputChain.chain[m_Index].duration;
                m_Index += 1;
            }

            if (m_Index >= m_InputChain.chain.Length) {
                if (!m_Loop) {
                    return;
                }
                m_Index = 0;
            }
            
            // Updates the directional input.
            m_Direction.OnUpdate(m_InputChain.chain[m_Index].direction);
            for (int i = 0; i < m_InputChain.chain[m_Index].actionInput.Length; i++) {
                m_Actions[i].OnUpdate(m_InputChain.chain[m_Index].actionInput[i], !m_InputChain.chain[m_Index].actionInput[i], dt);
            }

        }

        public void FullClear() {
            m_Direction.Clear();
            for (int i = 0; i < m_Actions.Length; i++) {
                m_Actions[i].ClearPressBuffer();
                m_Actions[i].ClearReleaseBuffer();
            }
        }

    }

}