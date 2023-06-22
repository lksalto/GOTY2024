using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    Collider2D c2d;
    Rigidbody2D rb;
    private void Start()
    {
        c2d = gameObject.GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {

            c2d.enabled = false; 
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.velocity = Vector3.zero; 
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {

            gameObject.transform.SetParent(collision.gameObject.transform);

            c2d.enabled = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.velocity = Vector3.zero;



        }
    }
}
