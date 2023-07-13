using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyArcher : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float detectorDelay = 0.1f;
    [SerializeField] float enemyRange = 0;
    [SerializeField] float atkCooldown = 3;
    [SerializeField] float knockbackForce = 15f;
    [SerializeField] float stunDuration = 0.05f;
    [SerializeField] float arrowSpeed = 10f;
    Quaternion rotation;
    Vector3 angleVect;
    float angle;
    Vector3 origin;
    Vector3 direction;

    enum ArcherState
    {
        IDLE = 1,
        AIMING = 2
    };
    ArcherState state;
    GameObject target;
    Rigidbody2D rb;

    [Header("Gizmo Parameters")]
    [SerializeField] Vector2 detectorSize = Vector2.one;
    
    [SerializeField] LayerMask detectorLayerMask;
    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmos = true;
    public bool playerDetected = false;
    public Vector2 offSet = Vector2.zero;
    float atkTimer;

    /*IEnumerator RechargeAttack(float cd)
    {
        state = BoarState.STUNNED;
        yield return new WaitForSeconds(cd);
        state = BoarState.IDLE;
    }*/

    private void Start()
    {
        state = ArcherState.IDLE;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DetectionCoroutine());
    }

    private void FixedUpdate()
    {
        ManageCooldowns();
        AttackPlayer();
    }


    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectorDelay);
        PerformDetection();
        StartCoroutine(DetectionCoroutine());
    }

    public void PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapBox((Vector2)transform.position + offSet, detectorSize, 0, detectorLayerMask);
        if (collider != null)
        {
            playerDetected = true;
            target = collider.gameObject;
        }
        else
        {
            playerDetected = false;
            target = null;
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
        atkTimer -= Time.deltaTime;
    }

    IEnumerator StunPlayer(PlayerMovement player)
    {
        player.canMove = false;
        yield return new WaitForSeconds(stunDuration);
        player.canMove = true;
    }

    //----------------------------TUDO VAI SER COMUM AOS INIMIGOS (MAS N SEI A MELHOR FORMA DE FAZER ISSO) 


    public void AttackPlayer()
    {
        if(atkTimer <= 0)
        {
            atkTimer = atkCooldown;
            if (playerDetected)
            {
                angleVect = transform.position - target.transform.position;
                angle = Mathf.Atan2(angleVect.y, angleVect.x) * Mathf.Rad2Deg;
                rotation = Quaternion.Euler(0f, 0f, angle);
                GameObject projectile = Instantiate(arrowPrefab, transform.position, rotation);
                Vector2 direction = (target.transform.position - transform.position).normalized;
                projectile.GetComponent<Rigidbody2D>().AddForce(direction/Vector2.Distance(gameObject.transform.position, target.transform.position) * arrowSpeed, ForceMode2D.Impulse);
            }
        }

    }
}
