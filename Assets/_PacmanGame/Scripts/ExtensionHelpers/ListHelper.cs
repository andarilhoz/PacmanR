using System.Collections.Generic;
using UnityEngine;

namespace _PacmanGame.Scripts.ExtensionHelpers
{
    public static class ListHelper
    {
        public static void AddIfNotNull<T>(this List<T> list, T item)
        {
            if(item != null) list.Add(item);
        }
    }
}