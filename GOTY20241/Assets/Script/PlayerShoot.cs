using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 50;
    [SerializeField] TextMeshProUGUI arrowText;
    public LineRenderer lineRenderer;
    public GameObject projectilePrefab;
    public float maxDistance = 100f;
    public LayerMask floorLayerMask;
    public int maxArrows = 10;
    public int arrowCount;
    Vector3 closestHitPoint = Vector3.zero;
    Quaternion rotation;
    Vector3 endPoint;
    
    bool isHitting;
    bool showLine;
    void Start()
    {
        showLine = true;
        lineRenderer.enabled = false;
        arrowCount = maxArrows;
        arrowText.text = arrowCount.ToString() + "/" + maxArrows.ToString();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            showLine = !showLine;
        }

        if (Input.GetButton("Fire1"))
        {
            CastRayFromPlayerToMouse();
            //StartCoroutine(DestroyLineRenderer());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if(arrowCount > 0)
            {
                //Instantiate(projectilePrefab, closestHitPoint, rotation);
                if (!isHitting)
                {
                    GameObject bullet = Instantiate(projectilePrefab, transform.position, rotation);
                    bullet.GetComponent<Rigidbody2D>().AddForce((endPoint - bullet.transform.position).normalized * bulletSpeed, ForceMode2D.Impulse);

                }
                else
                {
                    GameObject bullet = Instantiate(projectilePrefab, transform.position, rotation);
                    bullet.GetComponent<Rigidbody2D>().AddForce((closestHitPoint - bullet.transform.position).normalized * bulletSpeed, ForceMode2D.Impulse);
                    
                }
                arrowCount--;
                ShowArrowCount();
            }
            StartCoroutine(DestroyLineRenderer());


        }

    }

    void CastRayFromPlayerToMouse()
    {
        // Get the position of the mouse cursor in screen space
        Vector3 mousePosition = Input.mousePosition;
        Vector3 angleVect;
        float angle;
        // Cast a ray from the player's position to the mouse position
        Vector3 origin = transform.position;
        Vector3 direction = (mousePosition - Camera.main.WorldToScreenPoint(origin)).normalized;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, maxDistance, floorLayerMask);

        // Set the starting point of the line renderer to the player's position
        lineRenderer.SetPosition(0, origin);

        if (hits.Length > 0)
        {
            isHitting = true;
            
            float closestDistance = Mathf.Infinity;
            
            Vector2 closestHitNormal = Vector2.zero;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];

                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    closestHitPoint = hit.point;
                    closestHitNormal = hit.normal;
                }
            }

            lineRenderer.SetPosition(1, closestHitPoint);
            GameObject hitObject = hits[0].collider.gameObject;
            angleVect = transform.position - closestHitPoint;
            angle = Mathf.Atan2(angleVect.y, angleVect.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
            Vector3 hitPoint = closestHitPoint;

        }
        else
        {
            isHitting = false;
            angleVect = transform.position - endPoint;
            angle = Mathf.Atan2(angleVect.y, angleVect.x) * Mathf.Rad2Deg;
            // Spawn the projectile at the hit point with the fixed angle
            rotation = Quaternion.Euler(0f, 0f, angle);
            // If the ray doesn't hit anything, set the ending point of the line renderer to the maximum distance
            endPoint = origin + direction * maxDistance;
            lineRenderer.SetPosition(1, endPoint);
        }

        // Enable the line renderer
        if(showLine)
        {
            lineRenderer.enabled = true;
        }
        
    }

    IEnumerator DestroyLineRenderer()
    {
        yield return new WaitForSeconds(0f);
        lineRenderer.enabled = false;
    }

    public int GetAmmo()
    {
        return arrowCount;
    }
    public void SetAmmo(int qtt)
    {
        int max = maxArrows;
        arrowCount = max;
        ShowArrowCount();
    }
    public void ShowArrowCount()
    {
        arrowText.text = arrowCount.ToString() + "/" + maxArrows.ToString();
    }

}
