using UnityEngine;

namespace UnityHelper
{
    public static class UnityComponentExtension
    {
        public static T GetComponentInChildren<T>(GameObject gameObject, int maxDepth) where T : Component
        {
            return GetRecursive<T>(gameObject.transform, maxDepth, 0);
        }

        private static T GetRecursive<T>(Transform current, int maxDepth, int currentDepth) where T : Component
        {
            if (currentDepth > maxDepth) return null;

            // 1. Check if the component exists on the current object
            T component = current.GetComponent<T>();
            if (component != null) return component;

            // 2. If not found and we haven't hit max depth, check children
            if (currentDepth < maxDepth)
            {
                foreach (Transform child in current)
                {
                    T found = GetRecursive<T>(child, maxDepth, currentDepth + 1);
                    if (found != null) return found;
                }
            }

            return null;
        }
    }    
}
