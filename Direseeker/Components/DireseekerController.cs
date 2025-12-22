using System;
using UnityEngine;

namespace DireseekerMod.Components
{
	public class DireseekerController : MonoBehaviour
	{
		public void StartRageMode()
		{
			if (this.rageFlame) this.rageFlame.Play();
		}

		public void FlameBurst()
		{
			if (this.burstFlame) this.burstFlame.Play();
		}

		public ParticleSystem burstFlame;
		public ParticleSystem rageFlame;
	}
}
