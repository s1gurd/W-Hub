using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityActorChangeMaterial : MonoBehaviour, IActorAbility
    {
        public Material materialToApply;

        [InfoBox("Select materials that will not be replaced")]
        public List<Material> immutableMaterials = new List<Material>();

        public IActor Actor { get; set; }

        private Dictionary<Renderer, Material> _cachedRenderersWithMaterials = new Dictionary<Renderer, Material>();

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;

            if (!Actor.Abilities.Contains(this)) Actor.Abilities.Add(this);
        }

        public void Execute()
        {
            if (Actor == null) return;

            CacheRenderersWithMaterials();

            foreach (var rend in _cachedRenderersWithMaterials.Keys)
            {
                rend.material = materialToApply;
            }
        }

        public bool TrySetOriginalMaterials()
        {
            if (Actor == null || _cachedRenderersWithMaterials.Count == 0) return false;

            _cachedRenderersWithMaterials.ForEach(element => element.Key.material = element.Value);
            return true;
        }

        private void CacheRenderersWithMaterials()
        {
            if (_cachedRenderersWithMaterials.Count > 0 || Actor == null) return;

            var renderers = Actor.GameObject.GetComponentsInChildren<Renderer>().ToList();

            if (!renderers.Any()) return;

            foreach (var rend in renderers.Where(rend => !immutableMaterials.Contains(rend.sharedMaterial)))
            {
                _cachedRenderersWithMaterials.Add(rend, rend.material);
            }
        }
    }
}