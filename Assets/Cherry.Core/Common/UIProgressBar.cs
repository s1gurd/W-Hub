using System;
using System.Collections.Generic;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Example.Common
{
    [HideMonoScript][RequireComponent(typeof(Image))]
    public class UIProgressBar : MonoBehaviour, IMaxValueUIElement
    {
        [EnumToggleButtons] public ChangeType ProgressBarChangeType;
        
        [ValueDropdown("UIAssociatedIds")]
        public string AssociatedValueID = "";

        [ShowIf("@ProgressBarChangeType == ChangeType.ChangeByValue")][ValueDropdown("UIAssociatedIds")]
        public string MaxValueID = "";
        public string AssociatedID => AssociatedValueID;
        public string MaxValueAssociatedID => MaxValueID;
        public float MaxValue { get; set; }
        
        private UIReceiver _receiver;
        private Image _progressBar;
        
        private object _cachedCurrentProgressBarValue;
        private object _cachedInputValue;

        public enum ChangeType
        {
            ChangeByValue = 0,
            ChangeByTimer = 1
        }

        public void Awake()
        {
            _progressBar = GetComponent<Image>();
            if (_progressBar == null) Debug.LogError("[UI Progress Bar] Image component is required!");
        }
        
        public void SetMaxValue(object maxValue)
        {
            if (!maxValue.IsNumericType()) return;
            
            MaxValue = (float) Convert.ToDecimal(maxValue);

            RefreshProgressBarView();

            if (_cachedInputValue == null) return;
            
            SetData(_cachedInputValue);
            _cachedInputValue = null;
        }

        public void SetData(object damage)
        {
            if (!damage.IsNumericType() || _progressBar == null) return;

            if (_cachedCurrentProgressBarValue != null &&
                Convert.ToDecimal(_cachedCurrentProgressBarValue) == Convert.ToDecimal(damage)) return;
            
            var inputValue = new object();

            switch (ProgressBarChangeType)
            {
                case ChangeType.ChangeByValue:
                    _cachedInputValue = damage;
                    
                    if (MaxValue <= 0) return;

                    var convertedInput = (float) Convert.ToDecimal(damage);
                    inputValue = convertedInput / MaxValue;

                    break;
                case ChangeType.ChangeByTimer:
                    inputValue = (float) damage;
                    break;
            }

            _progressBar.fillAmount = (float) Convert.ToDecimal(inputValue);
            _cachedCurrentProgressBarValue =  Convert.ToDecimal(inputValue);
        }

        private void RefreshProgressBarView()
        {
            if (_cachedInputValue == null) return;
            _progressBar.fillAmount = (float) Convert.ToDecimal(_cachedInputValue) / MaxValue;
        }
        
        

        private List<string> UIAssociatedIds()
        {
#if UNITY_EDITOR
            if (_receiver == null) _receiver = GetComponentInParent<UIReceiver>();
            return _receiver == null ? null : _receiver.UIAssociatedIds;
#else
            return null;
#endif
        }
    }
}