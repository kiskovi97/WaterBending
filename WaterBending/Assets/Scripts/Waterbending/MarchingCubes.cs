using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GPUBased
{
    public class CubeCollection
    {
        public IEnumerable<MarchingCube> Cubes { get
            {
                return internalCubes;
            }
        }
        private MarchingCube[] internalCubes { get; set; }
        public Vector3 position { get { return internalCubes[0].position;  } }

        public float cubeSize = 20;
        public int matrixSize = 16;

        static readonly Vector3[] offsets = new Vector3[]
        {
            new Vector3(), Vector3.left , Vector3.right ,Vector3.up,Vector3.down ,Vector3.forward ,Vector3.back ,
             
            Vector3.left * 2, Vector3.right *2,
            Vector3.right + Vector3.down,Vector3.right + Vector3.up,Vector3.right + Vector3.forward,Vector3.right + Vector3.back,

            Vector3.left + Vector3.down, Vector3.up * 2,
            Vector3.up + Vector3.forward,Vector3.up + Vector3.back,
            Vector3.left + Vector3.up, Vector3.down *2,
            Vector3.down + Vector3.forward,Vector3.down + Vector3.back,

            Vector3.left + Vector3.forward,Vector3.forward *2,
            Vector3.left + Vector3.back,Vector3.back *2,
        };

        static int C = 0;
        static int L = 1;
        static int R = 2;
        static int U = 3;
        static int D = 4;
        static int F = 5;
        static int B = 6;

        static int LL = 7;
        static int LD = 13;
        static int DL = 13;
        static int LU = 17;
        static int UL = 17;
        static int LF = 21;
        static int FL = 21;
        static int LB = 23;
        static int BL = 23;

        static int RR = 8;
        static int RD = 9;
        static int DR = 9;
        static int RU = 10;
        static int UR = 10;
        static int RF = 11;
        static int FR = 11;
        static int RB = 12;
        static int BR = 12;

        static int UU = 14;
        static int UF = 15;
        static int FU = 15;
        static int UB = 16;
        static int BU = 16;

        static int DD = 18;
        static int DF = 19;
        static int FD = 19;
        static int DB = 20;
        static int BD = 20;

        static int FF = 22;
        static int BB = 24;

        public void Init(GameObject template, Vector3 position)
        {
            internalCubes = new MarchingCube[offsets.Length];
            for (int i = 0; i < offsets.Length; i++)
            {
                var obj = Object.Instantiate(template, position + offsets[i] * cubeSize, new Quaternion());
                var marchingCubeShader = obj.GetComponent<MarchingCubeShader>();
                internalCubes[i] = new MarchingCube(matrixSize, marchingCubeShader);
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
            var firstCube = internalCubes[firstIndex];
            for (int i = 0; i < array.Length - 1; i++)
            {
                var prevIndex = array[i];
                var nextIndex = array[i + 1];
                internalCubes[prevIndex] = internalCubes[nextIndex];
            }
            var lastIndex = array[array.Length - 1];
            firstCube.Shift(offset * cubeSize * array.Length);
            internalCubes[lastIndex] = firstCube;
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

        private float radius = 1.5f;

        private void Start()
        {
            var parameters = marchingCubesObject.GetComponent<MarchingCubeShader>().parameters;
            size = parameters.MatrixSize;
            correction = (size - 1f) / (size);

            if (particles == null || particles.Length < particleSystem.main.maxParticles)
                particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

            cubeCollection = new CubeCollection
            {
                cubeSize = marchingCubesObject.transform.lossyScale.x * correction,
                matrixSize = size
            };
            cubeCollection.Init(marchingCubesObject, particleSystem.transform.position);
        }

        private void LateUpdate()
        {
            var dir = cubeCollection.position - particleSystem.transform.position;
            var scale = marchingCubesObject.transform.lossyScale;
            if (dir.x < -scale.x * correction / 2)
            {
                cubeCollection.MoveLeft();
            }
            if (dir.x > scale.x * correction / 2)
            {
                cubeCollection.MoveRight();
            }
            if (dir.y < -scale.y * correction / 2)
            {
                cubeCollection.MoveUp();
            }
            if (dir.y > scale.y * correction / 2)
            {
                cubeCollection.MoveDown();
            }
            if (dir.z < -scale.z * correction / 2)
            {
                cubeCollection.MoveBack();
            }
            if (dir.z > scale.z * correction / 2)
            {
                cubeCollection.MoveFront();
            }

            int numParticlesAlive = particleSystem.GetParticles(particles);
            int i = 0;
            foreach (var cube in cubeCollection.Cubes)
            {
                if (cube != null)
                {
                    cube.Generate(numParticlesAlive, particles);
                    cube.SetMatrix();
                }
            }
        }
    }
}