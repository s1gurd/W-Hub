using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IPerkUpgrade : IPerkBase
    {
        float PerkWeight { get; }
        int AvailabilityLevel { get; }
        int AvailableLevelsNumber { get; }
    }
}