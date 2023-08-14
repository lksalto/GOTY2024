using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoar : MonoBehaviour
{
    //public Animator cameraAnim;

    [Header("Enemy Param")]
    [SerializeField] float enemyRange = 0;
    public float atkCooldown = 1.2f;
    [SerializeField] float knockbackForce = 15f;
    [SerializeField] float stunDuration = 0.3f;
    [SerializeField] int dmg = 1;
    [SerializeField] float acceleration = 0.5f;
    [SerializeField] float topSpeed;
    [SerializeField] AudioClip cabecada;
    AudioSource aud;
    public float enemySpeed;
    float initialSpeed;

    
    enum BoarState
    {
        IDLE = 1,
        CHARGING = 2,
        STUNNED = 3

    };
    float atkTimer;
    [SerializeField] SpriteRenderer sr;
    GameObject target;
    Rigidbody2D rb;

    [Header("Gizmo Parameters")]
    [SerializeField] Vector2 detectorSize = Vector2.one;
    [SerializeField] float detectorDelay = 0.1f;
    [SerializeField] LayerMask detectorLayerMask;
    [SerializeField] GameObject particles;
    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmos = true;
    public bool playerDetected = false;
    public Vector2 offSet = Vector2.zero;
    BoarState state;
    void Awake()
    {
        state = BoarState.IDLE;
        rb = GetComponent<Rigidbody2D>();
        aud = GetComponent<AudioSource>();
        //cameraAnim = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
        StartCoroutine(DetectionCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float clampedX = Mathf.Clamp(rb.position.x, leftBoundary.position.x, rightBoundary.position.x);
        //rb.position = new Vector2(clampedX, rb.position.y);
        //transform.position = new Vector2(clampedX, transform.position.y);
        ManageCooldowns();
        ChargePlayer();
        
    }


    //----------------------------TUDO VAI SER COMUM AOS INIMIGOS (MAS N SEI A MELHOR FORMA DE FAZER ISSO) 



    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectorDelay);
        if (state != BoarState.STUNNED)
        {
            PerformDetection();
        }
        StartCoroutine(DetectionCoroutine());
    }

    public void PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapBox((Vector2)transform.position + offSet, detectorSize, 0, detectorLayerMask);
        if (collider != null)
        {
            playerDetected = true;
            target = collider.gameObject;
            state = BoarState.CHARGING;
            particles.SetActive(true);
        }
        else
        {
            playerDetected = false;
            target = null;
            state = BoarState.IDLE;
            particles.SetActive(false);
        }
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

    public void ManageCooldowns()
    {
        atkTimer += Time.deltaTime;
    }

    IEnumerator StunPlayer(PlayerMovement player)
    {
        player.canMove = false;
        yield return new WaitForSeconds(stunDuration);
        player.canMove = true;
    }

    //----------------------------TUDO VAI SER COMUM AOS INIMIGOS (MAS N SEI A MELHOR FORMA DE FAZER ISSO) 

    public void ChargePlayer()
    {
        if (state != BoarState.STUNNED)
        {
            if (playerDetected)
            {
                if (enemySpeed <= topSpeed)
                {
                    enemySpeed += acceleration * Time.deltaTime;
                }
                if (Vector2.Distance(rb.position, target.transform.position) > enemyRange)
                {
                    Vector2 moveDirection = new Vector2(target.transform.position.x - rb.position.x, 0f);
                    sr.flipX = target.transform.position.x < transform.position.x;
                    rb.AddForce(moveDirection.normalized * acceleration * 15 * Time.deltaTime, ForceMode2D.Impulse);

                    if (Mathf.Approximately(target.transform.position.x, rb.position.x))
                    {
                        if (Mathf.Approximately(target.transform.position.y, rb.position.y + 0.493f))
                        {
                            {
                                target.GetComponent<Rigidbody2D>().AddForce(Vector2.right, ForceMode2D.Impulse);
                               
                            }
                        }
                            

                        StartCoroutine(TurnAround(atkCooldown / 2));
                    }
                }
            }


        }
        
    }
    

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (state != BoarState.STUNNED)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerLife pLife = collision.gameObject.GetComponent<PlayerLife>();
                if (pLife.canBeHit)
                {
                    pLife.TakeHit(dmg);
                    pLife.canBeHit = false;
                }
                StartCoroutine(StunPlayer(collision.gameObject.GetComponent<PlayerMovement>()));
                Rigidbody2D rbb = collision.gameObject.GetComponent<Rigidbody2D>();
                Vector2 v2 = rbb.velocity;
                rbb.velocity = Vector2.zero;
                //Vector2 knockbackDirection = (collision.gameObject.transform.position - gameObject.transform.position).normalized;
                //rbb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                if(collision.transform.position.x < gameObject.transform.position.x)
                {
                    rbb.AddForce((Vector2.left + Vector2.up) * knockbackForce, ForceMode2D.Impulse);
                }
                else
                {
                    rbb.AddForce((Vector2.right + Vector2.up) * knockbackForce, ForceMode2D.Impulse);
                }
                StartCoroutine(RechargeAttack(atkCooldown));

                
                //cameraAnim.SetTrigger("Shake");
            }
        }
    }

    public IEnumerator RechargeAttack(float cd)
    {
        if(Random.Range(0,10)> 8)
        {
            aud.PlayOneShot(cabecada);
        }
 
        state = BoarState.STUNNED;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(cd);
        state = BoarState.IDLE;
    }

    IEnumerator TurnAround(float cd)
    {
        state = BoarState.STUNNED;
        enemySpeed = 0.5f;
        rb.velocity = Vector2.Lerp(rb.velocity, rb.velocity / 2f, 0.5f);
        yield return new WaitForSeconds(cd);
        state = BoarState.IDLE;
        //enemySpeed = initialSpeed;
    }

}
