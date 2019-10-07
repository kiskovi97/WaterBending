using Assets.Scripts.GPUBased;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class TerrainCube : MonoBehaviour
{
    public MarchingCubeShader shader;
    public TerrainCubes cubes;
    public float progress = 0.0f;
    public Slider slider;
    public float target = 0.5f;
    private float[] matrix;
    public bool Moving = false;

    public float ground = 0.4f;
    public float groundFactor = 0.6f;
    public Vector2[] perlings = new Vector2[5]
    {
        new Vector2( 1f, 1.8f ),new Vector2( 4f, -0.2f ),new Vector2( 10f, 0.05f ),new Vector2( 2f, 0.3f ),new Vector2( 1.5f, -0.8f )
    };

    int size;

    // Start is called before the first frame update
    public void Start()
    {
        first = true;
        if (shader == null)
        {
            shader = GetComponent<MarchingCubeShader>();
        }
        slider.gameObject.SetActive(true);
        size = shader.parameters.MatrixMultiplyer * 8;
        matrix = new float[size * size * size];
        if (shader.meshFilter != null)
        {
            if (shader.meshFilter.mesh != null)
            {
                shader.meshFilter.mesh.Clear();
            }
        }
    }

    public Vector3 realOffset = new Vector3();
    private IEnumerator Generate()
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
                    var noise = ground - offset.y - transform.position.y;
                    noise *= groundFactor;

                    for (int x=0; x < perlings.Length;x++)
                    {
                        noise += PerlinNoise(perlings[x].x, perlings[x].y, offset - realOffset + transform.position);
                    }

                    /*noise += PerlinNoise(1f, 1.8f, offset - realOffset + transform.position);
                    noise += PerlinNoise(4f, -0.2f, offset - realOffset + transform.position);
                    noise += PerlinNoise(10f, 0.05f, offset - realOffset + transform.position);
                    noise += PerlinNoise(2f, 0.3f, offset - realOffset + transform.position);
                    noise += PerlinNoise(1.5f, -0.8f, offset - realOffset + transform.position);*/
                    matrix[GetIndex(i, j, k)] = noise;
                    progress = i / (float)size + j / (float)(size * size) + k / (float)(size * size * size);
                    slider.value = progress;
                }
            }
            yield return null;
        }
        slider.gameObject.SetActive(false);
        shader.parameters.Target = target;
        shader.SetInput(matrix);
        Debug.Log("Cube Done");
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
            first = false;
            realOffset += Vector3.left * Time.deltaTime;
            StartCoroutine(Generate());
        }
        var position = cubes.player.position;
        if ((position - transform.position).magnitude > cubes.radius * 1.1f)
        {
            cubes.RemoveMe(gameObject);
        }
        if (shader.empty)
        {
            //cubes.RemoveMe(gameObject);
        }
    }
}
