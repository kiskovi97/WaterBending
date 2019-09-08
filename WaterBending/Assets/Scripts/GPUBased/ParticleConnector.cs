using Assets.Scripts.GPUBased;
using UnityEngine;

public class ParticleConnector : MonoBehaviour
{

    public MarchingCubeShader marchingCube;

    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    private static readonly int size = MarchingCubeParameters.MatrixSize;
    private float[] matrix = new float[size * size * size];
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        InitializeIfNeeded();
        if (marchingCube == null)
        {
            marchingCube = GetComponent<MarchingCubeShader>();
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        offset = transform.position;
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = particleSystem.GetParticles(particles);

        Generate(numParticlesAlive);

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
                    matrix[GetIndex(i, j, k)] = -1;
                }
        for (int i = 0; i < numParticlesAlive; i++)
        {
            AddPoint(particles[i].position + offset);
        }
    }

    private void AddPoint(Vector3 inpoint)
    {
        var point = inpoint / CubeInformation.size;

        int i1 = (int)point.x;
        int i2 = (int)point.x + 1;
        float x2 = i2 - point.x;
        float x1 = point.x - i1;

        int j1 = (int)point.y;
        int j2 = (int)point.y + 1;
        float y2 = j2 - point.y;
        float y1 = point.y - j1;

        int k1 = (int)point.z;
        int k2 = (int)point.z + 1;
        float z2 = k2 - point.z;
        float z1 = point.z - k1;

        if (i1 >= size || i2 >= size || j1 >= size || j2 >= size || k1 >= size || k2 >= size)
        {
            return;
        }
        if (i1 < 0 || i2 < 0 || j1 < 0 || j2 < 0 || k1 < 0 || k2 < 0)
        {
            return;
        }
        var multiple = 1f;
        matrix[GetIndex(i1, j1, k1)] += Mathf.Sqrt(x1 * x1 + y1 * y1 + z1 * z1) * multiple;
        matrix[GetIndex(i2, j1, k1)] += Mathf.Sqrt(x2 * x2 + y1 * y1 + z1 * z1) * multiple;
        matrix[GetIndex(i1, j2, k1)] += Mathf.Sqrt(x1 * x1 + y2 * y2 + z1 * z1) * multiple;
        matrix[GetIndex(i2, j2, k1)] += Mathf.Sqrt(x2 * x2 + y2 * y2 + z1 * z1) * multiple;
        matrix[GetIndex(i1, j1, k2)] += Mathf.Sqrt(x1 * x1 + y1 * y1 + z2 * z2) * multiple;
        matrix[GetIndex(i2, j1, k2)] += Mathf.Sqrt(x2 * x2 + y1 * y1 + z2 * z2) * multiple;
        matrix[GetIndex(i1, j2, k2)] += Mathf.Sqrt(x1 * x1 + y2 * y2 + z2 * z2) * multiple;
        matrix[GetIndex(i2, j2, k2)] += Mathf.Sqrt(x2 * x2 + y2 * y2 + z2 * z2) * multiple;
    }

    void InitializeIfNeeded()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }
}
