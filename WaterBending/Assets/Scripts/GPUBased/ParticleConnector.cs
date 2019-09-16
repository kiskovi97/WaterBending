using Assets.Scripts.GPUBased;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleConnector : MonoBehaviour
{
    private readonly float scale = 1f;
    private readonly float multiple = 2f;
    public MarchingCubeShader marchingCube;
    public BoidParticleShader particleShader;

    private ParticlePhysicsSystem particlePhysicsSystem;
    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    public bool UseParticleSystem = true;


    private static int size;
    private float[] matrix;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        size = MarchingCubeParameters.MatrixSize;
        particlePhysicsSystem = new ParticlePhysicsSystem(offset, particleShader);
        matrix = new float[size * size * size];

        if (marchingCube == null)
        {
            marchingCube = GetComponent<MarchingCubeShader>();
        }

        particleSystem = GetComponent<ParticleSystem>();

        InitializeIfNeeded();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        offset = transform.localPosition;

        if (UseParticleSystem && particleSystem != null)
        {
            int numParticlesAlive = particleSystem.GetParticles(particles);
            Generate(numParticlesAlive);
        } else
        {
            particlePhysicsSystem.Step(Time.deltaTime, offset);
            Generate(0);
        }

        marchingCube.SetInput(matrix);
    }

    int GetIndex(int i, int j, int k)
    {
        return i + j * size + k * size * size;
    }

    void Generate(int numParticlesAlive)
    {
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                for (int k = 0; k < size; k++)
                {
                    matrix[GetIndex(i, j, k)] = 0f;
                }
        if (UseParticleSystem && particleSystem != null)
        {
            for (int i = 0; i < numParticlesAlive; i++)
            {
                var position = particles[i].position;
                position = transform.InverseTransformPoint(position) + transform.localPosition;
                AddPoint(position * size + Vector3.one * size / 2f, Vector3.up);
            }
        } else
        {
            foreach (var particle in particlePhysicsSystem.Particles)
            {
                AddPoint(particle.Position + offset, particle.Direction);
            }
        }
    }

    private void AddPoint(Vector3 inpoint, Vector3 direction)
    {
        Debug.DrawLine(inpoint / size + direction.normalized * 0.2f, inpoint / size, Color.red);
        //return;
        var point = inpoint * scale;

        var indexes = GetCloseIndexes(point);

        foreach(var index in indexes)
        {
            //Debug.DrawLine(index, inpoint, Color.cyan);
            var distance = (point - index).magnitude;
            
            if (distance < MarchingCubeParameters.radius && IndexIsOkay(index))
            {
                matrix[GetIndex((int)index.x, (int)index.y, (int)index.z)] += (1 - (distance / MarchingCubeParameters.radius)) * multiple;
            }
        }
    }

    private bool IndexIsOkay(Vector3 point)
    {
        return point.x >= 0 && point.x < size 
            && point.y >= 0 && point.y < size
            && point.z >= 0 && point.z < size;
    }

    private Vector3[] GetCloseIndexes(Vector3 point)
    {
        var radius = MarchingCubeParameters.radius;
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

    void InitializeIfNeeded()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }
}
