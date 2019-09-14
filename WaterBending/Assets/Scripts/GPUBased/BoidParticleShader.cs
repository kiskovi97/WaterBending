using Assets.Scripts.GPUBased;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BoidParticleShader : MonoBehaviour
{
    public ComputeShader compute;

    public int particleCount = 0;

    public Particle[] particleList;

    ComputeBuffer particles;
    ComputeBuffer otherParticles;

    bool other = false;

    ComputeBuffer PartBuffer
    {
        get
        {
            if (other)
            {
                return otherParticles;
            }
            return particles;
        }
    }

    ComputeBuffer OtherPartBuffer
    {
        get
        {
            if (other)
            {
                return particles;
            }
            return otherParticles;
        }
    }

    public void SetInput(IEnumerable<Particle> inputList)
    {
        particleCount = inputList.Count();

        if (particleCount % 10 != 0)
            throw new System.ArgumentException("MatrixSize must be divisible be 8");

        particles = new ComputeBuffer(particleCount, sizeof(float) * 6);
        otherParticles = new ComputeBuffer(particleCount, sizeof(float) * 6);

        particleList = new Particle[particleCount];

        PartBuffer.SetData(inputList.ToArray());
        OtherPartBuffer.SetData(inputList.ToArray());
    }

    void Update()
    {
        ComputeStepFrame();
        other = !other;
    }

    private void ComputeStepFrame()
    {
        if (particles != null)
        {
            int kernelHandle = compute.FindKernel("CSMain");

            compute.SetBuffer(kernelHandle, "PartBuffer", PartBuffer);
            compute.SetBuffer(kernelHandle, "OtherPartBuffer", OtherPartBuffer);
            compute.SetInt("ParticleCount", particleCount);
            compute.SetFloat("deltaTime", Time.deltaTime);
            compute.SetFloat("size", MarchingCubeParameters.MatrixSize);

            compute.Dispatch(kernelHandle, particleCount / 10, 1, 1);
            PartBuffer.GetData(particleList);
        }
    }

}
