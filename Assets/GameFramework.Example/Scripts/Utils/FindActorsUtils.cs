using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Enums;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Example.Utils
{
    public static class FindActorsUtils
    {
        //Not used for now, consider delete it later
        public static List<Transform> GetActorsList(GameObject source, TargetType targetType, string name, string tag)
        {
            var targets = new List<Transform>();

            switch (targetType)
            {
                case TargetType.ComponentName:
                    Object.FindObjectsOfType<MonoBehaviour>().OfType<IComponentName>()
                        .Where(n => n.ComponentName.Equals(name,
                            StringComparison.Ordinal))
                        .ForEach(n => targets.Add((n as MonoBehaviour)?.gameObject.transform));
                    break;
                case TargetType.ChooseByTag:
                    GameObject.FindGameObjectsWithTag(tag).ForEach(o => targets.Add(o.transform));
                    break;
                case TargetType.Spawner:
                    var t = source.GetComponent<IActor>()?.Spawner;
                    if (t != null)
                    {
                        targets.Add(t.transform);
                    }

                    break;
                case TargetType.None:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return targets;
        }

        public static Transform ChooseActor(Transform origin, IReadOnlyList<Transform> targets, ChooseTargetStrategy s)
        {
            Transform t;
            
            switch (targets.Count)
            {
                case 0: return null;
                case 1: return targets[0];
                default:
                    if (targets.Count == 0) return null;

                    t = targets[0];
                    float3 currentPosition = origin.position;
                    var currentDistance = math.distancesq(currentPosition, t.position);

                    if (s == ChooseTargetStrategy.Random) return targets[UnityEngine.Random.Range(0, targets.Count)];
                    
                    for (var i = 1; i < targets.Count; i++)
                    {
                        var tempDistanceSq = math.distancesq(currentPosition, targets[i].position);
                        
                        if ((s != ChooseTargetStrategy.Nearest || !(tempDistanceSq < currentDistance)) &&
                            (s != ChooseTargetStrategy.Farthest || !(tempDistanceSq > currentDistance))) continue;
                        
                        currentDistance = tempDistanceSq;
                        t = targets[i];
                    }

                    break;
            }

            return t;
        }
    }
}