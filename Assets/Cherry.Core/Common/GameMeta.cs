using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Example.Common
{
    public static class GameMeta
    {
        public static int PointsToLevelUp = 900;
        public static int PointsForKill = 1250;
        
        public static List<GameObject> AvailablePerksList
        {
            get
            {
                if (availablePerksList.Count == 0)
                {
                    var availablePerksObject = Object.FindObjectOfType(typeof(AvailablePerks)) as AvailablePerks;
                    if (availablePerksObject != null)
                        availablePerksList = availablePerksObject.AvailablePerksList;
                }

                return availablePerksList;
            }
        }
        

        private static List<GameObject> availablePerksList = new List<GameObject>();
        
        public static List<GameObject> PresetPerksList
        {
            get
            {
                if (presetPerksList.Count == 0)
                {
                    var availablePerksObject = Object.FindObjectOfType(typeof(AvailablePerks)) as AvailablePerks;
                    if (availablePerksObject != null)
                        presetPerksList = availablePerksObject.PresetPerksList;
                }

                return presetPerksList;
            }
        }
        

        private static List<GameObject> presetPerksList = new List<GameObject>();
    }
}