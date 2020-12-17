using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils;
using Unity.Entities;
using GameFramework.Example.Utils.LowLevel;

namespace GameFramework.Example.Systems
{
    public class ExecuteInUpdateSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<ExecuteInUpdate>());
        }

        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;

            Entities.With(_query).ForEach(
                (Entity entity, ExecuteInUpdate exec) =>
                {
                    if (!exec.Enabled) return;
                    foreach (var a in exec.Abilities)
                    {
                        a.Execute();
                    }
                });
        }
    }
}