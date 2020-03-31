using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [Serializable]
    public class CollisionAction
    {
        [InfoBox("Objects without layer set will always collide")]
        public LayerMask collisionLayerMask = ~0;
        
        [InfoBox("Filter by tags works in addition to Collision Layer Mask")]
        public bool useTagFilter = false;

        [ShowIf("useTagFilter")] [EnumToggleButtons] 
        public TagFilterMode filterMode = TagFilterMode.IncludeOnly;

        [ShowIf("useTagFilter")][ValueDropdown("Tags")]
        public List<string> filterTags;
        
        [ValidateInput("MustBeAbility", "Ability MonoBehaviours must derive from IActorAbility!")]
        [SerializeField]
        public List<MonoBehaviour> actions;

        public bool executeOnCollisionWithSpawner = false;
        
        public bool destroyAfterAction = false;

        private bool MustBeAbility(List<MonoBehaviour> a)
        {
            return !a.Exists(t => !(t is IActorAbility)) || a.Count == 0;
        }
        
        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }        
        
    }

    public enum TagFilterMode
    {
        IncludeOnly = 0,
        Exclude = 1
    }
}