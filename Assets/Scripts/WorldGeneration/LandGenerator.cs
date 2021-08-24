using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour
{
    [Header("Params")]
    public int SizeX;
    public float Scale;
    public float Frequency;
    public float Amplitude;
    public float Height;
    public Gradient LandColor;

    [Header("Components")]
    public PolygonCollider2D Polygon;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void Generate()
    {
        float[,] Map = Noise.GenerateNoiseMap(SizeX, 1, Vector2.zero, Scale, Frequency, Amplitude);
        transform.position = new Vector3(transform.position.x, -Height, 0);
        Mesh mesh = GenerateMeshData(Map, Height).CreateMesh();
        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(Map, LandColor);
        CreatePolygon();
    }

    public void CreatePolygon()
    {

        int[] triangles = GetComponent<MeshFilter>().sharedMesh.triangles;
        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;


        Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int e = 0; e < 3; e++)
            {
                int vert1 = triangles[i + e];
                int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                if (edges.ContainsKey(edge))
                {
                    edges.Remove(edge);
                }
                else
                {
                    edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                }
            }
        }


        Dictionary<int, int> lookup = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> edge in edges.Values)
        {
            if (lookup.ContainsKey(edge.Key) == false)
            {
                lookup.Add(edge.Key, edge.Value);
            }
        }


        PolygonCollider2D polygonCollider = Polygon;
        polygonCollider.pathCount = 0;


        int startVert = 0;
        int nextVert = startVert;
        int highestVert = startVert;
        List<Vector2> colliderPath = new List<Vector2>();
        while (true)
        {
            colliderPath.Add(vertices[nextVert]);

            nextVert = lookup[nextVert];

            if (nextVert > highestVert)
            {
                highestVert = nextVert;
            }

            if (nextVert == startVert)
            {
                polygonCollider.pathCount++;
                polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                colliderPath.Clear();

                if (lookup.ContainsKey(highestVert + 1))
                {
                    startVert = highestVert + 1;
                    nextVert = startVert;
                    continue;
                }
                break;
            }
        }
    }

    public MeshData GenerateMeshData(float[,] Map, float Height)
    {
        int xSize = Map.GetLength(0) - 1;
        int ySize = Map.GetLength(1) - 1;

        MeshData meshData = new MeshData(xSize, ySize);

        for(int i = 0, y = 0; y <= ySize; y++)
        {
            for(int x = 0; x <= xSize; x++)
            {
                meshData.vertices[i] = new Vector3(x, Map[x, y] > 0 ? Map[x, y] * Height : -20, 0);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        for(int y = 0; y < ySize; y++)
        {
            for(int x = 0; x < xSize; x++)
            {
                meshData.triangles[tris + 0] = vert + 0;
                meshData.triangles[tris + 1] = vert + xSize + 1;
                meshData.triangles[tris + 2] = vert + 1;
                meshData.triangles[tris + 3] = vert + 1;
                meshData.triangles[tris + 4] = vert + xSize + 1;
                meshData.triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        return meshData;
    }
}
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public MeshData(int x, int y)
    {
        vertices = new Vector3[(x + 1) * (y + 1)];
        triangles = new int[x * y * 6];
    }
}