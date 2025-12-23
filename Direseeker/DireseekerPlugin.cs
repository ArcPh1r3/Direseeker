using BepInEx;
using DireseekerMod.Modules;
using R2API.Utils;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Direseeker
{
	[BepInDependency("com.Moffein.AccurateEnemies", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	[BepInPlugin("com.rob.Direseeker", "Direseeker", "2.0.0")]
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
