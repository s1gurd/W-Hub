using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace GameFramework.Example.Common
{
    public struct CustomDeviceState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('C', 'U', 'S', 'T');

        [InputControl(name = "customStick_1", format = "VEC2", layout = "Stick", displayName = "First Stick")]
        public Vector2 customStick_1;


        [InputControl(name = "customStick_2", format = "VEC2", layout = "Stick", displayName = "Second Stick")]
        public Vector2 customStick_2;


        [InputControl(name = "customStick_3", format = "VEC2", layout = "Stick", displayName = "Third Stick")]
        public Vector2 customStick_3;
        
        [InputControl(name = "customStick_4", format = "VEC2", layout = "Stick", displayName = "Third Stick")]
        public Vector2 customStick_4;
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(stateType = typeof(CustomDeviceState))]
    public class CustomDevice : InputDevice, IInputUpdateCallbackReceiver
    {
#if UNITY_EDITOR
        static CustomDevice()
        {
            Initialize();
        }

#endif

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            InputSystem.RegisterLayout<CustomDevice>(
                matches: new InputDeviceMatcher()
                    .WithInterface("CustomDevice"));
        }

        public void OnUpdate()
        {
        }

        public StickControl customStick_1 { get; private set; }
        public StickControl customStick_2 { get; private set; }
        public StickControl customStick_3 { get; private set; }
        public StickControl customStick_4 { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            customStick_1 = GetChildControl<StickControl>("customStick_1");
            customStick_2 = GetChildControl<StickControl>("customStick_2");
            customStick_3 = GetChildControl<StickControl>("customStick_3");
            customStick_3 = GetChildControl<StickControl>("customStick_4");
        }

        public static CustomDevice current { get; private set; }

        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
        }
    }
}