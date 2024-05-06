﻿using RoR2;
using UnityEngine;
using EntityStates;
using RobDriver.Modules.Components;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using R2API;

namespace RobDriver.SkillStates.Driver.RocketLauncher
{
    public class Barrage : BaseDriverSkillState
    {
        public static float damageCoefficient = 3f;
        public static float procCoefficient = 1f;
        public float baseShotDuration = 0.05f;
        public static float recoil = 11f;

        protected virtual int baseRocketCount => 7;
        protected virtual float maxSpread => 0f;
        protected virtual GameObject projectilePrefab => Modules.Projectiles.missileProjectilePrefab;
        protected virtual float ammoMod => 1.5f;
        protected virtual float _damageCoefficient => Barrage.damageCoefficient;

        private int remainingShots;
        private float shotTimer;
        private float shotDuration;
        protected string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.SetAimTimer(5f);
            this.muzzleString = "ShotgunMuzzle";
            this.shotDuration = this.baseShotDuration / this.attackSpeedStat;
            this.remainingShots = Mathf.Clamp(Mathf.RoundToInt(this.baseRocketCount * this.attackSpeedStat), this.baseRocketCount, 40);

            this.shotTimer = this.shotDuration;
            this.remainingShots--;
            this.Fire();
        }

        public virtual void Fire()
        {
            if (this.iDrive) this.iDrive.ConsumeAmmo(this.ammoMod * (2f / this.baseRocketCount));

            base.PlayAnimation("Gesture, Override", "FireBazooka", "Shoot.playbackRate", 1.4f);
            base.PlayAnimation("AimPitch", "Shoot");

            Util.PlaySound("sfx_driver_rocket_launcher_shoot", base.gameObject);

            float recoilAmplitude = Barrage.recoil / this.attackSpeedStat;

            base.AddRecoil2(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
            this.characterBody.AddSpreadBloom(2f);
            EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion(), this.gameObject, this.muzzleString, false);

            if (base.isAuthority)
            {
                Ray aimRay = this.GetAimRay();
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, this.maxSpread, 1f, 1f, 0f, 0f);

                var projectileDamage = this.projectilePrefab.GetComponent<ProjectileDamage>();
                projectileDamage.damageType = iDrive.DamageType;

                var moddedDamage = this.projectilePrefab.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                moddedDamage.Add(iDrive.ModdedDamageType);

                // copied from moff's rocket
                // the fact that this item literally has to be hardcoded into character skillstates makes me so fucking angry you have no idea
                if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
                {
                    float damageMult = DriverPlugin.GetICBMDamageMult(this.characterBody);

                    Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                    Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                    Vector3 direction = Quaternion.AngleAxis(-1.5f, axis) * aimRay.direction;
                    Quaternion rotation = Quaternion.AngleAxis(1.5f, axis);
                    Ray aimRay2 = new Ray(aimRay.origin, direction);
                    for (int i = 0; i < 3; i++)
                    {
                        ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimRay2.direction), 
                            this.gameObject, damageMult * this.damageStat * Barrage.damageCoefficient, 1200f, this.RollCrit(), DamageColorIndex.Default, null, 120f);
                        
                        aimRay2.direction = rotation * aimRay2.direction;
                    }
                }
                else
                {
                    ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction),
                        this.gameObject, this.damageStat * Barrage.damageCoefficient, 1200f, this.RollCrit(), DamageColorIndex.Default, null, 120f);
                }

                projectileDamage.damageType = DamageType.Generic;
                moddedDamage.Remove(iDrive.ModdedDamageType);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.shotTimer -= Time.fixedDeltaTime;

            if (this.shotTimer <= 0f)
            {
                if (this.remainingShots > 0)
                {
                    this.shotTimer = this.shotDuration;
                    this.remainingShots--;
                    this.Fire();
                }
                else
                {
                    if (base.isAuthority)
                    {
                        this.outer.SetNextStateToMain();
                    }
                }
            }

            if (this.iDrive && this.iDrive.weaponDef.nameToken != this.cachedWeaponDef.nameToken)
            {
                base.PlayAnimation("Gesture, Override", this.iDrive.weaponDef.equipAnimationString);
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.GetModelAnimator().SetTrigger("endAim");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}