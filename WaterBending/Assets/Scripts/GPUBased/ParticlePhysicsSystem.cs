using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GPUBased
{
    class ParticlePhysicsSystem
    {
        private static readonly int NumberOfParticles = 100;
        private BoidParticleShader particleShader;

        private Vector3 offset;

        public IEnumerable<Particle> Particles
        {
            get
            {
                return particleShader.particleList.Select(
                    (particle) => new Particle() { Position = particle.Position - offset, Direction = particle.Direction }
                );
            }
        }

        public ParticlePhysicsSystem(Vector3 offset, BoidParticleShader particleShader)
        {
            this.particleShader = particleShader;
            this.offset = offset;
            var parts = new List<Particle>();
            for (int i = 0; i < NumberOfParticles; i++)
            {
                var dir = Random.rotation * Vector3.forward * 5f;
                var x = Random.Range(0, MarchingCubeParameters.MatrixSize);
                var y = Random.Range(0, MarchingCubeParameters.MatrixSize);
                var z = Random.Range(0, MarchingCubeParameters.MatrixSize);
                var pos = new Vector3(x, y, z);
                parts.Add(new Particle() { Position = pos, Direction = dir });
            }
            particleShader.SetInput(parts.ToArray());
        }

        public void Step(float time, Vector3 offset)
        {
            this.offset = offset;
        }
    }
}
