namespace GameFramework.Example.Enums
{
    public enum FillMode
    {
        UseEachObjectOnce = 0,
        FillAllSpawnPoints = 1,
        PlaceEachObjectXTimes = 2
    }

    public enum RotationOfSpawns
    {
        UseSpawnPointRotation = 0,
        UseRandomRotationY = 1,
        UseRandomRotationXYZ = 2,
        UseZeroRotation = 3,
        SpawnerMovement = 4
    }

    public enum SpawnPosition
    {
        UseSpawnPoints = 0,
        RandomPositionOnNavMesh = 1,
        UseSpawnerPosition = 2
    }
    
    public enum SpawnDirection
    {
        Forward = 0,
        Backward = 1
    }

    public enum FillOrder
    {
        SequentialOrder = 0,
        RandomOrder = 1
    }

    public enum SpawnPointsSource
    {
        Manually = 0,
        FindByTag = 1
    }
}