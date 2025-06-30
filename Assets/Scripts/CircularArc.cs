using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteAlways]
public class CircularArc : MonoBehaviour
{
    [FormerlySerializedAs("baseAngleDegrees")] [Header("Arc Shape")] [Range(0.0f, 360.0f)]
    public float angleDegrees = 180f;

    public float extraAngleDegrees = 0f;
    public float maxAngleDegrees = 360.0f;
    public float minAngleDegrees = 20.0f;

    public float radius = 2f;
    public float thickness = 0.3f;

    [Header("Detail")] [Min(3)] public int resolution = 64; // Number of segments for 360 degrees

    private MeshFilter meshFilter;
    private PolygonCollider2D polyCollider;

    private float starterAngleDegrees;


    void Update()
    {
        angleDegrees = starterAngleDegrees + extraAngleDegrees;
        GenerateMesh();
    }

    void OnValidate()
    {
        GenerateMesh();
    }

    void Reset()
    {
        GenerateMesh();
    }

    void Start()
    {
        starterAngleDegrees = angleDegrees + extraAngleDegrees;
    }


    void GenerateMesh()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        int segmentCount = Mathf.Max(2, Mathf.CeilToInt((angleDegrees / 360f) * resolution));
        float angleStep = Mathf.Deg2Rad * (angleDegrees / segmentCount);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float innerRadius = radius - thickness;
        float startAngle = -Mathf.Deg2Rad * (angleDegrees / 2f);

        // Vertices and UVs
        for (int i = 0; i <= segmentCount; i++)
        {
            float angle = startAngle + i * angleStep;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector3 outer = new Vector3(cos * radius, sin * radius);
            Vector3 inner = new Vector3(cos * innerRadius, sin * innerRadius);

            vertices.Add(inner); // inner
            vertices.Add(outer); // outer

            float t = (float)i / segmentCount; // [0..1] across arc
            uvs.Add(new Vector2(0, t));
            uvs.Add(new Vector2(1, t));

        }

        // Triangles
        for (int i = 0; i < segmentCount; i++)
        {
            int idx = i * 2;

            triangles.Add(idx);
            triangles.Add(idx + 1);
            triangles.Add(idx + 2);

            triangles.Add(idx + 1);
            triangles.Add(idx + 3);
            triangles.Add(idx + 2);
        }

        Mesh mesh = new Mesh();
        mesh.name = "CircularArc";
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;

        polyCollider = GetComponent<PolygonCollider2D>();
        if (!polyCollider)
            polyCollider = gameObject.AddComponent<PolygonCollider2D>();

        Vector2[] colliderPath = new Vector2[(segmentCount + 1) * 2];
        for (int i = 0; i <= segmentCount; i++)
        {
            Vector2 inner = vertices[i * 2];       // inner point
            Vector2 outer = vertices[i * 2 + 1];   // outer point
            Vector2 halfway = Vector2.Lerp(inner, outer, 0.5f); // midpoint
            colliderPath[i] = halfway;
        }

        for (int i = segmentCount; i >= 0; i--)
        {
            colliderPath[segmentCount + 1 + (segmentCount - i)] = vertices[i * 2]; // inner
        }

        polyCollider.pathCount = 1;
        polyCollider.SetPath(0, colliderPath);
    }


    public bool IsAddAngleDegreeValid(float value)
    {
        return !((angleDegrees + value) > maxAngleDegrees) && !((angleDegrees + value) < minAngleDegrees);
    }


    public void AddExtraAngleDegrees(float value, float duration)
    {
        extraAngleDegrees += value;
    }

    public void RemoveExtraAngleDegrees(float value, float duration)
    {
        extraAngleDegrees -= value;
    }
}