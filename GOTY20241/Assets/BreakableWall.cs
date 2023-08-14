using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyBoar>() != null)
        {
            if (Mathf.Abs(collision.gameObject.GetComponent<Rigidbody2D>().velocity.x) > 2.5f)
            {
                collision.gameObject.GetComponent<EnemyBoar>().RechargeAttack(collision.gameObject.GetComponent<EnemyBoar>().atkCooldown);
                Destroy(gameObject,0.1f);
            }
        }
    }
}
