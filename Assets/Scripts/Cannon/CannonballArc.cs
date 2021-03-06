using UnityEngine;

public class CannonballArc : MonoBehaviour {
    [SerializeField] int iterations = 20;

    public LineRenderer lineRenderer;

    void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void UpdateArc(float speed, float distance, float gravity, float angle, Vector3 direction, bool valid) {
        Vector2[] arcPoints = TrajectoryMath.ProjectileArcPoints(iterations, speed, distance, gravity, angle);
        Vector3[] points3d = new Vector3[arcPoints.Length];

        for (int i = 0; i < arcPoints.Length; i++) {
            points3d[i] = new Vector3(0, arcPoints[i].y, arcPoints[i].x);
        }

        lineRenderer.positionCount = arcPoints.Length;
        lineRenderer.SetPositions(points3d);

        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void SetColor(Color color) {
        lineRenderer.sharedMaterial.color = color;
    }
}