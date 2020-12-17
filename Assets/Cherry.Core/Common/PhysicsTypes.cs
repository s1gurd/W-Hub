using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [Serializable]
    public struct CollisionAction
    {
        [InfoBox("Objects without layer set will always collide")]
        public LayerMask collisionLayerMask;
        
        [InfoBox("Filter by tags works in addition to Collision Layer Mask")]
        public bool useTagFilter;

        [ShowIf("useTagFilter")] [EnumToggleButtons] 
        public TagFilterMode filterMode;

        [ShowIf("useTagFilter")][ValueDropdown("Tags")]
        public List<string> filterTags;
        
        public bool executeOnCollisionWithSpawner;
        
        public bool destroyAfterAction;
        
        [ValidateInput("MustBeAbility", "Ability MonoBehaviours must derive from IActorAbility!")]
        [SerializeField]
        public List<MonoBehaviour> actions;
  
        private bool MustBeAbility(List<MonoBehaviour> a)
        {
            return !a.Exists(t => !(t is IActorAbility)) || a.Count == 0;
        }
        
        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }
    }
    
    [Serializable]
    public struct CollisionSettings
    {
        [InfoBox("Objects without layer set will always collide")]
        public LayerMask collisionLayerMask;
        
        [InfoBox("Filter by tags works in addition to Collision Layer Mask")]
        public bool useTagFilter;

        [ShowIf("useTagFilter")] [EnumToggleButtons] 
        public TagFilterMode filterMode;

        [ShowIf("useTagFilter")][ValueDropdown("Tags")]
        public List<string> filterTags;
        
        public bool executeOnCollisionWithSpawner;
        
        public bool destroyAfterAction;
        
        
        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }        
        
    }
}