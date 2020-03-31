using GameFramework.Example.Common;
using GameFramework.Example.Components;
using Unity.Entities;
using GameFramework.Example.Utils.LowLevel;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class TimersSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<TimerData>(),
                ComponentType.ReadOnly<TimerComponent>());
        }

        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;
            
            Entities.With(_query).ForEach(
                (Entity entity, TimerComponent timer) =>
                {
                    for (var i = timer.TimedActions.Count-1; i >= 0; i--)
                    {
                        var timerAction = timer.TimedActions[i];
                        timerAction.Delay -= dt;
                        timer.TimedActions[i] = timerAction;
                        
                        if (!(timerAction.Delay <= 0f)) continue;
                        timer.TimedActions[i].Act.Invoke();
                        timer.TimedActions.RemoveAt(i);
                    }
                });
        }
    }
}