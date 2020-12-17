using Cherry.Core;

namespace GameFramework.Example.Components
{
    public partial class CheatsPanel
    {
        private readonly Resolver _resolver = new Resolver();
        
        private void CreateStickSettingsMenuButton()
        {
            backButton.gameObject.SetActive(true);
            SetBackButtonAction(CreateCheatButton);

            var button = Instantiate(cheatButtonTemplate, content.transform);
            var btn = button.GetComponent<CheatButton>();
            btn.ButtonAction = () =>
            {
                RemoveOldButtons();
                CreateChooseStickTypeButtons();
            };

            btn.ButtonName = "Stick Settings";
        }
        
        private void CreateChooseStickTypeButtons()
        {
            SetBackButtonAction(CreateMenuButtons);

            if (_resolver.GameSettings.AdaptiveStickEnabled)
            {
                var button = Instantiate(cheatButtonTemplate, content.transform);
                var btn = button.GetComponent<CheatButton>();
                btn.ButtonAction = () =>
                {
                    _resolver.GameSettings.AdaptiveStickEnabled = false;
                    RemoveOldButtons();
                    CreateChooseStickTypeButtons();
                };
                btn.ButtonName = "Disable Adaptive Stick";
            }
            else
            {
                var button = Instantiate(cheatButtonTemplate, content.transform);
                var btn = button.GetComponent<CheatButton>();
                btn.ButtonAction = () =>
                {
                    _resolver.GameSettings.AdaptiveStickEnabled = true;
                    RemoveOldButtons();
                    CreateChooseStickTypeButtons();
                };
                btn.ButtonName = "Enable Adaptive Stick"; 
            }
        }
    }
}