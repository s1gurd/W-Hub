using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [HideMonoScript]
    public class BindablePerk : MonoBehaviour, IPerkBase
    {
        public string perkName;
        public Sprite perkImage;
        public GameObject perkPrefab;
        
        public string PerkName => perkName;

        public Sprite PerkImage => perkImage;

        public GameObject PerkPrefab => perkPrefab;
        
        public void SpawnPerk(IActor target)
        {
            var perk = perkPrefab.GetComponent<IPerkAbility>();

            var newComponents = (perk as IPerkAbilityBindable)?.CopyBindablePerk(target);
            
            if (newComponents == null) return;

            UpdatePerkButton(target, newComponents, out var updatedButtonReceiver);
            
            if (updatedButtonReceiver == null || !newComponents.Any()) return;

            foreach (var bindable in newComponents.OfType<IBindable>())
            {
                bindable.UpdateBindingIndex(updatedButtonReceiver.bindingIndex, target.ActorEntity);
            }
        }

        private void UpdatePerkButton(IActor target, List<Component> copiedComponents,
            out CustomButtonController updatedButtonController)
        {
            updatedButtonController = null;

            var targetPlayerAbility =
                target.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;

            var addActionToInputAbility = copiedComponents.OfType<AbilityAddActionsToPlayerInput>().FirstOrDefault();

            if (addActionToInputAbility == null || targetPlayerAbility == null) return;

            var buttonToUpdate = targetPlayerAbility.UIReceiverList
                .SelectMany(u => ((UIReceiver) u).customButtons)
                .FirstOrDefault(b => b.bindingIndex == addActionToInputAbility.customBinding.index);

            var stickControlAvailable = copiedComponents.OfType<IAimable>().FirstOrDefault()?.AimingAvailable;
            var repeatedInvokingOnHold = copiedComponents.OfType<AbilityWeapon>().FirstOrDefault()?.aimingProperties
                .evaluateActionOptions;

            if (buttonToUpdate != null)
            {
                buttonToUpdate.SetupCustomButton(perkName, perkImage, stickControlAvailable ?? false,
                    repeatedInvokingOnHold != null &&
                    repeatedInvokingOnHold == EvaluateActionOptions.RepeatingEvaluation);
            }

            updatedButtonController = buttonToUpdate;
        }
    }

    public struct ApplyPresetPerksData : IComponentData
    {
    }
}