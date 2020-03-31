using GameFramework.Example.Common;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityLifespan : MonoBehaviour, IActorAbility, ITimer
    {
        public float lifespan = 3f;
        public bool startOnSpawn = true;
        
        public bool TimerActive { get; set; }
        
        public TimerComponent Timer =>
            _timer != null ? _timer : _timer = this.gameObject.AddComponent<TimerComponent>();
        
        private Entity _entity;
        private TimerComponent _timer;
        
        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = entity;
            dstManager.AddComponent<TimerData>(entity);
            
            if (startOnSpawn) Execute();
        }

        public void Execute()
        {
            Timer.TimedActions.AddAction(() => {this.gameObject.DestroyWithEntity(_entity);},lifespan);
        }

        public void FinishTimer()
        {
            
        }

        public void StartTimer()
        {
            
        }
    }
}