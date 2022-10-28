namespace TrafficSystem
{
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

        public static bool IsEqualTo(this Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b) <= 1f;
        }

        public static bool IsEqualTo(this float a, float b)
        {
            return Mathf.Abs(a - b) <= 0.01f;
        }
    }
}