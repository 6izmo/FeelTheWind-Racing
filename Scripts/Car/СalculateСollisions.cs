using UnityEngine;

public static class СalculateСollisions
{
    public enum Direction
    {
        Forward,
        Backward,
        Side,
    }

    const float forwardCollisionAngleMin = 100f;
    const float forwardCollisionAngleMax = 180f;

    public static Direction DirectionCollsion(Vector3 normal, Vector3 direction)
    {
        float angle;
        float angleCos;
        angleCos = Mathf.Acos((Vector3.Dot(normal, direction)) / (normal.magnitude * direction.magnitude));
        angle = angleCos * Mathf.Rad2Deg;

        if (forwardCollisionAngleMin < angle && angle < forwardCollisionAngleMax)
            return Direction.Forward;
        else
            return Direction.Side;
    }
}
