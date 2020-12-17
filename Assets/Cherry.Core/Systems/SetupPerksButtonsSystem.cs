using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Utils;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    public class SetupPerksButtonsSystem : ComponentSystem
    {
        private EntityQuery _playerQuery;
        private EntityQuery _uiAvailablePerksPanelQuery;
        private EntityQuery _applyPresetPerksQuery;
        private Actor _panelActor = null;
        private UIPerkListAbility _list = null;

        protected override void OnCreate()
        {
            _playerQuery = GetEntityQuery(
                ComponentType.ReadOnly<AbilityActorPlayer>(),
                ComponentType.ReadWrite<PerksSelectionAvailableData>(),
                ComponentType.ReadOnly<UserInputData>());

            _uiAvailablePerksPanelQuery = GetEntityQuery(
                ComponentType.ReadOnly<Actor>(),
                ComponentType.ReadOnly<UIPerkListAbility>(),
                ComponentType.ReadWrite<UIAvailablePerksPanelData>());
            
            _applyPresetPerksQuery = GetEntityQuery(
                ComponentType.ReadOnly<AbilityActorPlayer>(),
                ComponentType.ReadWrite<ApplyPresetPerksData>());
        }

        protected override void OnUpdate()
        {
                    Entities.With(_playerQuery).ForEach(
                        (Entity playerEntity, AbilityActorPlayer actorPlayer) =>
                        {
                            Entities.With(_uiAvailablePerksPanelQuery).ForEach(
                                (Entity panelEntity, Actor actor, UIPerkListAbility listAbility) =>
                                {
                                    _panelActor = actor;
                                    _list = listAbility;
                                });
                            
                            if ((_panelActor == null) || (_list == null) || (_panelActor.Spawner != actorPlayer.Actor)) return;
                            
                            _list.Execute();
                            PostUpdateCommands.RemoveComponent<PerksSelectionAvailableData>(playerEntity);
                        });
                
            
            Entities.With(_applyPresetPerksQuery).ForEach(
                (Entity playerEntity, AbilityActorPlayer actorPlayer, ref ApplyPresetPerksData presetPerksData) =>
                {
                    if (!actorPlayer.UIReceiverList.Any()) return;
                    
                    var perksPresets = GameMeta.PresetPerksList.Select(p => p.GetComponent<IPerkBase>()).ToList();
                    perksPresets.ForEach(p => p.SpawnPerk(actorPlayer.Actor));
                    
                    PostUpdateCommands.RemoveComponent<ApplyPresetPerksData>(playerEntity);
                });
        }
    }
}