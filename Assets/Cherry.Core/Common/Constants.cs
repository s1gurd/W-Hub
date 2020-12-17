namespace GameFramework.Example.Common
{
    public static class Constants
    {
        public const float MIN_MOVEMENT_THRESH = 0.05f;
        public const float FOLLOW_MOVEMENT_AXIS_THRESH = 0.002f;
        public const float FOLLOW_MOVEMENT_SQDIST_THRESH = 0.0005f;
        public const float NETWORK_SYNC_MIN_DISTSQ_THRESH = 0.03f;
        public const float NETWORK_SYNC_MAX_DISTSQ_THRESH = 5f;
        public const float FOLLOW_ROTATION_ANGLE_THRESH = 0.01f;
        public const float INPUT_THRESH = 0.1f;
        public const float INPUT_SQDIST_THRESH = 0.1f;
        public const float VERTICAL_LOOK_ANGLE_THRESH = 75f;
        public const int INPUT_BUFFER_CAPACITY = 10;
        public const int COLLISION_BUFFER_CAPACITY = 32;
        public const float WAYPOINT_SQDIST_THRESH = 0.5f;
        public const int NETWORK_SEND_SKIP_FRAMES = 1;
    }
}