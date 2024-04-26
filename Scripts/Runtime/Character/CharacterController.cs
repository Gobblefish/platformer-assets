/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
// Gobblefish.
using Gobblefish.Input;
using Gobblefish.Animation;
// Platformer.
using Platformer.Physics;
using Platformer.Character;
using Platformer.Entities.Components;

namespace Platformer.Character {

    ///<summary>
    /// Controls a character.
    ///<summary>
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(InputSystem)) ]
    public class CharacterController : MonoBehaviour {

        #region Variables.

        /* --- Components --- */

        // The input system attached to this body.
        private InputSystem m_Input = null;
        public InputSystem Input => m_Input;

        // The rigidbody attached to this controller.
        private Rigidbody2D m_Body = null;
        public Rigidbody2D Body => m_Body;

        // The main collider attached to this body.
        private CircleCollider2D m_Collider = null;
        public CircleCollider2D Collider => m_Collider;

        // The component used to animate this character.
        [SerializeField]
        private CharacterAnimator m_Animator;
        public CharacterAnimator Animator => m_Animator;

        /* --- Members --- */

        // Checks whether the character is on the ground.
        [SerializeField, ReadOnly]
        private bool m_OnGround = false;
        public bool OnGround => m_OnGround;

        // Checks whether the character is facing a wall.
        [SerializeField, ReadOnly]
        private bool m_FacingWall = false;
        public bool FacingWall => m_FacingWall;

        // Checks what direction the controller is facing.
        [SerializeField, ReadOnly]
        private float m_FacingDirection = 1f;
        public float FacingDirection => m_FacingDirection;

        // Whether the direction that this is facing is locked.
        [SerializeField, ReadOnly]
        private bool m_DirectionLocked = false;

        // Checks whether the character is rising.
        [SerializeField, ReadOnly]
        private bool m_Rising = false;
        public bool Rising => !m_OnGround && m_Rising;

        // Checks whether the character is falling.
        [SerializeField, ReadOnly]
        private bool m_Falling = false;
        public bool Falling => !m_OnGround && m_Falling;

        // Checks whether this character is currently disabled.
        [SerializeField, ReadOnly]
        private Timer m_DisableTimer = new Timer(0f, 0f);
        public bool Disabled => m_DisableTimer.Active;
        private bool m_Dying = false;

        [SerializeField]
        private UnityEngine.VFX.VisualEffect m_DeathEffect;
        [SerializeField]
        private Gobblefish.Audio.AudioSnippet m_DeathSound;
        [SerializeField]
        private Gobblefish.Animation.SpriteAnimator m_DeathAnimation;

        // The block that this character respawns at.
        [SerializeField, ReadOnly]
        private Respawn m_Respawn;
        public Respawn CurrentRespawn => m_Respawn;

        [SerializeField, ReadOnly]
        private Interactable m_Interactable;
        public Interactable CurrentInteractable => m_Interactable;

        // Actions.
        [SerializeField]
        private DefaultAction m_DefaultAction;
        public DefaultAction Default => m_DefaultAction;

        // Used for reference for the power actions.
        [SerializeField]
        private List<CharacterAction> m_PowerActions = new List<CharacterAction>();

        [SerializeField]
        private Sprite m_DefaultBody;

        public float deathWeight = 3f;
        public float deathSpeed = 10f;

        #endregion

        // Runs once on instantiation.
        void Awake() {
            m_Input = GetComponent<InputSystem>();
            m_Body = GetComponent<Rigidbody2D>();
            m_Collider = GetComponent<CircleCollider2D>();
        }

        // Runs once before the first frame.
        void Start() {
            m_DefaultAction.Enable(this, true);
            // EnableAllAbilityActions();
            // DisableAllAbilityActions();
        }


        public void Reset() {
            if (m_Respawn == null && tag == "Player") {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            if (m_Dying) {
                return;
            }

            Gobblefish.Graphics.GraphicsManager.Starmap.AddPoint(transform.position);

            // The visual feedback played when dying.
            PhysicsManager.Time.RunHitStop(16);
            m_Animator.PlayAnimation("OnDeath");
            m_Animator.PlayAudioVisualEffect(m_DeathEffect, m_DeathSound);
            Gobblefish.Graphics.GraphicsManager.CamShake.ShakeCamera(0.1f, 0.2f);

            // Noting the death in the stats.
            // LevelManager.AddDeath();
            // LevelManager.Reset();

            // Resetting the character.
            m_Respawn.CreateCorpse(this);

            float respawnDelay = 1.2f;
            float floatTime = 0.6f;

            Disable(respawnDelay);
            m_DefaultAction.Enable(this, false, true);
            DisableAllAbilityActions();
            m_Animator.SetPowerIndicator(null);

            // m_Body.velocity = -m_Body.velocity; // (-Vector3.up * 2f);
            // if (m_Body.velocity.sqrMagnitude == 0f) {
            //     m_Body.velocity = Vector3.up * 2f;
            // }
            // if (m_Body.velocity.sqrMagnitude < 3f * 3f) {
            //     m_Body.velocity = m_Body.velocity.normalized * 3f;
            // }
            // if (m_Body.velocity.sqrMagnitude > 15f * 15f) {
            // }

            m_Body.SetWeight(deathWeight);
            // if (Mathf.Abs(m_Body.velocity.x) > 2f * -m_Body.velocity.y) {
            //     m_Body.velocity = new Vector2(Mathf.Sign(m_Body.velocity.x), -0.5f);
            // }
            m_Body.velocity = -m_Body.velocity.normalized * deathSpeed;
            // Vector2.up + Random.Range(-0.4f, 0.4f) * Vector2.right;
            // m_Body.velocity = m_Body.velocity.normalized * deathSpeed;

            // m_Body.ReleaseAll();
            // m_Body.AddTorque(10f);

            m_Dying = true;
            

            // transform.position = m_Respawn.RespawnPosition;
            StartCoroutine(IERespawn(respawnDelay, floatTime));

        }

        private IEnumerator IERespawn(float respawnDelay, float floatTime) {
            yield return new WaitForSeconds(floatTime);

            m_Animator.gameObject.SetActive(false);
            m_Body.SetWeight(0f);
            m_Body.SetVelocity(Vector3.zero);

            yield return new WaitForSeconds(respawnDelay - floatTime);
            m_Respawn.CreateNewShell(this);
            m_Animator.gameObject.SetActive(true);
            m_DeathAnimation.Stop();

            // yield return new WaitForSeconds(delay);
            m_Body.SetVelocity(Vector3.up * 1f);
            m_Body.ReleaseXY();
            m_Dying = false;
            m_DefaultAction.Enable(this, true);
            m_Animator.StopAnimation("OnDeath");

        }

        public void SetRespawn(Respawn respawn) {
            if (m_Respawn != null) {
                m_Respawn.Deactivate();
            }
            m_Respawn = respawn;
            m_Respawn.Activate();
        }


        public void SetInteractable(Interactable interactable) {
            m_Interactable = interactable;
        }

        public void Disable(float duration) {
            m_Input.Direction.Clear();
            foreach (ActionInput actionInput in m_Input.Actions) {
                actionInput.ClearPressBuffer();
                actionInput.ClearReleaseBuffer();
            }
            m_DisableTimer.Start(duration);
        }

        public void LockDirection(bool lockDirection, float direction = 0f) {
            m_DirectionLocked = lockDirection;
            if (direction == 0f) {
                return;
            }
            m_FacingDirection = direction;
        }

        void Update() {
            if (m_DisableTimer.Active) { return; }

            m_DefaultAction.InputUpdate(this);
            for (int i = 0; i < m_PowerActions.Count; i++) {
                m_PowerActions[i].InputUpdate(this);
            }

        }

        void FixedUpdate() {
            m_DisableTimer.TickDown(Time.fixedDeltaTime);

            m_Rising = m_Body.Rising();
            m_Falling = m_Body.Falling();
            // m_DirectionLocked = m_DisableTimer.Active;
            m_OnGround = PhysicsManager.Collisions.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Vector3.down, PhysicsManager.CollisionLayers.Ground);
            m_FacingDirection = m_DirectionLocked ? m_FacingDirection : m_Input.Direction.Horizontal != 0f ? m_Input.Direction.Horizontal : m_FacingDirection;
            m_FacingWall = PhysicsManager.Collisions.Touching(m_Body.position + m_Collider.offset, m_Collider.radius, Vector3.right * m_FacingDirection,  PhysicsManager.CollisionLayers.Ground);

            if (m_DisableTimer.Active) { 
                return; 
            }

            m_DefaultAction.PhysicsUpdate(this, Time.fixedDeltaTime);
            for (int i = 0; i < m_PowerActions.Count; i++) {
                m_PowerActions[i].PhysicsUpdate(this, Time.fixedDeltaTime);
            }

        }

        public CharacterAction GetPowerAction(string actionType) {
            for (int i = 0; i < m_PowerActions.Count; i++) {
                if (m_PowerActions[i].GetType().ToString() == actionType) {
                    return m_PowerActions[i];
                }
            }
            return null;
        }

        public void EnableAllAbilityActions() {
            for (int i = 0; i < m_PowerActions.Count; i++) {
                m_PowerActions[i].Enable(this, true);
            }
        }

        public void DisableAllAbilityActions() {
            for (int i = 0; i < m_PowerActions.Count; i++) {
                m_PowerActions[i].Enable(this, false);
            }
        }

    }

}

