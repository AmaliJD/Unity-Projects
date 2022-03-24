using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSize : MonoBehaviour
{
    public RectTransform grid;
    public int height;
    float WHRatio;

    private void Start()
    {
        WHRatio = grid.sizeDelta.x / grid.sizeDelta.y;
        Camera.main.orthographicSize = grid.sizeDelta.y / 2;
        Screen.SetResolution((int)(height * WHRatio), height, true);
        //Screen.fullScreen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown("f"))
        {
            if(Screen.fullScreen)
            {
                Screen.SetResolution((int)(height * WHRatio), height, true);
            }
            else
            {
                Screen.SetResolution(Screen.width, Screen.height, true);
            }
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}
