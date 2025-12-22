using System;
using EntityStates;
using EntityStates.LemurianBruiserMonster;
using EntityStates.Mage.Weapon;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DireseekerMod.States
{
	public class Flamethrower : BaseState
    {
        private float lastUpdateTime;   //Maximum cope
        public override void OnEnter()
		{
			base.OnEnter();
			lastUpdateTime = Time.time;
			this.stopwatch = 0f;
			this.entryDuration = Flamethrower.baseEntryDuration / this.attackSpeedStat;
			this.exitDuration = Flamethrower.baseExitDuration / this.attackSpeedStat;
			this.flamethrowerDuration = Flamethrower.baseFlamethrowerDuration;
			Transform modelTransform = base.GetModelTransform();
			bool flag = base.characterBody;
			if (flag)
			{
				base.characterBody.SetAimTimer(this.entryDuration + this.flamethrowerDuration + 1f);
			}
			bool flag2 = modelTransform;
			if (flag2)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				modelTransform.GetComponent<AimAnimator>().enabled = true;
			}
			float num = this.flamethrowerDuration * Flamebreath.tickFrequency;
			this.tickDamageCoefficient = Flamebreath.totalDamageCoefficient / num;
			bool flag3 = base.isAuthority && base.characterBody;
			if (flag3)
			{
				this.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			}
			base.PlayAnimation("Gesture, Override", "PrepFlamebreath", "PrepFlamebreath.playbackRate", this.entryDuration);
		}

		public override void OnExit()
		{
			Util.PlaySound(Flamebreath.endAttackSoundString, base.gameObject);
			base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
			bool flag = this.flamethrowerEffectInstance;
			if (flag)
			{
				EntityState.Destroy(this.flamethrowerEffectInstance.gameObject);
			}
			bool flag2 = this.secondaryFlamethrowerEffectInstance;
			if (flag2)
			{
				EntityState.Destroy(this.secondaryFlamethrowerEffectInstance.gameObject);
			}
			base.OnExit();
		}

		private void FireFlame(string muzzleString)
		{
			bool flag = base.isAuthority && this.muzzleTransform;
			if (flag)
			{
				var b = new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = this.muzzleTransform.position,
					aimVector = this.muzzleTransform.forward,
					minSpread = 0f,
					maxSpread = Flamebreath.maxSpread,
					damage = this.tickDamageCoefficient * this.damageStat,
					force = Flamebreath.force,
					muzzleName = muzzleString,
					hitEffectPrefab = Flamebreath.impactEffectPrefab,
					isCrit = this.isCrit,
					radius = Flamebreath.radius * 3f,
					falloffModel = BulletAttack.FalloffModel.None,
					stopperMask = LayerIndex.world.mask,
					procCoefficient = Flamebreath.procCoefficientPerTick,
					maxDistance = Flamebreath.maxDistance,
					smartCollision = true,
					damageType = DamageType.PercentIgniteOnHit
				};
				b.damageType.damageSource = DamageSource.Secondary;
				b.Fire();
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003338 File Offset: 0x00001538
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			float deltaTime = Time.time - lastUpdateTime;
			lastUpdateTime = Time.time;
			this.stopwatch += deltaTime;
			bool flag = this.stopwatch >= this.entryDuration && this.stopwatch < this.entryDuration + this.flamethrowerDuration && !this.hasBegunFlamethrower;
			if (flag)
			{
				this.hasBegunFlamethrower = true;
				Util.PlaySound(Flamebreath.startAttackSoundString, base.gameObject);
				base.PlayAnimation("Gesture, Override", "Flamebreath", "Flamebreath.playbackRate", this.flamethrowerDuration);
				bool flag2 = this.childLocator;
				if (flag2)
				{
					this.muzzleTransform = this.childLocator.FindChild("MuzzleMouth");
					bool flag3 = Flamebreath.flamethrowerEffectPrefab;
					if (flag3)
					{
						this.flamethrowerEffectInstance = UnityEngine.Object.Instantiate<GameObject>(Flamebreath.flamethrowerEffectPrefab, this.muzzleTransform).transform;
						this.flamethrowerEffectInstance.transform.localPosition = Vector3.zero;
						this.flamethrowerEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
					}
					bool flag4 = Flamethrower.flamethrowerEffectPrefab;
					if (flag4)
					{
						this.secondaryFlamethrowerEffectInstance = UnityEngine.Object.Instantiate<GameObject>(Flamethrower.flamethrowerEffectPrefab, this.muzzleTransform).transform;
						this.secondaryFlamethrowerEffectInstance.transform.localScale *= 1.25f;
						this.secondaryFlamethrowerEffectInstance.transform.localPosition = Vector3.zero;
						this.secondaryFlamethrowerEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
					}
				}
			}
			bool flag5 = this.stopwatch >= this.entryDuration + this.flamethrowerDuration && this.hasBegunFlamethrower;
			if (flag5)
			{
				this.hasBegunFlamethrower = false;
				base.PlayCrossfade("Gesture, Override", "ExitFlamebreath", "ExitFlamebreath.playbackRate", this.exitDuration, 0.1f);
			}
			bool flag6 = this.hasBegunFlamethrower;
			if (flag6)
			{
				this.flamethrowerStopwatch += deltaTime;
				bool flag7 = this.flamethrowerStopwatch > 1f / Flamebreath.tickFrequency;
				if (flag7)
				{
					this.flamethrowerStopwatch -= 1f / Flamebreath.tickFrequency;
					this.FireFlame("MuzzleCenter");
				}
			}
			else
			{
				bool flag8 = this.flamethrowerEffectInstance;
				if (flag8)
				{
					EntityState.Destroy(this.flamethrowerEffectInstance.gameObject);
				}
			}
			bool flag9 = this.stopwatch >= this.flamethrowerDuration + this.entryDuration + this.exitDuration && base.isAuthority;
			if (flag9)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000035D4 File Offset: 0x000017D4
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static GameObject flamethrowerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/FlamebreathEffect.prefab").WaitForCompletion();
		public static float baseEntryDuration = 0.5f;
		public static float baseExitDuration = 0.75f;
		public static float baseFlamethrowerDuration = 2.25f;

		private float tickDamageCoefficient;
		private float flamethrowerStopwatch;
		private float stopwatch;
		private float entryDuration;
		private float exitDuration;
		private float flamethrowerDuration;
		private bool hasBegunFlamethrower;
		private ChildLocator childLocator;
		private Transform flamethrowerEffectInstance;
		private Transform secondaryFlamethrowerEffectInstance;
		private Transform muzzleTransform;
		private bool isCrit;
		private const float flamethrowerEffectBaseDistance = 16f;
	}
}
