using System;
using DireseekerMod.States;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace DireseekerMod.Modules
{
	public static class Skills
	{
		public static SkillDef epicFlamePillarSkillDef;

		public static void RegisterSkills()
		{
			GameObject bodyPrefab = Prefabs.direseekerBodyPrefab;
			foreach (GenericSkill obj in bodyPrefab.GetComponentsInChildren<GenericSkill>())
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			SkillLocator componentInChildren = bodyPrefab.GetComponentInChildren<SkillLocator>();
			SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
			skillDef.activationState = new SerializableEntityStateType(typeof(ChargeUltraFireball));
			skillDef.activationStateMachineName = "Weapon";
			skillDef.baseMaxStock = 3;
			skillDef.baseRechargeInterval = 16f;
			skillDef.beginSkillCooldownOnSkillEnd = true;
			skillDef.canceledFromSprinting = false;
			skillDef.fullRestockOnAssign = true;
			skillDef.interruptPriority = InterruptPriority.Any;
			skillDef.resetCooldownTimerOnUse = false;
			skillDef.isCombatSkill = true;
			skillDef.mustKeyPress = false;
			skillDef.cancelSprintingOnActivation = false;
			skillDef.rechargeStock = 1;
			skillDef.requiredStock = 1;
			skillDef.stockToConsume = 1;
			ContentAddition.AddSkillDef(skillDef);
			componentInChildren.primary = bodyPrefab.AddComponent<GenericSkill>();
			SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
			skillFamily.variants = new SkillFamily.Variant[1];
			componentInChildren.primary.SetFieldValue("_skillFamily", skillFamily);
			SkillFamily skillFamily2 = componentInChildren.primary.skillFamily;
			skillFamily2.variants[0] = new SkillFamily.Variant
			{
				skillDef = skillDef,
				viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
			};
			ContentAddition.AddSkillFamily(skillFamily);

			skillDef = ScriptableObject.CreateInstance<SkillDef>();
			skillDef.activationState = new SerializableEntityStateType(typeof(Flamethrower));
			skillDef.activationStateMachineName = "Body";
			skillDef.baseMaxStock = 1;
			skillDef.baseRechargeInterval = 0f;
			skillDef.beginSkillCooldownOnSkillEnd = false;
			skillDef.canceledFromSprinting = false;
			skillDef.fullRestockOnAssign = true;
			skillDef.interruptPriority = InterruptPriority.Any;
			skillDef.resetCooldownTimerOnUse = false;
			skillDef.isCombatSkill = true;
			skillDef.mustKeyPress = false;
			skillDef.cancelSprintingOnActivation = true;
			skillDef.rechargeStock = 1;
			skillDef.requiredStock = 1;
			skillDef.stockToConsume = 0;
			ContentAddition.AddSkillDef(skillDef);
			componentInChildren.secondary = bodyPrefab.AddComponent<GenericSkill>();
			skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
			skillFamily.variants = new SkillFamily.Variant[1];
			componentInChildren.secondary.SetFieldValue("_skillFamily", skillFamily);
			skillFamily2 = componentInChildren.secondary.skillFamily;
			skillFamily2.variants[0] = new SkillFamily.Variant
			{
				skillDef = skillDef,
				viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
			};
			ContentAddition.AddSkillFamily(skillFamily);

			epicFlamePillarSkillDef = ScriptableObject.CreateInstance<SkillDef>();
			epicFlamePillarSkillDef.activationState = new SerializableEntityStateType(typeof(FlamePillars));
			epicFlamePillarSkillDef.activationStateMachineName = "Body";
			epicFlamePillarSkillDef.baseMaxStock = 3;
			epicFlamePillarSkillDef.baseRechargeInterval = 3f;
			epicFlamePillarSkillDef.beginSkillCooldownOnSkillEnd = false;
			epicFlamePillarSkillDef.canceledFromSprinting = false;
			epicFlamePillarSkillDef.fullRestockOnAssign = true;
			epicFlamePillarSkillDef.interruptPriority = InterruptPriority.Any;
			epicFlamePillarSkillDef.resetCooldownTimerOnUse = true;
			epicFlamePillarSkillDef.isCombatSkill = true;
			epicFlamePillarSkillDef.mustKeyPress = false;
			epicFlamePillarSkillDef.cancelSprintingOnActivation = true;
			epicFlamePillarSkillDef.rechargeStock = 3;
			epicFlamePillarSkillDef.requiredStock = 1;
			epicFlamePillarSkillDef.stockToConsume = 1;
			ContentAddition.AddSkillDef(skillDef);
			componentInChildren.utility = bodyPrefab.AddComponent<GenericSkill>();
			skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
			skillFamily.variants = new SkillFamily.Variant[1];
			componentInChildren.utility.SetFieldValue("_skillFamily", skillFamily);
			skillFamily2 = componentInChildren.utility.skillFamily;
			skillFamily2.variants[0] = new SkillFamily.Variant
			{
				skillDef = skillDef,
				viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
			};
			ContentAddition.AddSkillFamily(skillFamily);


            skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.activationState = new SerializableEntityStateType(typeof(FlamePillars));
            skillDef.activationStateMachineName = "Body";
            skillDef.baseMaxStock = 1;
            skillDef.baseRechargeInterval = 12f;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.canceledFromSprinting = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.interruptPriority = InterruptPriority.Any;
            skillDef.resetCooldownTimerOnUse = false;
            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = false;
            skillDef.cancelSprintingOnActivation = true;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 1;
            skillDef.stockToConsume = 1;

            skillDef = ScriptableObject.CreateInstance<SkillDef>();
			skillDef.activationState = new SerializableEntityStateType(typeof(Enrage));
			skillDef.activationStateMachineName = "Body";
			skillDef.baseMaxStock = 1;
			skillDef.baseRechargeInterval = 0f;
			skillDef.beginSkillCooldownOnSkillEnd = false;
			skillDef.canceledFromSprinting = false;
			skillDef.fullRestockOnAssign = true;
			skillDef.interruptPriority = InterruptPriority.PrioritySkill;
			skillDef.resetCooldownTimerOnUse = false;
			skillDef.isCombatSkill = true;
			skillDef.mustKeyPress = false;
			skillDef.cancelSprintingOnActivation = true;
			skillDef.rechargeStock = 0;
			skillDef.requiredStock = 1;
			skillDef.stockToConsume = 1;
			ContentAddition.AddSkillDef(skillDef);
			componentInChildren.special = bodyPrefab.AddComponent<GenericSkill>();
			skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
			skillFamily.variants = new SkillFamily.Variant[1];
			componentInChildren.special.SetFieldValue("_skillFamily", skillFamily);
			skillFamily2 = componentInChildren.special.skillFamily;
			skillFamily2.variants[0] = new SkillFamily.Variant
			{
				skillDef = skillDef,
				viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
			};
			ContentAddition.AddSkillFamily(skillFamily);
		}
	}
}
