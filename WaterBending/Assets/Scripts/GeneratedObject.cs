using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GeneratedObject : MonoBehaviour
{
    Mesh mesh;
    Dictionary<Vector3, int> meshVertexes = new Dictionary<Vector3, int>();

    List<List<int>> subTriangles = new List<List<int>>();
    List<Vector2> myUV = new List<Vector2>();
    protected virtual void Clear()
    {
        meshVertexes = new Dictionary<Vector3, int>();
        myUV = new List<Vector2>();
        subTriangles = new List<List<int>>();
        for (int i = 0; i < GetComponent<MeshRenderer>().sharedMaterials.Length; i++)
        {
            subTriangles.Add(new List<int>());
        }
        GetMesh();
    }
    private void GetMesh()
    {
#if UNITY_EDITOR
        MeshFilter mf = GetComponent<MeshFilter>();   //a better way of getting the meshfilter using Generics
        Mesh meshCopy = Instantiate(mf.sharedMesh) as Mesh;  //make a deep copy
        mesh = mf.mesh = meshCopy;
#else
        mesh = GetComponent<MeshFilter>().mesh;
#endif
    }
    protected void AddTriangle(Triangle triangle)
    {
        if (subTriangles.Count <= triangle.material)
        {
            Debug.Log("Need material : " + triangle.material);
            return;
        }
        Matrix4x4 matrix = gameObject.transform.worldToLocalMatrix;
        Vector3 to = transform.position;
        if (triangle.uvs.Length < 3) return;

        Addpoint(triangle.A, matrix, to, triangle.material, triangle.uvs[0]);
        Addpoint(triangle.B, matrix, to, triangle.material, triangle.uvs[1]);
        Addpoint(triangle.C, matrix, to, triangle.material, triangle.uvs[2]);
    }

    private void Addpoint(Vector3 point, Matrix4x4 matrix, Vector3 to, int material, Vector2 uv)
    {
        var okay = meshVertexes.TryGetValue(matrix * (point - to), out int value);
        if (okay)
        {
            subTriangles[material].Add(value);
        } else
        {
            subTriangles[material].Add(meshVertexes.Count);
            meshVertexes.Add(matrix * (point - to), meshVertexes.Count);
            myUV.Add(uv);
        }
    }

    public void CreateMesh()
    {
        if (meshVertexes == null) return;
        mesh.Clear();
        if (meshVertexes.Count < 3)
        {
           // DestorySelf();
            return;
        }
        mesh.vertices = meshVertexes.OrderBy((item) => item.Value).Select((item)=>item.Key).ToArray();
        Smooth(mesh.vertices);
        mesh.subMeshCount = subTriangles.Count;
        for (int i = 0; i < subTriangles.Count; i++)
        {
            mesh.SetTriangles(subTriangles[i].ToArray(), i);
        }
        mesh.SetUVs(0, myUV);
        mesh.MarkDynamic();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void Smooth(Vector3[] vertexes)
    {
        for (int i= 0; i< subTriangles[0].Count; i+=3)
        {
            var A = subTriangles[0][i];
            var B = subTriangles[0][i + 1];
            var C = subTriangles[0][i + 2];
            vertexes[A] = vertexes[A] * 0.8f + vertexes[B] * 0.1f + vertexes[A] * 0.1f;
            vertexes[B] = vertexes[B] * 0.8f + vertexes[A] * 0.1f + vertexes[C] * 0.1f;
            vertexes[C] = vertexes[C] * 0.8f + vertexes[B] * 0.1f + vertexes[C] * 0.1f;
        }
    }

    public virtual void DestorySelf()
    {
        DestroyImmediate(gameObject);
    }
}