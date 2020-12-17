using System;
using GameFramework.Example.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [Serializable]
    public struct AimingProperties
    {
        [Space] [EnumToggleButtons] public EvaluateActionOptions evaluateActionOptions;
        [Space][EnumToggleButtons] public AimingType aimingType;
        
        public GameObject aimingAreaPrefab;
        public GameObject sightPrefab;
        public GameObject circlePrefab;

        [ShowIf("@aimingType == AimingType.SightControl")]
        public float sightDistance;
    }
}