using System;
using System.Collections.Generic;
using GameFramework.Example.AI;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using UnityEngine;

namespace GameFramework.Example.Utils
{
    public static class AIUtils
    {
        public static bool FilterTag(this Transform t, AIBehaviourSetting behaviourSetting)
        {
            if (!behaviourSetting.BehaviourInstance.NeedTarget)
            {
                Debug.LogError("[FILTER BY TAG UTILITY] FilterTag called for Behaviour that doesn't need target!");
                return true;
            }

            var contains = behaviourSetting.targetFilterTags.Contains(t.tag);
            
            switch (behaviourSetting.targetFilterMode)
            {
                case TagFilterMode.IncludeOnly:
                    return contains;
                case TagFilterMode.Exclude:
                    return !contains;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public static bool ActionPossible(this List<IActorAbility> abilities)
        {
            var possible = false;
            
            foreach (var a in abilities)
            {
                if (a is IEnableable enableable)
                {
                    possible |= enableable.Enabled;
                }
                else
                {
                    possible = true;
                }

                return possible;
            }

            return false;
        }
    }
}