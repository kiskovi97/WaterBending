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
        private static readonly float realRadius = 6;
        private static readonly float radius = realRadius * realRadius;

        private readonly Particle[] myParticles = new Particle[NumberOfParticles];
        private readonly Particle[] myParticlesOther = new Particle[NumberOfParticles];

        private bool other = false;

        private Particle[] actualP {
            get
            {
                if (other)
                {
                    return myParticlesOther;
                } else
                {
                    return myParticles;
                }
            }
        }
        private Particle[] otherP
        {
            get
            {
                if (other)
                {
                    return myParticles;
                }
                else
                {
                    return myParticlesOther;
                }
            }
        }

        private void Flip()
        {
            other = !other;
        }

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
            myParticles = parts.ToArray();
            myParticlesOther = parts.ToArray();
        }

        public void Step(float time, Vector3 offset)
        {
            Flip();
            this.offset = offset;
            for (int i = 0; i < NumberOfParticles; i++)
            {

                actualP[i] = new Particle
                {
                    Position = otherP[i].Position + otherP[i].Direction * time * speed,
                    Direction = otherP[i].Direction
                };
                var force = ALignment(otherP, otherP[i].Position) * 3;
                force += Cohesion(otherP, otherP[i].Position) * 2;
                force += Separation(otherP, otherP[i].Position) * 2;
                actualP[i].Position = OnEdge(actualP[i]) + actualP[i].Position;
                actualP[i].Direction = (force * 0.5f * time * speed + actualP[i].Direction).normalized * speed;
            }
        }

        private Vector3 ALignment(Particle[] particles, Vector3 position)
        {
            var direction = new Vector3(0, 0, 0);

            foreach (var particle in particles)
            {
                if ((particle.Position - position).sqrMagnitude < radius)
                {
                    direction += particle.Direction;
                }
            }
            return direction.normalized;
        }

        private Vector3 Cohesion(Particle[] particles, Vector3 position)
        {
            var center = new Vector3(0, 0, 0);
            var count = 0;

            foreach (var particle in particles)
            {
                if ((particle.Position - position).sqrMagnitude < radius)
                {
                    count++;
                    center += particle.Position;
                }
            }

            center = center / count;
            return  center - position;
        }

        private Vector3 Separation(Particle[] particles, Vector3 position)
        {
            var force = new Vector3(0, 0, 0);
            var count = 0;

            foreach (var particle in particles)
            {
                var distance = (particle.Position - position).sqrMagnitude;
                if (distance < realRadius)
                {
                    count++;
                    force += (position - particle.Position) * (realRadius - distance);
                }
            }
            
            return  force;
        }

        private Vector3 OnEdge(Particle particle)
        {
            var x = 0;
            var y = 0;
            var z = 0;
            if (particle.Position.x > MarchingCubeParameters.MatrixSize)
            {
                x -= MarchingCubeParameters.MatrixSize;
            }
            if (particle.Position.x < 0)
            {
                x += MarchingCubeParameters.MatrixSize;
            }
            if (particle.Position.y > MarchingCubeParameters.MatrixSize)
            {
                y -= MarchingCubeParameters.MatrixSize;
            }
            if (particle.Position.y < 0)
            {
                y += MarchingCubeParameters.MatrixSize;
            }
            if (particle.Position.z > MarchingCubeParameters.MatrixSize)
            {
                z -= MarchingCubeParameters.MatrixSize;
            }
            if (particle.Position.z < 0)
            {
                z += MarchingCubeParameters.MatrixSize;
            }
            return new Vector3(x, y, z);
        }
    }
}
