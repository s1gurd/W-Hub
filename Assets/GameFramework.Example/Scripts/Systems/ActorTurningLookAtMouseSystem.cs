using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorTurningLookAtMouseSystem:ComponentSystem
    {
        protected override void OnUpdate()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return;
            
            Entities.WithAll<ActorRotationLookAtMouseData>().ForEach(
                (Entity entity, Rigidbody rigidBody, ref PlayerInputData input, ref ActorRotationLookAtMouseData rotation) =>
                {
                    var mousePos = new Vector3(input.Mouse.x, input.Mouse.y, 0);
                    var camRay = mainCamera.ScreenPointToRay(mousePos);

                    if (!Physics.Raycast(camRay, out var floorHit, rotation.CamRayLen, rotation.Layer)) return;
                    
                    var position = rigidBody.gameObject.transform.position;
                    var playerToMouse = floorHit.point - new Vector3(position.x, position.y, position.z);
                    playerToMouse.y = 0f;
                    var newRot = Quaternion.LookRotation(playerToMouse);
                    rigidBody.MoveRotation(newRot);
                });
        }
    }
}