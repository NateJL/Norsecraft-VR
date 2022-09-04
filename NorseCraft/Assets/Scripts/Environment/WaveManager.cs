using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class WaveManager : MonoBehaviour
{
    public float scale = 1f, speed = .1f, width = 10, length = 10,
    noiseStrength = 1f, noiseWalk = 1f;
    public int resX = 15, resZ = 15;
    public bool debug;
    float time;
    Vector3[] baseHeight;
    Mesh mesh;
    MeshCollider mCollider;
    Dictionary<int, GameObject> floatingObjs = new Dictionary<int, GameObject>();
    Vector3 floatVel = Vector3.zero;

    void Start()
    {
        InitPlane();
        mesh = GetComponent<MeshFilter>().mesh;
        baseHeight = mesh.vertices;
        mCollider = GetComponent<MeshCollider>();
    }

    void InitPlane()
    {
        // You can change that line to provide another MeshFilter
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        #region Vertices
        Vector3[] vertices = new Vector3[resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
            {
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width;
                vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
            }
        }
        #endregion

        #region Normals
        Vector3[] normals = new Vector3[vertices.Length];
        for (int n = 0; n < normals.Length; n++)
            normals[n] = Vector3.up;
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }
        #endregion

        #region Triangles
        int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int face = 0; face < nbFaces; face++)
        {
            // Retrieve lower left corner from face ind
            int i = face % (resX - 1) + (face / (resZ - 1) * resX);

            triangles[t++] = i + resX;
            triangles[t++] = i + 1;
            triangles[t++] = i;

            triangles[t++] = i + resX;
            triangles[t++] = i + resX + 1;
            triangles[t++] = i + 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        //mesh.Optimize();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Floating")
        {
            Debug.Log("Found floating");
            floatingObjs.Add(GetClosestVert(c.gameObject.transform.position), c.gameObject);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (floatingObjs.ContainsValue(c.gameObject))
        {
            Debug.Log("Removing Floating");
            floatingObjs.Remove(GetClosestVert(c.gameObject.transform.position));
        }
    }

    void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position, new Vector3(width, scale, length));
        }
    }

    void Update()
    {
        CalcWaves();
    }

    void CalcWaves()
    {
        UpdateClosestVerts();
        time += speed * Time.deltaTime;
        float sin, noise, noiseShift = Mathf.Sin(time * noiseWalk);
        for (int i = 0; i < baseHeight.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            sin = Mathf.Sin(time + (baseHeight[i].x + baseHeight[i].z)) * scale;
            noise = Mathf.PerlinNoise(baseHeight[i].x + noiseShift, baseHeight[i].y + noiseShift) * noiseStrength;
            vertex.y = sin + noise;
            baseHeight[i] = vertex;

            if (floatingObjs.ContainsKey(i))
            {
                floatingObjs[i].transform.position = Vector3.SmoothDamp(floatingObjs[i].transform.position, transform.position + vertex, ref floatVel, .25f);
                floatingObjs[i].transform.rotation = Quaternion.RotateTowards(floatingObjs[i].transform.rotation, Quaternion.LookRotation(mesh.normals[i]), 90 * Time.deltaTime);
            }
        }
        mesh.vertices = baseHeight;
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void UpdateClosestVerts()
    {
        foreach (var g in floatingObjs)
        {
            if (g.Key != GetClosestVert(g.Value.transform.position))
                UpdateKey(g.Key, GetClosestVert(g.Value.transform.position));
        }
    }

    int GetClosestVert(Vector3 pos)
    {
        pos -= transform.position;
        int output = -1;
        float dist = float.MaxValue;

        for (int i = 0; i < baseHeight.Length; i++)
        {
            if (Vector3.Distance(pos, baseHeight[i]) < dist)
            {
                dist = Vector3.Distance(pos, baseHeight[i]);
                output = i;
            }
        }

        return output;
    }

    void UpdateKey(int key, int newKey)
    {
        GameObject val = floatingObjs[key];
        floatingObjs.Remove(key);
        floatingObjs.Add(newKey, val);
    }
}
