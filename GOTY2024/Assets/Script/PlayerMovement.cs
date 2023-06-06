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
    [SerializeField] bool canJump;
    [SerializeField] float jumpCoolDown = 1.5f;
    [SerializeField] float jumpTimer;
    public float moveSpeed = 5;
    public float jumpForce = 5;

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
        Turn();
        if (!Input.GetButton("Fire1"))
        {
            Disarm() ;
            if (canMove)
            {
                Move();
            }

            if (Input.GetKeyDown(KeyCode.W) && canJump)
            {
                Jump();
            }
            if (Input.GetButtonDown("Jump") && canDodge)
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
            Debug.Log("FLIPOU");
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
