﻿using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    public class AmmoDisplay : MonoBehaviour
    {
        public HUD targetHUD;
        public LanguageTextMeshController targetText;

        private DriverController iDrive;
        private DriverPassive passive;

        private void FixedUpdate()
        {
            if (this.targetHUD)
            {
                if (this.targetHUD.targetBodyObject)
                {
                    if (!this.iDrive) this.iDrive = this.targetHUD.targetBodyObject.GetComponent<DriverController>();
                    if (!this.passive) this.passive = this.targetHUD.targetBodyObject.GetComponent<DriverPassive>();
                }
            }

            if (this.targetText)
            {
                if (this.iDrive && this.passive)
                {
                    if (this.iDrive.maxWeaponTimer <= 0f)
                    {
                        this.targetText.token = "";
                        return;
                    }

                    if (this.iDrive.weaponTimer <= 0f)
                    {
                        this.targetText.token = "<color=#C80000>0 / " + Mathf.CeilToInt(this.iDrive.maxWeaponTimer).ToString() + Helpers.colorSuffix;
                    }
                    else
                    {
                        if (this.passive.isBullets || this.passive.isRyan)
                        {
                            if (this.iDrive.HasSpecialBullets)
                            {
                                this.targetText.token = $"<color=#{ColorUtility.ToHtmlStringRGBA(this.iDrive.currentBulletDef.trailColor)}>" +
                                    Mathf.CeilToInt(this.iDrive.weaponTimer).ToString() + " / " + Mathf.CeilToInt(this.iDrive.maxWeaponTimer).ToString() +
                                    " - " + this.iDrive.currentBulletDef.nameToken + Helpers.colorSuffix;
                            }
                            else this.targetText.token = "";
                        }
                        else this.targetText.token = Mathf.CeilToInt(this.iDrive.weaponTimer).ToString() + " / " + Mathf.CeilToInt(this.iDrive.maxWeaponTimer).ToString();
                    }
                }
                else
                {
                    this.targetText.token = "";
                }
            }
        }
    }
}