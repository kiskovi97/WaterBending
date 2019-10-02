using Assets.Scripts.GPUBased;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCube : MonoBehaviour
{
    public MarchingCubeShader shader;
    public Texture2D texture;
    public float target = 0.5f;
    private float[] matrix;
    int size;

    // Start is called before the first frame update
    void Start()
    {
        if (shader == null)
        {
            shader = GetComponent<MarchingCubeShader>();
        }
        size = MarchingCubeParameters.MatrixSize;
        matrix = new float[size * size * size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    //matrix[GetIndex(i, j, k)] =  0.5f - j / (float) size;
                    matrix[GetIndex(i, j, k)] = Noise(5f,1f,i / (float)size ,j / (float)size, k / (float)size);
                    //matrix[GetIndex(i, j, k)] += Noise(2f,0.5f,i / (float)size ,j / (float)size, k / (float)size);
                }
            }
        }
    }

    private float Noise(float smooth, float max, float i, float j, float k)
    {
        float ab = Mathf.PerlinNoise(i * smooth, j * smooth);
        float bc = Mathf.PerlinNoise(j * smooth, k * smooth);
        float ac = Mathf.PerlinNoise(i * smooth, k * smooth);
        float ba = Mathf.PerlinNoise(j * smooth, i * smooth);
        float cb = Mathf.PerlinNoise(k * smooth, j * smooth);
        float ca = Mathf.PerlinNoise(k * smooth, i * smooth);
        return (ab + bc + ac + ba+ cb + ca) / 6f * max;
    }

    private int GetIndex(int i, int j, int k)
    {
        return i + j * size + k * size * size;
    }

    int offset = 0;

    // Update is called once per frame
    void Update()
    {
        if (offset > 10)
        {
            MarchingCubeParameters.Target = target;
            shader.SetInput(matrix);
            offset = 0;
        }
        offset++;
    }
}
