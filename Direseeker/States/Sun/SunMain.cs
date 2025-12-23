using RoR2;

namespace DireseekerMod.States.Sun
{
    public class SunMain : SunBase
    {
        private GenericOwnership ownership;

        protected override bool shouldEnableSunController => true;

        public override bool shouldSpawnEffect => false;

        protected override float desiredVfxScale => 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            ownership = GetComponent<GenericOwnership>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && !ownership.ownerObject)
            {
                outer.SetNextState(new SunDeath());
            }
        }
    }
}