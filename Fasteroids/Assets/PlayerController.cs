using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float maxspeed, accelaration, speedy;
    public Rigidbody2D sprite_body;
    public Image left, right;

    private float speedx;
    private bool moveLeft, moveRight;
    private bool touchingBounds;
    private Rigidbody2D rb;
    private EventSystem es;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }


    void FixedUpdate()
    {
        MovementX();
        MovementY();
        Rotation();
    }

    void MovementX()
    {
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            moveLeft = true;
            moveRight = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveRight = true;
            moveLeft = false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveLeft = true;
            moveRight = false;
        }
        else
        {
            moveRight = false;
            moveLeft = false;
        }

        if (moveLeft && moveRight)
        {
            LerpSpeed(0);
        }
        else if (moveRight)
        {
            LerpSpeed(maxspeed);
        }
        else if (moveLeft)
        {
            LerpSpeed(-maxspeed);
        }

        rb.velocity = new Vector2(speedx, rb.velocity.y);
    }

    void MovementY()
    {
        rb.velocity = new Vector2(rb.velocity.x, speedy);
    }

    void Rotation()
    {
        //sprite_body.rotation = -45 * (speedx/maxspeed);
        //sprite_body.transform.rotation = Quaternion.Euler(new Vector3(0,0,-45 * (speedx / maxspeed)));
        if(touchingBounds)
        {
            sprite_body.transform.rotation = Quaternion.Lerp(sprite_body.transform.rotation, Quaternion.identity, accelaration * 2 * Time.fixedDeltaTime);
            moveRight = false;
            moveLeft = false;
            speedx = 0;
        }
        else
        {
            sprite_body.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45 * (speedx / maxspeed)));
        }
    }

    public void LerpSpeed(float value)
    {
        speedx = Mathf.Lerp(speedx, value, accelaration * Time.fixedDeltaTime);
    }

    public void LerpSpeedRight(bool b)
    {
        moveRight = b;
        right.color = b ? new Color(1, 1, 1, 0.1647f) : Color.clear; ;
    }

    public void LerpSpeedLeft(bool b)
    {
        moveLeft = b;
        left.color = b ? new Color(1, 1, 1, 0.1647f) : Color.clear; ;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bounds")
        {
            touchingBounds = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bounds")
        {
            touchingBounds = false;
        }
    }
}
