using System.Collections;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilitySetTag : MonoBehaviour, IActorAbility
    {
        public IActor Actor { get; set; }

        [ValueDropdown("Tags")] public string newTag;

        public bool applyOnStart = true;
        
        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }
        private void Start()
        {
            if (applyOnStart) Execute();
        }
        
        public void AddComponentData(ref Entity entity, IActor actor)
        {
        }
        
        public void Execute()
        {
            if (newTag != string.Empty)
            {
                gameObject.tag = newTag;
            }
        }
    }
}