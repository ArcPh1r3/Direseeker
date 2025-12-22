using System;
using System.Runtime.CompilerServices;
using DireseekerMod.Components;
using DireseekerMod.Modules;
using EntityStates;
using EntityStates.LemurianBruiserMonster;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace DireseekerMod.States
{
	public class FireUltraFireball : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireUltraFireball.baseDuration / this.attackSpeedStat;
			this.fireDuration = FireUltraFireball.baseFireDuration / this.attackSpeedStat;
			this.direController = base.GetComponent<DireseekerController>();
			bool flag = this.direController;
			if (flag)
			{
				this.direController.FlameBurst();
			}
			base.PlayAnimation("Gesture, Additive", "FireMegaFireball", "FireMegaFireball.playbackRate", this.duration);
			Util.PlaySound("sfx_direseeker_fire", base.gameObject);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			string muzzleName = "MuzzleMouth";
			bool isAuthority = base.isAuthority;
			if (isAuthority)
			{
				int num = Mathf.FloorToInt(base.fixedAge / this.fireDuration * (float)FireUltraFireball.projectileCount);
				bool flag = this.projectilesFired <= num && this.projectilesFired < FireUltraFireball.projectileCount;
				if (flag)
				{
					bool flag2 = FireMegaFireball.muzzleflashEffectPrefab;
					if (flag2)
					{
						EffectManager.SimpleMuzzleFlash(FireMegaFireball.muzzleflashEffectPrefab, base.gameObject, muzzleName, false);
					}
					Ray aimRay = base.GetAimRay();
					float speedOverride = FireUltraFireball.projectileSpeed;
					if (Direseeker.DireseekerPlugin.AccurateEnemiesLoaded && Direseeker.DireseekerPlugin.AccurateEnemiesCompat)
                    {
						aimRay = PredictAimRay(aimRay, speedOverride);
                    }

					Vector3 aimPoint = aimRay.GetPoint(100);
					if (Physics.Raycast(aimRay, out RaycastHit hitInfo, 100, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal))
					{
						aimPoint = hitInfo.point;
					}
					Vector3 mouthOrigin = FindModelChild(muzzleName).position;

					aimRay = new Ray(mouthOrigin, aimPoint - mouthOrigin);

					float bonusYaw = (float)Mathf.FloorToInt((float)this.projectilesFired - (float)(FireUltraFireball.projectileCount - 1) / 2f) / (float)(FireUltraFireball.projectileCount - 1) * FireUltraFireball.totalYawSpread;
					Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, 0f);
					ProjectileManager.instance.FireProjectile(Projectiles.fireballPrefab, mouthOrigin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireUltraFireball.damageCoefficient, FireUltraFireball.force, base.RollCrit(), DamageColorIndex.Default, null, speedOverride);
					this.projectilesFired++;
				}
			}
			bool flag3 = base.fixedAge >= this.duration && base.isAuthority;
			if (flag3)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		public static int projectileCount = 15;
		public static float totalYawSpread = 25f;
		public static float baseDuration = 1.5f;
		public static float baseFireDuration = 0.25f;
		public static float damageCoefficient = 1.4f;
		public static float projectileSpeed = 80f;
		public static float force = 1200f;

		private float duration;
		private float fireDuration;
		private int projectilesFired;
		private DireseekerController direController;

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private Ray PredictAimRay(Ray aimRay, float speedOverride)
        {
			if (base.characterBody && !base.characterBody.isPlayerControlled && (AccurateEnemies.AccurateEnemiesPlugin.alwaysAllowBosses || !AccurateEnemies.AccurateEnemiesPlugin.eliteOnly || base.characterBody.isElite))
			{
				Ray newAimRay;
				HurtBox targetHurtbox = AccurateEnemies.Util.GetMasterAITargetHurtbox(base.characterBody.master);
				if (speedOverride > 0f)
				{
					speedOverride = AccurateEnemies.AccurateEnemiesPlugin.GetProjectileSimpleModifiers(speedOverride);
					newAimRay = AccurateEnemies.Util.PredictAimray(aimRay, base.GetTeam(), AccurateEnemies.AccurateEnemiesPlugin.basePredictionAngle, speedOverride, targetHurtbox);
				}
				else
				{
					newAimRay = AccurateEnemies.Util.PredictAimrayPS(aimRay, base.GetTeam(), AccurateEnemies.AccurateEnemiesPlugin.basePredictionAngle, Projectiles.fireballPrefab, targetHurtbox);
				}
				return newAimRay;
			}
			return aimRay;
        }
	}
}
