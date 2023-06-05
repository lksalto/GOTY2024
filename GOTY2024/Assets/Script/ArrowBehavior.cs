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
            /*// Assuming you have references to the parent and child objects
            Transform parentTransform = collision.transform;
            Transform childTransform = gameObject.transform;

            // Get the child's local position, rotation, and scale
            Vector3 childLocalPosition = childTransform.localPosition;
            Quaternion childLocalRotation = childTransform.localRotation;
            Vector3 childLocalScale = childTransform.localScale;

            // Set the parent of the child object           

            // Reset the child's local position, rotation, and scale to maintain its properties
            childTransform.localPosition = childLocalPosition;
            childTransform.localRotation = childLocalRotation;
            childTransform.localScale = childLocalScale;*/

            gameObject.transform.SetParent(collision.gameObject.transform);

            c2d.enabled = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.velocity = Vector3.zero;



        }
    }
}
