using System;

namespace GameFramework.Example.Common
{
    public struct TimerAction
    {
        public Action Act;
        public float Delay;
    }
    
    public struct TimerActionEditor
    {
        public string Act;
        public float Delay;
    }
}