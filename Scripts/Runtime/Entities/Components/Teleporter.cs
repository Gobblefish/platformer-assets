/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using UnityEngine.U2D;
// Gobblefish.
using Gobblefish.Audio;
// Platformer.
using Platformer.Physics;
using Platformer.Character;
using CharacterController = Platformer.Character.CharacterController;
using IReset = Platformer.Entities.Utility.IReset;

namespace Platformer.Entities {

    ///<summary>
    /// A block that teleports the character between two points.
    ///<summary>
    public class Teleporter : MonoBehaviour, IReset {

        public enum PortalType {
            A, B, C, D
        }

        // The type of portal that this.
        [SerializeField]
        private PortalType m_PortalType = PortalType.A;
        public PortalType Type => m_PortalType;

        [SerializeField]
        private Transform m_ActiveIndicator;

        [SerializeField]
        private UnityEvent m_OnTeleportEvent = new UnityEvent();

        public void OnTeleport() {

            Teleporter targetPortal = FindPortalTarget();
            CharacterController character = Platformer.PlayerManager.Character;

            if (targetPortal != null) {
                Teleport(character, targetPortal);
            }

        }

        // Finds the other portal that this portal teleports the character to.
        public Teleporter FindPortalTarget() {
            Teleporter[] portals = (Teleporter[])GameObject.FindObjectsOfType<Teleporter>();
            for (int i = 0; i < portals.Length; i++) {
                if (IsPortalTarget(portals[i])) {
                    return portals[i];
                }
            }
            return null;
            // return Game.Level.CurrentRoom.Entities.Find(entity => IsPortalTarget(entity.GetComponent<PortalBlock>(), m_PortalType));
        }

        // Checks whether something is the portal block.
        public bool IsPortalTarget(Teleporter portal) {
            if (portal == null || portal == this) { return false; }

            if (portal.Type == m_PortalType) {
                return true;
            }
            return false;
        }

        // Teleports the character to the given block.
        public void Teleport(CharacterController character, Teleporter targetPortal) {

            targetPortal.OnTeleportedTo();

            // Disable the orb for a bit.
            // m_ActiveIndicator.gameObject.SetActive(false);
            // m_Hitbox.enabled = false;
            // StartCoroutine(IEReset());
            m_OnTeleportEvent.Invoke();

            character.Body.velocity = new Vector2(character.Body.velocity.x, Mathf.Abs(character.Body.velocity.y));
            character.transform.position = targetPortal.transform.position;

        }

        public void OnTeleportedTo() {
            // Disable the orb for a bit.
            m_OnTeleportEvent.Invoke();
        }

        public void OnStartResetting() {
            // print("is this being called");
            // m_Body.Stop();
            // m_Body.Freeze();
        }

        public void OnFinishResetting() {
            m_ActiveIndicator.gameObject.SetActive(true);
        }


    }

}