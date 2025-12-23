using DireseekerMod.Components;
using DireseekerMod.Modules;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace DireseekerMod.States
{
	public class Enrage : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();

			this.stopwatch = 0f;
			this.entryDuration = Enrage.baseEntryDuration / this.attackSpeedStat;
			this.exitDuration = Enrage.baseExitDuration / this.attackSpeedStat;
			this.childLocator = base.GetModelChildLocator();
			this.direController = this.GetComponent<DireseekerController>();

			if (this.direController) this.direController.StartRageMode();

			if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);

			base.PlayAnimation("Gesture, Override", "PrepFlamebreath", "PrepFlamebreath.playbackRate", this.entryDuration);
            //Util.PlaySound("Play_magmaWorm_spawn_VO", base.gameObject);
            Util.PlaySound("sfx_direseeker_woosh", base.gameObject);
        }

		private void GrantItems()
		{
			if (NetworkServer.active)
			{
				if (base.characterBody.master && base.characterBody.master.inventory)
				{
					base.characterBody.master.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor, 1);
					base.characterBody.master.inventory.GiveItem(RoR2Content.Items.AlienHead, 10);
					base.characterBody.master.inventory.GiveItem(RoR2Content.Items.Hoof, 3);
					base.characterBody.master.inventory.GiveItem(RoR2Content.Items.Syringe, 3);
				}
			}
		}

		public override void OnExit()
		{
			base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
			base.OnExit();
		}

		private void SpawnSun()
		{
			if (!NetworkServer.active) return;

			Transform chest = this.FindModelChild("Chest");
            GameObject sun = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.sunPrefab, chest.position, Quaternion.identity, transform);
			sun.transform.parent = chest;
            sun.GetComponent<GenericOwnership>().ownerObject = base.gameObject;
            NetworkServer.Spawn(sun);
            sun.GetComponent<DireseekerSunNetworkController>().RpcPosition(chest.gameObject);
        }

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += GetDeltaTime();

			if (this.stopwatch >= this.entryDuration && !this.hasEnraged)
			{
				this.hasEnraged = true;
                this.characterBody.baseRegen = -20f;
                this.GrantItems();
				this.SpawnSun();
                ///AkSoundEngine.StopPlayingID(this.roarStartPlayID);
                //Util.PlaySound("DireseekerRage", base.gameObject);
                //Util.PlaySound("DireseekerRoar", base.gameObject);
                Util.PlaySound("sfx_direseeker_roar", base.gameObject);

				GameObject fx = GameObject.Instantiate(Modules.Assets.roarEffect);
				Transform muzzle = this.FindModelChild("MuzzleMouth");
				fx.transform.position = muzzle.position;
				fx.transform.rotation = muzzle.rotation;
				fx.transform.parent = muzzle;
				GameObject.Destroy(fx, 30f);

                BlastAttack bbbbbbbbb = new BlastAttack();
                bbbbbbbbb.attacker = this.gameObject;
                bbbbbbbbb.inflictor = this.gameObject;
                bbbbbbbbb.teamIndex = TeamIndex.Neutral;
                bbbbbbbbb.procCoefficient = 0f;
                bbbbbbbbb.radius = 120f;
                bbbbbbbbb.baseForce = 8000;
                bbbbbbbbb.bonusForce = Vector3.up * 200f;
                bbbbbbbbb.baseDamage = 0f;
                bbbbbbbbb.falloffModel = BlastAttack.FalloffModel.Linear;
                bbbbbbbbb.damageColorIndex = DamageColorIndex.Default;
                bbbbbbbbb.attackerFiltering = AttackerFiltering.NeverHitSelf;
                bbbbbbbbb.damageType = DamageType.Stun1s;

                bbbbbbbbb.position = this.transform.position;
                bbbbbbbbb.Fire();

                stoppedSound = true;
				Transform modelTransform = base.GetModelTransform();
				if (modelTransform)
                {
                    CharacterModel cm = modelTransform.gameObject.GetComponent<CharacterModel>();
					if (cm)
                    {
                        if (cm.temporaryOverlays == null) cm.temporaryOverlays = new List<TemporaryOverlayInstance>();
                        TemporaryOverlayInstance temporaryOverlay = TemporaryOverlayManager.AddOverlay(cm.gameObject);
                        temporaryOverlay.duration = 1000f;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 10f, 1f, 10f);
                        temporaryOverlay.destroyComponentOnEnd = false;
                        temporaryOverlay.originalMaterial = onFireMat;
                        temporaryOverlay.inspectorCharacterModel = cm;
						temporaryOverlay.Start();
                    }
                }
				base.PlayAnimation("Gesture, Override", "Flamebreath", "Flamebreath.playbackRate", this.exitDuration);
			}
			bool flag3 = this.stopwatch >= this.entryDuration + 0.75f * this.exitDuration && !this.heck;
			if (flag3)
			{
				this.heck = true;
				base.PlayCrossfade("Gesture, Override", "ExitFlamebreath", "ExitFlamebreath.playbackRate", 0.75f * this.exitDuration, 0.1f);
				bool active = NetworkServer.active;
				if (active)
				{
					//base.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
				}
			}
			bool flag4 = this.stopwatch >= this.entryDuration + this.exitDuration && base.isAuthority;
			if (flag4)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

        private static Material onFireMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matOnFire.mat").WaitForCompletion();
        public static float baseEntryDuration = 1.5f;
		public static float baseExitDuration = 6f;

		private float stopwatch;
		private float entryDuration;
		private float exitDuration;
		private bool hasEnraged;
		private bool heck;
		private bool stoppedSound = false;
		private ChildLocator childLocator;
		private DireseekerController direController;
	}
}
