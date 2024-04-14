﻿using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;

namespace RobDriver.Modules.Components
{
    public class WeaponIcon : MonoBehaviour
    {
		public HUD targetHUD;
		public DriverController iDrive;

		public GameObject displayRoot;
		public PlayerCharacterMasterController playerCharacterMasterController;
		public RawImage iconImage;

		public GameObject flashPanelObject;
		public GameObject reminderFlashPanelObject;
		public GameObject isReadyPanelObject;
		public TooltipProvider tooltipProvider;
		public GameObject durationDisplay;
		public Image durationBar;
		public Image durationBarRed;

		private void Update()
        {
			// REWRITE THIS ASAP
			if (!this.iDrive)
            {
				if (!this.playerCharacterMasterController)
                {
					this.playerCharacterMasterController = (this.targetHUD.targetMaster ? this.targetHUD.targetMaster.GetComponent<PlayerCharacterMasterController>() : null);
				}

				if (this.playerCharacterMasterController && this.playerCharacterMasterController.master.hasBody)
                {
					DriverController fuckYou = this.playerCharacterMasterController.master.GetBody().GetComponent<DriverController>();
					if (fuckYou) this.SetTarget(fuckYou);
                }
            }
			else
            {
				this.UpdateDisplay();
            }
        }

		public void SetTarget(DriverController driver)
        {
			this.iDrive = driver;
			this.iDrive.onWeaponUpdate += this.SetDisplay;
			this.SetDisplay(this.iDrive);
        }

		private void UpdateDisplay()
        {
			if (this.iDrive.passive.isPistolOnly)
            {
				this.durationDisplay.SetActive(false);
				return;
            }

			if (this.iDrive.maxWeaponTimer > 0f)
            {
				this.durationDisplay.SetActive(true);

				float fill = Util.Remap(this.iDrive.weaponTimer, 0f, this.iDrive.maxWeaponTimer, 0f, 1f);

				if (this.durationBarRed)
				{
					if (fill >= 1f) this.durationBarRed.fillAmount = 1f;
					this.durationBarRed.fillAmount = Mathf.Lerp(this.durationBarRed.fillAmount, fill, Time.deltaTime * 2f);
				}

				this.durationBar.fillAmount = fill;
            }
			else
            {
				this.durationDisplay.SetActive(false);
            }
        }

		private void SetDisplay(DriverController z)
		{
			if (!this.iDrive) return;

			this.DoStockFlash();

            if (this.displayRoot) this.displayRoot.SetActive(true); 
            if (this.isReadyPanelObject) this.isReadyPanelObject.SetActive(true);

            if (this.iconImage)
			{
				this.iconImage.texture = this.iDrive.weaponDef.icon;
				this.iconImage.color = Color.white;
				this.iconImage.enabled = true;
			}
            if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = this.iDrive.weaponDef.nameToken;
				this.tooltipProvider.bodyToken = this.iDrive.weaponDef.descriptionToken;
				this.tooltipProvider.titleColor = Modules.Survivors.Driver.characterColor;
				this.tooltipProvider.bodyColor = Color.gray;
			}
        }

        private void DoReminderFlash()
		{
			if (this.reminderFlashPanelObject)
			{
				AnimateUIAlpha animateUI = this.reminderFlashPanelObject.GetComponent<AnimateUIAlpha>();
				if (animateUI) animateUI.time = 0f;
				this.reminderFlashPanelObject.SetActive(true);
			}
		}

		private void DoStockFlash()
		{
			this.DoReminderFlash();
			if (this.flashPanelObject)
			{
				AnimateUIAlpha animateUI = this.flashPanelObject.GetComponent<AnimateUIAlpha>();
				if (animateUI) animateUI.time = 0f;
				this.flashPanelObject.SetActive(true);
			}
		}
	}
}