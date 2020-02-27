using System;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [Serializable]
    public struct CustomBinding
    {
        [SerializeField]
        public int index;
        [ValidateInput("MustBeAbility", "Ability MonoBehaviours must derive from IActorAbility!")]
        [SerializeField]
        public MonoBehaviour action;
        
        private bool MustBeAbility(MonoBehaviour action)
        {
            return action is IActorAbility || action is null;
        }
    }
}