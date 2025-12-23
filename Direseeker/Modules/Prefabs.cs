using System;
using System.Collections.Generic;
using DireseekerMod.Components;
using KinematicCharacterController;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace DireseekerMod.Modules
{
	public static class Prefabs
	{
		public static void CreatePrefab()
		{
			Prefabs.direseekerBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LemurianBruiser/LemurianBruiserBody.prefab").WaitForCompletion().InstantiateClone("DireseekerBody", true);

			Rigidbody rb = Prefabs.direseekerBodyPrefab.GetComponent<Rigidbody>();
			if (rb) rb.mass = 900;


			//Add DeathReward
			//This causes all its drops to be replaced by the Perforator. Why?
			/*GameObject WormPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion();
			DeathRewards deathReward = Prefabs.bodyPrefab.GetComponent<DeathRewards>();
			deathReward.bossDropTable = WormPrefab.GetComponent<DeathRewards>().bossDropTable;*/
			UnityEngine.Object.Destroy(Prefabs.direseekerBodyPrefab.GetComponent<SetStateOnHurt>());
			CharacterBody component = Prefabs.direseekerBodyPrefab.GetComponent<CharacterBody>();
			component.name = "DireseekerBossBody";
			component.baseNameToken = "DIRESEEKER_BOSS_BODY_NAME";
			component.subtitleNameToken = "DIRESEEKER_BOSS_BODY_SUBTITLE";
			component.baseMoveSpeed = 11f;
			component.baseMaxHealth = 2800f;
			component.levelMaxHealth = component.baseMaxHealth * 0.3f;
			component.baseDamage = 20f;
			component.levelDamage = component.baseDamage * 0.2f;
			component.isChampion = true;
			component.portraitIcon = Assets.bossPortrait;
			component.bodyFlags |= CharacterBody.BodyFlags.Ungrabbable;
            component.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;
            Prefabs.direseekerBodyPrefab.GetComponent<SfxLocator>().deathSound = "sfx_direseeker_death";
			Prefabs.direseekerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.localScale *= 1.5f;
			Prefabs.direseekerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.localPosition = Vector3.up * -5.5f;

			foreach (KinematicCharacterMotor kinematicCharacterMotor in Prefabs.direseekerBodyPrefab.GetComponentsInChildren<KinematicCharacterMotor>())
			{
				kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 1.5f, kinematicCharacterMotor.Capsule.height * 1.5f, 1.5f);
			}

			GameObject direseekerHorn = Assets.mainAssetBundle.LoadAsset<GameObject>("DireHorn").InstantiateClone("DireseekerHorn", false);
			GameObject DireseekerHornBroken = Assets.mainAssetBundle.LoadAsset<GameObject>("DireHornBroken").InstantiateClone("DireseekerHornBroken", false);
			GameObject DireseekerRageFlame = Assets.mainAssetBundle.LoadAsset<GameObject>("DireseekerRageFlame").InstantiateClone("DireseekerRageFlame", false);
			GameObject DireseekerBurstFlame = Assets.mainAssetBundle.LoadAsset<GameObject>("DireseekerBurstFlame").InstantiateClone("DireseekerBurstFlame", false);
			
			ChildLocator childLocator = Prefabs.direseekerBodyPrefab.GetComponentInChildren<ChildLocator>();
			direseekerHorn.transform.SetParent(childLocator.FindChild("Head"));
			direseekerHorn.transform.localPosition = new Vector3(-2.5f, 1f, -0.5f);
			direseekerHorn.transform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 90f));
			direseekerHorn.transform.localScale = new Vector3(100f, 100f, 100f);
			Shader shader = Addressables.LoadAssetAsync<Shader>("RoR2/Base/Shaders/HGStandard.shader").WaitForCompletion();
			Material hornMaterial = direseekerHorn.GetComponentInChildren<MeshRenderer>().material;
			hornMaterial.shader = shader;

			DireseekerHornBroken.transform.SetParent(childLocator.FindChild("Head"));
			DireseekerHornBroken.transform.localPosition = new Vector3(2.5f, 1f, -0.5f);
			DireseekerHornBroken.transform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 90f));
			DireseekerHornBroken.transform.localScale = new Vector3(100f, -100f, 100f);

			DireseekerRageFlame.transform.SetParent(childLocator.FindChild("Head"));
			DireseekerRageFlame.transform.localPosition = new Vector3(0f, 1f, 0f);
			DireseekerRageFlame.transform.localRotation = Quaternion.Euler(new Vector3(270f, 180f, 0f));
			DireseekerRageFlame.transform.localScale = new Vector3(4f, 4f, 4f);

			DireseekerBurstFlame.transform.SetParent(childLocator.FindChild("Head"));
			DireseekerBurstFlame.transform.localPosition = new Vector3(0f, 1f, 0f);
			DireseekerBurstFlame.transform.localRotation = Quaternion.Euler(new Vector3(270f, 180f, 0f));
			DireseekerBurstFlame.transform.localScale = new Vector3(6f, 6f, 6f);

			DireseekerController direseekerController = Prefabs.direseekerBodyPrefab.AddComponent<DireseekerController>();
			direseekerController.burstFlame = DireseekerBurstFlame.GetComponent<ParticleSystem>();
			direseekerController.rageFlame = DireseekerRageFlame.GetComponent<ParticleSystem>();
			direseekerController.rageFlame.Stop();

			CharacterModel direseekerCharacterModel = Prefabs.direseekerBodyPrefab.GetComponentInChildren<CharacterModel>();
			direseekerCharacterModel.gameObject.name = "mdlDireseeker";
			Material direseekerMaterialCloned = new Material(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Lemurian.matLemurianBruiser_mat).WaitForCompletion());
			direseekerMaterialCloned.name = "matDireseekerMesh";
			direseekerMaterialCloned.SetTexture("_MainTex", Assets.mainAssetBundle.LoadAsset<Material>("matDireseeker").GetTexture("_MainTex"));
			direseekerMaterialCloned.SetTexture("_EmTex", Assets.mainAssetBundle.LoadAsset<Material>("matDireseeker").GetTexture("_EmissionMap"));
			direseekerMaterialCloned.SetFloat("_EmPower", 50f);

			//clone the SkinDef from original monster
			SkinDef origSkin = Addressables.LoadAssetAsync<SkinDef>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_LemurianBruiser.skinLemurianBruiserBodyDefault_asset).WaitForCompletion();
			SkinDef newDefaultSkin = UnityEngine.Object.Instantiate(origSkin);

			//SkinDef isn't enough. The actual info is now stored in the SkinDefParams asset.
			SkinDefParams origSkinDefParams = Addressables.LoadAssetAsync<SkinDefParams>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_LemurianBruiser_skinLemurianBruiserBodyDefault.params_asset).WaitForCompletion();
			newDefaultSkin.skinDefParams = UnityEngine.Object.Instantiate(origSkinDefParams);
			//vanilla skins use an address instead of a direct reference. We're removing the address and using a direct reference instead.
			newDefaultSkin.skinDefParamsAddress = new AssetReferenceT<SkinDefParams>("");
			newDefaultSkin.rootObject = direseekerCharacterModel.gameObject;

			//the data in SkinDefParams is set to reference the renderer from the original body. we must replace that with our renderer, which we will do in the rendererinfo and meshreplacements below.
			Renderer bodyRenderer = direseekerCharacterModel.transform.Find("LemurianBruiserMesh").GetComponent<Renderer>();

			////this would be the way to modify the existing rendererinfo, but if we're doing this much work, might as well just pass in a new rendererinfo with this info.
			//defaultSkin.skinDefParams.rendererInfos[0].defaultMaterial = direseekerMaterialCloned;
			////vanilla skins use an address instead of a direct reference. We're removing the address to do the opposite
			//defaultSkin.skinDefParams.rendererInfos[0].defaultMaterialAddress = new AssetReferenceT<Material>("");
			//defaultSkin.skinDefParams.rendererInfos[0].renderer = bodyRenderer;

			CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[]
			{
				//defaultSkin.skinDefParams.rendererInfos[0],
				new CharacterModel.RendererInfo
				{
					renderer = bodyRenderer,
					defaultMaterial = direseekerMaterialCloned,
					defaultShadowCastingMode = ShadowCastingMode.On,
					ignoreOverlays = false,
				},
				new CharacterModel.RendererInfo
				{
					renderer = direseekerHorn.GetComponentInChildren<MeshRenderer>(),
					defaultMaterial = hornMaterial,
					defaultShadowCastingMode = ShadowCastingMode.On,
					ignoreOverlays = true
				},
				new CharacterModel.RendererInfo
				{
					renderer = DireseekerHornBroken.GetComponentInChildren<MeshRenderer>(),
					defaultMaterial = hornMaterial,
					defaultShadowCastingMode = ShadowCastingMode.On,
					ignoreOverlays = true
				}
			};
			newDefaultSkin.skinDefParams.rendererInfos = newRendererInfos;

			//must also replace the referenced renderer in the meshreplacements
			newDefaultSkin.skinDefParams.meshReplacements[0].renderer = bodyRenderer;

			//finally, replace the skin in the ModelSkinController with ours
			direseekerCharacterModel.gameObject.GetComponent<ModelSkinController>().skins[0] = newDefaultSkin;

			// wow this process got a whole lot messier huh

			Prefabs.masterPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LemurianBruiser/LemurianBruiserMaster.prefab").WaitForCompletion().InstantiateClone("DireseekerBossMaster", true);
			CharacterMaster characterMaster = Prefabs.masterPrefab.GetComponent<CharacterMaster>();
			characterMaster.bodyPrefab = Prefabs.direseekerBodyPrefab;
			characterMaster.isBoss = true;
			Prefabs.CreateAI();

			ContentAddition.AddBody(Prefabs.direseekerBodyPrefab);
			ContentAddition.AddMaster(Prefabs.masterPrefab);
		}

		private static void CreateAI()
		{
			foreach (AISkillDriver obj in Prefabs.masterPrefab.GetComponentsInChildren<AISkillDriver>())
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			AISkillDriver aiskillDriver = Prefabs.masterPrefab.AddComponent<AISkillDriver>();
			aiskillDriver.customName = "Enrage";
			aiskillDriver.requireSkillReady = true;
			aiskillDriver.movementType = AISkillDriver.MovementType.Stop;
			aiskillDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			aiskillDriver.selectionRequiresTargetLoS = false;
			aiskillDriver.activationRequiresAimConfirmation = false;
			aiskillDriver.activationRequiresTargetLoS = false;
			aiskillDriver.maxDistance = float.PositiveInfinity;
			aiskillDriver.minDistance = 0f;
			aiskillDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver.ignoreNodeGraph = false;
			aiskillDriver.moveInputScale = 1f;
			aiskillDriver.driverUpdateTimerOverride = -1f;
			aiskillDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver.maxUserHealthFraction = 0.4f;
			aiskillDriver.skillSlot = SkillSlot.Special;
			AISkillDriver aiskillDriver2 = Prefabs.masterPrefab.AddComponent<AISkillDriver>();
			aiskillDriver2.customName = "FlamePillar";
			aiskillDriver2.requireSkillReady = true;
			aiskillDriver2.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver2.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			aiskillDriver2.selectionRequiresTargetLoS = true;
			aiskillDriver2.activationRequiresAimConfirmation = true;
			aiskillDriver2.activationRequiresTargetLoS = false;
			aiskillDriver2.maxDistance = 120f;
			aiskillDriver2.minDistance = 5f;
			aiskillDriver2.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver2.ignoreNodeGraph = false;
			aiskillDriver2.moveInputScale = 1f;
			aiskillDriver2.driverUpdateTimerOverride = -1f;
			aiskillDriver2.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver2.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver2.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver2.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver2.maxUserHealthFraction = 0.8f;
			aiskillDriver2.skillSlot = SkillSlot.Utility;
			AISkillDriver aiskillDriver3 = Prefabs.masterPrefab.AddComponent<AISkillDriver>();
			aiskillDriver3.customName = "Flamethrower";
			aiskillDriver3.requireSkillReady = true;
			aiskillDriver3.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver3.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			aiskillDriver3.selectionRequiresTargetLoS = true;
			aiskillDriver3.activationRequiresAimConfirmation = true;
			aiskillDriver3.activationRequiresTargetLoS = false;
			aiskillDriver3.maxDistance = 20f;
			aiskillDriver3.minDistance = 0f;
			aiskillDriver3.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver3.ignoreNodeGraph = false;
			aiskillDriver3.moveInputScale = 1f;
			aiskillDriver3.driverUpdateTimerOverride = -1f;
			aiskillDriver3.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver3.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver3.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver3.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver3.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver3.skillSlot = SkillSlot.Secondary;
			AISkillDriver aiskillDriver4 = Prefabs.masterPrefab.AddComponent<AISkillDriver>();
			aiskillDriver4.customName = "RunAndShoot";
			aiskillDriver4.requireSkillReady = true;
			aiskillDriver4.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver4.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			aiskillDriver4.selectionRequiresTargetLoS = true;
			aiskillDriver4.activationRequiresAimConfirmation = true;
			aiskillDriver4.activationRequiresTargetLoS = false;
			aiskillDriver4.maxDistance = 50f;
			aiskillDriver4.minDistance = 0f;
			aiskillDriver4.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver4.ignoreNodeGraph = false;
			aiskillDriver4.moveInputScale = 1f;
			aiskillDriver4.driverUpdateTimerOverride = 2f;
			aiskillDriver4.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver4.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver4.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver4.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver4.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver4.skillSlot = SkillSlot.Primary;
			AISkillDriver aiskillDriver5 = Prefabs.masterPrefab.AddComponent<AISkillDriver>();
			aiskillDriver5.customName = "StopAndShoot";
			aiskillDriver5.requireSkillReady = true;
			aiskillDriver5.movementType = AISkillDriver.MovementType.Stop;
			aiskillDriver5.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			aiskillDriver5.selectionRequiresTargetLoS = true;
			aiskillDriver5.activationRequiresAimConfirmation = true;
			aiskillDriver5.activationRequiresTargetLoS = false;
			aiskillDriver5.maxDistance = 100f;
			aiskillDriver5.minDistance = 50f;
			aiskillDriver5.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver5.ignoreNodeGraph = false;
			aiskillDriver5.moveInputScale = 1f;
			aiskillDriver5.driverUpdateTimerOverride = 2f;
			aiskillDriver5.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver5.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver5.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver5.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver5.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver5.skillSlot = SkillSlot.Primary;
			AISkillDriver aiskillDriver6 = Prefabs.masterPrefab.AddComponent<AISkillDriver>();
			aiskillDriver6.customName = "Chase";
			aiskillDriver6.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver6.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			aiskillDriver6.activationRequiresAimConfirmation = false;
			aiskillDriver6.activationRequiresTargetLoS = false;
			aiskillDriver6.maxDistance = float.PositiveInfinity;
			aiskillDriver6.minDistance = 0f;
			aiskillDriver6.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver6.ignoreNodeGraph = false;
			aiskillDriver6.moveInputScale = 1f;
			aiskillDriver6.driverUpdateTimerOverride = -1f;
			aiskillDriver6.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver6.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver6.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver6.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver6.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver6.skillSlot = SkillSlot.None;
		}

		public static GameObject direseekerBodyPrefab;
		public static GameObject masterPrefab;

	}
}
