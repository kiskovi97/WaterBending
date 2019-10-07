using Assets.Scripts.GPUBased;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCubes : MonoBehaviour
{
    public GameObject terrainCubes;

    public Transform player;

    public float radius;

    public Dictionary<string, GameObject> cubes = new Dictionary<string, GameObject>();

    public Queue<GameObject> leftOver = new Queue<GameObject>();

    public int minY = -1;
    public int maxY = 1;

    private float offsetRandom;
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        var size = terrainCubes.GetComponent<MarchingCubeShader>().parameters.MatrixSize;
        offset = (size - 1f) / size * 0.95f;
        offsetRandom = Random.value * 100f;
    }

    private void Update()
    {
        GenerateFromPoint(player.position);
    }

    public void RemoveMe(GameObject obj)
    {
        var key = cubes.FirstOrDefault(x => x.Value == obj).Key;
        if (!cubes.Remove(key))
        {
            Debug.Log("Problem");
        }
        leftOver.Enqueue(obj);
        obj.SetActive(false);
    }

    void GenerateFromPoint(Vector3 point)
    {
        var outOffset = point / offset;
        int x = Mathf.RoundToInt(outOffset.x);
        int z = Mathf.RoundToInt(outOffset.z);
        for (var i = x - radius; i < x + radius; i++)
            for (var j = minY; j <= maxY; j++)
                for (var k = z - radius; k < z + radius; k++)
                {
                    var vector = new Vector3(i, j, k);
                    if ((vector - outOffset).magnitude < radius)
                    {
                        CheckObject(vector);
                    }
                }
    }

    void CheckObject(Vector3 point)
    {
        var key = $"{point.x},{point.y},{point.z}";
        if (!cubes.ContainsKey(key))
        {
            cubes.Add(key, GenerateObjects(point));
        }
    }

    GameObject GenerateObjects(Vector3 point)
    {
        GameObject obj;
        if (leftOver.Count > 0)
        {
            obj = leftOver.Dequeue();
            obj.transform.position = point * offset;
            obj.SetActive(true);
        } else
        {
            obj = Instantiate(terrainCubes, point * offset, new Quaternion());
        }
        
        var cube = obj.GetComponent<TerrainCube>();
        cube.realOffset = Vector3.left * offsetRandom;
        cube.cubes = this;
        cube.Start();
        return obj;
    }
}
