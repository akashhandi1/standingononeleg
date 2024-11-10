using UnityEngine;

public static class MathUtilities
{
    // Method to calculate the Euclidean distance between two 3D points
    public static float CalculateDistance(Vector3 pointA, Vector3 pointB)
    {
        return Vector3.Distance(pointA, pointB);
    }
}

