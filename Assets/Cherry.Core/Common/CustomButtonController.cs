using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace GameFramework.Example.Common
{
    [HideMonoScript]
    public class CustomButtonController : TimerBaseBehaviour
    {
        public int bindingIndex;
        //public TextMeshProUGUI buttonName;
        public Image buttonImage;
        public Image cooldownProgressBar;
        public bool reverseProgressBarDirection = false;
        public GameObject cooldownTimerRoot;
        public TextMeshProUGUI cooldownTimerText;
        
        public OnScreenStick onScreenStickComponent;
        public OnScreenCustomButton onScreenButtonComponent;

        public void SetupCustomButton(string perkName, Sprite perkSprite, bool stickControlAvailable, bool repeatedInvokingOnHold)
        {
            //buttonName.SetText(perkName);
            buttonImage.sprite = perkSprite;

            SetupCustomButton(stickControlAvailable, repeatedInvokingOnHold);
        }
        
        public void SetupCustomButton(bool stickControlAvailable, bool repeatedInvokingOnHold)
        {
            onScreenStickComponent.enabled = stickControlAvailable;
            onScreenButtonComponent.SetupButton(repeatedInvokingOnHold);
        }

        public void SetButtonOnCooldown(bool onCooldown)
        {
            onScreenStickComponent.enabled = !onCooldown;
        }
        
        public void SetCooldownProgressBar(float value)
        {
            cooldownProgressBar.fillAmount = value;
        }

        public void SetCooldownText(int value)
        {
            if (cooldownTimerRoot == null) return;
            cooldownTimerRoot.SetActive(true);
            if (cooldownTimerText != null) cooldownTimerText.text = value.ToString();
        }

        public void HideCoolDownText()
        {
            if (cooldownTimerRoot == null) return;
            cooldownTimerRoot.SetActive(false);
        }
    }
}