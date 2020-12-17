using Cherry.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class UISettingsScreenView : MonoBehaviour
    {
        [SerializeField] private Button settingsButton;

        [SerializeField] private GameObject settingsScreen;
        [SerializeField] private Button backButton;
        [SerializeField] private Toggle adaptiveStickToggle;

        private readonly Resolver _resolver = new Resolver();

        public void Awake()
        {
            settingsScreen.SetActive(false);

            settingsButton.onClick.AddListener(() => SetSettingsScreenVisibility(!settingsScreen.activeSelf));
            backButton.onClick.AddListener(() =>
            {
                if (settingsScreen.activeSelf) SetSettingsScreenVisibility(false);
            });

            adaptiveStickToggle.onValueChanged.AddListener(isOn => _resolver.GameSettings.AdaptiveStickEnabled = isOn);
        }

        private void SetSettingsScreenVisibility(bool isVisible)
        {
            settingsScreen.SetActive(isVisible);

            if (isVisible)
            {
                adaptiveStickToggle.isOn = _resolver.GameSettings.AdaptiveStickEnabled;
            }
        }
    }
}