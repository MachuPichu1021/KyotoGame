using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;

    private void LateUpdate()
    {
        transform.position = cameraPos.position;
    }
}
