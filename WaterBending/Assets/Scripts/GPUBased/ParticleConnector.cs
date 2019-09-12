using Assets.Scripts.GPUBased;
using UnityEngine;

public class ParticleConnector : MonoBehaviour
{
    private readonly float scale = 1f;
    private readonly float multiple = 0.8f;
    public MarchingCubeShader marchingCube;
    
    private ParticlePhysicsSystem particlePhysicsSystem;
    

    private static int size;
    private float[] matrix;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        size = MarchingCubeParameters.MatrixSize;
        particlePhysicsSystem = new ParticlePhysicsSystem(offset);
        matrix = new float[size * size * size];
        
        if (marchingCube == null)
        {
            marchingCube = GetComponent<MarchingCubeShader>();
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        offset = transform.position;

        particlePhysicsSystem.Step(Time.deltaTime, offset);
        
        Generate();

        marchingCube.SetInput(matrix);
    }

    int GetIndex(int i, int j, int k)
    {
        return i + j * size + k * size * size;
    }

    void Generate()
    {
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                for (int k = 0; k < size; k++)
                {
                    matrix[GetIndex(i, j, k)] = 0f;
                }
        foreach(var particle in particlePhysicsSystem.Particles)
        {
            AddPoint(particle.Position + offset, particle.Direction);
        }
        //for (int i = 0; i < numParticlesAlive; i++)
        //{
        //    AddPoint(particles[i].position + offset);
        //}
    }

    private Vector3 prev = new Vector3(0.3f, 0, 0);
    private Vector3 prev1 = new Vector3(0, 0.3f, 0);
    private Vector3 prev2 = new Vector3(0, 0, 0.3f);

    private void AddPoint(Vector3 inpoint, Vector3 direction)
    {
        Debug.DrawLine(inpoint + direction.normalized*0.5f, inpoint, Color.red);
        //return;
        var point = inpoint * scale;

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
        matrix[GetIndex(i1, j1, k1)] += Mathf.Sqrt(x1 * x1 + y1 * y1 + z1 * z1) * multiple;
        matrix[GetIndex(i2, j1, k1)] += Mathf.Sqrt(x2 * x2 + y1 * y1 + z1 * z1) * multiple;
        matrix[GetIndex(i1, j2, k1)] += Mathf.Sqrt(x1 * x1 + y2 * y2 + z1 * z1) * multiple;
        matrix[GetIndex(i2, j2, k1)] += Mathf.Sqrt(x2 * x2 + y2 * y2 + z1 * z1) * multiple;
        matrix[GetIndex(i1, j1, k2)] += Mathf.Sqrt(x1 * x1 + y1 * y1 + z2 * z2) * multiple;
        matrix[GetIndex(i2, j1, k2)] += Mathf.Sqrt(x2 * x2 + y1 * y1 + z2 * z2) * multiple;
        matrix[GetIndex(i1, j2, k2)] += Mathf.Sqrt(x1 * x1 + y2 * y2 + z2 * z2) * multiple;
        matrix[GetIndex(i2, j2, k2)] += Mathf.Sqrt(x2 * x2 + y2 * y2 + z2 * z2) * multiple;
    }
}
