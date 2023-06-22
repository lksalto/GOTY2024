using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] GameObject bridge;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Arrow"))
        {
            bridge.transform.SetParent(null);
            bridge.GetComponent<Rigidbody2D>().isKinematic = false;
            bridge.GetComponent<Rigidbody2D>().mass = 90f;
            Destroy(gameObject);
        }
    }




}
