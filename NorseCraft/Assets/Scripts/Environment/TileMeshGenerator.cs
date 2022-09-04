using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileMeshGenerator
{
    //public static int TileSize = 500;
    //public static int VertWidth = 50;
    //public static int QuadSize = 10;

    //public static float MinHeight = -3;
    //public static float MaxHeight = 15;
    //public static float normalHeight = 1;

    /*public enum TileType
    {
        Normal,
        BottomLeft,
        BottomEdge,
        BottomRight,
        LeftEdge,
        RightEdge,
        TopLeft,
        TopEdge,
        TopRight
    }*/

    

    /*
    public static GameObject generateTileBaseMesh(GameObject newTileMesh)
    {
        MeshData meshData = new MeshData(VertWidth);
        newTileMesh.GetComponent<MeshFilter>().sharedMesh = meshData.mesh;

        for(int i = 0, z = 0; z <= VertWidth; z++)
        {
            for(int x = 0; x <= VertWidth; x++)
            {
                meshData.vertices[i] = new Vector3((x * QuadSize), 0, (z * QuadSize));
                meshData.colors[i] = Color.gray;
            }
        }

        int vert = 0;
        int tris = 0;
        for(int z = 0; z < VertWidth; z++)
        {
            for(int x = 0; x < VertWidth; x++)
            {
                meshData.triangles[tris + 0] = vert + 0;            // bottom left
                meshData.triangles[tris + 1] = vert + VertWidth + 1;    // top left
                meshData.triangles[tris + 2] = vert + VertWidth + 2;    // top right

                meshData.triangles[tris + 3] = vert + 1;            // bottom right
                meshData.triangles[tris + 4] = vert + 0;            // bottom left
                meshData.triangles[tris + 5] = vert + VertWidth + 2;    // top right
                vert++;
                tris += 6;
            }
            vert++;
        }

        meshData.mesh.Clear();

        //newTileMesh.GetComponent<MeshFilter>();
        meshData.mesh.vertices = meshData.vertices;
        meshData.mesh.triangles = meshData.triangles;
        meshData.mesh.colors = meshData.colors;

        meshData.mesh.RecalculateNormals();
        //newTileMesh.GetComponent<MeshCollider>().sharedMesh = meshData.mesh;

        return newTileMesh;
    }*/
}

[System.Serializable]
public class MeshData
{
    public Color sandColor;
    public Color grassLightColor;
    public Color grassDarkColor;
    public Mesh mesh;
    public Vector3[] vertices;
    public Vector3[] spawnVertices;
    public int[] triangles;
    public Color[] colors;

    public MeshData(int vertWidth)
    {
        mesh = new Mesh();
        vertices = new Vector3[(vertWidth + 1) * (vertWidth + 1)];
        triangles = new int[vertWidth * vertWidth * 6];
        colors = new Color[vertices.Length];
    }
}
