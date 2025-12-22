using RoR2;
using RoR2.Projectile;
using RoR2BepInExPack.GameAssetPaths;
using UnityEngine;

namespace DireseekerMod.Components
{
	public class DireseekerController : MonoBehaviour
	{
        public ParticleSystem burstFlame;
        public ParticleSystem rageFlame;

		private CharacterBody characterBody;
		private HealthComponent healthComponent;
		private bool rage = false;
		private float bombaStopwatch;

		private void Awake()
		{
			this.characterBody = this.GetComponent<CharacterBody>();
			this.healthComponent = this.GetComponent<HealthComponent>();
		}

		private void FixedUpdate()
		{
			this.bombaStopwatch -= Time.fixedDeltaTime;

			if (this.rage)
			{
				if (this.bombaStopwatch <= 0f)
				{
					this.bombaStopwatch = 0.5f;
					this.AttemptShitMeatball();
				}
			}
		}

		private void AttemptShitMeatball()
		{
			// should i be firing it from the server?
			// i think i should

			// i'm gonna fire it from auth instead
			if (this.characterBody && this.healthComponent && this.healthComponent.alive)
			{
				Util.PlaySound("sfx_direseeker_magma", this.gameObject);
				if (this.characterBody.hasEffectiveAuthority) this.ShitMeatball();
			}
		}

		private void ShitMeatball()
		{
			Vector3 pos = this.characterBody.corePosition;
            int ballsCount = Random.Range(1, 5);

            for (int i = 0; i < ballsCount; i++)
            {
                float speed = Random.Range(12f, 48f);
                Vector3 lookVector = (pos + (Vector3.up * Random.Range(-4f, 16f)) + (7f * Random.insideUnitSphere)) - pos;

                ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                {
                    crit = Util.CheckRoll(this.characterBody.crit),
                    damage = 1.2f * this.characterBody.damage,
                    damageColorIndex = DamageColorIndex.Default,
                    force = 1000f,
                    owner = this.gameObject,
                    position = pos,
                    procChainMask = default(ProcChainMask),
                    projectilePrefab = Modules.Projectiles.fireballPrefab,
                    rotation = Util.QuaternionSafeLookRotation(lookVector),
                    speedOverride = speed,
                    useFuseOverride = false,
                    useSpeedOverride = true
                });
            }
        }

        public void StartRageMode()
		{
			this.rage = true;
			if (this.rageFlame) this.rageFlame.Play();
		}

		public void FlameBurst()
		{
			if (this.burstFlame) this.burstFlame.Play();
		}
	}
}