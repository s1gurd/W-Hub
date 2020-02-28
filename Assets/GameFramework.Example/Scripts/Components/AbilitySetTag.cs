using System.Collections;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilitySetTag : MonoBehaviour, IActorAbility
    {
        [ValueDropdown("Tags")] public string newTag;

#if UNITY_EDITOR
        private static IEnumerable Tags()
        {
            return UnityEditorInternal.InternalEditorUtility.tags;
        }
#endif
        private void Awake()
        {
            if (newTag != string.Empty)
            {
                gameObject.tag = newTag;
            }
        }
        
        public void AddComponentData(ref Entity entity)
        {
        }

        public void Execute()
        {
        }
    }
}