using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] bool canJump;
    [SerializeField] float jumpCoolDown = 1.5f;
    [SerializeField] float jumpTimer;
    public float moveSpeed = 5;
    public float jumpForce = 5;

    float moveDirection;

    Rigidbody2D rb;
    CapsuleCollider2D cc2d;
    [SerializeField] SpriteRenderer sr;

    public bool canDodge = false;
    public bool canMove = true;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1;
    public float dodgeTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        jumpTimer = jumpCoolDown;
        rb = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CapsuleCollider2D>();
        //sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageCooldowns();
        if(canMove)
        {
            Move();
        }
        
        if(Input.GetKeyDown(KeyCode.W) && canJump)
        {
            Jump();
        }
        if (Input.GetButtonDown("Jump") && canDodge)
        {
            StartCoroutine(Dodge());
        }
    }

    private void Move()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }
    private void Jump()
    {
        if (canJump)
            jumpTimer = jumpCoolDown;
            canJump = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    IEnumerator Dodge()
    {
        sr.color = Color.red;
        canDodge = false;
        canMove = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        //rb.isKinematic = true;
        //cc2d.enabled = false;
        rb.AddForce(new Vector2(moveDirection, 0) * jumpForce * 2, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(dodgeDuration);
        rb.gravityScale = 1;
        sr.color = Color.white;
        canMove = true;
        canDodge = true;
        //cc2d.enabled = true;
        //rb.isKinematic = true

    }

    private void ManageCooldowns()
    {
        dodgeTimer -= Time.deltaTime;
        canDodge = dodgeTimer < 0;
        jumpTimer -= Time.deltaTime;
        canJump = jumpTimer < 0;
    }
}
