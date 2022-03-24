using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D playerbody;
    private AudioSource explode_sfx, hit_sfx;
    private GameObject asteroid;
    private float velocityX, velocityY, speed;
    public int strength, STREN;
    public float size;
    void Awake()
    {
        playerbody = transform.parent.GetComponent<AsteroidParent>().playerbody;
        asteroid = transform.parent.GetComponent<AsteroidParent>().asteroid;
        explode_sfx = transform.parent.GetComponent<AsteroidParent>().explode;
        hit_sfx = transform.parent.GetComponent<AsteroidParent>().hit;

        int look = Random.Range(0, 9);
        transform.GetChild(look).gameObject.SetActive(true);

        size = Random.Range(1f, 25f);
        transform.localScale = new Vector3(size * Random.Range(1.05f, .95f), size * Random.Range(1.05f, .95f), 1);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        strength = (int)(transform.localScale.x / 5) + 1;

        velocityX = Random.Range(-5f / (float)strength, 5f / (float)strength);
        velocityY = Random.Range(-5f / (float)strength, 5f / (float)strength);

        STREN = strength;
    }

    public void Init(float s, float velX, float velY)
    {
        size = s;
        transform.localScale = new Vector3(size * Random.Range(1.05f, .95f), size * Random.Range(1.05f, .95f), 1);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        strength = (int)(transform.localScale.x / 5) + 1;

        velocityX = velX;// Random.Range(-5f / strength, 5f / strength);
        velocityY = velY;// Random.Range(-5f / strength, 5f / strength);

        STREN = strength;
    }

    private void Update()
    {
        float newspeed = Mathf.Clamp(playerbody.velocity.magnitude / 14, playerbody.GetComponent<PlayerController>().dead ? 0.2f : 0, 20f);
        //speed = .2f;
        speed = Mathf.Lerp(speed, newspeed, speed > newspeed ? .8f : .01f);
        transform.Translate(new Vector2(velocityX, velocityY) * speed, Space.World);

        if(size < 2f)
            Destroy(gameObject);

        if (transform.position.x < Camera.main.transform.position.x - 70f
            || transform.position.x > Camera.main.transform.position.x + 70f
            || transform.position.y < Camera.main.transform.position.y - 60f
            || transform.position.y > Camera.main.transform.position.y + 60f)
        {
            Destroy(gameObject);
        }
    }

    public void Hit()
    {
        strength--;
        hit_sfx.PlayOneShot(hit_sfx.clip, .2f);

        if (strength == 0)
        {
            explode_sfx.PlayOneShot(explode_sfx.clip, .2f);
            int rand = Random.Range(2, 4);
            float randX = Random.Range(-.5f, 1f);
            float randY = Random.Range(-.4f, 1f);

            Vector3 partitionsx, partitionsy;
            float[] partsX = new float[3], partsY = new float[3];

            if(rand == 3)
            {
                partitionsx = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                partitionsx = partitionsx.normalized;

                partitionsy = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                partitionsy = partitionsy.normalized;
            }
            else
            {
                partitionsx = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                partitionsx = partitionsx.normalized;

                partitionsy = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                partitionsy = partitionsy.normalized;
            }

            partsX[0] = partitionsx.x;
            partsX[1] = partitionsx.y;
            partsX[2] = partitionsx.z;

            partsY[0] = partitionsy.x;
            partsY[1] = partitionsy.y;
            partsY[2] = partitionsy.z;

            for (int i = 0; i < rand; i++)
            {
                GameObject ast = Instantiate(asteroid, transform.position + new Vector3(Random.Range(-size/rand, size/rand), Random.Range(-size / rand, size / rand), 0), Quaternion.identity, transform.parent);
                ast.GetComponent<Asteroid>().Init(size / rand, velocityX * partsX[i] * rand/*Random.Range(1f, (float)rand)*/, velocityY * partsY[i] * rand/*Random.Range(1f, (float)rand)*/);
            }
            transform.parent.GetComponent<AsteroidParent>().score += 1;
            Destroy(gameObject);
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "beam")
        {
            strength--;
            Destroy(collision.gameObject);

            if(strength == 0)
            {
                explode_sfx.PlayOneShot(explode_sfx.clip, .2f);
                Destroy(gameObject);
            }
        }
    }*/
}
