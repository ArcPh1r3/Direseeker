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

            Assets.bossPortrait = Assets.mainAssetBundle.LoadAsset<Sprite>("texDireseekerIcon").texture;
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

			Assets.mainAssetBundle.LoadAsset<Material>("matPillarPrediction").shader = shader;
			Assets.flamePillarPredictionEffect = Assets.LoadEffect("FlamePillarPredictionEffect", "");

            roarEffect = altAssetBundle.LoadAsset<GameObject>("DireseekerRoar");

            Material roarMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/FallBoots/matBootShockwave.mat").WaitForCompletion());
            roarMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampGolemElectric.png").WaitForCompletion());

            roarEffect.transform.Find("Nova").GetComponent<ParticleSystemRenderer>().material = roarMat;
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
            ppd.maxDuration = 3.5f;
            ppd.destroyOnEnd = true;

            var sc = pp.AddComponent<SphereCollider>();
            sc.contactOffset = 0.01f;
            sc.isTrigger = true;
            sc.radius = 80f;
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