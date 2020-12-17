using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using UnityEngine;

namespace GameFramework.Example.Components
{
    public partial class CheatsPanel
    {
        public static List<GameObject> PerksList
        {
            get
            {
                if (_perksList.Count != 0) return _perksList;
                
                var availablePerksObject = FindObjectOfType(typeof(AvailablePerks)) as AvailablePerks;
                if (availablePerksObject != null)
                    _perksList = availablePerksObject.CheatPerksList;

                return _perksList;
            }
        }

        private static List<GameObject> _perksList = new List<GameObject>();
        
        public static IActor PlayerToUi
        {
            get
            {
                if (!ReferenceEquals(_playerToUi, null) && _playerToUi.GameObject != null) return _playerToUi;
                
                var playersPrefabs = FindObjectsOfType<AbilityPlayerInput>().ToList();
                _playerToUi = playersPrefabs.FirstOrDefault(p => p != null && p.inputSource == InputSource.UserInput)
                    ?.Actor;
                
                return _playerToUi;
            }
        }

        private static IActor _playerToUi;

        private void CreatePerksMenuButtons()
        {
            backButton.gameObject.SetActive(true);
            SetBackButtonAction(CreateMenuButtons);

            var button = Instantiate(cheatButtonTemplate, content.transform);
            var btn = button.GetComponent<CheatButton>();
            btn.ButtonAction = () =>
            {
                RemoveOldButtons();
                CreateChooseBindablePerksButton();
            };

            btn.ButtonName = "Bindable Perks";
            
            button = Instantiate(cheatButtonTemplate, content.transform);
            btn = button.GetComponent<CheatButton>();
            btn.ButtonAction = () =>
            {
                RemoveOldButtons();
                ShowAndApplyBasePerk();
            };

            btn.ButtonName = "Base Perks";
        }

        private void CreateChooseBindablePerksButton()
        {
            SetBackButtonAction(CreatePerksMenuButtons);

            var button = Instantiate(cheatButtonTemplate, content.transform);
            var btn = button.GetComponent<CheatButton>();
            btn.ButtonAction = () =>
            {
                RemoveOldButtons();
                ShowAvailableBindablePerks(2);
            };
            btn.ButtonName = "Choose first perk";

            button = Instantiate(cheatButtonTemplate, content.transform);
            btn = button.GetComponent<CheatButton>();
            btn.ButtonAction = () =>
            {
                RemoveOldButtons();
                ShowAvailableBindablePerks(3);
            };
            btn.ButtonName = "Choose second perk";

            button = Instantiate(cheatButtonTemplate, content.transform);
            btn = button.GetComponent<CheatButton>();
            btn.ButtonAction = () =>
            {
                RemoveOldButtons();
                ShowAvailableBindablePerks(4);
            };
            btn.ButtonName = "Choose third perk";
        }

         private void ShowAvailableBindablePerks(int perkIndexToChange)
         {
             SetBackButtonAction(CreateChooseBindablePerksButton);

             var availablePerks = PerksList
                 .Where(p =>
                 {
                     var bindablePerk = p.GetComponent<BindablePerk>();

                     if (bindablePerk == null) return false;
                     var index = bindablePerk.perkPrefab.GetComponent<AbilityAddActionsToPlayerInput>()?.customBinding.index;
                     
                     return index != null && index == perkIndexToChange;
                 }).ToList();

             foreach (var go in availablePerks)
             {
                 var perk = go.GetComponent<BindablePerk>();
        
                 var button = Instantiate(cheatButtonTemplate, content.transform);
                 var btn = button.GetComponent<CheatButton>();
                 btn.ButtonAction = () => ChangeBindablePerk(perkIndexToChange, perk);
                 btn.ButtonName = perk.PerkName;
             }
         }
         
         private void ShowAndApplyBasePerk()
         {
             var availablePerks = PerksList
                 .Where(p =>
                 {
                     var perk = p.GetComponent<PerkUpgradeBase>();

                     return perk != null && !perk.GetType().IsSubclassOf(typeof(PerkUpgradeBase));
                     
                 }).ToList();

             foreach (var go in availablePerks)
             {
                 var perk = go.GetComponent<PerkUpgradeBase>();
        
                 var button = Instantiate(cheatButtonTemplate, content.transform);
                 var btn = button.GetComponent<CheatButton>();
                 btn.ButtonAction = () =>
                 {
                     if (ReferenceEquals(PlayerToUi, null)) return;
                     perk.SpawnPerk(_playerToUi);
                 };
                 btn.ButtonName = perk.PerkName;
             }
         }
        
         private void ChangeBindablePerk(int bindingIndex, BindablePerk perk)
         {

             if (ReferenceEquals(PlayerToUi, null)) return;
        
             var perkToApply = perk.perkPrefab.GetComponent<IPerkAbilityBindable>();
             var newComponents = perkToApply?.CopyBindablePerk(PlayerToUi, bindingIndex);
        
             if (newComponents == null) return;
        
             var targetPlayerAbility = PlayerToUi.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;
        
             if (targetPlayerAbility == null) return;
        
             var buttonToUpdate = targetPlayerAbility.UIReceiverList
                 .SelectMany(u => ((UIReceiver) u).customButtons)
                 .FirstOrDefault(b => b.bindingIndex == bindingIndex);
             
             var stickControlAvailable = newComponents.OfType<IAimable>().FirstOrDefault()?.AimingAvailable;
             var repeatedInvokingOnHold = newComponents.OfType<AbilityWeapon>().FirstOrDefault()?.aimingProperties.evaluateActionOptions;
        
             if (buttonToUpdate != null)
             {
                 buttonToUpdate.SetupCustomButton(perk.perkName, perk.perkImage, stickControlAvailable ?? false,
                     repeatedInvokingOnHold != null && repeatedInvokingOnHold == EvaluateActionOptions.RepeatingEvaluation);
             }

             if (ReferenceEquals(buttonToUpdate, null) || !newComponents.Any()) return;
        
             foreach (var bindable in newComponents.OfType<IBindable>())
             {
                 bindable.UpdateBindingIndex(buttonToUpdate.bindingIndex, PlayerToUi.ActorEntity);
             }
             
             if (stickControlAvailable == null || (bool) !stickControlAvailable || repeatedInvokingOnHold == null) return;

             var weaponAbilities = newComponents.OfType<AbilityWeapon>().ToList();
        
             foreach (var ability in weaponAbilities)
             {
                 ability.aimingProperties.evaluateActionOptions = (EvaluateActionOptions) repeatedInvokingOnHold;
             }
        }
    }
}