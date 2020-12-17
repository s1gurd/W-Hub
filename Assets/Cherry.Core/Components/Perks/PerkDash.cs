using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cherry.Core.Components.Interfaces;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Loading;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class PerkDash : CooldownBehaviour, IActorAbility, IPerkAbility, IPerkAbilityBindable, ILevelable,
        ICooldownable, IAimable
    {
        [Sirenix.OdinInspector.ReadOnly] public int perkLevel = 1;

        [TitleGroup("Perk Settings")]
        [LevelableValue] public float force = 25;
        public float timer = 0.1f;
        public float cooldownTime;
        
        [TitleGroup("Aiming Settings")]
        public bool aimingAvailable;
        public bool deactivateAimingOnCooldown;
        
        public AimingProperties aimingProperties;
        public AimingAnimationProperties aimingAnimProperties;

        [Title("VFX Settings")]
        public List<GameObject> dashFX = new List<GameObject>();

        [Title("Perk Related Components")]
        public List<MonoBehaviour> perkRelatedComponents = new List<MonoBehaviour>();

        [Space] [TitleGroup("Levelable properties")] [OnValueChanged("SetLevelableProperty")]
        public List<LevelableProperties> levelablePropertiesList = new List<LevelableProperties>();
        
        public DuplicateHandlingProperties duplicateHandlingProperties;
        
        public IActor Actor { get; set; }
        public bool ActionExecutionAllowed { get; set; }
        public GameObject SpawnedAimingPrefab { get; set; }
        
        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        private Vector3 _dashVector = new Vector3();

        public List<MonoBehaviour> PerkRelatedComponents
        {
            get
            {
                perkRelatedComponents.RemoveAll(c => ReferenceEquals(c, null));
                return perkRelatedComponents;
            }
            set => perkRelatedComponents = value;
        }

        public int Level
        {
            get => perkLevel;
            set => perkLevel = value;
        }

        public List<LevelableProperties> LevelablePropertiesList
        {
            get => levelablePropertiesList;
            set => levelablePropertiesList = value;
        }

        public List<FieldInfo> LevelablePropertiesInfoCached
        {
            get
            {
                if (_levelablePropertiesInfoCached.Any()) return _levelablePropertiesInfoCached;
                return _levelablePropertiesInfoCached = this.GetFieldsWithAttributeInfo<LevelableValue>();
            }
        }

        public float CooldownTime
        {
            get => cooldownTime;
            set => cooldownTime = value;
        }

        public int BindingIndex { get; set; } = -1;

        public bool AimingAvailable
        {
            get => aimingAvailable;
            set => aimingAvailable = value;
        }

        public bool DeactivateAimingOnCooldown
        {
            get => deactivateAimingOnCooldown;
            set => deactivateAimingOnCooldown = value;
        }

        public bool OnHoldAttackActive { get; set; }

        public AimingProperties AimingProperties
        {
            get => aimingProperties;
            set => aimingProperties = value;
        }
        public AimingAnimationProperties AimingAnimProperties
        {
            get => aimingAnimProperties;
            set => aimingAnimProperties = value;
        }
        
        private List<FieldInfo> _levelablePropertiesInfoCached = new List<FieldInfo>();
        private Vector3 _previousVelocity;
        private IActor _target;

        private bool _aimingActive;
        
        private EntityManager _dstManager;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            
            _dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            if (AimingAnimProperties.HasActorAimingAnimation)
            {
                _dstManager.AddComponentData(entity, new AimingAnimProperties
                {
                    AnimHash = Animator.StringToHash(AimingAnimProperties.ActorAimingAnimationName)
                });
            }

            if (!Actor.Abilities.Contains(this)) Actor.Abilities.Add(this);
        }

        public void Execute()
        {
            Apply(Actor);
        }

        public void Apply(IActor target)
        {
            _target = target;
            
            this.CheckPerkDuplicates(target, out var continuePerkApply);
            
            if (!continuePerkApply) return;

            ApplyActionWithCooldown(cooldownTime, ApplyDash);
        }

        public void ApplyDash()
        {
            if (_target != Actor.Owner)
            {
                var ownerActorPlayer =
                    Actor.Owner.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;

                if (ownerActorPlayer == null) return;

                this.SetAbilityLevel(ownerActorPlayer.Level, LevelablePropertiesInfoCached, Actor, _target);
            }

            if (!_target.AppliedPerks.Contains(this)) _target.AppliedPerks.Add(this);

            var targetRigidbody = _target.GameObject.GetComponent<Rigidbody>();

            if (targetRigidbody == null) return;

            _previousVelocity = targetRigidbody.velocity;

            var dashVector = new Vector3();

            if (_aimingActive)
            {
                switch (AimingProperties.aimingType)
                {
                    case AimingType.AimingArea:
                        dashVector = _dashVector;
                        break;
                    case AimingType.SightControl:
                        transform.LookAt(SpawnedAimingPrefab.transform);
                        dashVector = _target.GameObject.transform.forward;
                        break;
                }
            }
            else
            {
                dashVector = _target.GameObject.transform.forward;
            }
            
            var movement = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<ActorMovementData>(_target.ActorEntity);
            movement.Input = dashVector * force;
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(_target.ActorEntity, movement);
            
            targetRigidbody.AddForce(dashVector * force, ForceMode.Impulse);

            Actor.SimpleSpawnObjects(dashFX);

            Timer.TimedActions.AddAction(() =>
            { 
                if (targetRigidbody == null) return;
                
                targetRigidbody.velocity = _previousVelocity;
                _dashVector = Vector3.zero;
            }, timer);

            _aimingActive = false;
        }

        public void SetLevel(int level)
        {
            this.SetAbilityLevel(level, LevelablePropertiesInfoCached, Actor);
        }

        public void Remove()
        {
            if (_target != null && _target.AppliedPerks.Contains(this)) _target.AppliedPerks.Remove(this);

            foreach (var component in perkRelatedComponents)
            {
                Destroy(component);
            }

            Destroy(this);
        }
        
        public override void FinishTimer()
        {
            base.FinishTimer();
            this.FinishAbilityCooldownTimer(Actor);
        }

        public override void StartTimer()
        {
            base.StartTimer();
            this.StartAbilityCooldownTimer(Actor);
        }
        
        public void EvaluateAim(Vector2 pos)
        {
            _aimingActive = true;
            this.EvaluateAim(Actor as Actor, pos);
        }
        
        public void EvaluateAimBySelectedType(Vector2 pos)
        {
            switch (AimingProperties.aimingType)
            {
                case AimingType.AimingArea:
                    EvaluateAimByArea(pos);
                    break;
                case AimingType.SightControl:
                    EvaluateAimBySight(pos);
                    break;
            }
        }

        public void EvaluateAimByArea(Vector2 pos)
        {
            _dashVector = Quaternion.Euler(0, -180, 0) * AbilityUtils.EvaluateAimByArea(this, pos);
        }

        public void EvaluateAimBySight(Vector2 pos)
        {
            this.EvaluateAimBySight(Actor, pos);
        }

        public void ResetAiming()
        {
            this.ResetAiming(Actor);
        }


        public void SetLevelableProperty()
        {
            this.SetLevelableProperty(LevelablePropertiesInfoCached);
        }
    }
}