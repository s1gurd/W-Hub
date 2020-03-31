using Unity.Entities;

namespace GameFramework.Example.Utils.LowLevel
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FixedUpdateGroup : ComponentSystemGroup
    {
    }
    
}