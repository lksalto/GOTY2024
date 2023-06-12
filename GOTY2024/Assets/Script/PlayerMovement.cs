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
    [SerializeField] float jumpForce;
    [SerializeField] int jumpCounter = 1;
    [SerializeField] int maxJumpCounter = 1;
    public float moveSpeed = 5;
    public float fJumpForce = 5;

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
    //Debugger
    [SerializeField] bool debugColor;
    [SerializeField] float comp;
    [SerializeField] float rayX;
    [SerializeField] bool rocketJump = true;
    [SerializeField] int rocketCount;
    [SerializeField] int rocketMax;
    [SerializeField] SpriteRenderer sprite;


    // Start is called before the first frame update
    void Start()
    {
        originalTransform = hands.transform;
        myPlayer = gameObject;
        jumpTimer = jumpCoolDown;
        rb = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CapsuleCollider2D>();
        handsSprites = hands.GetComponent<SpriteRenderer>();
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
            Disarm() ;
            if (canMove)
            {
                Move();
            }

            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && canDodge)
            {
                StartCoroutine(Dodge());
            }
        }
        else
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
        if(jumpCounter > 0)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0f, fJumpForce), ForceMode2D.Impulse);
            jumpCounter--;
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
            //if (hit.collider.CompareTag("Floor")|| )
            //{

                if (transform.position.y > hit.collider.transform.position.y)
                    isGrounded = true;

                //Debug.Log("mid");
                //bCanJump = true;
                //rocketJump = true;
            //}

        }
        for (int i = 0; i < hitsL.Length; i++)
        {
            RaycastHit2D hitL = hitsL[i];
            //if (hitL.collider.CompareTag("Floor"))
            //{

                if (transform.position.y > hitL.collider.transform.position.y + hitL.collider.transform.localScale.y / 2)
                    isGrounded = true;

                //Debug.Log("left");
                //bCanJump = true;
                //rocketJump = true;

            //}

        }
        for (int i = 0; i < hitsR.Length; i++)
        {
            RaycastHit2D hitR = hitsR[i];
            //if (hitR.collider.CompareTag("Floor"))
            //{

                if (transform.position.y > hitR.collider.transform.position.y + hitR.collider.transform.localScale.y / 2)
                    isGrounded = true;

                //Debug.Log("right");
                //bCanJump = true;
                //rocketJump = true;
            //}
        }
        if (debugColor)
        {
            if(jumpCounter == maxJumpCounter)
            {
                sr.color = Color.green;
            }
            else if(jumpCounter > 0)
            {
                sr.color = Color.yellow;
            }
            else
            {
                sprite.color = Color.red;
            }
            
        }


        bCanJump = isGrounded;
        if (bCanJump)
        {
            canDodge = true;
            jumpCounter = maxJumpCounter;
        }
        rocketJump = !isGrounded;
    }
    IEnumerator Dodge()
    {
        sr.color = Color.yellow;
        canDodge = false;
        canMove = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        //rb.isKinematic = true;
        //cc2d.enabled = false;
        rb.AddForce(new Vector2(moveDirection, 0) * jumpForce/1.7f, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(dodgeDuration);
        rb.gravityScale = 1;
        sr.color = Color.white;
        canMove = true;
        
        //cc2d.enabled = true;
        //rb.isKinematic = true

    }

    private void ManageCooldowns()
    {
        //dodgeTimer -= Time.deltaTime;
        //canDodge = dodgeTimer < 0;
        jumpTimer -= Time.deltaTime;
    }

    private void Aim()
    {
        hands.SetActive(true);
        sr.sprite = playerSprites[1];
        hands.GetComponent<SpriteRenderer>().sprite = armDisarm[1];
        dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        rotationZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //hands.transform.localScale = new Vector3(myPlayer.transform.localScale.x, 1, 1);
        hands.transform.rotation = rotation;
        handsSprites.flipX = (angle > 90 || angle < -90);
    }

    private void Disarm()
    {
        hands.SetActive(false);
        sr.sprite = playerSprites[0];
        hands.GetComponent<SpriteRenderer>().sprite = armDisarm[0];
        
        dir = Vector2.down;
        angle = -90 * Mathf.Rad2Deg;
        rotation = Quaternion.AngleAxis(angle,new Vector3(0,0,1));
        angleAim = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        dirAim = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //rotationZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //hands.transform.localScale = new Vector3(myPlayer.transform.localScale.x, 1, 1);
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
