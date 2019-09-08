using Assets.Scripts.GPUBased;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderUser : MonoBehaviour
{
    private static readonly int trianglePerBox = 5; //5;
    private static readonly int MatrixSize = 16;
    private static readonly int BufferSize = MatrixSize * MatrixSize * MatrixSize;
    private static readonly int TriangleCount = MatrixSize * MatrixSize * MatrixSize * trianglePerBox;
    private static readonly int VertexCount = TriangleCount * 3;


    public ComputeShader compute;
    MeshFilter meshFilter;
    
    public float[] inputList;

    Vector3[] vertecies;
    int[] triangles;

    ComputeBuffer inputBuffer;
    ComputeBuffer triangleConnectionTable;
    ComputeBuffer vertexBuffer;
    ComputeBuffer triangleBuffer;

    // Start is called before the first frame update
    void Start()
    {
        if (MatrixSize % 8 != 0)
            throw new System.ArgumentException("MatrixSize must be divisible be 8");

        meshFilter = GetComponent<MeshFilter>();

        inputList = new float[BufferSize];

        vertecies = new Vector3[VertexCount];
        triangles = new int[VertexCount];

        int kernelHandle = compute.FindKernel("CSMain");

        triangleConnectionTable = new ComputeBuffer(256 * 16, sizeof(int));
        triangleConnectionTable.SetData(MarchingCubeParameters.TriangleConnectionTable);

        inputBuffer = new ComputeBuffer(MatrixSize * MatrixSize * MatrixSize * 3, sizeof(float));
        vertexBuffer = new ComputeBuffer(VertexCount * 3, sizeof(float));
        triangleBuffer = new ComputeBuffer(VertexCount, sizeof(int));
    }

    private void ClearTriangles()
    {
        for (int i = 0; i < TriangleCount; i++)
        {
            vertecies[i] = new Vector3(-1, -1, -1);
            triangles[i] = -1;
        }
    }

    private void ComputeStepFrame()
    {
        int kernelHandle = compute.FindKernel("CSMain");
        for (int i = 4; i < MatrixSize / 2; i++)
            for (int j = 4; j < MatrixSize / 2; j++)
                for (int k = 4; k < MatrixSize / 2; k++)
                {
                    var index = i + j * MatrixSize + k * MatrixSize * MatrixSize;
                    inputList[index] = 1f;
                }
        inputBuffer.SetData(inputList);
        compute.SetBuffer(kernelHandle, "inputBuffer", inputBuffer);
        compute.SetBuffer(kernelHandle, "vertexBuffer", vertexBuffer);
        compute.SetBuffer(kernelHandle, "triangleBuffer", triangleBuffer);
        compute.SetBuffer(kernelHandle, "TriangleConnectionTable", triangleConnectionTable);

        compute.SetInt("matrixSize", MatrixSize);
        compute.SetInt("_Width", MatrixSize);
        compute.SetInt("_Height", MatrixSize);
        compute.SetInt("vertexBufferSize", trianglePerBox * 3);
        compute.Dispatch(kernelHandle, MatrixSize / 8, MatrixSize / 8, MatrixSize / 8);
    }

    // Update is called once per frame
    void Update()
    {
        ComputeStepFrame();
        SetMesh();
    }

    private void SetMesh()
    {
        ClearTriangles();

        vertexBuffer.GetData(vertecies);
        meshFilter.mesh.vertices = vertecies;

        triangleBuffer.GetData(triangles);
        meshFilter.mesh.SetIndices(triangles, MeshTopology.Triangles, 0);

        meshFilter.mesh.RecalculateNormals();

        //for (int i=0; i< VertexCount; i += 3)
        //{
        //    Debug.DrawLine(vertecies[triangles[i]], vertecies[triangles[i + 1]], Color.red);
        //    Debug.DrawLine(vertecies[triangles[i + 1]], vertecies[triangles[i + 2]], Color.red);
        //    Debug.DrawLine(vertecies[triangles[i + 2]], vertecies[triangles[i]], Color.red);
        //}
    }
}
