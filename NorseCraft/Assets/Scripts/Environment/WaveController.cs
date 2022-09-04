using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WaveController : MonoBehaviour
{
    [Header("Parameters")]
    public float scale = 0.5f;
    public float speed = 0.5f;
    public float noiseStrength = 0f;
    public float noiseWalk = 1f;

    [Header("Offsets")]
    public float offsetX;
    public float offsetZ;

    [Header("Data")]
    public float width;
    public float length;

    private Vector3[] baseHeight;

    private Mesh mesh;

    [Space(20)]
    public float maxHeight = 0.5f;
    public float minHeight = 0.0f;
    public float raiseLowerRate = 0.1f;
    private bool raising;

	// Use this for initialization
	void Start ()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        width = mesh.bounds.size.x;
        length = mesh.bounds.size.z;
        offsetX = transform.position.x;
        offsetZ = transform.position.z;
        raising = true;
	}
 
    void Update()
    {
        //MoveWithPlayer();
        //SineWaveGenerator();
        RaiseAndLower();
    }

    private void RaiseAndLower()
    {
        float height = transform.position.y;
        if (raising)
        {
            height += raiseLowerRate * Time.deltaTime;
            if(height >= maxHeight)
                raising = false;
        }
        else
        {
            height -= raiseLowerRate * Time.deltaTime;
            if (height <= minHeight)
                raising = true;
        }
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }

    private void MoveWithPlayer()
    {
        if(GameManager.manager != null)
        {
            if(GameManager.manager.player != null)
            {
                GameObject player = GameManager.manager.player;
                transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            }
        }
    }

    private void SineWaveGenerator()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        if (baseHeight == null)
            baseHeight = mesh.vertices;

        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    private void PerlinNoiseGenerator()
    {
        if (baseHeight == null)
            baseHeight = mesh.vertices;

        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            var vertex = baseHeight[i];
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    /*
     * Calculate height of given coordinate
     */
    private float CalculateHeight(int x, int z)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float zCoord = (float)z / length * scale + offsetZ;

        return Mathf.PerlinNoise(xCoord, zCoord);
    }
}
