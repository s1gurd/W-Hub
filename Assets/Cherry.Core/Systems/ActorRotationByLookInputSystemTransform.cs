using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorRotationByLookInputSystemTransform : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<ActorRotationByLookInputData>(),
                ComponentType.ReadOnly<PlayerInputData>(),
                ComponentType.Exclude<StopRotationData>());
        }

        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;

            Entities.With(_query).ForEach((Entity entity, Transform transform,
                ref ActorRotationByLookInputData rotation, ref PlayerInputData input) =>
            {
                if (transform == null) return;
                
                var bodyRotation = new Vector3();
                
                float dX, dY;
                var invert = rotation.Invert ? 1f : -1f;

                if (rotation.RotateY)
                {
                    dX = input.Look.y * (rotation.Sensitivity * dt) * invert;
                    bodyRotation += new Vector3(dX, 0, 0);
                }

                if (rotation.RotateX)
                {
                    dY = input.Look.x * (rotation.Sensitivity * dt);
                    bodyRotation += new Vector3(0, dY, 0);
                }


                transform.Rotate(bodyRotation);
                var tempEuler = transform.localRotation.eulerAngles;
                const float thresh = Constants.VERTICAL_LOOK_ANGLE_THRESH;

                if (tempEuler.x > thresh && tempEuler.x < 180)
                {
                   transform.localRotation = Quaternion.Euler(thresh,tempEuler.y, 0);
                }

                if (tempEuler.x < 360 - thresh && tempEuler.x > 180)
                {
                    transform.localRotation = Quaternion.Euler(360 - thresh, tempEuler.y, 0);
                }
                
            });
        }
    }
}