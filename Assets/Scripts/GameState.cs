using System.Collections.Generic;
using UnityEngine;

namespace V3D.Controllers
{
    public static class GameState
    {
        public static bool cursorLock;
        public static Dictionary<Collider, ShowInteractiveContext> InteractableLabels = new Dictionary<Collider, ShowInteractiveContext>();
        public static ShowInteractiveContext _currentActive;
        public static PopupController _currentPopup;
        public static SliderController _currentSlider;
    }
}