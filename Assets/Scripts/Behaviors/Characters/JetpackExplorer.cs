﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Tempest.Nightmare {

    public class JetpackExplorer : BaseExplorerBehavior {

        public float jetpackVelocityFactor = 2f;
        public float maxJetpackTime = 1f;
        public float fallingJetpackForceFactor = 2f;

        private GameObject fuelBarCanvas;
        private Image positiveFuelImage;

        private float jetpackTimeRemaining;
        private bool jetpackOn;

        public override void Awake() {
            base.Awake();
            fuelBarCanvas = transform.Find("FuelCanvas").gameObject;
            positiveFuelImage = fuelBarCanvas.transform.Find("PositiveFuel").GetComponent<Image>();
        }

        public override void Update() {
            base.Update();
            HandleFuelBar();
        }

        protected override void UpdateVerticalMovement() {
            base.UpdateVerticalMovement();
            if (jetpackOn) {
                if (currentSpeed.y <= 0f) {
                    currentSpeed.y += maxSpeed * gravityFactor * jetpackVelocityFactor * Time.deltaTime * fallingJetpackForceFactor;
                } else {
                    currentSpeed.y += maxSpeed * gravityFactor * jetpackVelocityFactor * Time.deltaTime;
                }
                currentSpeed.y = Mathf.Min(currentSpeed.y, maxSpeed * terminalVelocityFactor);
                jetpackTimeRemaining -= HasPowerup(Powerup.THIRD_JUMP) ? Time.deltaTime : Time.deltaTime * 2f;
                if (jetpackTimeRemaining <= 0f) {
                    jetpackTimeRemaining = 0f;
                    jetpackOn = false;
                }
            } else {
                jetpackTimeRemaining += Time.deltaTime;
                jetpackTimeRemaining = Mathf.Min(jetpackTimeRemaining, maxJetpackTime);
            }
        }

        private void HandleFuelBar() {
            positiveFuelImage.fillAmount = jetpackTimeRemaining / maxJetpackTime;
            fuelBarCanvas.SetActive(photonView.isMine && jetpackTimeRemaining != maxJetpackTime);
        }

        public override void BecameGrounded() {
            // ignored callback.
        }

        public override void GrabbedWall(bool grabbedLeft) {
            // ignored callback.
        }

        public override void InputsReceived(float horizontalScale, float verticalScale, bool grabHeld) {
            currentControllerState = new Vector3(horizontalScale, verticalScale);
            this.grabHeld = grabHeld;
        }

        public override void ActionPressed() {
            // If we just jumped, got hit, or are in the death animation, ignore this action.
            if (Time.time - damageTime < damageRecovery ||
                Time.time - deathEventTime < deathAnimationTime) {
                return;
            }

            if (grounded) {
                currentSpeed.y = maxSpeed * jumpFactor;
                jumpTime = Time.time;
            } else if (holdingWallLeft) {
                currentSpeed.y = Mathf.Sin(Mathf.PI / 4) * maxSpeed * wallJumpFactor;
                currentSpeed.x = Mathf.Cos(Mathf.PI / 4) * maxSpeed * wallJumpFactor;
                jumpTime = Time.time;
                wallJumpTime = Time.time;
                holdingWallLeft = false;
            } else if (holdingWallRight) {
                currentSpeed.y = Mathf.Sin(Mathf.PI * 3 / 4) * maxSpeed * wallJumpFactor;
                currentSpeed.x = Mathf.Cos(Mathf.PI * 3 / 4) * maxSpeed * wallJumpFactor;
                jumpTime = Time.time;
                wallJumpTime = Time.time;
                holdingWallRight = false;
            } else {
                jetpackOn = true;
            }
        }

        public override void ActionReleased() {
            jetpackOn = false;
        }
    }
}
