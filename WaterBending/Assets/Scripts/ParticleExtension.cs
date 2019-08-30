using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleExtension : GeneratedObject
{
    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    private MarchingCubes marchingCubes;

    // Start is called before the first frame update
    void Start()
    {
        InitializeIfNeeded();
        marchingCubes = new MarchingCubes(new Vector3(-10.2f, -10.2f, -5.2f));
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = particleSystem.GetParticles(particles);

        Generate(numParticlesAlive);
    }

    void Generate(int numParticlesAlive)
    {
        Clear();

        InitializeIfNeeded();
        var triangles = marchingCubes.GetTriangles(particles.Select((item) => transform.TransformPoint(item.position)).ToArray(), numParticlesAlive);
        foreach (var triangle in triangles)
        {
            AddTriangle(triangle);
        }
        CreateMesh();
    }

    void InitializeIfNeeded()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }
}
