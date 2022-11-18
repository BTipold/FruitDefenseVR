using UnityEngine;
using System.Collections.Generic;

public static class Extension
{
    public static List<T> GetActiveComponentsInChildrenRecursively<T>(this Transform _transform, List<T> _componentList)
    {
        foreach (Transform t in _transform)
        {
            if (t.gameObject.activeSelf)
            {
                T[] components = t.GetComponents<T>();

                foreach (T component in components)
                {
                    if (component != null)
                    {
                        _componentList.Add(component);
                    }
                }

                GetComponentsInChildrenRecursively<T>(t, _componentList);
            }
        }

        return _componentList;
    }

    public static List<T> GetComponentsInChildrenRecursively<T>(this Transform _transform, List<T> _componentList)
    {
        foreach (Transform t in _transform)
        {
            T[] components = t.GetComponents<T>();

            foreach (T component in components)
            {
                if (component != null)
                {
                    _componentList.Add(component);
                }
            }

            GetComponentsInChildrenRecursively<T>(t, _componentList);
        }

        return _componentList;
    }
}