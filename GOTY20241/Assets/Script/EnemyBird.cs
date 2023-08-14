using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBird : MonoBehaviour
{
    public enum BirdState {ROAMING, ATTACKING, RECHARGING};
   
    [SerializeField] int life = 1;
    [SerializeField] int knockbackForce = 2;
    [SerializeField] List<Transform> points;
    [SerializeField] float initialSpeed = 4f;
    [SerializeField] float atkCooldown = 3f;
    float speed;
    [SerializeField] List<Vector3> positions;
    int i;
    BirdState state;

    Transform target;
    Vector3 realTarget;
    GameObject player;


    [Header("Gizmo Parameters")]
    [SerializeField] Vector2 detectorSize = Vector2.one;
    [SerializeField] float detectorDelay = 0.1f;
    [SerializeField] LayerMask detectorLayerMask;
    //[SerializeField] GameObject particles;
    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmos = true;
    public bool playerDetected = false;
    public Vector2 offSet = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        state = BirdState.ROAMING;
        StartCoroutine(DetectionCoroutine());
        speed = initialSpeed;
        i = 0;
        foreach (Transform t in points)
        {
            positions.Add(t.position);
        }
        transform.position = positions[0];

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(state);
        if(state == BirdState.ROAMING || state == BirdState.RECHARGING)
        {
            
            Roam();
        }
        else if(state == BirdState.ATTACKING)
        {
            Attack();
        }
        
    }

    public void Roam()
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

    public void Attack()
    {
        if(realTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, realTarget, speed * 3 * Time.deltaTime);
            if (Mathf.Approximately(transform.position.x, realTarget.x) && Mathf.Approximately(transform.position.y, realTarget.y))
            {
                StartCoroutine(RechargeAttack(atkCooldown));
            }
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
        yield return new WaitForSeconds(0.1f);
        player.canMove = true;
    }

    IEnumerator DetectionCoroutine()
    {
        
        yield return new WaitForSeconds(detectorDelay);
        if (state == BirdState.ROAMING)
        {
            PerformDetection();
        }
        StartCoroutine(DetectionCoroutine());
    }

    public void PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapBox((Vector2)transform.position + offSet, detectorSize, 0, detectorLayerMask);
        if(state == BirdState.ROAMING)
        {
            
            if (collider != null && state == BirdState.ROAMING)
            {
                state = BirdState.ATTACKING;
                playerDetected = true;
                player = collider.gameObject;
                target = player.transform;
                realTarget = target.position;
                Debug.Log(collider.name);
                //particles.SetActive(true);
            }
            else if(collider == null)
            {
                
                Debug.Log("oi");
                //state = BirdState.ROAMING;
            }
        }
        
    }
    public IEnumerator RechargeAttack(float cd)
    {
        
        state = BirdState.RECHARGING;
        yield return new WaitForSeconds(cd);
        state = BirdState.ROAMING;

    }
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = gizmoIdleColor;
            if (playerDetected)
            {
                Gizmos.color = gizmoDetectedColor;

            }

            Gizmos.DrawCube((Vector2)transform.position + offSet, detectorSize);
        }
    }
}
