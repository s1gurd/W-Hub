using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [Serializable]
    public class MovementDynamics
    {
        [InfoBox("Only 2 keyframes in curves are supported!")]
        public AnimationCurve curve = new AnimationCurve();

        public float timeScale = 1f;
    }

    [Serializable]
    public struct MovementDynamicsInner
    {
        public bool useDynamics;
        public float timeScaleIn, timeScaleOut;
        public Curve curveIn, curveOut;
    }

    [Serializable]
    public struct Curve
    {
        public Keyframe keyframe0, keyframe1;
    }

    [Serializable]
    public class MovementAnimationProperies
    {
        [ToggleLeft] public bool HasMovementAnimation = false;

        [ShowIf("@HasMovementAnimation == true")]
        public string MovementAnimationName;

        [ShowIf("@HasMovementAnimation == true")]
        public string MovementAnimationSpeedFactorName;

        [ShowIf("@HasMovementAnimation == true")]
        public float MovementAnimationSpeedFactorMultiplier = 1f;
    }
}