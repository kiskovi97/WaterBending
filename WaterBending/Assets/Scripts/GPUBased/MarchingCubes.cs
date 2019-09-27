using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GPUBased
{
    public class CubeCollection
    {
        public MarchingCubeShader[] Cubes { get; internal set; }
        public float size = 20;

        static readonly Vector3[] offsets = new Vector3[]
        {
            new Vector3(), Vector3.left , Vector3.left *2,
            Vector3.left + Vector3.down, Vector3.left + Vector3.up, Vector3.left + Vector3.forward, Vector3.left + Vector3.back,
            Vector3.right , Vector3.right *2,
            Vector3.right + Vector3.down,Vector3.right + Vector3.up,Vector3.right + Vector3.forward,Vector3.right + Vector3.back,

            Vector3.up , Vector3.up *2,
            Vector3.up + Vector3.forward,Vector3.up + Vector3.back,
            Vector3.down , Vector3.down *2,
            Vector3.down + Vector3.forward,Vector3.down + Vector3.back,

            Vector3.forward , Vector3.forward *2,
            Vector3.back , Vector3.back *2,
        };

        static int C = 0;
        static int L = 1;
        static int LL = 2;
        static int LD = 3;
        static int DL = 3;
        static int LU = 4;
        static int UL = 4;
        static int LF = 5;
        static int FL = 5;
        static int LB = 6;
        static int BL = 6;

        static int R = 7;
        static int RR = 8;
        static int RD = 9;
        static int DR = 9;
        static int RU = 10;
        static int UR = 10;
        static int RF = 11;
        static int FR = 11;
        static int RB = 12;
        static int BR = 12;

        static int U = 13;
        static int UU = 14;
        static int UF = 15;
        static int FU = 15;
        static int UB = 16;
        static int BU = 16;

        static int D = 17;
        static int DD = 18;
        static int DF = 19;
        static int FD = 19;
        static int DB = 20;
        static int BD = 20;

        static int F = 21;
        static int FF = 22;
        static int B = 23;
        static int BB = 24;

        public void Init(GameObject template, Vector3 position)
        {
            Cubes = new MarchingCubeShader[offsets.Length];
            for (int i = 0; i < offsets.Length; i++)
            {
                var obj = Object.Instantiate(template, position + offsets[i] * size, new Quaternion());
                Cubes[i] = obj.GetComponent<MarchingCubeShader>();
            }

        }

        public void MoveLeft()
        {
            var offset = Vector3.right;
            ShiftArray(new int[] { LL, L, C, R, RR }, offset);
            ShiftArray(new int[] { LD, D, RD }, offset);
            ShiftArray(new int[] { LU, U, RU }, offset);
            ShiftArray(new int[] { LF, F, RF }, offset);
            ShiftArray(new int[] { LB, B, RB }, offset);

            ShiftArray(new int[] { FF }, offset);
            ShiftArray(new int[] { BB }, offset);
            ShiftArray(new int[] { UU }, offset);
            ShiftArray(new int[] { DD }, offset);

            ShiftArray(new int[] { UF }, offset);
            ShiftArray(new int[] { UB }, offset);
            ShiftArray(new int[] { DF }, offset);
            ShiftArray(new int[] { DB }, offset);
        }

        public void MoveRight()
        {
            var offset = Vector3.left;
            ShiftArray(new int[] { RR, R, C, L, LL }, offset);
            ShiftArray(new int[] { RD, D, LD }, offset);
            ShiftArray(new int[] { RU, U, LU }, offset);
            ShiftArray(new int[] { RF, F, LF }, offset);
            ShiftArray(new int[] { RB, B, LB }, offset);

            ShiftArray(new int[] { FF }, offset);
            ShiftArray(new int[] { BB }, offset);
            ShiftArray(new int[] { UU }, offset);
            ShiftArray(new int[] { DD }, offset);

            ShiftArray(new int[] { UF }, offset);
            ShiftArray(new int[] { UB }, offset);
            ShiftArray(new int[] { DF }, offset);
            ShiftArray(new int[] { DB }, offset);
        }

        public void MoveUp()
        {
            var offset = Vector3.up;
            ShiftArray(new int[] { DD, D, C, U, UU }, offset);
            ShiftArray(new int[] { DL, L, UL }, offset);
            ShiftArray(new int[] { DR, R, UR }, offset);
            ShiftArray(new int[] { DF, F, UF }, offset);
            ShiftArray(new int[] { DB, B, UB }, offset);

            ShiftArray(new int[] { FF }, offset);
            ShiftArray(new int[] { BB }, offset);
            ShiftArray(new int[] { RR }, offset);
            ShiftArray(new int[] { LL }, offset);

            ShiftArray(new int[] { RF }, offset);
            ShiftArray(new int[] { RB }, offset);
            ShiftArray(new int[] { LF }, offset);
            ShiftArray(new int[] { LB }, offset);
        }

        public void MoveDown()
        {
            var offset = Vector3.down;
            ShiftArray(new int[] { UU, U, C, D, DD }, offset);
            ShiftArray(new int[] { UL, L, DL }, offset);
            ShiftArray(new int[] { UR, R, DR }, offset);
            ShiftArray(new int[] { UF, F, DF }, offset);
            ShiftArray(new int[] { UB, B, DB }, offset);

            ShiftArray(new int[] { FF }, offset);
            ShiftArray(new int[] { BB }, offset);
            ShiftArray(new int[] { RR }, offset);
            ShiftArray(new int[] { LL }, offset);

            ShiftArray(new int[] { RF }, offset);
            ShiftArray(new int[] { RB }, offset);
            ShiftArray(new int[] { LF }, offset);
            ShiftArray(new int[] { LB }, offset);
        }

        public void MoveFront()
        {
            var offset = Vector3.back;
            ShiftArray(new int[] { FF, F, C, B, BB }, offset);
            ShiftArray(new int[] { FL, L, BL }, offset);
            ShiftArray(new int[] { FR, R, BR }, offset);
            ShiftArray(new int[] { FU, U, BU }, offset);
            ShiftArray(new int[] { FD, D, BD }, offset);

            ShiftArray(new int[] { UU }, offset);
            ShiftArray(new int[] { DD }, offset);
            ShiftArray(new int[] { RR }, offset);
            ShiftArray(new int[] { LL }, offset);

            ShiftArray(new int[] { RD }, offset);
            ShiftArray(new int[] { RU }, offset);
            ShiftArray(new int[] { LD }, offset);
            ShiftArray(new int[] { LU }, offset);
        }

        public void MoveBack()
        {
            var offset = Vector3.forward;
            ShiftArray(new int[] { BB, B, C, F, FF }, offset);
            ShiftArray(new int[] { BL, L, FL }, offset);
            ShiftArray(new int[] { BR, R, FR }, offset);
            ShiftArray(new int[] { BU, U, FU }, offset);
            ShiftArray(new int[] { BD, D, FD }, offset);

            ShiftArray(new int[] { UU }, offset);
            ShiftArray(new int[] { DD }, offset);
            ShiftArray(new int[] { RR }, offset);
            ShiftArray(new int[] { LL }, offset);

            ShiftArray(new int[] { RD }, offset);
            ShiftArray(new int[] { RU }, offset);
            ShiftArray(new int[] { LD }, offset);
            ShiftArray(new int[] { LU }, offset);
        }

        private void ShiftArray(int[] array, Vector3 offset)
        {
            var firstIndex = array[0];
            var firstCube = Cubes[firstIndex];
            for (int i = 0; i < array.Length - 1; i++)
            {
                var prevIndex = array[i];
                var nextIndex = array[i + 1];
                Cubes[prevIndex] = Cubes[nextIndex];
            }
            var lastIndex = array[array.Length - 1];
            firstCube.transform.position += offset * size * array.Length;
            firstCube.meshFilter.mesh.Clear();
            Cubes[lastIndex] = firstCube;
        }

    }

    public class MarchingCubes : MonoBehaviour
    {
        private CubeCollection cubeCollection;
        public GameObject marchingCubesObject;

        public new ParticleSystem particleSystem;

        private ParticleSystem.Particle[] particles;

        private static int size;
        private static float correction;
        private float[] matrix;

        private float radius = 1.5f;

        private void Start()
        {
            size = MarchingCubeParameters.MatrixSize;
            correction = (size - 1f) / (size);
            matrix = new float[size * size * size];

            if (particles == null || particles.Length < particleSystem.main.maxParticles)
                particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

            cubeCollection = new CubeCollection
            {
                size = marchingCubesObject.transform.lossyScale.x * correction
            };
            cubeCollection.Init(marchingCubesObject, particleSystem.transform.position);
        }

        private void LateUpdate()
        {
            if ((cubeCollection.Cubes[0].transform.position - particleSystem.transform.position).x < -marchingCubesObject.transform.lossyScale.x * correction / 2)
            {
                cubeCollection.MoveLeft();
            }
            if ((cubeCollection.Cubes[0].transform.position - particleSystem.transform.position).x > marchingCubesObject.transform.lossyScale.x * correction / 2)
            {
                cubeCollection.MoveRight();
            }
            if ((cubeCollection.Cubes[0].transform.position - particleSystem.transform.position).y < -marchingCubesObject.transform.lossyScale.y * correction / 2)
            {
                cubeCollection.MoveUp();
            }
            if ((cubeCollection.Cubes[0].transform.position - particleSystem.transform.position).y > marchingCubesObject.transform.lossyScale.y * correction / 2)
            {
                cubeCollection.MoveDown();
            }
            if ((cubeCollection.Cubes[0].transform.position - particleSystem.transform.position).z < -marchingCubesObject.transform.lossyScale.z * correction / 2)
            {
                cubeCollection.MoveBack();
            }
            if ((cubeCollection.Cubes[0].transform.position - particleSystem.transform.position).z > marchingCubesObject.transform.lossyScale.z * correction / 2)
            {
                cubeCollection.MoveFront();
            }
            int numParticlesAlive = particleSystem.GetParticles(particles);
            var cubes = GetShader(particleSystem.transform.position);
            foreach (var cube in cubes)
            {
                if (cube != null)
                {
                    if (!Generate(numParticlesAlive, cube.transform))
                    {
                        cube.SetInput(matrix);
                    }
                }
            }

        }

        private MarchingCubeShader[] GetShader(Vector3 position)
        {
            var list = new List<MarchingCubeShader>();
            foreach (MarchingCubeShader cube in cubeCollection.Cubes)
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

        bool Generate(int numParticlesAlive, Transform cubeTransform)
        {
            var pure = true;
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
                pure = AddPoint(position * size + Vector3.one * size / 2f, Vector3.up) && pure;
            }
            return pure;
        }

        private bool AddPoint(Vector3 inpoint, Vector3 direction)
        {
            var pure = true;
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
                    pure = false;
                }
            }
            return pure;
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