using System;
using DireseekerMod.States;
using DireseekerMod.States.Missions.DireseekerEncounter;
using DireseekerMod.States.Sun;
using EntityStates;
using R2API;
using RoR2;
using UnityEngine;

namespace DireseekerMod.Modules
{
	public static class States
	{
		public static void RegisterStates()
		{
			GameObject bodyPrefab = Prefabs.direseekerBodyPrefab;

			bool temp;
			ContentAddition.AddEntityState<Listening>(out temp);
			ContentAddition.AddEntityState<SpawnState>(out temp);
			ContentAddition.AddEntityState<ChargeUltraFireball>(out temp);
			ContentAddition.AddEntityState<FireUltraFireball>(out temp);
			ContentAddition.AddEntityState<Flamethrower>(out temp);
			ContentAddition.AddEntityState<FlamePillar>(out temp);
			ContentAddition.AddEntityState<FlamePillars>(out temp);
			ContentAddition.AddEntityState<Enrage>(out temp);
            ContentAddition.AddEntityState<SunBase>(out temp);
            ContentAddition.AddEntityState<SunDeath>(out temp);
            ContentAddition.AddEntityState<SunMain>(out temp);
            ContentAddition.AddEntityState<SunSpawn>(out temp);

            EntityStateMachine componentInChildren = bodyPrefab.GetComponentInChildren<EntityStateMachine>();
			bool flag = componentInChildren;
			if (flag)
			{
				componentInChildren.initialStateType = new SerializableEntityStateType(typeof(SpawnState));
			}
		}
	}
}
