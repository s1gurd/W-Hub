using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class PerkChangeBulletCollisionMovement : MonoBehaviour, IActorAbility, IPerkAbilityForSpawned
    {
        public bool ApplyToAllProjectiles = true;

        [HideIf("ApplyToAllProjectiles")] public string componentName = "";

        public CollisionSettings collisionSettings;
        public CollisionMovementSettings collisionMovementSettings;

        public DuplicateHandlingProperties duplicateHandlingProperties;
        
        public IActor Actor { get; set; }

        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
        }

        public void Execute()
        {
        }

        public void AddCollision(GameObject target)
        {
            var collision = target.GetComponent<AbilityCollision>();

            if (collision == null) return;

            if (!collisionSettings.useTagFilter || !collision.collisionActions.Any())
            {
                collision.collisionActions.Add(AddNewCollisionAction());
                return;
            }

            var modifiedCollisionActionList = new List<CollisionAction>();
            var perkApplied = false;


            foreach (var action in collision.collisionActions)
            {
                var currentAction = action;

                var abilityCollisionMovement =
                    (AbilityCollisionMovement) currentAction.actions.FirstOrDefault(act =>
                        act is AbilityCollisionMovement);

                if (!currentAction.useTagFilter)
                {
                    if (abilityCollisionMovement != null)
                    {
                        currentAction.useTagFilter = true;
                        currentAction.filterMode = collisionSettings.filterMode == TagFilterMode.IncludeOnly
                            ? TagFilterMode.Exclude
                            : TagFilterMode.IncludeOnly;
                        currentAction.filterTags.Clear();
                        currentAction.filterTags.AddRange(collisionSettings.filterTags);
                    }

                    modifiedCollisionActionList.Add(currentAction);
                    continue;
                }

                if (abilityCollisionMovement == null)
                {
                    if (collisionSettings.filterTags.ContainsItems(currentAction.filterTags))
                    {
                        currentAction.destroyAfterAction = collisionSettings.destroyAfterAction;
                    }

                    modifiedCollisionActionList.Add(currentAction);
                    continue;
                }

                if (currentAction.filterTags.SequenceEqual(collisionSettings.filterTags) &&
                    currentAction.actions.All(act => act is AbilityCollisionMovement))
                {
                    var updatedActions = currentAction.actions;

                    for (var i = 0; i < currentAction.actions.Count; ++i)
                    {
                        ((AbilityCollisionMovement) updatedActions[i]).collisionMovementSettings =
                            collisionMovementSettings;
                    }

                    currentAction.actions = updatedActions;

                    modifiedCollisionActionList.Add(currentAction);
                    perkApplied = true;

                    continue;
                }


                if (currentAction.filterTags.ContainsItems(collisionSettings.filterTags))
                {
                    currentAction.filterTags.RemoveAll(t => collisionSettings.filterTags.Contains(t));

                    if (currentAction.filterTags.Any())
                    {
                        modifiedCollisionActionList.Add(currentAction);
                        continue;
                    }
                }

                if (currentAction.filterMode != collisionSettings.filterMode) continue;

                switch (currentAction.filterMode)
                {
                    case TagFilterMode.IncludeOnly:
                        if (collisionSettings.filterMode == currentAction.filterMode)
                        {
                            if (currentAction.actions.Any(act => !(act is AbilityCollisionMovement)))
                            {
                                currentAction.filterMode = TagFilterMode.Exclude;
                                currentAction.filterTags.AddRange(collisionSettings.filterTags);
                                modifiedCollisionActionList.Add(currentAction);
                                continue;
                            }
                        }

                        break;
                    case TagFilterMode.Exclude:
                        if (collisionSettings.filterMode == currentAction.filterMode)
                        {
                            if (currentAction.actions.Any(act => !(act is AbilityCollisionMovement)))
                            {
                                currentAction.filterMode = TagFilterMode.IncludeOnly;
                                currentAction.filterTags.AddRange(collisionSettings.filterTags);
                                modifiedCollisionActionList.Add(currentAction);
                                continue;
                            }
                        }

                        break;
                }

                if (abilityCollisionMovement.collisionMovementSettings == collisionMovementSettings)
                {
                    currentAction.collisionLayerMask = collisionSettings.collisionLayerMask;
                    currentAction.executeOnCollisionWithSpawner = collisionSettings.executeOnCollisionWithSpawner;
                    currentAction.destroyAfterAction = collisionSettings.destroyAfterAction;

                    modifiedCollisionActionList.Add(currentAction);
                    perkApplied = true;
                }
            }

            if (!perkApplied)
            {
                modifiedCollisionActionList.Add(AddNewCollisionAction());
            }

            collision.collisionActions = modifiedCollisionActionList;

            CollisionAction AddNewCollisionAction()
            {
                var targetActor = target.GetComponent<Actor>();

                if (targetActor == null) return new CollisionAction();

                var newComp = target.AddComponent<AbilityCollisionMovement>();
                newComp.collisionMovementSettings = collisionMovementSettings;

                var targetActorActorEntity = targetActor.ActorEntity;

                newComp.AddComponentData(ref targetActorActorEntity, targetActor);

                return new CollisionAction
                {
                    collisionLayerMask = collisionSettings.collisionLayerMask,
                    useTagFilter = collisionSettings.useTagFilter,
                    filterMode = collisionSettings.filterMode,
                    filterTags = collisionSettings.filterTags,
                    actions = new List<MonoBehaviour> {newComp},
                    executeOnCollisionWithSpawner = collisionSettings.executeOnCollisionWithSpawner,
                    destroyAfterAction = collisionSettings.destroyAfterAction
                };
            }
        }

        public void Apply(IActor target)
        {
            this.CheckPerkDuplicates(target, out var continuePerkApply);
            
            if (!continuePerkApply) return;
            
            var copy = target.GameObject.CopyComponent(this) as PerkChangeBulletCollisionMovement;
            
            if (copy == null)
            {
                Debug.LogError("[PERK WEAPONS HIT EFFECT] Error copying perk to Actor!");
                return;
            }
            
            if (!Actor.Spawner.AppliedPerks.Contains(copy)) Actor.Spawner.AppliedPerks.Add(copy);

            var projectiles = target.GameObject.GetComponents<AbilityWeapon>().ToList();
            if (!ApplyToAllProjectiles)
                projectiles = projectiles.Where(p => p.ComponentName.Equals(componentName, StringComparison.Ordinal))
                    .ToList();
            foreach (var p in projectiles)
            {
                p.SpawnCallbacks.Add(copy.AddCollisionAction);
            }
        }

        public void AddCollisionAction(GameObject target)
        {
            var perk = target.CopyComponent(this) as PerkChangeBulletCollisionMovement;
            
            if (perk == null) return;

            perk.AddCollision(perk.gameObject);
        }
        
        public void Remove()
        {
            if (Actor.Spawner.AppliedPerks.Contains(this)) Actor.Spawner.AppliedPerks.Remove(this);
            Destroy(this);
        }
    }
}