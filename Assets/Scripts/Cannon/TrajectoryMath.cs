using UnityEngine;

public static class TrajectoryMath
{
    public static bool CalculateLaunchAngle(float speed, float distance, float yOffset, float gravity, out float angle0, out float angle1) {
        angle0 = angle1 = 0;

        float speedSquared = speed * speed;

        float operandA = Mathf.Pow(speed, 4);
        float operandB = gravity * (gravity * (distance * distance) + (2 * yOffset * speedSquared));

        // Target is not in range
        if (operandB > operandA)
            return false;

        float root = Mathf.Sqrt(operandA - operandB);

        angle0 = Mathf.Atan((speedSquared + root) / (gravity * distance));
        angle1 = Mathf.Atan((speedSquared - root) / (gravity * distance));

        return true;
    }


    public static float TimeOfFlight(float speed, float angle, float yOffset, float gravity) {
        float ySpeed = speed * Mathf.Sin(angle);

        float time = (ySpeed + Mathf.Sqrt((ySpeed * ySpeed) + 2 * gravity * yOffset)) / gravity;

        return time;
    }


    public static Vector2[] ProjectileArcPoints(int iterations, float speed, float distance, float gravity, float angle) {
        float iterationSize = distance / iterations;

        float radians = angle;

        Vector2[] points = new Vector2[iterations + 1];

        for (int i = 0; i <= iterations; i++) {
            float x = iterationSize * i;
            float t = x / (speed * Mathf.Cos(radians));
            float y = -0.5f * gravity * (t * t) + speed * Mathf.Sin(radians) * t;

            Vector2 p = new Vector2(x, y);

            points[i] = p;
        }

        return points;
    }
}
