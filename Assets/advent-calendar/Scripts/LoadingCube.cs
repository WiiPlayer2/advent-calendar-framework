using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCube : MonoBehaviour
{
    [SerializeField]
    private float rpm = 60f;

    private void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rpm * 6f, Space.World);
    }
}
