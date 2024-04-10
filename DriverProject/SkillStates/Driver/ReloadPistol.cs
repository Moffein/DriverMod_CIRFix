﻿using EntityStates;
using UnityEngine;
using RoR2;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;

namespace RobDriver.SkillStates.Driver
{
    public class ReloadPistol : BaseDriverSkillState
    {
        public float baseDuration = 2.4f;
        public string animString = "ReloadPistol";
        public InterruptPriority interruptPriority = InterruptPriority.PrioritySkill;
        public CameraParamsOverrideHandle camParamsOverrideHandle;
        public bool aiming;

        private bool wasAiming;
        private float duration;
        private bool heheheha;

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.iDrive.passive.isPistolOnly) this.baseDuration *= 0.5f;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.GetModelAnimator().SetFloat("aimBlend", 1f);
            base.PlayCrossfade("Gesture, Override", this.animString, "Action.playbackRate", this.duration * 1.1f, 0.1f);
            Util.PlaySound("sfx_driver_reload_01", this.gameObject);
            this.wasAiming = this.aiming;

            if (NetworkServer.active && this.aiming) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.camParamsOverrideHandle.isValid && !this.aiming) this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
            if (NetworkServer.active && this.aiming) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            //AHHHHH
            if (this.inputBank.skill3.down && this.skillLocator.utility.skillDef == Modules.Survivors.Driver.skateboardSkillDef) base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.25f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.aiming)
            {
                this.characterBody.isSprinting = false;
                this.characterBody.SetAimTimer(1f);
            }

            if (base.isAuthority && this.aiming && !this.inputBank.skill2.down)
            {
                this.aiming = false;
                if (this.camParamsOverrideHandle.isValid) this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
                this.GetModelAnimator().SetFloat("aimBlend", 0f);
                if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }

            if (!this.aiming && this.wasAiming && base.fixedAge >= (0.8f * this.duration) && !this.heheheha)
            {
                this.heheheha = true;
                base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.25f);
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.iDrive.FinishReload();

                if (this.aiming)
                {
                    this.outer.SetNextState(new SteadyAim
                    {
                        skipAnim = true,
                        camParamsOverrideHandle = this.camParamsOverrideHandle
                    });
                }
                else this.outer.SetNextStateToMain();

                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return this.interruptPriority;
        }
    }
}