using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CubeInformation
{
    public static float isolevel = 0.6f;
    public static float size = 1f;

    public static Vector3 up = Vector3.forward;
    public static Vector3 right = Vector3.up;
    public static Vector3 back = Vector3.right;

    public Vector3 Center
    {
        get
        {
            return (FLD + BRU) / 2;
        }
    }

    public float FrontLeftDown = 0;
    private Vector3 FLD;

    public float FrontLeftUp = 0;
    public Vector3 FLU;

    public float FrontRightDown = 0;
    public Vector3 FRD;

    public float FrontRightUp = 0;
    public Vector3 FRU;

    public float BackLeftDown = 0;
    public Vector3 BLD;

    public float BackLeftUp = 0;
    public Vector3 BLU;

    public float BackRightDown = 0;
    public Vector3 BRD;

    public float BackRightUp = 0;
    public Vector3 BRU;

    public void DrawAll()
    {

        bool okay = false;

        if (FrontLeftDown > isolevel)
        {
            Debug.DrawLine(Center, FLD, Color.green);
            okay = true;
        }
        if (FrontLeftUp > isolevel)
        {
            Debug.DrawLine(Center, FLU, Color.green);
            okay = true;
        }
        if (FrontRightDown > isolevel)
        {
            Debug.DrawLine(Center, FRD, Color.green);
            okay = true;
        }
        if (FrontRightUp > isolevel)
        {
            Debug.DrawLine(Center, FRU, Color.green);
            okay = true;
        }


        if (BackLeftDown > isolevel)
        {
            Debug.DrawLine(Center, BLD, Color.green);
            okay = true;
        }
        if (BackLeftUp > isolevel)
        {
            Debug.DrawLine(Center, BLU, Color.green);
            okay = true;
        }
        if (BackRightDown > isolevel)
        {
            Debug.DrawLine(Center, BRD, Color.green);
            okay = true;
        }
        if (BackRightUp > isolevel)
        {
            Debug.DrawLine(Center, BRU, Color.green);
            okay = true;
        }

        if (okay)
        {
            Debug.DrawLine(BLD, BRD, Color.blue);
            Debug.DrawLine(BRD, FRD, Color.blue);
            Debug.DrawLine(FRD, FLD, Color.blue);
            Debug.DrawLine(FLD, BLD, Color.blue);

            Debug.DrawLine(BLU, BRU, Color.blue);
            Debug.DrawLine(BRU, FRU, Color.blue);
            Debug.DrawLine(FRU, FLU, Color.blue);
            Debug.DrawLine(FLU, BLU, Color.blue);

            Debug.DrawLine(BLD, BLD, Color.blue);
            Debug.DrawLine(BRD, BRU, Color.blue);
            Debug.DrawLine(FRD, FRU, Color.blue);
            Debug.DrawLine(FLD, FLU, Color.blue);
        }

    }

    public CubeInformation(Vector3 FrontLeftDownPoint)
    {
        FLD = FrontLeftDownPoint;
        FLU = FLD + size * (up);
        FRD = FLD + size * (right);
        FRU = FLD + size * (right + up);

        BLD = FLD + size * (back);
        BLU = FLD + size * (back + up);
        BRD = FLD + size * (back + right);
        BRU = FLD + size * (back + right + up);
    }

    public Vector3 GetEdge(int i)
    {
        var output = BLD;
        switch (i)
        {
            case 0:
                output = getNormal(BackLeftDown, BLD, BackRightDown, BRD);
                break;
            case 1:
                output = getNormal(BackRightDown, BRD, FrontRightDown, FRD);
                break;
            case 2:
                output = getNormal(FrontRightDown, FRD, FrontLeftDown, FLD);
                break;
            case 3:
                output = getNormal(FrontLeftDown, FLD, BackLeftDown, BLD);
                break;
            case 4:
                output = getNormal(BackLeftUp, BLU, BackRightUp, BRU);
                break;
            case 5:
                output = getNormal(BackRightUp, BRU, FrontRightUp, FRU);
                break;
            case 6:
                output = getNormal(FrontRightUp, FRU, FrontLeftUp, FLU);
                break;
            case 7:
                output = getNormal(FrontLeftUp, FLU, BackLeftUp, BLU);
                break;
            case 8:
                output = getNormal(BackLeftDown, BLD, BackLeftUp, BLU);
                break;
            case 9:
                output = getNormal(BackRightDown, BRD, BackRightUp, BRU);
                break;
            case 10:
                output = getNormal(FrontRightDown, FRD, FrontRightUp, FRU);
                break;
            case 11:
                output = getNormal(FrontLeftDown, FLD, FrontLeftUp, FLU);
                break;
        }
        return output;
    }

    public Vector3 getNormal(float a, Vector3 A, float b, Vector3 B)
    {
        float difa = Mathf.Abs(a - isolevel);
        float difb = Mathf.Abs(b - isolevel);
        return (A * difa + B * difb) / (difa + difb);
    }

    public int ToIndex()
    {
        var cubeindex = 0;
        if (BackLeftDown < isolevel) cubeindex |= 1;
        if (BackRightDown < isolevel) cubeindex |= 2;
        if (FrontRightDown < isolevel) cubeindex |= 4;
        if (FrontLeftDown < isolevel) cubeindex |= 8;
        if (BackLeftUp < isolevel) cubeindex |= 16;
        if (BackRightUp < isolevel) cubeindex |= 32;
        if (FrontRightUp < isolevel) cubeindex |= 64;
        if (FrontLeftUp < isolevel) cubeindex |= 128;
        return cubeindex;
    }
}
