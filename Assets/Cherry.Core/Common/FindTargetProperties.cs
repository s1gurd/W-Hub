using System;
using System.Collections;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [Serializable]
    public class FindTargetProperties
    {
        [EnumToggleButtons] public TargetType targetType;

        [ShowIf("targetType", TargetType.ComponentName)]
        public string actorWithComponentName;

        [ShowIf("targetType", TargetType.ChooseByTag)] [ValueDropdown("Tags")]
        public string targetTag;

        [ShowIf("targetType", TargetType.ChooseByTag)]
        public bool ignoreSpawner;

        [HideIf("targetType", TargetType.Spawner)] [EnumToggleButtons]
        public ChooseTargetStrategy strategy;

        [ShowIf("strategy", ChooseTargetStrategy.Nearest)] [InfoBox("Value of 0 means unrestricted distance")]
        public float maxDistanceThreshold = 0f;
        
        public bool SearchCompleted { get; set; }

        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }
    }
}