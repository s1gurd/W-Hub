using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IPerkAbilityBindable : IBindable
    {
        List<MonoBehaviour> PerkRelatedComponents { get; set; }
    }
}