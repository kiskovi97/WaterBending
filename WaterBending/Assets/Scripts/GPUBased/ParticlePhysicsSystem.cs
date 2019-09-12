using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GPUBased
{
    class ParticlePhysicsSystem
    {
        private static readonly int NumberOfParticles = 50;
        private static readonly float speed = 2f;

        private readonly List<Particle> myParticles = new List<Particle>();

        private Vector3 offset;

        public IEnumerable<Particle> Particles
        {
            get
            {
                return myParticles.Select(
                    (particle) => new Particle() { Position = particle.Position - offset, Direction = particle.Direction }
                );
            }
        }

        public ParticlePhysicsSystem(Vector3 offset)
        {
            this.offset = offset;
            for (int i = 0; i < NumberOfParticles; i++)
            {
                var dir = Random.rotation * Vector3.forward * 5f;
                var x = Random.Range(0, MarchingCubeParameters.MatrixSize);
                var y = Random.Range(0, MarchingCubeParameters.MatrixSize);
                var z = Random.Range(0, MarchingCubeParameters.MatrixSize);
                var pos = new Vector3(x, y, z);
                myParticles.Add(new Particle() { Position = pos, Direction = dir });
            }
        }

        public void Step(float time, Vector3 offset)
        {
            this.offset = offset;
            foreach (var particle in myParticles)
            {
                particle.Position += particle.Direction * time * speed;
                if (particle.Position.x > MarchingCubeParameters.MatrixSize)
                {
                    particle.Position -= new Vector3(MarchingCubeParameters.MatrixSize, 0, 0);
                }
                if (particle.Position.x < 0)
                {
                    particle.Position += new Vector3(MarchingCubeParameters.MatrixSize, 0, 0);
                }
                if (particle.Position.y > MarchingCubeParameters.MatrixSize)
                {
                    particle.Position -= new Vector3(0, MarchingCubeParameters.MatrixSize, 0);
                }
                if (particle.Position.y < 0)
                {
                    particle.Position += new Vector3(0, MarchingCubeParameters.MatrixSize, 0);
                }
                if (particle.Position.z > MarchingCubeParameters.MatrixSize)
                {
                    particle.Position -= new Vector3(0, 0, MarchingCubeParameters.MatrixSize);
                }
                if (particle.Position.z < 0)
                {
                    particle.Position += new Vector3(0, 0, MarchingCubeParameters.MatrixSize);
                }
            }
        }
    }
}
