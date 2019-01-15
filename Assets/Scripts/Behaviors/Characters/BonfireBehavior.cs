﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Tempest.Nightmare {

    public class BonfireBehavior : Photon.PunBehaviour, IPunObservable {

        public float requiredCharges = 30f;
        public float regressionFactor = 0.2f;
        public float litNotificationDuration = 5f;

        public float lightBoxScaleBase = 1f;
        public float lightBoxScaleUnlit = 4f;
        public float lightBoxScaleLit = 10f;
        
        public Animator fireAnimator;
        public AudioSource soundSource;
        public AudioSource victorySource;
        public AudioSource defeatSource;

        public LayerMask whatIsPlayer;
        public LayerMask whatIsDeadPlayer;

        private LightBoxBehavior lightBox;
        private GameObject progressCanvas;
        private Image positiveProgressBar;
        private CircleCollider2D circleCollider;
        private float currentCharges;
        private float timeLit;
        private bool playersNearby;
        private bool deadPlayersNearby;

        // Use this for initialization
        void Awake() {
            victorySource.volume = ControlBindingContainer.GetInstance().effectVolume * 0.4f;
            defeatSource.volume = victorySource.volume;

            lightBox = GetComponentInChildren<LightBoxBehavior>();
            lightBox.IsMine = true;
            lightBox.IsActive = false;
            lightBox.DefaultScale = new Vector3(lightBoxScaleUnlit, lightBoxScaleUnlit);
            lightBox.ActiveScale = new Vector3(lightBoxScaleLit, lightBoxScaleLit);

            progressCanvas = transform.Find("BonfireCanvas").gameObject;
            positiveProgressBar = progressCanvas.transform.Find("PositiveProgress").GetComponent<Image>();
            circleCollider = GetComponent<CircleCollider2D>();
            currentCharges = 0f;
            timeLit = 0f;
        }

        // Update is called once per frame
        void Update() {
            HandlePlayerEvents();
            HandleSprite();
            HandleProgressBar();
            HandleLightBox();
            HandleSound();
        }

        private void HandlePlayerEvents() {
            if (photonView.isMine && currentCharges < requiredCharges) {
                Collider2D[] otherPlayers = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius * (transform.localScale.x + transform.localScale.y) / 2, whatIsPlayer);
                if (otherPlayers.Length == 0) {
                    currentCharges -= Time.deltaTime * regressionFactor;
                    currentCharges = Mathf.Max(currentCharges, 0f);
                } else {
                    float multiplier = 0;
                    foreach (Collider2D collider in otherPlayers) {
                        BaseExplorer behavior = collider.GetComponentInParent<BaseExplorer>();
                        if (behavior == null) continue;
                        float explorerModifier = 1.0f + (behavior.GetBonfireSpeed() * 0.05f);
                        if (behavior.HasPowerup(Powerup.DOUBLE_OBJECTIVE_SPEED)) {
                            explorerModifier *= 2f;
                        }
                        float embers = Time.deltaTime * explorerModifier;
                        multiplier += explorerModifier;
                        behavior.photonView.RPC("ReceiveObjectiveEmbers", PhotonTargets.All, embers);
                    }
                    currentCharges += Time.deltaTime * multiplier;
                    if (currentCharges >= requiredCharges) {
                        currentCharges = requiredCharges;
                        photonView.RPC("NotifyLit", PhotonTargets.All);
                        photonView.RPC("PlayLitSound", PhotonTargets.All);
                    }
                }
            }
        }

        private void HandleSprite() {
            fireAnimator.SetBool("InProgress", currentCharges != 0);
            fireAnimator.SetBool("Complete", IsLit());
        }

        private void HandleProgressBar() {
            if (IsLit() || currentCharges == 0f) {
                progressCanvas.SetActive(false);
            } else {
                progressCanvas.SetActive(true);
                positiveProgressBar.fillAmount = currentCharges / requiredCharges;
            }
        }

        private void HandleLightBox() {
            float completion = currentCharges / requiredCharges;
            float unlitScale = completion * lightBoxScaleUnlit;
            lightBox.DefaultScale = new Vector3(unlitScale + lightBoxScaleBase, unlitScale + lightBoxScaleBase);
            lightBox.IsMine = currentCharges > 0f;
            lightBox.IsActive = currentCharges >= requiredCharges;
        }

        private void HandleSound() {
            if (currentCharges > 0) {
                soundSource.volume = ControlBindingContainer.GetInstance().effectVolume;
            } else {
                soundSource.volume = 0f;
            }
        }

        [PunRPC]
        public void NotifyLit() {
            currentCharges = requiredCharges;
            timeLit = Time.time;
        }

        [PunRPC]
        public void PlayLitSound() {
            if (PlayerStateContainer.Instance.TeamSelection == PlayerStateContainer.NIGHTMARE) {
                defeatSource.Play();
            } else {
                victorySource.Play();
            }
        }

        public bool IsLit() {
            return currentCharges >= requiredCharges;
        }

        public bool ShowLitNotification() {
            return Time.time - timeLit < litNotificationDuration;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.isWriting) {
                stream.SendNext(currentCharges);
            } else {
                currentCharges = (float)stream.ReceiveNext();
            }
        }
    }
}
