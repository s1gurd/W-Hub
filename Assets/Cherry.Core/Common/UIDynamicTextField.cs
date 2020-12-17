using System.Collections.Generic;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [HideMonoScript]
    public class UIDynamicTextField : MonoBehaviour, IUIElement
    {
        public string FieldText;

        [ValueDropdown("UIAssociatedIds")] public string AssociatedFieldID = "";

        private TextMeshProUGUI _textMeshPro;

        private object _cachedFieldValue = new object();
        private UIReceiver _receiver = null;

        public string AssociatedID => AssociatedFieldID;

        public void Awake()
        {
            _textMeshPro = this.GetComponent<TextMeshProUGUI>();
            if (_textMeshPro == null)
                Debug.LogError("[UI Text Field] TextMeshProGUI Component is required !");
        }

        public void SetData(object damage)
        {
            if (_textMeshPro == null) return;

            if (!damage.IsNumericType() && !(damage is string)) return;

            if (_cachedFieldValue.Equals(damage)) return;

            _textMeshPro.SetText($"{FieldText}{damage}");
            _cachedFieldValue = damage;
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