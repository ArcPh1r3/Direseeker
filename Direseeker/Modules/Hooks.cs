using HarmonyLib;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace DireseekerMod.Modules
{
	public class Hooks
	{
        public Vector3[] buttonPosition;
        public Vector3[] buttonRotation;

        private void InitButtonPositions()
		{
			this.buttonPosition = new Vector3[]
			{
				new Vector3(2f, -140.5f, -439f),
				new Vector3(110f, -179.2f, -150f),
				new Vector3(203f, -75f, -195.5f),
				new Vector3(51f, -86f, -200.6f),
				new Vector3(-154f, -153f, -103.6f),
				new Vector3(-0.5f, -135.5f, -12.5f),
				new Vector3(-37.5f, -127f, -287.8f)
			};
			this.buttonRotation = new Vector3[]
			{
				new Vector3(0f, 60f, 5f),
				new Vector3(0f, 45f, 0f),
				new Vector3(290f, 340f, 0f),
				new Vector3(270f, 0f, 0f),
				new Vector3(270f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 90f, 110f)
			};
		}

		public void ApplyHooks()
		{
			this.InitButtonPositions();

			On.RoR2.SceneDirector.Start += (orig, self) =>
			{
				if (NetworkServer.active && SceneManager.GetActiveScene().name == "dampcavesimple")
				{
					UnityEngine.Object.Destroy(GameObject.Find("HOLDER: Newt Statues and Preplaced Chests").transform.Find("GoldChest").gameObject);
					for (int i = 0; i < this.buttonPosition.Length; i++)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Assets.direseekerButton);
						gameObject.transform.position = this.buttonPosition[i];
						gameObject.transform.rotation = Quaternion.Euler(this.buttonRotation[i]);
						NetworkServer.Spawn(gameObject);
					}

					GameObject encounter = UnityEngine.Object.Instantiate<GameObject>(Assets.direseekerEncounter);
                    BossGroup bossGroup = encounter.GetComponentInChildren<BossGroup>();

                    if (Direseeker.Modules.DireseekerConfig.legendaryChest)
                    {
                        bossGroup.bossDropChance = 9999f;
                        bossGroup.forceTier3Reward = false;
                        bossGroup.bossDrops = new System.Collections.Generic.List<UniquePickup>();
                        bossGroup.bossDrops.Add(new UniquePickup
                        {
                            decayValue = 0f,
                            upgradeValue = 0,
                            pickupIndex = PickupCatalog.FindPickupIndex(Assets.fuckedItemDef.itemIndex)
                        });
                    }
					else
					{
                        bossGroup.bossDropChance = 0f;
                        bossGroup.bossDrops = new System.Collections.Generic.List<UniquePickup>();
                        bossGroup.forceTier3Reward = true;
                    }
                    bossGroup.scaleRewardsByPlayerCount = Direseeker.Modules.DireseekerConfig.multipleDrops;

					if (Direseeker.DireseekerPlugin.GoldChestForAllLoaded && Direseeker.Modules.DireseekerConfig.legendaryChest) bossGroup.scaleRewardsByPlayerCount = false;

                    NetworkServer.Spawn(encounter);
				}
				orig(self);
			};

            On.RoR2.PickupDropletController.OnCollisionEnter += PickupDropletController_OnCollisionEnter;
        }

        private static void PickupDropletController_OnCollisionEnter(On.RoR2.PickupDropletController.orig_OnCollisionEnter orig, PickupDropletController self, Collision collision)
        {
			if (self)
			{
				if (self.pickupIndex == PickupCatalog.FindPickupIndex(Modules.Assets.fuckedItemDef.itemIndex))
				{
					if (NetworkServer.active && self.alive)
					{
						// yeah, this isn't good
                        self.alive = false;
                        SpawnChest(self.transform.position);
                        UnityEngine.Object.Destroy(self.gameObject);
					}
					return;
				}
			}
            orig(self, collision);
        }

        public static void SpawnChest(Vector3 pos)
        {
            Quaternion rot = Quaternion.identity;
            RaycastHit raycastHit;
            if (Physics.Raycast(pos + Vector3.up, Vector3.down, out raycastHit, 5f, LayerIndex.world.mask))
            {
                pos = raycastHit.point;
                rot = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
            }

			GameObject chest = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/GoldChest/GoldChest.prefab").WaitForCompletion(), pos, rot);
			chest.GetComponent<PurchaseInteraction>().costType = CostTypeIndex.None;
			chest.GetComponent<PurchaseInteraction>().cost = 0;
			NetworkServer.Spawn(chest);

            EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/PodGroundImpact.prefab").WaitForCompletion(),
new EffectData
{
    origin = pos,
    rotation = Quaternion.identity,
    scale = 2f
}, true);
        }
    }
}