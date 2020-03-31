using System.Collections;

namespace GameFramework.Example.Utils
{
    public static class EditorUtils
    {
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