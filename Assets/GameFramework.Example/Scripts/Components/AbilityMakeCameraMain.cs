using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityMakeCameraMain : MonoBehaviour, IActorAbility
    {
        public Camera mainCamera;

        public void AddComponentData(ref Entity entity)
        {
        }

        public void Execute()
        {
            if (mainCamera == null)
            {
                mainCamera = this.gameObject.GetComponent<Camera>();
                
                if (mainCamera == null)
                {
                    mainCamera = this.gameObject.transform.GetComponentInChildren<Camera>();
                }
                if (mainCamera == null)
                {
                    Debug.LogError("[MAKE CAMERA MAIN] Camera is neither set nor found in children!");
                    return;
                }
            }

            foreach (var c in FindObjectsOfType<Camera>())
            {
                c.gameObject.SetActive(c.gameObject == this.gameObject);
            }
        }
    }
}