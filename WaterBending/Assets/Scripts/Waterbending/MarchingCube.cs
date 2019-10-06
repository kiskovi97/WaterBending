using Assets.Scripts.GPUBased;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MarchingCube
{
    private float[] matrix;

    public MarchingCubeShader shader;

    private Transform transform;

    private bool pure = true;

    private int size;

    public Vector3 position { get { return shader.transform.position; } }

    public MarchingCube(int size, MarchingCubeShader shader)
    {
        this.size = size;
        this.shader = shader;
        transform = shader.transform;
        matrix = new float[size * size * size];
        ResetMatrix();
    }

    public void Generate(int numParticlesAlive, ParticleSystem.Particle[] particles)
    {
        var transformMatrix = transform.worldToLocalMatrix;
        var offset = transform.position;
        if (!pure)
        {
            ResetMatrix();
        }
        for (int i = 0; i < numParticlesAlive; i++)
        {
            var position = particles[i].position;
            position -= offset;
            position = transformMatrix * position;
            AddPoint(position * size + Vector3.one * size / 2f, Vector3.up);
        }
    }

    public void SetMatrix()
    {
        if (!pure)
        {
            shader.SetInput(matrix);
        }
    }

    public void Shift(Vector3 offset)
    {
        transform.position += offset;
        shader.meshFilter.mesh.Clear();
    }

    private void ResetMatrix()
    {
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                for (int k = 0; k < size; k++)
                {
                    matrix[GetIndex(i, j, k)] = 0f;
                }
        pure = true;
    }

    private int GetIndex(int i, int j, int k)
    {
        return i + j * size + k * size * size;
    }

    private void AddPoint(Vector3 inpoint, Vector3 direction)
    {
        var point = inpoint;
        var indexes = GetCloseIndexes(point);
        foreach (var index in indexes)
        {
            if (IndexIsOkay(index))
            {
                var distance = (point - index).magnitude;

                if (distance < shader.parameters.radius)
                {
                    matrix[GetIndex((int)index.x, (int)index.y, (int)index.z)] += (1 - (distance / shader.parameters.radius));
                    pure = false;
                }
            }
        }
    }

    private Vector3[] GetCloseIndexes(Vector3 point)
    {
        var radius = shader.parameters.radius;
        int minX = Mathf.RoundToInt(point.x - radius);
        int minY = Mathf.RoundToInt(point.y - radius);
        int minZ = Mathf.RoundToInt(point.z - radius);

        int maxX = Mathf.RoundToInt(point.x + radius);
        int maxY = Mathf.RoundToInt(point.y + radius);
        int maxZ = Mathf.RoundToInt(point.z + radius);
        var list = new List<Vector3>();
        for (int i = minX; i <= maxX; i++)
            for (int j = minY; j <= maxY; j++)
                for (int k = minZ; k <= maxZ; k++)
                {
                    list.Add(new Vector3(i, j, k));
                }
        return list.ToArray();
    }

    private bool IndexIsOkay(Vector3 point)
    {
        return point.x >= 0 && point.x < size
            && point.y >= 0 && point.y < size
            && point.z >= 0 && point.z < size;
    }
}
