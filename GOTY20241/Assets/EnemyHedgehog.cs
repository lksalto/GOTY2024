using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHedgehog : MonoBehaviour
{
    [SerializeField] int life = 1;
    [SerializeField] int knockbackForce = 7;
    [SerializeField] List<Transform> points;
    [SerializeField] float speed = 4f;
    [SerializeField] List<Vector3> positions;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        foreach(Transform t in points)
        {
            positions.Add(t.position);
        }
        transform.position = positions[0];

    }

    // Update is called once per frame
    void Update()
    {
        if (i > points.Count - 1)
        {
            i = 0;
        }
        transform.position = Vector3.MoveTowards(transform.position, positions[i], speed * Time.deltaTime);

        if (transform.position == positions[i])
        {
            i++;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerLife pLife = collision.gameObject.GetComponent<PlayerLife>();
            if (pLife.canBeHit)
            {
                pLife.TakeHit(1);
                pLife.canBeHit = false;
            }
            StartCoroutine(StunPlayer(collision.gameObject.GetComponent<PlayerMovement>()));
            Rigidbody2D rbb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 v2 = rbb.velocity;
            rbb.velocity = Vector2.zero;
            //Vector2 knockbackDirection = (collision.gameObject.transform.position - gameObject.transform.position).normalized;
            //rbb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            if (collision.transform.position.x < gameObject.transform.position.x)
            {
                rbb.AddForce((Vector2.left + Vector2.up) * knockbackForce, ForceMode2D.Impulse);
            }
            else
            {
                rbb.AddForce((Vector2.right + Vector2.up) * knockbackForce, ForceMode2D.Impulse);
            }


            //cameraAnim.SetTrigger("Shake");
        }
    }
    IEnumerator StunPlayer(PlayerMovement player)
    {
        player.canMove = false;
        yield return new WaitForSeconds(0.5f);
        player.canMove = true;
    }

    

}
