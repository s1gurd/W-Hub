using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityActorApplyPerk : TimerBaseBehaviour, IActorAbilityTarget
    {
        [ValidateInput("MustBePerk", "Perk MonoBehaviours must derive from IPerkAbility!")]
        public MonoBehaviour perkToApply;

        [EnumToggleButtons] public DuplicateHandling perkDuplicateHandling;
        public IActor Actor { get; set; }
        public IActor TargetActor { get; set; }
        public IActor AbilityOwnerActor { get; set; }

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
        }

        public void Execute()
        {
            if (perkToApply == null || TargetActor == null) return;

            ApplyPerk();
        }

        private void ApplyPerk()
        {
            var existingPerk = TargetActor.AppliedPerks.Find(perk => perk.GetType() == perkToApply.GetType());
            
            if (existingPerk != null)
            {
                switch (perkDuplicateHandling)
                {
                    case DuplicateHandling.Abort:
                        return;
                    case DuplicateHandling.Replace:
                        if (existingPerk is TimerBaseBehaviour timerBaseBehaviour)
                        {
                            timerBaseBehaviour.ResetTimer();
                            var perkActor = (existingPerk as IActorAbility)?.Actor;
                            if (perkActor == null) return;
                            TryResetPerkGameObjectLifespan(perkActor.GameObject);
                            
                            return;
                        }
                        
                        existingPerk.Remove();
                        break;
                }
            }

            var spawnAbility = gameObject.AddComponent<AbilityActorSimpleSpawn>();

            spawnAbility.Actor = TargetActor;

            spawnAbility.objectToSpawn = perkToApply.gameObject;
            spawnAbility.ownerType = OwnerType.CurrentActorOwner;
            spawnAbility.spawnerType = SpawnerType.CurrentActor;
            spawnAbility.DestroyAfterSpawn = true;
            
            spawnAbility.Execute();
        }

        private void TryResetPerkGameObjectLifespan(GameObject perkGameObject)
        {
            var abilityLifespan = perkGameObject.GetComponent<AbilityLifespan>();
            
            if (abilityLifespan == null) return;

            abilityLifespan.Timer.TimedActions.Clear();
            abilityLifespan.Execute();
        }


        private bool MustBePerk(MonoBehaviour perk)
        {
            return perk == null || perk is IPerkAbility;
        }
    }

    public enum DuplicateHandling
    {
        Abort = 0,
        Replace = 1
    }
}