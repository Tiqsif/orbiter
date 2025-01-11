using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMath
{
    public static Quaternion CustomLerp(Quaternion start, Quaternion end, float t)
    {
        // Ensure t is clamped between 0 and 1
        t = Mathf.Clamp01(t);

        // Calculate the dot product to check if the quaternions are aligned
        float dot = Quaternion.Dot(start, end);

        // If the dot product is negative, invert one quaternion to take the shortest path
        if (dot < 0f)
        {
            end = new Quaternion(-end.x, -end.y, -end.z, -end.w);
            dot = -dot;
        }

        // Linearly interpolate each component of the quaternion
        Quaternion result = new Quaternion(
            Mathf.Lerp(start.x, end.x, t),
            Mathf.Lerp(start.y, end.y, t),
            Mathf.Lerp(start.z, end.z, t),
            Mathf.Lerp(start.w, end.w, t)
        );

        // Normalize the result to ensure it's a valid rotation
        return Normalize(result);
    }

    private static Quaternion Normalize(Quaternion q)
    {
        float magnitude = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        return new Quaternion(q.x / magnitude, q.y / magnitude, q.z / magnitude, q.w / magnitude);
    }
}
