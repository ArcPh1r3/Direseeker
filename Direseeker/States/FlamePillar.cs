using System;
using System.Collections.Generic;
using System.Linq;
using EntityStates;
using EntityStates.LemurianBruiserMonster;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace DireseekerMod.States
{
	public class FlamePillar : BaseState
	{
        private static Material onFireMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matOnFire.mat").WaitForCompletion();
        private static GameObject predictionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Meteor/MeteorStrikePredictionEffect.prefab").WaitForCompletion();
        private static GameObject genericDelayBlast = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/GenericDelayBlast.prefab").WaitForCompletion();
		private static GameObject magmaOrb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaOrbExplosion.prefab").WaitForCompletion();
        public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			bool flag = base.modelLocator;
			if (flag)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				this.aimAnimator = base.modelLocator.modelTransform.GetComponent<AimAnimator>();
				bool flag2 = this.aimAnimator;
				if (flag2)
				{
					this.aimAnimator.enabled = true;
				}
				bool flag3 = component;
				if (flag3)
				{
					Transform modelTransform = base.GetModelTransform();
					bool flag4 = modelTransform;
					if (flag4)
                    {
                        CharacterModel cm = modelTransform.gameObject.GetComponent<CharacterModel>();
						if (cm)
                        {
                            if (cm.temporaryOverlays == null) cm.temporaryOverlays = new List<TemporaryOverlayInstance>();
                            TemporaryOverlayInstance temporaryOverlay = TemporaryOverlayManager.AddOverlay(cm.gameObject);
                            temporaryOverlay.duration = 3f;
                            temporaryOverlay.animateShaderAlpha = true;
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = onFireMat;
							temporaryOverlay.inspectorCharacterModel = cm;
							temporaryOverlay.Start();
                        }
					}
					Transform transform = component.FindChild("MuzzleMouth");
					bool flag5 = transform && ChargeMegaFireball.chargeEffectPrefab;
					if (flag5)
					{
						this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMegaFireball.chargeEffectPrefab, transform.position, transform.rotation);
						this.chargeInstance.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
						bool flag6 = component2;
						if (flag6)
						{
							component2.newDuration = 0.5f;
						}
					}
				}
			}
			base.PlayAnimation("Gesture, Override", "PrepFlamebreath", "PrepFlamebreath.playbackRate", FlamePillar.entryDuration);
			this.subState = FlamePillar.SubState.Prep;
			Util.PlaySound("sfx_direseeker_woosh", base.gameObject);
			//Util.PlaySound("Play_magmaWorm_spawn_VO", base.gameObject);
			bool active = NetworkServer.active;
			if (active)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
				bool flag7 = base.teamComponent;
				if (flag7)
				{
					bullseyeSearch.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
				}
				bullseyeSearch.maxDistanceFilter = FlamePillar.maxDistance;
				bullseyeSearch.maxAngleFilter = 90f;
				Ray aimRay = base.GetAimRay();
				bullseyeSearch.searchOrigin = aimRay.origin;
				bullseyeSearch.searchDirection = aimRay.direction;
				bullseyeSearch.filterByLoS = false;
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
				bullseyeSearch.RefreshCandidates();
				HurtBox hurtBox = Enumerable.FirstOrDefault<HurtBox>(bullseyeSearch.GetResults());
				bool flag8 = hurtBox;
				if (flag8)
				{
					this.predictor = new FlamePillar.Predictor(base.transform);
					this.predictor.SetTargetTransform(hurtBox.transform);
				}
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			bool flag = this.chargeInstance;
			if (flag)
			{
				EntityState.Destroy(this.chargeInstance);
			}
			base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			this.stopwatch += GetDeltaTime();
			switch (this.subState)
			{
				case FlamePillar.SubState.Prep:
					{
						bool flag = this.predictor != null;
						if (flag)
						{
							this.predictor.Update();
						}
						bool flag2 = this.stopwatch <= FlamePillar.trackingDuration;
						if (flag2)
						{
							bool flag3 = this.predictor != null;
							if (flag3)
							{
								this.predictionOk = this.predictor.GetPredictedTargetPosition(FlamePillar.entryDuration - FlamePillar.trackingDuration, out this.predictedTargetPosition);
							}
						}
						else
						{
							bool flag4 = !this.hasShownPrediction;
							if (flag4)
							{
								this.hasShownPrediction = true;
								this.PlacePredictedAttack();
							}
						}
						bool flag5 = this.stopwatch >= FlamePillar.entryDuration;
						if (flag5)
						{
							this.predictor = null;
							this.subState = FlamePillar.SubState.FirePillar;
							this.stopwatch = 0f;
							base.PlayAnimation("Gesture, Override", "Flamebreath", "Flamebreath.playbackRate", FlamePillar.entryDuration);
							bool flag6 = this.chargeInstance;
							if (flag6)
							{
								EntityState.Destroy(this.chargeInstance);
							}
						}
						break;
					}
				case FlamePillar.SubState.FirePillar:
					{
						bool flag7 = this.stopwatch >= FlamePillar.fireDuration;
						if (flag7)
						{
							this.subState = FlamePillar.SubState.Exit;
							this.stopwatch = 0f;
							base.PlayCrossfade("Gesture, Override", "ExitFlamebreath", "ExitFlamebreath.playbackRate", FlamePillar.fireDuration, 0.1f);
						}
						break;
					}
				case FlamePillar.SubState.Exit:
					{
						bool flag8 = this.stopwatch >= FlamePillar.exitDuration && base.isAuthority;
						if (flag8)
						{
							this.outer.SetNextStateToMain();
						}
						break;
					}
			}
		}

		protected virtual void PlacePredictedAttack()
		{
			this.PlaceSingleDelayBlast(this.predictedTargetPosition, 0f);
		}

		protected void PlaceSingleDelayBlast(Vector3 position, float delay)
		{
			EffectManager.SpawnEffect(predictionEffect, new EffectData
			{
				origin = position,
				scale = FlamePillar.pillarRadius,
				rotation = Quaternion.identity
			}, true);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(genericDelayBlast, position, Quaternion.identity);
			DelayBlast component = gameObject.GetComponent<DelayBlast>();
			component.position = position;
			component.baseDamage = this.damageStat * FlamePillar.pillarDamageCoefficient;
			component.baseForce = FlamePillar.pillarForce;
			component.bonusForce = FlamePillar.pillarVerticalForce * Vector3.up;
			component.attacker = base.gameObject;
			component.radius = FlamePillar.pillarRadius;
			component.crit = base.RollCrit();
			component.maxTimer = FlamePillar.entryDuration - FlamePillar.trackingDuration + delay;
			component.falloffModel = BlastAttack.FalloffModel.None;
			component.explosionEffect = magmaOrb;
			component.maxTimer = FlamePillar.pillarDelay;
			component.damageType.damageSource = DamageSource.Utility;
			gameObject.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(component.attacker);
		}

		public static float entryDuration = 1.5f;
		public static float fireDuration = 0.1f;
		public static float exitDuration = 0.15f;
		public static float maxDistance = 128f;
		public static float trackingDuration = 0.85f;
		public static float pillarDamageCoefficient = 6f;
		public static float pillarForce = 2000f;
		public static float pillarVerticalForce = 4000f;
		public static float pillarRadius = 6f;
		public static float pillarDelay = 2f;

		private bool hasShownPrediction;
		private bool predictionOk;
		protected Vector3 predictedTargetPosition;
		private AimAnimator aimAnimator;
		private GameObject chargeInstance;
		private float stopwatch;
		private FlamePillar.SubState subState;
		private FlamePillar.Predictor predictor;

		private enum SubState
		{
			Prep,
			FirePillar,
			Exit
		}

		private class Predictor
		{
			public Predictor(Transform bodyTransform)
			{
				this.bodyTransform = bodyTransform;
			}

			public bool hasTargetTransform
			{
				get
				{
					return this.targetTransform;
				}
			}

			public bool isPredictionReady
			{
				get
				{
					return this.collectedPositions > 2;
				}
			}

			private void PushTargetPosition(Vector3 newTargetPosition)
			{
				this.targetPosition2 = this.targetPosition1;
				this.targetPosition1 = this.targetPosition0;
				this.targetPosition0 = newTargetPosition;
				this.collectedPositions++;
			}

			public void SetTargetTransform(Transform newTargetTransform)
			{
				this.targetTransform = newTargetTransform;
				this.targetPosition2 = (this.targetPosition1 = (this.targetPosition0 = newTargetTransform.position));
				this.collectedPositions = 1;
			}

			public void Update()
			{
				bool flag = this.targetTransform;
				if (flag)
				{
					this.PushTargetPosition(this.targetTransform.position);
				}
			}

			public bool GetPredictedTargetPosition(float time, out Vector3 predictedPosition)
			{
				Vector3 lhs = this.targetPosition1 - this.targetPosition2;
				Vector3 vector = this.targetPosition0 - this.targetPosition1;
				lhs.y = 0f;
				vector.y = 0f;
				bool flag = lhs == Vector3.zero || vector == Vector3.zero;
				FlamePillar.Predictor.ExtrapolationType extrapolationType;
				if (flag)
				{
					extrapolationType = FlamePillar.Predictor.ExtrapolationType.None;
				}
				else
				{
					Vector3 normalized = lhs.normalized;
					Vector3 normalized2 = vector.normalized;
					bool flag2 = Vector3.Dot(normalized, normalized2) > 0.98f;
					if (flag2)
					{
						extrapolationType = FlamePillar.Predictor.ExtrapolationType.Linear;
					}
					else
					{
						extrapolationType = FlamePillar.Predictor.ExtrapolationType.Polar;
					}
				}
				float num = 1f / Time.fixedDeltaTime;
				predictedPosition = this.targetPosition0;
				FlamePillar.Predictor.ExtrapolationType extrapolationType2 = extrapolationType;
				if (extrapolationType2 != FlamePillar.Predictor.ExtrapolationType.Linear)
				{
					if (extrapolationType2 == FlamePillar.Predictor.ExtrapolationType.Polar)
					{
						Vector3 position = this.bodyTransform.position;
						Vector3 v = Util.Vector3XZToVector2XY(this.targetPosition2 - position);
						Vector3 v2 = Util.Vector3XZToVector2XY(this.targetPosition1 - position);
						Vector3 v3 = Util.Vector3XZToVector2XY(this.targetPosition0 - position);
						float magnitude = v.magnitude;
						float magnitude2 = v2.magnitude;
						float magnitude3 = v3.magnitude;
						float num2 = Vector2.SignedAngle(v, v2) * num;
						float num3 = Vector2.SignedAngle(v2, v3) * num;
						float num4 = (magnitude2 - magnitude) * num;
						float num5 = (magnitude3 - magnitude2) * num;
						float num6 = (num2 + num3) * 0.5f;
						float num7 = (num4 + num5) * 0.5f;
						float num8 = magnitude3 + num7 * time;
						bool flag3 = num8 < 0f;
						if (flag3)
						{
							num8 = 0f;
						}
						Vector2 vector2 = Util.RotateVector2(v3, num6 * time);
						vector2 *= num8 * magnitude3;
						predictedPosition = position;
						predictedPosition.x += vector2.x;
						predictedPosition.z += vector2.y;
					}
				}
				else
				{
					predictedPosition = this.targetPosition0 + vector * (time * num);
				}
				RaycastHit raycastHit;
				bool flag4 = Physics.Raycast(new Ray(predictedPosition + Vector3.up * 1f, Vector3.down), out raycastHit, 200f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
				bool result;
				if (flag4)
				{
					predictedPosition = raycastHit.point;
					result = true;
				}
				else
				{
					result = false;
				}
				return result;
			}

			private Transform bodyTransform;
			private Transform targetTransform;
			private Vector3 targetPosition0;
			private Vector3 targetPosition1;
			private Vector3 targetPosition2;
			private int collectedPositions;

			private enum ExtrapolationType
			{
				None,
				Linear,
				Polar
			}
		}
	}
}