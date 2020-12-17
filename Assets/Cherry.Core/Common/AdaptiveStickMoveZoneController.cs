using Cherry.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace GameFramework.Example.Common
{
    [HideMonoScript]
    public class AdaptiveStickMoveZoneController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public OnScreenStick screenStick;
        public Image stickTouchArea;
        public RectTransform stickRectTransform;

        private Vector3 _initialPosition;

        private readonly Resolver _resolver = new Resolver();

        public void Awake()
        {
            _initialPosition = stickRectTransform.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (stickTouchArea.enabled == _resolver.GameSettings.AdaptiveStickEnabled)
            {
                stickTouchArea.enabled = !_resolver.GameSettings.AdaptiveStickEnabled;
            }
            
            if (!_resolver.GameSettings.AdaptiveStickEnabled) return;

            stickRectTransform.position = eventData.position;
            screenStick.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_resolver.GameSettings.AdaptiveStickEnabled) return;

            stickRectTransform.position = _initialPosition;
            screenStick.OnPointerUp(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_resolver.GameSettings.AdaptiveStickEnabled) return;

            screenStick.OnDrag(eventData);
        }
    }
}