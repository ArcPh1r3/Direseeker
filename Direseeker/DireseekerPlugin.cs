using System;
using System.Diagnostics;
using BepInEx;
using DireseekerMod.Modules;
using R2API.Utils;

namespace Direseeker
{
	[BepInDependency("com.Moffein.AccurateEnemies", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	[BepInPlugin("com.rob.Direseeker", "Direseeker", "1.5.1")]
	public class DireseekerPlugin : BaseUnityPlugin
	{
		public static bool AccurateEnemiesLoaded = false;
		public static bool AccurateEnemiesCompat = true;

		public static PluginInfo pluginInfo;
		public void Awake()
		{
			AccurateEnemiesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AccurateEnemies");
			pluginInfo = Info;
            DireseekerMod.Modules.Assets.PopulateAssets();
			Tokens.RegisterLanguageTokens();
			Prefabs.CreatePrefab();
			States.RegisterStates();
			Skills.RegisterSkills();
			Projectiles.CreateProjectiles();
			SpawnCards.CreateSpawnCards();
            DireseekerMod.Modules.Assets.UpdateAssets();
			new Hooks().ApplyHooks();
		}
	}
}
