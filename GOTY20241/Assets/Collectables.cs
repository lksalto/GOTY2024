using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    [SerializeField] int type; // 1-life, 2-arrow
    [SerializeField] int qtt;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //arrow
            if(type == 1)
            {
                PlayerShoot playerArrows = collision.gameObject.GetComponent<PlayerShoot>();
                playerArrows.SetAmmo(0);
                Debug.Log("ammo");
            }
            //life
            else if(type == 2)
            {
                PlayerLife playerLife = collision.gameObject.GetComponent<PlayerLife>();
                playerLife.AddLife(qtt);
                Debug.Log("life");
            }
        }
        Destroy(gameObject);
    }

}
