using System;
using System.Collections.Generic;
using DireseekerMod.Components;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DireseekerMod.States.Missions.DireseekerEncounter
{
	public class Listening : EntityState
	{
        public static float delayBeforeBeginningEncounter = 3f;
        public static int buttonsPressedToTriggerEncounter = Direseeker.Modules.DireseekerConfig.requiredButtons.Value;

        private ScriptedCombatEncounter scriptedCombatEncounter;
        private List<DireseekerButtonController> buttonList = new List<DireseekerButtonController>();

        private const float delayBeforeRegisteringButtons = 2f;

        private bool hasRegisteredButtons;
        private int previousPressedButtonCount;
        private bool beginEncounterCountdown;
        private float encounterCountdown;

        public override void OnEnter()
		{
			base.OnEnter();
			this.scriptedCombatEncounter = base.GetComponent<ScriptedCombatEncounter>();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				if (base.fixedAge >= delayBeforeRegisteringButtons)
				{
					this.RegisterButtons();
				}

				if (this.hasRegisteredButtons)
				{
					int num = 0;
					for (int i = 0; i < this.buttonList.Count; i++)
					{
						if (this.buttonList[i].isPressedServer)
						{
							num++;
						}
					}
					int num2 = Listening.buttonsPressedToTriggerEncounter - 1;
					bool flag3 = this.previousPressedButtonCount < num2 && num >= num2;
					if (flag3)
					{
						Chat.SendBroadcastChat(new Chat.SimpleChatMessage
						{
							baseToken = "DIRESEEKER_SPAWN_WARNING"
						});
					}
					bool flag4 = num >= Listening.buttonsPressedToTriggerEncounter && !this.beginEncounterCountdown;
					if (flag4)
					{
						this.encounterCountdown = Listening.delayBeforeBeginningEncounter;
						this.beginEncounterCountdown = true;
						Chat.SendBroadcastChat(new Chat.SimpleChatMessage
						{
							baseToken = "DIRESEEKER_SPAWN_BEGIN"
						});
					}
					bool flag5 = this.beginEncounterCountdown;
					if (flag5)
					{
						this.encounterCountdown -= Time.fixedDeltaTime;
						bool flag6 = this.encounterCountdown <= 0f;
						if (flag6)
						{
							this.scriptedCombatEncounter.BeginEncounter();
							this.outer.SetNextState(new Idle());
						}
					}
					this.previousPressedButtonCount = num;
				}
			}
		}

		private void RegisterButtons()
		{
			bool flag = this.hasRegisteredButtons;
			if (!flag)
			{
				foreach (DireseekerButtonController item in UnityEngine.Object.FindObjectsOfType<DireseekerButtonController>())
				{
					this.buttonList.Add(item);
				}
				this.hasRegisteredButtons = true;
			}
		}
	}
}