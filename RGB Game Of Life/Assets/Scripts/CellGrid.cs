using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
    public int x, y;
    public GameObject cell;
    public float spawnrateRed, spawnrateGreen, spawnrateBlue, spawnrateWhite;
    private float timerRed, timerGreen, timerBlue, timerWhite;
    public bool fullcolor;

    public int simulationSpeed;
    private int simtimer = 0;

    private RectTransform gridtransform;
    private int numberOfCells;
    private Cell[,] cellgrid;

    private float timer, time = 60+32, time2 = 60+40, time3 = 60 + 46, time4 = 122;
    private int T;

    private void Awake()
    {
        numberOfCells = x * y;
        gridtransform = GetComponent<RectTransform>();
        gridtransform.sizeDelta = new Vector2(x, y);
    }

    private void Start()
    {
        cellgrid = new Cell[y, x];

        for (int i = 0; i < y; i++)
        {
            for(int j = 0; j < x; j++)
            {
                cellgrid[i, j] = Instantiate(cell, gameObject.transform).GetComponent<Cell>();
                cellgrid[i, j].Setup();
                cellgrid[i, j].setGrid(GetComponent<CellGrid>());
                //Debug.Log(i + ", " + j);
            }
        }

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                if (j > 0) { cellgrid[i, j].addNeighbor(cellgrid[i, j - 1]); }
                else { cellgrid[i, j].addNeighbor(cellgrid[i, x - 1]); }

                if (j < x-1) { cellgrid[i, j].addNeighbor(cellgrid[i, j + 1]); }
                else { cellgrid[i, j].addNeighbor(cellgrid[i, 0]); }

                if (i > 0) { cellgrid[i, j].addNeighbor(cellgrid[i - 1, j]); }
                else { cellgrid[i, j].addNeighbor(cellgrid[y - 1, j]); }

                if (i < y-1) { cellgrid[i, j].addNeighbor(cellgrid[i + 1, j]); }
                else { cellgrid[i, j].addNeighbor(cellgrid[0, j]); }
                
                if (j > 0 && i > 0) { cellgrid[i, j].addNeighbor(cellgrid[i - 1, j - 1]); }
                else if (i > 0) { cellgrid[i, j].addNeighbor(cellgrid[i - 1, x - 1]); }
                else if (j > 0) { cellgrid[i, j].addNeighbor(cellgrid[y - 1, j - 1]); }
                else { cellgrid[i, j].addNeighbor(cellgrid[y - 1, x - 1]); }

                if (j < x - 1 && i < y - 1) { cellgrid[i, j].addNeighbor(cellgrid[i + 1, j + 1]); }
                else if (i < y - 1) { cellgrid[i, j].addNeighbor(cellgrid[i + 1, 0]); }
                else if (j < x - 1) { cellgrid[i, j].addNeighbor(cellgrid[0, j + 1]); }
                else { cellgrid[i, j].addNeighbor(cellgrid[0, 0]); }

                if (i > 0 && j < x - 1) { cellgrid[i, j].addNeighbor(cellgrid[i - 1, j + 1]); }
                else if (i > 0) { cellgrid[i, j].addNeighbor(cellgrid[i - 1, 0]); }
                else if (j < x - 1) { cellgrid[i, j].addNeighbor(cellgrid[y - 1, j + 1]); }
                else { cellgrid[i, j].addNeighbor(cellgrid[y - 1, 0]); }

                if (i < y - 1 && j > 0) { cellgrid[i, j].addNeighbor(cellgrid[i + 1, j - 1]); }
                else if (i < y - 1) { cellgrid[i, j].addNeighbor(cellgrid[i + 1, x - 1]); }
                else if (j > 0) { cellgrid[i, j].addNeighbor(cellgrid[0, j - 1]); }
                else { cellgrid[i, j].addNeighbor(cellgrid[0, x - 1]); }
            }
        }

        int Ri = Random.Range(0, y);
        int Rj = Random.Range(0, x);
        cellgrid[Ri, Rj].addRed(1);

        int Gi = Random.Range(0, y);
        int Gj = Random.Range(0, x);
        cellgrid[Gi, Gj].addGreen(1);

        int Bi = Random.Range(0, y);
        int Bj = Random.Range(0, x);
        cellgrid[Bi, Bj].addBlue(1);

        timerRed = Random.Range(-.7f, 0f);
        timerGreen = Random.Range(-.7f, 0f);
        timerBlue = Random.Range(-.7f, 0f);
        timerWhite = Random.Range(-.7f, 0f);
    }

    private void Update()
    {
        simtimer++;
        if (simtimer < simulationSpeed) return;
        simtimer = 0;

        int D = Random.Range(0, 200);
        if(D == 0)
        {
            int DI = Random.Range(0, y);
            int DJ = Random.Range(0, x);
            cellgrid[DI, DJ].type = 4;
            cellgrid[DI, DJ].dead = false;
            cellgrid[DI, DJ].mature = true;
        }

        if (timerRed >= spawnrateRed && spawnrateRed > 0)
        {
            int I = Random.Range(0, y);
            int J = Random.Range(0, x);
            timerRed = 0;

            int R = Random.Range(0, 3);
            if(R==1)
                cellgrid[I, J].addRed(1f);

            cellgrid[I, J].dead = false;
        }
        if (timerGreen >= spawnrateGreen && spawnrateGreen > 0)
        {
            int I = Random.Range(0, y);
            int J = Random.Range(0, x);
            timerGreen = 0;

            int R = Random.Range(0, 3);
            if (R == 1)
                cellgrid[I, J].addGreen(1);

            cellgrid[I, J].dead = false;
        }
        if (timerBlue >= spawnrateBlue && spawnrateBlue > 0)
        {
            int I = Random.Range(0, y);
            int J = Random.Range(0, x);
            timerBlue = 0;

            int R = Random.Range(0, 3);
            if (R == 1)
                cellgrid[I, J].addBlue(1);

            cellgrid[I, J].dead = false;
        }
        if (timerWhite >= spawnrateWhite && spawnrateWhite > 0)
        {
            int I = Random.Range(0, y);
            int J = Random.Range(0, x);
            timerWhite = 0;

            int R = Random.Range(0, 3);
            if (R == 1)
                cellgrid[I, J].colormode = 4;

            cellgrid[I, J].dead = false;
        }

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                cellgrid[i, j].grab();
                cellgrid[i, j].setColor();
                cellgrid[i, j].UpdateFunction();
            }
        }

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                cellgrid[i, j].updateOutput();
            }
        }

        timerRed += Time.deltaTime;
        timerGreen += Time.deltaTime;
        timerBlue += Time.deltaTime;
        timerWhite += Time.deltaTime;

        timer += Time.deltaTime;
        if(T == 0 && timer >= time)
        {
            T++;
            StartCoroutine(Sweep());
        }

        if (T == 1 && timer >= time2)
        {
            T++;
            StartCoroutine(Sweep2());
        }

        if (T == 2 && timer >= time3)
        {
            T++;
            StartCoroutine(Sweep3());
        }

        if (T == 3 && timer >= time4)
        {
            T++;
            StartCoroutine(Sweep4());
        }
    }

    IEnumerator Sweep()
    {
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                cellgrid[i, j].type = Random.Range(1, 4);
                cellgrid[i, j].dead = false;
                cellgrid[i, j].spark = 1;
            }
            yield return null;
        }
    }

    IEnumerator Sweep2()
    {
        spawnrateRed = .1f;
        spawnrateGreen = .1f;
        spawnrateBlue = .1f;
        for (int j = 0; j < x; j++)
        {
            for (int i = 0; i < y; i++)
            {
                cellgrid[i, j].type = Random.Range(1, 3);
                cellgrid[i, j].dead = false;
                cellgrid[i, j].spark = 0;
            }
            yield return null;
        }
    }

    IEnumerator Sweep3()
    {
        spawnrateRed = .1f;
        spawnrateGreen = .1f;
        spawnrateBlue = .1f;
        for (int j = x - 1; j > -1; j--)
        {
            for (int i = 0; i < y; i++)
            {
                cellgrid[i, j].type = Random.Range(0, 2);
                cellgrid[i, j].dead = false;
                cellgrid[i, j].spark = 0;
            }
            yield return null;
        }
    }

    IEnumerator Sweep4()
    {
        for (int i = y-1; i > -1; i--)
        {
            for (int j = 0; j < x; j++)
            {
                cellgrid[i, j].type = Random.Range(0, 1);
                cellgrid[i, j].dead = false;
                cellgrid[i, j].spark = 1;
            }
            yield return null;
        }
    }
}
