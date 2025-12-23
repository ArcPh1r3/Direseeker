using DireseekerMod.Components;
using DireseekerMod.States.Missions.DireseekerEncounter;
using EntityStates;
using R2API;
using RoR2;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using static RoR2.SolusWing.SolusWingPodAI.Simulation.SimulationState;

namespace DireseekerMod.Modules
{
	public static class Assets
	{
		public static GameObject roarEffect;
        public static GameObject sunPrefab;

        public static void PopulateAssets()
		{
			if (Assets.mainAssetBundle == null)
			{
				using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Direseeker.direseeker"))
				{
					Assets.mainAssetBundle = AssetBundle.LoadFromStream(manifestResourceStream);
				}
			}

            if (Assets.altAssetBundle == null)
            {
                using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Direseeker.direseeker2"))
                {
                    Assets.altAssetBundle = AssetBundle.LoadFromStream(manifestResourceStream);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Direseeker.direseeker_bank2.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            Assets.bossPortrait = Assets.altAssetBundle.LoadAsset<Sprite>("texDireseekerIcon").texture;
			Assets.charPortrait = Assets.mainAssetBundle.LoadAsset<Sprite>("texDireseekerPlayerIcon").texture;
			Assets.direseekerEncounter = Assets.mainAssetBundle.LoadAsset<GameObject>("BossEncounter");
			Assets.direseekerEncounter.AddComponent<NetworkIdentity>();
			Assets.direseekerEncounter.RegisterNetworkPrefab();    //Apparently this auto adds it to the contentpack?

			Assets.direseekerButton = Assets.mainAssetBundle.LoadAsset<GameObject>("DireseekerButton");
			Shader shader = Addressables.LoadAssetAsync<Shader>("RoR2/Base/Shaders/HGStandard.shader").WaitForCompletion();
			Material material = Assets.direseekerButton.GetComponentInChildren<SkinnedMeshRenderer>().material;
			material.shader = shader;
			Assets.direseekerButton.AddComponent<DireseekerButtonController>();
			Assets.direseekerButton.AddComponent<NetworkIdentity>();
			Assets.direseekerButton.RegisterNetworkPrefab();	//Apparently this auto adds it to the contentpack?

            roarEffect = altAssetBundle.LoadAsset<GameObject>("DireseekerRoar");

            roarEffect.transform.Find("Nova").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matTeamAreaIndicatorFullMonster.mat").WaitForCompletion();
            roarEffect.transform.Find("Distortion").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matDistortion.mat").WaitForCompletion();

            var pp = roarEffect.transform.Find("PP").gameObject;
            pp.layer = 20;

            var ppv = pp.AddComponent<PostProcessVolume>();
            ppv.sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/GlobalContent/ppLocalBlur.asset").WaitForCompletion();
            ppv.blendDistance = 120f;
            ppv.priority = 6f;
            ppv.weight = 1f;
            ppv.isGlobal = false;

            var ppd = pp.AddComponent<PostProcessDuration>();
            ppd.ppVolume = ppv;
            ppd.ppWeightCurve = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().GetComponentInChildren<PostProcessDuration>().ppWeightCurve;
            ppd.maxDuration = 5f;
            ppd.destroyOnEnd = true;

            var sc = pp.AddComponent<SphereCollider>();
            sc.contactOffset = 0.01f;
            sc.isTrigger = true;
            sc.radius = 120f;

			flamePillarPredictionEffect = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Items/MeteorAttackOnHighDamage/RunicMeteorStrikePredictionEffect.prefab").WaitForCompletion(), "DireseekerPredictionEffect", true);
            ContentAddition.AddEffect(flamePillarPredictionEffect);

            sunPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Grandparent/GrandParentSun.prefab").WaitForCompletion(), "DireseekerSun", true);

            //Transfering over some data we need from the old script; buff definitions, SFX definitions
            DireseekerSunController sunScript = sunPrefab.AddComponent<DireseekerSunController>();
            GrandParentSunController baseSunScript = sunPrefab.GetComponent<GrandParentSunController>();
            sunScript.buffApplyEffect = baseSunScript.buffApplyEffect;
            sunScript.buffDef = baseSunScript.buffDef;
            sunScript.activeLoopDef = baseSunScript.activeLoopDef;
            sunScript.damageLoopDef = baseSunScript.damageLoopDef;
            sunScript.stopSoundName = baseSunScript.stopSoundName;
            UnityEngine.Object.DestroyImmediate(baseSunScript); //VERY important to remove this once we're done transfering data, since we now have our own controller.

            //Simple script for syncing positions
            sunPrefab.AddComponent<DireseekerSunNetworkController>();

            //EntityStateMachine that can go die in the actual sun. reset the NetworkStateMachine value just in case
            UnityEngine.Object.DestroyImmediate(sunPrefab.GetComponent<EntityStateMachine>());
            sunPrefab.AddComponent<EntityStateMachine>();
            EntityStateMachine esmDireseeker = sunPrefab.GetComponent<EntityStateMachine>();
            esmDireseeker.name = "Body";
            esmDireseeker.initialStateType = new SerializableEntityStateType(typeof(DireseekerMod.States.Sun.SunSpawn));
            sunPrefab.GetComponent<NetworkStateMachine>().stateMachines[0] = esmDireseeker;

            //VFX - Use StaticValues.cruelSunVfxSize to control the scale, changing anything here will cause it not to align with gameplay logic anymore.
            sunPrefab.transform.localScale = Vector3.one * 1f;
            sunPrefab.transform.Find("VfxRoot/LightSpinner/LightSpinner/Point Light").GetComponent<Light>().intensity *= 1f;
            sunPrefab.transform.Find("VfxRoot/LightSpinner/LightSpinner/Point Light").GetComponent<Light>().range = 200 * 1f;
            sunPrefab.transform.Find("VfxRoot/Mesh/SunMesh").transform.localScale = Vector3.one * 1f;
            sunPrefab.transform.Find("VfxRoot/Mesh/AreaIndicator").transform.localScale = Vector3.one * 180;

            //Removing some distracting effects that don't work well here (imo).
            UnityEngine.Object.DestroyImmediate(sunPrefab.transform.Find("VfxRoot/Mesh/SunMesh/MoonMesh").gameObject);
            //ParticleSystems need to have their modules referenced in a variable before we can assign anything to them. I have no fucking idea why.
            //Could destroy these instead of disabling them, but this framework might be useful later for tinkering with other particle settings.
            ParticleSystem psSparks = sunPrefab.transform.Find("VfxRoot/Particles/Sparks").GetComponent<ParticleSystem>();
            var psSparks_emission = psSparks.emission;
            psSparks_emission.enabled = false;
            ParticleSystem psGoo = sunPrefab.transform.Find("VfxRoot/Particles/Goo, Drip").GetComponent<ParticleSystem>();
            var psGoo_emission = psGoo.emission;
            psGoo_emission.enabled = false;
        }

		public static void UpdateAssets()
		{
			GameObject gameObject = Assets.direseekerEncounter.transform.GetChild(0).gameObject;
			gameObject.AddComponent<NetworkIdentity>();

			//Should fix the encounter not showing a healthbar in MP?
			gameObject.RegisterNetworkPrefab();

			ScriptedCombatEncounter scriptedCombatEncounter = gameObject.AddComponent<ScriptedCombatEncounter>();
			Assets.direseekerEncounter.transform.GetChild(0).GetChild(1).Translate(0f, 1f, 0f);
			scriptedCombatEncounter.spawns = new ScriptedCombatEncounter.SpawnInfo[]
			{
				new ScriptedCombatEncounter.SpawnInfo
				{
					explicitSpawnPosition = Assets.direseekerEncounter.transform.GetChild(0).GetChild(0),
					spawnCard = SpawnCards.bossSpawnCard
				}
			};
			scriptedCombatEncounter.randomizeSeed = false;
			scriptedCombatEncounter.teamIndex = TeamIndex.Monster;
			scriptedCombatEncounter.spawnOnStart = false;
			scriptedCombatEncounter.grantUniqueBonusScaling = true;
			BossGroup bossGroup = gameObject.AddComponent<BossGroup>();
			bossGroup.bossDropChance = 1f;
			bossGroup.dropPosition = Assets.direseekerEncounter.transform.GetChild(0).GetChild(1);
			bossGroup.forceTier3Reward = true;
			bossGroup.scaleRewardsByPlayerCount = true;
			bossGroup.shouldDisplayHealthBarOnHud = true;
			CombatSquad combatSquad = gameObject.AddComponent<CombatSquad>();
			EntityStateMachine entityStateMachine = gameObject.AddComponent<EntityStateMachine>();
			entityStateMachine.initialStateType = new SerializableEntityStateType(typeof(Listening));
			entityStateMachine.mainStateType = new SerializableEntityStateType(typeof(Listening));
		}
		
		private static GameObject LoadEffect(string resourceName, string soundName)
		{
			GameObject gameObject = Assets.mainAssetBundle.LoadAsset<GameObject>(resourceName);
			gameObject.AddComponent<DestroyOnTimer>().duration = 12f;
			gameObject.AddComponent<NetworkIdentity>();
			gameObject.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
			EffectComponent effectComponent = gameObject.AddComponent<EffectComponent>();
			effectComponent.applyScale = false;
			effectComponent.effectIndex = EffectIndex.Invalid;
			effectComponent.parentToReferencedTransform = true;
			effectComponent.positionAtReferencedTransform = true;
			effectComponent.soundName = soundName;
			ContentAddition.AddEffect(gameObject);
			return gameObject;
		}

		public static AssetBundle mainAssetBundle;
        public static AssetBundle altAssetBundle;
        public static Texture bossPortrait;
		public static Texture charPortrait;
		public static GameObject flamePillarPredictionEffect;
		public static GameObject direseekerEncounter;
		public static GameObject direseekerButton;
	}
}