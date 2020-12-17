using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class CheatButton : MonoBehaviour
    {
        public Button button;
        
        [SerializeField] private TextMeshProUGUI _buttonName;

        public string ButtonName
        {
            set => SetButtonName(value);
        }

        public Action ButtonAction
        {
            set => SetButtonAction(value);
        }


        private void SetButtonName(string value)
        {
            _buttonName.SetText(value);
            gameObject.name = value;
        }

        private void SetButtonAction(Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action.Invoke);
        }
    }
}