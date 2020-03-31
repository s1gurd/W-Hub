using System;
using System.Reflection;
using UnityEngine;

namespace GameFramework.Example.Utils
{
    public static class ComponentUtils
    {
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pInfos = type.GetProperties(flags);
            foreach (var pInfo in pInfos)
            {
                
                if (!pInfo.CanWrite) continue;
                
                try {
                    pInfo.SetValue(comp, pInfo.GetValue(other, null), null);
                }
                catch { Debug.LogError("[COMPONENT REPLICATOR] Error while copying properties"); } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
            FieldInfo[] fInfos = type.GetFields(flags);
            foreach (var fInfo in fInfos) {
                fInfo.SetValue(comp, fInfo.GetValue(other));
            }
            return comp as T;
        }
        
        public static void CopyComponent(this GameObject go, Component sample)
        {
            go.AddComponent(sample.GetType()).GetCopyOf(sample);
        }
    }
}