using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GPUBased
{
    public class MarchingCubes : MonoBehaviour
    {
        private MarchingCubeShader[] marchingCubes;
        public GameObject marchingCubesObject;

        public new ParticleSystem particleSystem;

        private ParticleSystem.Particle[] particles;

        private static int size;
        private float[] matrix;

        private void Start()
        {
            size = MarchingCubeParameters.MatrixSize;
            matrix = new float[size * size * size];

            var shaders = new List<MarchingCubeShader>();

            if (particles == null || particles.Length < particleSystem.main.maxParticles)
                particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

            for (int i=0; i<5; i++) 
                for (int j=0; j<5; j++)
                    for (int k = 0; k < 5; k++)
                    {
                    var obj = Instantiate(marchingCubesObject);
                    var cube = obj.GetComponent<MarchingCubeShader>();
                    shaders.Add(cube);
                    var size = MarchingCubeParameters.MatrixSize;
                    var correction = ((size - 1f) / size);
                    obj.transform.position = new Vector3(i * obj.transform.lossyScale.x * correction, 
                        j * obj.transform.lossyScale.y * correction, 
                        k * obj.transform.lossyScale.z * correction);
                }
            marchingCubes = shaders.ToArray();
        }

        private void LateUpdate()
        {
            int numParticlesAlive = particleSystem.GetParticles(particles);
            var cubes = GetShader(particleSystem.transform.position);
            foreach (var cube in cubes)
            {
                if (cube != null)
                {
                    Generate(numParticlesAlive, cube.transform);
                    cube.SetInput(matrix);
                }
            }

        }

        private MarchingCubeShader[] GetShader(Vector3 position)
        {
            var list = new List<MarchingCubeShader>();
            foreach (MarchingCubeShader cube in marchingCubes)
            {
                var pos = cube.transform.position;
                var scale = cube.transform.lossyScale;
                if (IsInCube(scale, position - pos))
                {
                    list.Add(cube);
                }

            }
            return list.ToArray();
        }

        private float radius = 1f;

        public bool IsInCube(Vector3 scale, Vector3 pos)
        {
            var a = Mathf.Abs(pos.x) < scale.x * radius;
            var b = Mathf.Abs(pos.y) < scale.y * radius;
            var c = Mathf.Abs(pos.z) < scale.z * radius;
            return a && b && c;
        }

        int GetIndex(int i, int j, int k)
        {
            return i + j * size + k * size * size;
        }

        void Generate(int numParticlesAlive, Transform cubeTransform)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int k = 0; k < size; k++)
                    {
                        matrix[GetIndex(i, j, k)] = 0f;
                    }
            for (int i = 0; i < numParticlesAlive; i++)
            {
                var position = particles[i].position;
                position = cubeTransform.InverseTransformPoint(position);
                AddPoint(position * size + Vector3.one * size / 2f, Vector3.up);
            }
        }

        private void AddPoint(Vector3 inpoint, Vector3 direction)
        {
            Debug.DrawLine(inpoint / size + direction.normalized * 0.2f, inpoint / size, Color.red);
            //return;
            var point = inpoint;

            var indexes = GetCloseIndexes(point);

            foreach (var index in indexes)
            {
                //Debug.DrawLine(index, inpoint, Color.cyan);
                var distance = (point - index).magnitude;

                if (distance < MarchingCubeParameters.radius && IndexIsOkay(index))
                {
                    matrix[GetIndex((int)index.x, (int)index.y, (int)index.z)] += (1 - (distance / MarchingCubeParameters.radius));
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
    }
}