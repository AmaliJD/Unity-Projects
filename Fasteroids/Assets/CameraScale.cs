using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour
{
    public GameObject bounds_left, bounds_right;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        bounds_left.transform.localPosition = new Vector3(cam.orthographicSize * -.6f, 0, 10);
        bounds_right.transform.localPosition = new Vector3(cam.orthographicSize * .6f, 0, 10);

        bounds_left.transform.localScale = new Vector3(cam.orthographicSize / 8, cam.orthographicSize * 3, 1);
        bounds_right.transform.localScale = new Vector3(cam.orthographicSize / 8, cam.orthographicSize * 3, 1);
    }
}
