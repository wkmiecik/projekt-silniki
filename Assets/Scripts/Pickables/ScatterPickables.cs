using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterPickables : MonoBehaviour
{
    // Editor preview
    [Header("Editor preview settings")]
    [SerializeField] bool generate;
    [SerializeField] bool showBarrelsScatterPreview = false;


    // Transforms to avoid when scattering objects
    [Header("General scattering settings")]
    [SerializeField] Transform[] transformsToAvoid;
    [SerializeField] float avoidDistance = 50;


    // Barrels
    [Header("Barrels")]
    GameObject hierarchyParent;
    [SerializeField] GameObject barrelPrefab;
    [SerializeField] int numberOfBarrels = 0;
    Vector2[] barrelsPoints;


    void Start() {
        hierarchyParent = GameObject.Find("Barrels");
        ScatterBarrels();
    }


    void OnValidate() {
        if (generate) {
            generate = false;

            if (numberOfBarrels > 0) {
                barrelsPoints = GeneratePointsForBarrels();
            }
        }
    }


    void OnDrawGizmos() {
        if (showBarrelsScatterPreview && barrelsPoints != null) {
            Gizmos.color = Color.green;
            for (int i = 0; i < barrelsPoints.Length; i++) {
                Gizmos.DrawCube(new Vector3(barrelsPoints[i].x, 7, barrelsPoints[i].y), Vector3.one * 2);
            }
        }
    }


    void ScatterBarrels() {
        if (numberOfBarrels > 0) {
            barrelsPoints = GeneratePointsForBarrels();
            for (int i = 0; i < barrelsPoints.Length; i++) {
                GameObject barrel = Instantiate(barrelPrefab, new Vector3(barrelsPoints[i].x, 7, barrelsPoints[i].y), Quaternion.Euler(90, Random.value * 360, 0));
                barrel.transform.SetParent(hierarchyParent.transform);
            }
        }
    }


    Vector2[] GeneratePointsForBarrels() {
        Vector2[] points = new Vector2[numberOfBarrels];

        // Check if generator should avoid certain points
        if (transformsToAvoid == null) {
            // Generate selected amount of random points
            for (int i = 0; i < numberOfBarrels; i++) {
                // Random point in map range
                points[i] = new Vector2(Random.Range(-2400f, 2400f), Random.Range(-2400f, 2400f));
            }
        } else {
            // Generate selected amount of random points but not arount selected transforms
            for (int i = 0; i < numberOfBarrels; i++) {
                bool badPoint = false;

                // Generate random point and check if it is far enough from every transform to avoid
                // Repeat if needed
                do {
                    points[i] = new Vector2(Random.Range(-2400f, 2400f), Random.Range(-2400f, 2400f));
                    badPoint = false;
                    for (int j = 0; j < transformsToAvoid.Length; j++) {
                        var avoidPoint = new Vector2(transformsToAvoid[j].position.x, transformsToAvoid[j].position.z);
                        if ((avoidPoint - points[i]).magnitude < avoidDistance) badPoint = true;
                    }
                } while (badPoint);
            }
        }

        return points;
    }
}
