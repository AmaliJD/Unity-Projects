using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConwaysGameOfLife : MonoBehaviour
{
    public Image image;
    public int range;
    private Texture2D texture;
    private Cell[,] grid;
    private Color[] colorlist;

    private int width, height;
    public bool clear;

    private bool paused;
    private int onX = -1, onY = -1;

    private void Awake()
    {
        texture = image.sprite.texture;
        width = texture.width;
        height = texture.height;

        colorlist = new Color[width * height];

        grid = new Cell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j] = new Cell();
            }
        }

        if(!clear)
            Init();

        texture.SetPixels(colorlist);
    }
    void Update()
    {
        if (paused)
        {
            if(Input.GetMouseButton(0))
            {
                int x = (int)Input.mousePosition.x;
                int y = (int)Input.mousePosition.y;
                int X = (int)(((float)x / (float)Screen.width) * (float)width);
                int Y = (int)(((float)y / (float)Screen.height) * (float)height);

                if (!(X == onX && Y == onY))
                {
                    onX = X; onY = Y;
                    grid[X, Y].value = 1 - grid[X, Y].value;

                    int index = (width * Y) + X;
                    colorlist[index] = grid[X, Y].value == 1 ? Color.white : Color.black;

                    texture.SetPixels(colorlist);
                    texture.Apply(false);
                }
            }

            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                Step();

                texture.SetPixels(colorlist);
                texture.Apply(false);
            }
            
        }
        else if(!paused)
        {
            Step();

            texture.SetPixels(colorlist);
            texture.Apply(false);
        }

        if(Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
        else if(Input.GetKeyDown("space"))
        {
            paused = !paused;
        }
    }
    private void Init()
    {
        int index = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int value = Random.Range(0, range);

                if(j == 0)//value == 0)
                {
                    grid[i, j].value = 1;
                    grid[i, j].newvalue = 1;
                    colorlist[index] = Color.white;
                }

                index++;
            }
        }
    }
    void Step()
    {
        int index = 0;
        for(int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int liveneighbors;
                int i_ = (i + 1) % width;
                int _i = (i - 1) % width;
                int j_ = (j + 1) % height;
                int _j = (j - 1) % height;
                _i = _i == -1 ? width - 1 : _i;
                _j = _j == -1 ? height - 1 : _j;
                liveneighbors = grid[_i, _j].value +
                                grid[_i, j].value +
                                grid[_i, j_].value +
                                grid[i, _j].value +
                                grid[i, j_].value +
                                grid[i_, _j].value +
                                grid[i_, j].value +
                                grid[i_, j_].value;

                switch (grid[i, j].value)
                {
                    case 1:
                        if (liveneighbors < 2 || liveneighbors > 3)
                        {
                            grid[i, j].newvalue = 0;
                            colorlist[index] = Color.black;
                        }
                        break;

                    case 0:
                        if (liveneighbors == 3)
                        {
                            grid[i, j].newvalue = 1;
                            colorlist[index] = Color.white;
                        }
                        break;
                }

                index++;
            }
        }

        foreach (Cell c in grid)
        {
            c.value = c.newvalue;
        }
    }
}

public class Cell
{
    public int value, newvalue;

    public Cell()
    {
        newvalue = 0;
        value = 0;
    }
}