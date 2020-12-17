using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilitySetupReceivedDamageValue : MonoBehaviour, IActorAbility
    {
        [SerializeField] private TextMeshProUGUI damageField;

        [TitleGroup("Color Settings")]
        [SerializeField] private Color enemyTextColor;
        [SerializeField] private Color playerTextColor;
        public IActor Actor { get; set; }
        
        private EntityManager _dstManager;
        
        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            
            _dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void SetDamageValue(string dmg)
        {
            damageField.SetText(dmg);

            damageField.color = _dstManager.HasComponent<UserInputData>(Actor.Spawner.ActorEntity)
                ? playerTextColor
                : enemyTextColor;
        }

        public void Execute()
        {
        }
    }
}