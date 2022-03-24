using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "asteroid")
        {
            collision.transform.parent.GetComponent<Asteroid>().Hit();
            Destroy(gameObject);
        }
        else if (collision.tag == "beamdestroy")
        {
            Destroy(gameObject);
        }
    }
}
