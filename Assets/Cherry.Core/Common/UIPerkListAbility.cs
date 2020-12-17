using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Example.Common
{
    public class UIPerkListAbility : MonoBehaviour, IActorAbility
    {
        public int NumberOfPerksToShow = 3;

        public GameObject PerkButtonPrefab;

        public Transform RootUIObject;
        public IActor Actor { get; set; }

        private List<GameObject> AvailablePerks => GameMeta.AvailablePerksList;

        [HideInInspector] public List<GameObject> spawnedButtons = new List<GameObject>();

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<UIAvailablePerksPanelData>(
                Actor.ActorEntity);
        }

        public void Execute()
        {
            var availablePerks = AvailablePerks
                .Where(a =>
                {
                    var perkUpgrade = a.GetComponent<IPerkUpgrade>();
                    var perk = perkUpgrade?.PerkPrefab.GetComponent<IPerkAbility>();
                    
                    var existingPerks = Actor.Spawner.AppliedPerks.Where(p => p.GetType() == perk?.GetType()).ToList();
                    var abilityPlayer =
                        Actor.Spawner.Abilities.FirstOrDefault(ability => ability is AbilityActorPlayer) as
                            AbilityActorPlayer;

                    if (abilityPlayer == null) return false;

                    return perkUpgrade?.AvailabilityLevel <= abilityPlayer.Level &&
                           perkUpgrade?.AvailableLevelsNumber > existingPerks.Count;
                })
                
                .Select(c => c.GetComponent<IPerkUpgrade>())
                .ToList();

            if (!availablePerks.Any()) return;

            var perksToShow = new List<IPerkUpgrade>();

            if (availablePerks.Count <= NumberOfPerksToShow)
            {
                perksToShow = availablePerks;
            }
            else
            {
                while (perksToShow.Count != NumberOfPerksToShow)
                {
                    var selectedPerk = GetRandomWeightedPerk(availablePerks);
                    if (selectedPerk != null && !perksToShow.Contains(selectedPerk)) perksToShow.Add(selectedPerk);
                }
            }

            foreach (var perk in perksToShow)
            {
                var b = Instantiate(PerkButtonPrefab, RootUIObject ? RootUIObject : transform);

                if (perk == null)
                {
                    Debug.LogError(
                        "[PERK UPGRADE SETUP] Perk Upgrade Prefab must have Perk Upgrade Component derived from IPerkUpgrade!");
                    continue;
                }

                var perkUpgradeButton = b.GetComponent<PerkUpgradeButton>();
                if (perkUpgradeButton == null)
                {
                    Debug.LogError("[PERK UPGRADE SETUP] Perk Button Prefab must have Perk Upgrade Button Component!");
                    continue;
                }

                if (perk.PerkImage != null) perkUpgradeButton.SetImage(perk.PerkImage);
                perkUpgradeButton.SetText(perk.PerkName);

                var button = b.GetComponent<Button>();
                if (button == null)
                {
                    Debug.LogError("[PERK UPGRADE SETUP] Perk Button Prefab must have Button Component!");
                    continue;
                }

                button.onClick.AddListener(() =>
                {
                    perk.SpawnPerk(Actor.Spawner);
                    CleanUp(NumberOfPerksToShow);
                });

                spawnedButtons.Add(b);
            }
        }

        public void CleanUp(int amount)
        {
            var amountToDestroy = amount <= spawnedButtons.Count ? amount : spawnedButtons.Count;

            var buttonsToDestroy = spawnedButtons.Take(amountToDestroy);

            foreach (var button in buttonsToDestroy)
            {
                Destroy(button);
            }

            spawnedButtons.RemoveRange(0, amountToDestroy);
        }

        private IPerkUpgrade GetRandomWeightedPerk(List<IPerkUpgrade> perksList)
        {
            if (perksList == null || !perksList.Any()) return null;

            var randomValue = Random.Range(0, 1.0f);

            var weightSum = perksList.Sum(perk => perk.PerkWeight);

            foreach (var perk in perksList)
            {
                if (perk.PerkWeight <= 0) return null;
                var perkDropChance = perk.PerkWeight / weightSum;

                if (randomValue < perkDropChance) return perk;

                weightSum -= perk.PerkWeight;
            }

            return null;
        }
    }

    public struct UIAvailablePerksPanelData : IComponentData
    {
    }
}