using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Tempest.Nightmare {

    public class GoblinBehavior : BaseNightmare {
        
        public float collisionDebounceTime = 1f;
        public float upgradeSpeedFactor = 0.2f;

        private bool hasUsedDash;
        private float lastCollisionTime;

        public override void Update() {
            base.Update();
            switch (currentState) {
                case MovementState.GROUNDED:
                case MovementState.WALL_SLIDE_LEFT:
                case MovementState.WALL_SLIDE_RIGHT:
                    hasUsedDash = false;
                    break;
            }
        }

        protected override void HandleAnimator() {
            base.HandleAnimator();
            animator.SetBool("IsAttacking", currentState == MovementState.DASHING);
        }

        public override void ActionPrimaryPressed() {
            base.ActionPrimaryPressed();
            switch (currentState) {
                case MovementState.GROUNDED:
                case MovementState.WALL_SLIDE_LEFT:
                case MovementState.WALL_SLIDE_RIGHT:
                    JumpPhysics();
                    break;
            }
        }

        public override void ActionSecondaryPressed(Vector3 mouseDirection) {
            base.ActionSecondaryPressed(mouseDirection);
            if (!hasUsedDash && DashPhysics(mouseDirection)) {
                hasUsedDash = true;
            }
        }

        public void OnTriggerEnter2D(Collider2D other) {
            if (!photonView.isMine) return;
            BaseExplorer associatedBehavior = other.gameObject.GetComponent<BaseExplorer>();
            if (associatedBehavior == null || associatedBehavior.IsOutOfHealth()) return;
            if (currentState == MovementState.DASHING && Time.time - lastCollisionTime > collisionDebounceTime) {
                associatedBehavior.photonView.RPC("TakeDamage", PhotonTargets.All, currentSpeed);
                this.currentSpeed *= -1;
                lastCollisionTime = Time.time;
                photonView.RPC("ReceiveObjectiveEmbers", PhotonTargets.All, 10f);
            }
        }

        public void OnTriggerStay2D(Collider2D other) {
            if (Time.time - lastCollisionTime > collisionDebounceTime) {
                OnTriggerEnter2D(other);
            }
        }

        protected override float MaxSpeed() {
            return base.MaxSpeed() + (upgradeSpeedFactor * NumUpgrades);
        }

        // Override this to remove perfect acceleration powerup.
        protected override Powerup[] GetUsablePowerups() {
            return new Powerup[] { Powerup.BETTER_VISION, Powerup.DREAMER_VISION };
        }

        protected override bool IsFlyer() {
            return false;
        }

		protected override int GetSightRange() {
            return 0;
        }

		protected override int GetShrineDuration() {
            return 0;
        }

		protected override int GetBonfireSpeed() {
            return 0;
        }

		protected override int GetUpgradeModifier() {
            return 0;
        }

		protected override int GetJumpHeight() {
            return 0;
        }

		protected override int GetMovementSpeed() {
            return 0;
        }

		protected override int GetReducedGravity() {
            return 0;
        }

		protected override int GetJetpackForce() {
            return 0;
        }

		protected override int GetResetDashOnWallSlide() {
            return 0;
        }
    }
}
