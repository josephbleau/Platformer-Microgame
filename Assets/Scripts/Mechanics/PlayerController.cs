using System;
using Platformer.Gameplay;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject, IAlive
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        private JumpState _jumpState = JumpState.Grounded;
        public JumpState previousJumpState = JumpState.Grounded;

        public JumpState jumpState
        {
            get => _jumpState;
            set
            {
                previousJumpState = _jumpState; 
                _jumpState = value; 
            }
            
        }
        
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        bool recoil, recoilLeft, recoilRight;
        
        float timeUntilControlIsRegained = 0.0f;
        
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void FixedUpdate()
        {
            timeUntilControlIsRegained -= Time.deltaTime;
            
            if (timeUntilControlIsRegained <= 0.0f)
            {
                if (recoilLeft || recoilRight)
                {
                    if (health.IsAlive)
                    {
                        animator.Play("Player-Idle");
                    }
                }

                recoilLeft = false;
                recoilRight = false;
            }

            timeUntilControlIsRegained = Math.Max(timeUntilControlIsRegained, 0f);
            
            base.FixedUpdate();
        }

        public override void Bounce(Vector2 dir)
        {
            if (controlEnabled) base.Bounce(dir);
        }

        public override void Bounce(float value)
        {
            if (controlEnabled) base.Bounce(value);
        }

        public void Recoil(bool left)
        {
            if (left)
            {
                recoilLeft = true;
            }
            else
            {
                recoilRight = true;
            }
        }
        
        private void GetPlayerInput()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");

                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;

                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
        }

        private void GetNonPlayerControlledMovement()
        {
            if (recoilLeft)
            {
                move.x = -1;
            }

            if (recoilRight)
            {
                move.x = 1;
            }
        }
        
        protected override void Update()
        {
            if (controlEnabled)
            {
                GetPlayerInput();
                GetNonPlayerControlledMovement();
            }
            else
            {
                move.x = 0;
            }
            
            UpdatePlayerState();
            base.Update();
        }

        void UpdatePlayerState()
        {
            jump = false;
            recoil = false;
            
            switch (jumpState)
            {
                case JumpState.PrepareToRecoil:
                    jumpState = JumpState.Recoil;
                    recoil = true;
                    break;
                case JumpState.Recoil:
                    if (IsGrounded)
                    {
                        jumpState = JumpState.RecoilEnded;
                    }
                    break;
                case JumpState.RecoilEnded:
                    jumpState = JumpState.Grounded;
                    break;
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    Schedule<PlayerLanded>().player = this;
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (recoil)
            {
                timeUntilControlIsRegained = 0.6f;
                velocity.y = 3;
                recoil = false;
            }
            
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            targetVelocity = move * maxSpeed;
            
            InformAnimator();
        }

        /// <summary>
        /// Pass details about the current players movement so that we can animate accordingly.
        /// </summary>
        private void InformAnimator()
        {
            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            if (recoilLeft)
                spriteRenderer.flipX = false;
            if (recoilRight)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            animator.SetBool("hurt", (recoilLeft || recoilRight));
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            PrepareToRecoil,
            Recoil,
            RecoilEnded,
            Landed
        }

        public void OnDeath()
        {
            Schedule<PlayerDeath>();
        }
    }
}