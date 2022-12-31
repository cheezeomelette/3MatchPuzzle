using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSet : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] float boardUnit;

    void Start()
    {
        mainCam.orthographicSize = boardUnit / mainCam.aspect;
    }
}
