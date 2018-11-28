using UnityEngine;

public static class Helper
{
    public static void ClampToOne(this Vector3 thisVector)
    {
        if(thisVector.sqrMagnitude > 1)
            thisVector.Normalize();
    }
}
