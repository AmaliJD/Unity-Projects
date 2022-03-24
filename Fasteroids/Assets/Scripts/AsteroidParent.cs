using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidParent : MonoBehaviour
{
    public Rigidbody2D playerbody;
    public AudioSource explode, hit;
    public GameObject asteroid;
    public Text scoretext, best_text;
    public int score, highscore;

    public int maxAsteroids;
    private Highscore h;

    private void Start()
    {
        h = FindObjectOfType<Highscore>();
        highscore = h.highscore;
        StartCoroutine(UpdateFunction());
    }

    private void Update()
    {
        scoretext.text = score + "";
        best_text.text = "Best: " + Mathf.Max(score, highscore);
        h.highscore = Mathf.Max(score, highscore);

        if(score <= highscore)
        {
            best_text.color = Color.white;
        }
        else
        {
            best_text.color = Color.yellow;
        }
    }

    IEnumerator UpdateFunction()
    {
        while(true)
        {
            /*float timer = 0;
            while (timer < .6f)
            {
                timer += Time.deltaTime;
                yield return null;
            }*/

            int astCount = 0;
            foreach(Transform t in transform)
            {
                if(t.GetComponent<Asteroid>().size >= 7)
                {
                    astCount++;
                }
            }
            if(astCount < maxAsteroids)
            {
                Spawn();
            }

            yield return new WaitForSeconds(0.6f);
        }
    }

    void Spawn()
    {
        //GameObject ast = Instantiate(asteroid, transform.position + new Vector3(Random.Range(-size / rand, size / rand), Random.Range(-size / rand, size / rand), 0), Quaternion.identity, transform);
        //ast.GetComponent<Asteroid>().Init(size / rand, velocityX * partsX[i] * Random.Range(1f, (float)rand), velocityY * partsY[i] * Random.Range(1f, (float)rand));
        GameObject go = Instantiate(asteroid, transform);
        Asteroid ast = go.GetComponent<Asteroid>();

        int xSide = Random.Range(0, 2);
        int ySide = Random.Range(0, 2);
        float xPos = 0;
        float yPos = 0;

        switch(xSide)
        {
            case 0:
                xPos = -32 - ast.size/2;
                break;
            case 1:
                xPos = 32 + ast.size/2;
                break;
        }

        switch (ySide)
        {
            case 0:
                yPos = -20 - ast.size/2;
                break;
            case 1:
                yPos = 20 + ast.size/2;
                break;
        }

        go.transform.position = new Vector2(xPos, yPos);

        int strength = (int)(ast.size / 5) + 1;

        float velocityX = Random.Range(1f, 4.5f / (float)strength) / 1.5f;
        float velocityY = Random.Range(1f, 4.5f / (float)strength) / 1.5f;

        Vector2 forward = new Vector2(playerbody.position.x - go.transform.position.x, playerbody.position.y - go.transform.position.y).normalized;
        //Vector2 orthogonal = new Vector2(forward.y, -forward.x);

        ast.Init(ast.size, forward.x * velocityX, forward.y * velocityY);
    }
}
