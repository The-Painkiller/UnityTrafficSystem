using UnityEngine;

public static class Extensions
{
    public static bool IsEqualTo(this Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b) <= 1f;
    }

    public static bool IsEqualTo(this float a, float b)
    {
        return Mathf.Abs(a-b) <= 0.01f;
    }
}
