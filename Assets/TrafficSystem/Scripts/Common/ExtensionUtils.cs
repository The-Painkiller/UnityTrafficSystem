using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionUtils
{
    public static bool Contains(this SignalDirectionID[] array, SignalDirectionID item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == item)
                return true;
        }
        return false;
    }
}
