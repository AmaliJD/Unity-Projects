using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float accelaration, maxspeed;

    public GameObject beam;
    public GameObject fire;
    public GameObject deathexplode;
    public TrailRenderer trail;

    public AudioSource beam_sfx, explode_sfx, flame_sfx;

    Rigidbody2D rb;
    float timer = 0, count = 0, reload = .15f, deadtimer = 0;

    public bool dead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (flame_sfx.isPlaying && dead) flame_sfx.Stop();
        if (dead)
        {
            deadtimer += Time.deltaTime;

            if(deadtimer > 2)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            return;
        }

        //reload = Mathf.Clamp(1 / rb.velocity.magnitude, .05f, .5f);
        reload = (.12f - ((Mathf.Clamp(rb.velocity.magnitude, 0, maxspeed) / maxspeed) * .12f)) + .1f;

        Debug.Log(reload);

        timer += Time.deltaTime;
        if((Input.GetKey("space") || Input.GetKey(KeyCode.LeftShift)) && timer > reload)
        {
            Shoot();
            count++;
            timer = 0;

            if(count == 5)
            {
                count = 0;
                timer = -reload;
            }
        }
        else if (Input.GetKeyUp("space"))
        {
            timer = 0;
            count = 0;
        }

        trail.emitting = true;
        float newX = rb.position.x;
        float newY = rb.position.y;

        if (rb.position.x < Camera.main.transform.position.x - 1.02f * 30f)
        {
            newX = Camera.main.transform.position.x + 1.02f * 30f;
        }
        else if (rb.position.x > Camera.main.transform.position.x + 1.02f * 30f)
        {
            newX = Camera.main.transform.position.x - 1.02f * 30f;
        }

        if (rb.position.y < Camera.main.transform.position.y - 1.02f * 16.875f)
        {
            newY = Camera.main.transform.position.y + 1.02f * 16.875f;
        }
        else if (rb.position.y > Camera.main.transform.position.y + 1.02f * 16.875f)
        {
            newY = Camera.main.transform.position.y - 1.02f * 16.875f;
        }

        if(newX != rb.position.x || newY != rb.position.y)
        {
            transform.position = new Vector2(newX, newY);
            trail.Clear();
            trail.emitting = false;
        }
    }

    void Shoot()
    {
        GameObject b = Instantiate(beam, transform.position, transform.rotation, null);
        b.GetComponent<Rigidbody2D>().AddForce(((rb.velocity.magnitude / 2) + 28) * transform.up, ForceMode2D.Impulse);
        beam_sfx.PlayOneShot(beam_sfx.clip, .2f);
    }

    void FixedUpdate()
    {
        if (dead) return;
        RotatePlayer();
        MovePlayer();
    }

    void RotatePlayer()
    {
        rb.rotation -= 6 * Input.GetAxisRaw("Horizontal");
    }

    void MovePlayer()
    {
        if(Input.GetAxisRaw("Vertical") > 0)
        {
            rb.AddForce(Input.GetAxisRaw("Vertical") * accelaration * transform.up, ForceMode2D.Force);
            fire.SetActive(true);
            if (!flame_sfx.isPlaying) flame_sfx.Play();
        }
        else
        {
            fire.SetActive(false);
            flame_sfx.Stop();
        }

        if(rb.velocity.magnitude > maxspeed)
        {
            rb.velocity = rb.velocity.normalized * maxspeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.position.x < Camera.main.transform.position.x - 29f
            || rb.position.x > Camera.main.transform.position.x + 29f
            || rb.position.y < Camera.main.transform.position.y - 16f
            || rb.position.y > Camera.main.transform.position.y + 16f)
        {
            return;
        }

        if (collision.gameObject.tag == "asteroid" && !dead)
        {
            explode_sfx.PlayOneShot(explode_sfx.clip, .3f);
            GameObject p = Instantiate(deathexplode, transform.position, Quaternion.identity, null);
            p.SetActive(true);
            dead = true;
            fire.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
