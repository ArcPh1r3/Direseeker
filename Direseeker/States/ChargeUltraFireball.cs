using System;
using EntityStates;
using EntityStates.LemurianBruiserMonster;
using RoR2;
using UnityEngine;

namespace DireseekerMod.States
{
	public class ChargeUltraFireball : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeUltraFireball.baseDuration / this.attackSpeedStat;
			Animator modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			Util.PlayAttackSpeedSound("sfx_direseeker_charge", base.gameObject, this.attackSpeedStat);
			bool flag = modelTransform;
			if (flag)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				bool flag2 = component;
				if (flag2)
				{
					Transform transform = component.FindChild("MuzzleMouth");
					bool flag3 = transform && ChargeMegaFireball.chargeEffectPrefab;
					if (flag3)
					{
						this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMegaFireball.chargeEffectPrefab, transform.position, transform.rotation);
						this.chargeInstance.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
						bool flag4 = component2;
						if (flag4)
						{
							component2.newDuration = this.duration;
						}
					}
				}
			}
			bool flag5 = modelAnimator;
			if (flag5)
			{
				base.PlayCrossfade("Gesture, Additive", "ChargeMegaFireball", "ChargeMegaFireball.playbackRate", this.duration, 0.1f);
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
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = base.fixedAge >= this.duration && base.isAuthority;
			if (flag)
			{
				FireUltraFireball nextState = new FireUltraFireball();
				this.outer.SetNextState(nextState);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		public static float baseDuration = 2f;
		private float duration;
		private GameObject chargeInstance;
	}
}
