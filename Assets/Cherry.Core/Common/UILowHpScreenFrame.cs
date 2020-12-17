using System;
using System.Collections.Generic;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [HideMonoScript]
    public class UILowHpScreenFrame : MonoBehaviour, IMaxValueUIElement
    {
        [ValueDropdown("UIAssociatedIds")] public string AssociatedValueID = "";

        [ValueDropdown("UIAssociatedIds")] public string MaxValueID = "";

        public float showFrameHpThresholdPercent;
        
        public string AssociatedID => AssociatedValueID;
        public string MaxValueAssociatedID => MaxValueID;

        private float _maxValue;
        private UIReceiver _receiver;

        public void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetData(object damage)
        {
            if (!damage.IsNumericType()) return;

            var convertedInput = (float) Convert.ToDecimal(damage);

            var currentFrameState = gameObject.activeSelf;
            var showFrame = convertedInput > 0 && convertedInput <= _maxValue * (showFrameHpThresholdPercent * 0.01f);

            if (currentFrameState != showFrame) gameObject.SetActive(showFrame);
        }

        public void SetMaxValue(object maxValue)
        {
            if (!maxValue.IsNumericType()) return;
            _maxValue = (float) Convert.ToDecimal(maxValue);
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