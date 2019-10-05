using Assets.Scripts.GPUBased;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCubes : MonoBehaviour
{
    public GameObject terrainCubes;

    public int X = 2;
    public int Y = 1;
    public int Z = 2;

    // Start is called before the first frame update
    void Start()
    {
        var size = 16;
        var offset = (size - 1f)/ size;
        var offsetRandom = UnityEngine.Random.value * 10f;
        for (int i=0; i<X; i++)
            for (int j= 0; j<Y; j++)
                for (int k=0; k<Z; k++)
                {
                    var obj = Instantiate(terrainCubes, new Vector3(i * offset, j * offset, k * offset), new Quaternion());
                    var cube = obj.GetComponent<TerrainCube>();
                    cube.realOffset = Vector3.left * offsetRandom;
                }
    }
}
