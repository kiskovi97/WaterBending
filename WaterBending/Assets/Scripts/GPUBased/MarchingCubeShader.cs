using Assets.Scripts.GPUBased;
using UnityEngine;
using System.Linq;

public class BufferContainer
{
    public ComputeBuffer inputBuffer;
    public ComputeBuffer triangleConnectionTable;
    public ComputeBuffer vertexBuffer;
    public Vector3[] vertecies;
    public ComputeBuffer triangleBuffer;
    public int[] triangles;

    public void SetAll()
    {
        vertecies = new Vector3[MarchingCubeParameters.VertexCount];
        triangles = new int[MarchingCubeParameters.VertexCount];
        inputBuffer = new ComputeBuffer(MarchingCubeParameters.BufferSize, sizeof(float));
        vertexBuffer = new ComputeBuffer(MarchingCubeParameters.VertexCount, sizeof(float) * 3);
        triangleBuffer = new ComputeBuffer(MarchingCubeParameters.VertexCount, sizeof(int));
        triangleConnectionTable = new ComputeBuffer(256 * 16, sizeof(int));

        triangleConnectionTable.SetData(MarchingCubeParameters.TriangleConnectionTable);
    }
}

public class MarchingCubeShader : MonoBehaviour
{
    public ComputeShader compute;
    private MeshFilter meshFilter;

    private BufferContainer bc = new BufferContainer();

    bool isChanged;

    public void SetInput(float[] inputList)
    {
        bc.inputBuffer.SetData(inputList);
        isChanged = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        isChanged = false;

        if (MarchingCubeParameters.MatrixSize % 8 != 0)
            throw new System.ArgumentException("MatrixSize must be divisible be 8");

        meshFilter = GetComponent<MeshFilter>();

        bc.SetAll();

        meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    void Update()
    {
        if (isChanged)
        {
            ClearTriangles();
            ComputeStepFrame();
            SetMesh();
            isChanged = false;
        } else
        {
            meshFilter.mesh.Clear();
        }
    }

    private void ClearTriangles()
    {
        for (long i = 0; i < MarchingCubeParameters.VertexCount; i++)
        {
            bc.vertecies[i] = new Vector3();
            bc.triangles[i] = -1;
        }
    }

    private void ComputeStepFrame()
    {
        int kernelHandle = compute.FindKernel("CSMain");

        compute.SetBuffer(kernelHandle, "inputBuffer", bc.inputBuffer);
        compute.SetBuffer(kernelHandle, "vertexBuffer", bc.vertexBuffer);
        compute.SetBuffer(kernelHandle, "triangleBuffer", bc.triangleBuffer);
        compute.SetBuffer(kernelHandle, "TriangleConnectionTable", bc.triangleConnectionTable);

        compute.SetInt("matrixSize", MarchingCubeParameters.MatrixSize);
        compute.SetInt("_Width", MarchingCubeParameters.MatrixSize);
        compute.SetInt("_Height", MarchingCubeParameters.MatrixSize);
        compute.SetInt("vertexBufferSize", MarchingCubeParameters.TrianglePerBox * 3);


        var Map = Matrix4x4.Translate(Vector3.one * MarchingCubeParameters.MatrixSize / -2);
        var scale = 1f / MarchingCubeParameters.MatrixSize;
        var Scale = new Matrix4x4(new Vector4(scale, 0, 0, 0), new Vector4(0, scale, 0, 0), new Vector4(0, 0, scale, 0), new Vector4(0, 0, 0, 1));

        compute.SetMatrix("ToWorld",  Scale * Map);

        compute.Dispatch(kernelHandle, MarchingCubeParameters.MatrixSize / 8, 
            MarchingCubeParameters.MatrixSize / 8, MarchingCubeParameters.MatrixSize / 8);

       

    }

    private void SetMesh()
    {
        ClearTriangles();

        meshFilter.mesh.MarkDynamic();
        meshFilter.mesh.Clear();

        bc.vertexBuffer.GetData(bc.vertecies);

        meshFilter.mesh.vertices = bc.vertecies;

        bc.triangleBuffer.GetData(bc.triangles);
        meshFilter.mesh.SetIndices(bc.triangles, MeshTopology.Triangles, 0);

        meshFilter.mesh.RecalculateNormals();
        //meshFilter.mesh.RecalculateTangents();
        //meshFilter.mesh.Optimize();

    }
}
