using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace GameFramework.Example.AI
{
    [Serializable]
    public class AIBehaviourSetting
    {
        [ValueDropdown("GetAIs")] public string behaviourType = "";

        [HideIf("@GetOrCreateAI(behaviourType) == null || HideCurve(behaviourType)")] [InfoBox("@GetCurveLabel(behaviourType)")]
        public AnimationCurve priorityCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [HideIf("@GetOrCreateAI(behaviourType) == null || HideCurve(behaviourType)")]
        public float curveMinSample = 0;

        [HideIf("@GetOrCreateAI(behaviourType) == null || HideCurve(behaviourType)")]
        public float curveMaxSample = 100;

        [HideIf("@GetOrCreateAI(behaviourType) == null")] [Range(0, 3)]
        public float basePriority = 1;

        [ShowIf("@ShowModes(behaviourType)")] [Space] [ValueDropdown("@GetModes(behaviourType)")]
        public string additionalMode = "";

        [ShowIf("@ShowActions(behaviourType)")]
        [Space]
        public int executeCustomInput = 1;

        [ShowIf("@ShowFilters(behaviourType)")] [Space] [EnumToggleButtons]
        public TagFilterMode targetFilterMode = TagFilterMode.IncludeOnly;

        [ShowIf("@ShowFilters(behaviourType)")] [ValueDropdown("Tags")]
        public List<string> targetFilterTags;

        public IAIBehaviour BehaviourInstance => GetOrCreateAI(behaviourType);

        private IAIBehaviour _behaviourInstance;

        public IActor Actor;

        private static IEnumerable<string> GetAIs()
        {
#if UNITY_EDITOR

            var l = new List<string> {String.Empty};
            l.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IAIBehaviour).IsAssignableFrom(p) && p.IsClass)
                .Convert(a => a.ToString().Split('.').Last()));
            return l;
#else
            return null;
#endif
        }

        private IEnumerable<string> GetModes(string type)
        {
#if UNITY_EDITOR

            var b = GetOrCreateAI(type);
            return b != null ? b.AdditionalModes : new string[0];
#else
            return null;
#endif
        }

        private IAIBehaviour GetOrCreateAI(string type)
        {
            if (type.Equals(string.Empty, StringComparison.Ordinal)) return null;

            if (_behaviourInstance != null && _behaviourInstance.GetType().Name.Split('.').Last()
                    .Equals(type, StringComparison.Ordinal))
                return _behaviourInstance;

            Type t = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()).Where(p => typeof(IAIBehaviour).IsAssignableFrom(p) && p.IsClass)
                .FirstOrDefault(p => p.Name.Split('.').Last().Equals(type, StringComparison.Ordinal));
            if (t == null)
            {
                Debug.LogError(
                    $"[AI BEHAVIOUR ROOT] Cannot create {type} type behaviour class! Aborting AI composition");
                behaviourType = "";
                return null;
            }

            _behaviourInstance = Activator.CreateInstance(t) as IAIBehaviour;
            return _behaviourInstance;
        }

        private string GetCurveLabel(string type)
        {
#if UNITY_EDITOR

            var b = GetOrCreateAI(type);
            if (b == null || b.XAxis.Equals(String.Empty, StringComparison.Ordinal)) return string.Empty;

            return "Curve for behaviour priority based on " + b.XAxis;
            
#else
            return String.Empty;
#endif
        }
        
        private bool HideCurve(string type)
        {
#if UNITY_EDITOR

            var b = GetOrCreateAI(type);
            return !(b != null && b.NeedCurve);

#else
            return false;
#endif
        }

        private bool ShowFilters(string type)
        {
#if UNITY_EDITOR

            var b = GetOrCreateAI(type);
            return b != null && b.NeedTarget;

#else
            return false;
#endif
        }

        private bool ShowActions(string type)
        {
#if UNITY_EDITOR

            var b = GetOrCreateAI(type);
            return b != null && b.NeedActions;
#else
            return false;
#endif
        }
        
        private bool ShowModes(string type)
        {
#if UNITY_EDITOR

            var b = GetOrCreateAI(type);
            return b != null && b.AdditionalModes.Length > 0;
#else
            return false;
#endif
        }
        
        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }
    }
}