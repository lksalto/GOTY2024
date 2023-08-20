using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Aiming
    GameObject myPlayer;
    Vector2 dir;
    Vector2 dirAim;
    float angle;
    float angleAim;
    Quaternion rotation;
    float rotationZ;
    [SerializeField] GameObject hands;
    Transform originalTransform;
    [SerializeField] List<Sprite> armDisarm;
    [SerializeField] List<Sprite> playerSprites;
    [SerializeField] SpriteRenderer handsSprites;
    //Jump
    [SerializeField] bool bCanJump;
    [SerializeField] float jumpCoolDown = 1.5f;
    [SerializeField] float jumpTimer;
    [SerializeField] int jumpCounter = 1;
    [SerializeField] int maxJumpCounter = 1;
    public float moveSpeed = 5;
    public float fJumpForce = 5;
    float initialGravityScale;
    float moveDirection;

    Rigidbody2D rb;
    CapsuleCollider2D cc2d;
    [SerializeField] SpriteRenderer sr;

    //Dodge
    public bool canDodge = false;
    public bool canMove = true;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1;
    public float dodgeTimer = 0;
    PlayerLife pLife;
    [SerializeField] List<LayerMask> dodgeLayers;
    //Debugger
    [SerializeField] bool debugColor;
    [SerializeField] bool isOnPlatform;
    [SerializeField] float comp;
    [SerializeField] float rayX;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] float platformTimer = 0.3f;
    


    // Start is called before the first frame update
    void Start()
    {
        originalTransform = hands.transform;
        myPlayer = gameObject;
        jumpTimer = jumpCoolDown;
        rb = GetComponent<Rigidbody2D>();
        initialGravityScale = rb.gravityScale;
        cc2d = GetComponent<CapsuleCollider2D>();
        handsSprites = hands.GetComponent<SpriteRenderer>();
        pLife = GetComponent<PlayerLife>();
        //sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageCooldowns();
        Debugger();
        Turn();
        if (!Input.GetButton("Fire1"))
        {
            if (canMove)
            {
                Disarm();
                Move();
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Jump();
                }
                if (Input.GetKeyDown(KeyCode.Mouse1) && canDodge)
                {
                    StartCoroutine(DodgeCooldown());
                }
            }
        }
        else if (Input.GetButton("Fire1") && canMove)
        {
            Aim();
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

    }

    private void Move()
    {

        moveDirection = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

    }
    void Jump()
    {
        if(bCanJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, fJumpForce), ForceMode2D.Impulse);
        }

    }

    void Debugger()
    {
        RaycastHit2D[] hits;
        RaycastHit2D[] hitsL;
        RaycastHit2D[] hitsR;
        hits = Physics2D.RaycastAll(transform.position, -transform.up, comp);
        hitsL = Physics2D.RaycastAll(new Vector2(transform.position.x - rayX, transform.position.y), -transform.up, comp);
        hitsR = Physics2D.RaycastAll(new Vector2(transform.position.x + rayX, transform.position.y), -transform.up, comp);
        bool isGrounded = false;
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), -transform.up * comp);
        Debug.DrawRay(new Vector2(transform.position.x - rayX, transform.position.y), -transform.up * comp);
        Debug.DrawRay(new Vector2(transform.position.x + rayX, transform.position.y), -transform.up * comp);

        // For each object that the raycast hits.
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider.CompareTag("Floor"))
            {

                if (transform.position.y > hit.point.y)
                    isGrounded = true;
            }
            if(hit.collider.CompareTag("Platform"))
            {
                isOnPlatform = true;
                if (Input.GetKey(KeyCode.S))
                {

                    StartCoroutine(DisablePlatform(hit.collider.gameObject.GetComponent<BoxCollider2D>()));
 
                }
            }

        }
        for (int i = 0; i < hitsL.Length; i++)
        {
            RaycastHit2D hitL = hitsL[i];
            if (hitL.collider.CompareTag("Floor") || hitL.collider.CompareTag("Platform"))
            {
                if (transform.position.y > hitL.point.y)
                    isGrounded = true;

            }
        }
        for (int i = 0; i < hitsR.Length; i++)
        {
            RaycastHit2D hitR = hitsR[i];
            if (hitR.collider.CompareTag("Floor") || hitR.collider.CompareTag("Platform"))
            {

            if (transform.position.y > hitR.point.y)
                    isGrounded = true;

            }
        }
        if (debugColor)
        {
            if(isGrounded)
            {
                sr.color = Color.green;
            }
            else
            {
                sprite.color = Color.red;
            }
            
        }
        bCanJump = isGrounded;
    }

    IEnumerator DisablePlatform(BoxCollider2D bc)
    {
        Physics2D.IgnoreCollision(bc, gameObject.GetComponent<BoxCollider2D>(), true);
        yield return new WaitForSeconds(platformTimer);
        Physics2D.IgnoreCollision(bc, gameObject.GetComponent<BoxCollider2D>(), false);
    }
    IEnumerator DodgeCooldown()
    {
        canDodge = false;
        StartCoroutine(Dodge());
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }



    IEnumerator Dodge()
    {
        pLife.canBeHit = false;
        Physics2D.IgnoreLayerCollision(10, 6, true);
        Physics2D.IgnoreLayerCollision(10, 11, true);
        sr.color = Color.yellow;
        if(bCanJump)
        {
            canMove = false;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(moveDirection, 0) * fJumpForce * 2f, ForceMode2D.Impulse);
        }
        
        yield return new WaitForSeconds(dodgeDuration);
        rb.gravityScale = initialGravityScale;
        sr.color = Color.white;
        canMove = true;
        pLife.canBeHit = true;
        Physics2D.IgnoreLayerCollision(10, 6, false);
        Physics2D.IgnoreLayerCollision(10, 11, false);
    }

    private void ManageCooldowns()
    {
        jumpTimer -= Time.deltaTime;
    }

    private void Aim()
    {
        hands.SetActive(true);
        sr.sprite = playerSprites[1];
        hands.GetComponent<SpriteRenderer>().sprite = armDisarm[1];
        dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rotationZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        hands.transform.rotation = rotation;
        //handsSprites.flipy = (angle > 90 || angle < -90);
    }

    private void Disarm()
    {
        hands.SetActive(false);
        sr.sprite = playerSprites[0];
        hands.GetComponent<SpriteRenderer>().sprite = armDisarm[0];
        dir = Vector2.down;
        angle = Mathf.Rad2Deg;
        rotation = Quaternion.AngleAxis(angle,new Vector3(0,0,1));
        angleAim = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        dirAim = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (angleAim > 90 || angleAim < -90)
        {
            handsSprites.flipY = true;
        }
        hands.transform.rotation = rotation;
    }
    private void Turn()
    {
        dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        sr.flipX = (angle > 90 || angle < -90);
    }

    public void ResetTransform()
    {
        // Reset the position, rotation, and scale to their original values
        transform.position = originalTransform.position;
        transform.rotation = originalTransform.rotation;
        transform.localScale = originalTransform.localScale;
    }

}
