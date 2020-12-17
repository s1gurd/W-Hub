using System;
using DG.Tweening;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Example.Common
{
    public class UIGameStatePanel : TimerBaseBehaviour, IActorAbility
    {
        public Button actionButton;
        public Button secondActionButton;
        public CanvasGroup canvasGroup;
        
        public float fadeInTime = 2f;
        public IActor Actor { get; set; }

        private Entity _entity;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            canvasGroup.alpha = 0f;
            _entity = entity;
        }

        private void OnEnable()
        {
            if (Actor == null) return;


            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, fadeInTime);
        }

        private void OnDisable()
        {
            canvasGroup.alpha = 0f;
        }

        public void Execute()
        {
        }
    }
}