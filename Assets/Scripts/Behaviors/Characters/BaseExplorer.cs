using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Tempest.Nightmare {

    public abstract class BaseExplorer : PhysicsCharacter {

		// Player rule params.
		public int maxHealth = 100;
		public int maxLives = 3;
        
        // Health bar timer, time is in seconds.
		public float healthBarFadeDelay = 1f;
        public float deathRenderTime = 3f;

        public float teleportCooldown = 60f;

		public LayerMask whatIsBonfire;
		public LayerMask whatIsExplorer;

		public CircleCollider2D saveCollider;
        
		// Light box params.
		public float defaultScale = 6f;
		public float activeScale = 40f;

		// Internal objects accessed by this behavior.
		protected LightBoxBehavior lightBox;
		private GameObject healthCanvas;
		private Image positiveHealthBar;
		private Renderer myRenderer;

        // Health values.
		private int currentHealth;
		private int currentLives;

        private float damageTime;
        private float teleportTime;

        public override void Awake() {
			base.Awake();
            // Handle character's light box.
			lightBox = GetComponentInChildren<LightBoxBehavior>();
			lightBox.IsMine = photonView.isMine;
			lightBox.IsActive = false;
			lightBox.IsDead = false;
			lightBox.DefaultScale = new Vector3(GetDefaultScale(), GetDefaultScale());
			lightBox.ActiveScale = new Vector3(activeScale, activeScale);

			// Setup internal components and initialize object variables.
			healthCanvas = transform.Find("DreamerCanvas").gameObject;
			positiveHealthBar = healthCanvas.transform.Find("PositiveHealth").GetComponent<Image>();
			myRenderer = GetComponent<Renderer>();

			// Initialize state values.
			currentHealth = maxHealth;
			currentLives = maxLives;
        }

        public override void Update() {
			base.Update();
			ResurrectIfAble();
			HandleLifeState();
			HandleNameState();
			HandlePowerupState();
			DeleteSelfIfAble();
        }

        // Brings the player back to life if they are within range of a bonfire that has living players near it.
		private void ResurrectIfAble() {
			if (!photonView.isMine || !IsDead() || IsOutOfLives())
				return;
			bool ableToRes = false;
			BaseExplorer savior = null;	
			Collider2D[] bonfires = Physics2D.OverlapCircleAll(transform.position, saveCollider.radius, whatIsBonfire);
			foreach (Collider2D fireCollider in bonfires) {
				BonfireBehavior behavior = fireCollider.gameObject.GetComponent<BonfireBehavior>();
				if (behavior != null && behavior.IsLit()) {
					ableToRes = true;
				}
			}
			Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, saveCollider.radius, whatIsExplorer);
			foreach (Collider2D collider in players) {
				BaseExplorer behavior = collider.gameObject.GetComponent<BaseExplorer>();
				if (behavior != null && !behavior.OutOfHealth()) {
					ableToRes = true;
					behavior.photonView.RPC("ReceiveRescueEmbers", PhotonTargets.All, 10);
					savior = behavior;
				}
			}
			if (ableToRes) {
				currentHealth = maxHealth;
				GeneratedGameManager behavior = FindObjectOfType<GeneratedGameManager>();
				behavior.photonView.RPC("DisplayAlert", PhotonTargets.Others, "An explorer has been saved!  His light shines once more.", false, PlayerStateContainer.EXPLORER);
				behavior.DisplayAlert("You have been saved!  Your light shines once more.", false, PlayerStateContainer.EXPLORER);
				PlayRelightSound();
				photonView.RPC("PlayRelightSound", PhotonTargets.Others);
			}
		}

        // Draws current health total, switches layers based on health totals, and hides player to other players if dead.
		private void HandleLifeState() {
			lightBox.IsDead = IsDead();
			if (IsDead()) {
				bool amNightmare = PlayerStateContainer.Instance.TeamSelection == PlayerStateContainer.NIGHTMARE;
				gameObject.layer = LayerMask.NameToLayer("Death");
				positiveHealthBar.fillAmount = 0f;
				healthCanvas.SetActive(!amNightmare);
				ToggleRenderers(!amNightmare);
				lightBox.IsActive = false;
			} else {
				gameObject.layer = LayerMask.NameToLayer(OutOfHealth() ? "Death" : "Explorer");
				positiveHealthBar.fillAmount = (float)currentHealth / (float)maxHealth;
				healthCanvas.SetActive(Time.time - damageTime < healthBarFadeDelay);
				ToggleRenderers(true);
			}
		}

		private void HandleNameState() {
			if (photonView.isMine) {
				nameCanvas.SetActive(false);
			} else if (PlayerStateContainer.Instance.TeamSelection != PlayerStateContainer.NIGHTMARE) {
				nameCanvas.SetActive(true);
			} else if (Time.time - damageTime < healthBarFadeDelay) {
				nameCanvas.SetActive(true);
			} else {
				nameCanvas.SetActive(false);
			}
		}

		// Toggles base renderer and health canvas if necessary.
		// Prevents multiple calls to change enabled state.
		private void ToggleRenderers(bool enabled) {
			if (myRenderer.enabled != enabled) {
				myRenderer.enabled = enabled;
				Renderer[] childRenderers = gameObject.GetComponentsInChildren<Renderer>();
				foreach (Renderer childRenderer in childRenderers) {
					if (childRenderer.gameObject.GetComponent<LightBoxBehavior>() == null) {
						childRenderer.enabled = enabled;
					}
				}
			}
		}

        [PunRPC]
        public void TeleportToPortal(Vector3 newPosition) {
            if (!photonView.isMine) return;
            float finalCooldown = teleportCooldown - (GetTalentRank(TalentEnum.PORTAL_COOLDOWN_REDUCTION) * 5.0f);
            if (Time.time - teleportTime < finalCooldown) return;
            transform.position = newPosition;
            teleportTime = Time.time;
        }

		[PunRPC]
		public override void PlayJumpSound(bool doubleJump) {
			if (PlayerStateContainer.Instance.TeamSelection != PlayerStateContainer.NIGHTMARE || !OutOfHealth()) {
				base.PlayJumpSound(doubleJump);
			}
		}

		[PunRPC]
		public override void PlayDashSound() {
			if (PlayerStateContainer.Instance.TeamSelection != PlayerStateContainer.NIGHTMARE || !OutOfHealth()) {
				base.PlayDashSound();
			}
		}

        private void HandlePowerupState() {
			if (HasPowerup(Powerup.BETTER_VISION)) {
				lightBox.DefaultScale = new Vector3(GetDefaultScale() * 3f, GetDefaultScale() * 3f);
			} else {
				lightBox.DefaultScale = new Vector3(GetDefaultScale(), GetDefaultScale());
			}
		}

        #region HealthState

		public override bool OutOfHealth() {
			return currentHealth <= 0;
		}

        protected override void SubtractHealth(int health) {
            currentHealth -= health;
        }

        // Returns whether or not the player is currently dead (out of health but still in the game).
        public bool IsDead() {
			return currentHealth <= 0 && Time.time - damageTime > deathRenderTime;
		}

		// Returns whether or not the player is out of the game (out of death time).
		public bool IsOutOfLives() {
			return currentLives <= 0;
		}

		public bool ImmuneToDamage() {
			return currentState == MovementState.HIT_FREEZE || currentState == MovementState.RAG_DOLL || OutOfHealth();
		}

		#endregion HealthState

		public override void LightTogglePressed() {
			base.LightTogglePressed();
			if (!OutOfHealth()) {
				lightBox.IsActive = !lightBox.IsActive;
			}
		}

		#region DamageHandling

		// Called by a nightmare behavior when collision occurs.
		[PunRPC]
		public void OnDamageTaken(Vector3 hitPosition, Vector3 hitSpeed, int damage, float freezeTime, float stunTime) {
			if (ImmuneToDamage()) return;
            TakeDamage(hitPosition, hitSpeed, damage, freezeTime, stunTime);
            damageTime = Time.time;
			if (!photonView.isMine) return;
			DieIfAble();
			if (!OutOfHealth()) {
				PlayHitSound();
				photonView.RPC("PlayHitSound", PhotonTargets.Others);
			}
		}

		private void DieIfAble() {
			if (currentHealth <= 0) {
				currentHealth = 0;
				currentLives--;

                PlayDeathSound();
                photonView.RPC("PlayDeathSound", PhotonTargets.Others);
                GeneratedGameManager behavior = FindObjectOfType<GeneratedGameManager>();
                if (IsOutOfLives()) {
                    behavior.DisplayAlert("Your light has gone out forever.  You can still spectate though.", false, PlayerStateContainer.EXPLORER);
                    behavior.photonView.RPC("DisplayAlert", PhotonTargets.Others, "An explorer has fallen, his light is out forever.", false, PlayerStateContainer.EXPLORER);
                } else {
                    behavior.DisplayAlert("Your light has gone out!  Go to a lit bonfire or another player to relight it.", false, PlayerStateContainer.EXPLORER);
                    behavior.photonView.RPC("DisplayAlert", PhotonTargets.Others, "Someone's light has gone out!  Help them relight it by finding them.", false, PlayerStateContainer.EXPLORER);
                }
            }
		}

		private void DeleteSelfIfAble() {
			if (photonView.isMine && IsOutOfLives() && Time.time - damageTime > deathRenderTime) {
				GeneratedGameManager gameManager = FindObjectOfType<GeneratedGameManager>();
				gameManager.Explorer = null;
				gameManager.ChangeMaskColor(0.5f);
				PhotonNetwork.Destroy(photonView);
				FindObjectOfType<CharacterInputManager>().ClearControllable();
			}
		}

		#endregion DamageHandling

		public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			base.OnPhotonSerializeView(stream, info);
			if (stream.isWriting) {
				stream.SendNext(currentHealth);
                stream.SendNext(currentLives);
				stream.SendNext(lightBox.IsActive);
			} else {
				currentHealth = (int)stream.ReceiveNext();
                currentLives = (int)stream.ReceiveNext();
				lightBox.IsActive = (bool)stream.ReceiveNext();
			}
		}

        // Called within EmpowerableCharacterBehavior to determine which powerups this character is eligible for.
		protected override Powerup[] GetUsablePowerups() {
			return new Powerup[] {
				Powerup.BETTER_VISION,
				Powerup.NIGHTMARE_VISION,
				Powerup.THIRD_JUMP,
				Powerup.DOUBLE_OBJECTIVE_SPEED
			};
		}

		public float GetDefaultScale() {
			float defaultScaleModifier = (GetTalentRank(TalentEnum.SIGHT_RANGE) * 0.05f) + 1.0f;
			return defaultScale * defaultScaleModifier;
		}

		[PunRPC]
		public void ReceiveObjectiveEmbers(float embers) {
			if (!photonView.isMine) return;
			PlayerStateContainer.Instance.ObjectiveEmbers += embers;
		}

		[PunRPC]
		public void ReceiveRescueEmbers(int embers) {
			if (!photonView.isMine) return;
			PlayerStateContainer.Instance.ObjectiveEmbers += embers;
		}

		[PunRPC]
		public void ReceiveUpgradeEmbers(int embers) {
			if (!photonView.isMine)  return;
			PlayerStateContainer.Instance.UpgradeEmbers += embers;
		}
    }
}
