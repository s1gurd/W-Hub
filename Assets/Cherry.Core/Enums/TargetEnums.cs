namespace GameFramework.Example.Enums
{
    public enum TargetType
    {
        Spawner = 1,
        ComponentName = 2,
        ChooseByTag = 3,
        None = 0
    }
    
    public enum ChooseTargetStrategy
    {
        Nearest = 0,
        Farthest = 1,
        Random = 2
    }
    
    public enum FollowType
    {
        Simple = 0,
        UseMovementComponent = 1
    }
    
}