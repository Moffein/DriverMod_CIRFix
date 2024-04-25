﻿namespace RobDriver.SkillStates.Driver
{
    public class WaitForReload : BaseDriverSkillState
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!base.isAuthority) return;

            if (this.iDrive.weaponTimer == this.iDrive.maxWeaponTimer ||
                this.iDrive.weaponDef.nameToken != this.iDrive.defaultWeaponDef.nameToken ||
                iDrive.HasSpecialBullets)
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge >= 1f)
            {
                this.outer.SetNextState(new ReloadPistol());
            }
        }
    }
}