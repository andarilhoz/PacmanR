using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraAdjust : MonoBehaviour
{
    private Camera cameraComponent;

    private const float SCENE_WIDTH = 5.5f;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
    }

    private void Start()
    {
        var unitsPerPixel = SCENE_WIDTH / Screen.width;
        var desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        cameraComponent.orthographicSize = desiredHalfHeight;
    }
}