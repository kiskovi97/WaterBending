using Assets.Scripts.GPUBased;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class BufferContainer
{
    [NonSerialized]
    public ComputeBuffer inputBuffer;
    [NonSerialized]
    public ComputeBuffer triangleConnectionTable;
    [NonSerialized]
    public ComputeBuffer vertexBuffer;
    [NonSerialized]
    public Vector3[] vertecies;
    [NonSerialized]
    public ComputeBuffer triangleBuffer;
    [NonSerialized]
    public int[] triangles;
    public MarchingCubeParameters marchingCubeParameters = new MarchingCubeParameters();

    public void SetAll()
    {
        vertecies = new Vector3[marchingCubeParameters.VertexCount];
        triangles = new int[marchingCubeParameters.VertexCount];
        inputBuffer = new ComputeBuffer(marchingCubeParameters.BufferSize, sizeof(float));
        vertexBuffer = new ComputeBuffer(marchingCubeParameters.VertexCount, sizeof(float) * 3);
        triangleBuffer = new ComputeBuffer(marchingCubeParameters.VertexCount, sizeof(int));
        triangleConnectionTable = new ComputeBuffer(256 * 16, sizeof(int));

        triangleConnectionTable.SetData(MarchingCubeParameters.TriangleConnectionTable);
    }

    public void RelaseAll()
    {
        inputBuffer.Release();
        triangleConnectionTable.Release();
        vertexBuffer.Release();
        triangleBuffer.Release();
    }
}

public class MarchingCubeShader : MonoBehaviour
{
    public bool ClearIfEnded = true;
    public ComputeShader compute;
    public MeshFilter meshFilter;

    public MarchingCubeParameters parameters { get { return bc.marchingCubeParameters; } }

    [SerializeField]
    public BufferContainer bc = new BufferContainer();

    bool isChanged = false;

    public void SetInput(float[] inputList)
    {
        bc.inputBuffer.SetData(inputList);
        isChanged = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (bc.marchingCubeParameters.MatrixSize % 8 != 0)
            throw new System.ArgumentException("MatrixSize must be divisible be 8");

        meshFilter = GetComponent<MeshFilter>();

        bc.SetAll(); 

        meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    void LateUpdate()
    {
        if (isChanged)
        {
            ClearTriangles();
            ComputeStepFrame();
            SetMesh();
            isChanged = false;
        } else
        {
            if (ClearIfEnded)
            {
                meshFilter.mesh.Clear();
            }
        }
    }

    private void ClearTriangles()
    {
        for (long i = 0; i < bc.marchingCubeParameters.VertexCount; i++)
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

        compute.SetInt("matrixSize", bc.marchingCubeParameters.MatrixSize);
        compute.SetInt("_Width", bc.marchingCubeParameters.MatrixSize);
        compute.SetInt("_Height", bc.marchingCubeParameters.MatrixSize);
        compute.SetFloat("Target", bc.marchingCubeParameters.Target);
        compute.SetInt("vertexBufferSize", bc.marchingCubeParameters.TrianglePerBox * 3);


        var Map = Matrix4x4.Translate(Vector3.one * bc.marchingCubeParameters.MatrixSize / -2);
        var scale = 1f / bc.marchingCubeParameters.MatrixSize;
        var Scale = new Matrix4x4(new Vector4(scale, 0, 0, 0), new Vector4(0, scale, 0, 0), new Vector4(0, 0, scale, 0), new Vector4(0, 0, 0, 1));

        compute.SetMatrix("ToWorld",  Scale * Map);

        compute.Dispatch(kernelHandle, bc.marchingCubeParameters.MatrixSize / 8,
            bc.marchingCubeParameters.MatrixSize / 8, bc.marchingCubeParameters.MatrixSize / 8);
    }

    private void SetMesh()
    {
        //ClearTriangles();

        meshFilter.mesh.MarkDynamic();
        meshFilter.mesh.Clear();

        bc.vertexBuffer.GetData(bc.vertecies);

        meshFilter.mesh.vertices = bc.vertecies;

        bc.triangleBuffer.GetData(bc.triangles);
        meshFilter.mesh.SetIndices(bc.triangles, MeshTopology.Triangles, 0);

        meshFilter.mesh.RecalculateNormals();
    }

    private void OnDestroy()
    {
        bc.RelaseAll();
    }
}
