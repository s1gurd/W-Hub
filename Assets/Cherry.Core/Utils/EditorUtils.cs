using System.Collections;
using UnityEngine;

namespace GameFramework.Example.Utils
{
    public static class EditorUtils
    {
        public static GameObject CreateDummySphere(Vector3 pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = pos;
            var col = go.GetComponent<Collider>();
            Object.Destroy(col);
            return go;
        }
        
        public static IEnumerable GetEditorTags()
        {
#if UNITY_EDITOR
            return UnityEditorInternal.InternalEditorUtility.tags;
#else
            return null;
#endif
        }
    }
}