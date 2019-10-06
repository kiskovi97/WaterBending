using Assets.Scripts.GPUBased;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainCube : MonoBehaviour
{
    public MarchingCubeShader shader;
    public float target = 0.5f;
    private float[] matrix;
    public bool Moving = false;
    int size;

    // Start is called before the first frame update
    void Start()
    {
        if (shader == null)
        {
            shader = GetComponent<MarchingCubeShader>();
        }
        
        size = shader.parameters.MatrixMultiplyer * 8;
        matrix = new float[size * size * size];
    }

    public Vector3 realOffset = new Vector3();
    private void Generate()
    {
        var offset = new Vector3();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    offset.x = (i / (float)size);
                    offset.y = (j / (float)size);
                    offset.z = (k / (float)size);
                    var noise = 0.4f - offset.y - transform.position.y;
                    noise *= 0.6f;
                    noise += PerlinNoise(1f, 1.8f, offset - realOffset + transform.position);
                    noise -= PerlinNoise(4f, 0.2f, offset - realOffset + transform.position);
                    noise += PerlinNoise(10f, 0.05f, offset - realOffset + transform.position);
                    noise += PerlinNoise(2f, 0.3f, offset - realOffset + transform.position);
                    noise -= PerlinNoise(1.5f, 0.8f, offset - realOffset + transform.position);
                    matrix[GetIndex(i, j, k)] = noise;
                }
            }
        }
    }

    private float PerlinNoise(float smooth, float max, Vector3 point)
    {
        return PerlinNoise(smooth, max, point.x, point.y, point.z);
    }

    private float PerlinNoise(float smooth, float max, float i, float j, float k)
    {
        float ab = Mathf.PerlinNoise(i * smooth, j * smooth);
        float bc = Mathf.PerlinNoise(j * smooth, k * smooth);
        float ac = Mathf.PerlinNoise(i * smooth, k * smooth);
        float ba = Mathf.PerlinNoise(j * smooth, i * smooth);
        float cb = Mathf.PerlinNoise(k * smooth, j * smooth);
        float ca = Mathf.PerlinNoise(k * smooth, i * smooth);
        return (ab + bc + ac + ba + cb + ca) / 6f * max;
    }

    private int GetIndex(int i, int j, int k)
    {
        return i + j * size + k * size * size;
    }

    bool first = true;
    // Update is called once per frame
    void Update()
    {
        if (first || Moving)
        {
            realOffset += Vector3.left * Time.deltaTime;
            Generate();
            shader.parameters.Target = target;
            shader.SetInput(matrix);
            first = false;
        }
    }
}
