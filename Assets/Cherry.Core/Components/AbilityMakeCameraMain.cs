using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityMakeCameraMain : MonoBehaviour, IActorAbility
    {
        public IActor Actor { get; set; }

        public Camera mainCamera;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
        }

        public void Execute()
        {
            if (mainCamera == null)
            {
                mainCamera = this.gameObject.GetComponent<Camera>();

                if (mainCamera == null)
                {
                    mainCamera = this.gameObject.GetComponentInChildren<Camera>();
                }

                if (mainCamera == null)
                {
                    Debug.LogError("[MAKE CAMERA MAIN] Camera is neither set nor found in children!");
                    return;
                }
            }

            mainCamera.gameObject.tag = "MainCamera";

            foreach (var c in FindObjectsOfType<Camera>())
            {
                c.gameObject.SetActive(c.gameObject == mainCamera.gameObject);
            }
        }
    }
}