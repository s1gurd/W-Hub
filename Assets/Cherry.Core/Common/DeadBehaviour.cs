using System;
using System.Collections.Generic;

namespace GameFramework.Example.Common
{
    [Serializable]
    public class DeadBehaviour
    {
        public bool RemoveTag = false;
        public bool RemoveRigidbody = true;
        public bool RemoveColliders = true;
        public bool RemoveInput = true;
        
        public List<string> OnDeathActionsComponentNames = new List<string>();
    }
}