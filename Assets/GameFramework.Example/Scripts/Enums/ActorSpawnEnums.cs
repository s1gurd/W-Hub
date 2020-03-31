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
        UseZeroRotation = 3
    }

    public enum SpawnPosition
    {
        UseSpawnPoints = 0,
        RandomPositionOnNavMesh = 1
    }

    public enum FillOrder
    {
        SequentialOrder = 0,
        RandomOrder = 1
    }
}