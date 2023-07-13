using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public float transitionDuration = 2f;
    bool isDestroyed = false;
    private Color initialColor;
    private void Awake()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
        // Start the color transition coroutine
        
    }

    private void Update()
    {
        if(isDestroyed)
        {
            StartCoroutine(BreakPlatform());
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && Mathf.Approximately(collision.gameObject.GetComponent<Rigidbody2D>().velocity.y ,0))
        {

            isDestroyed = true;
        }
    }

    IEnumerator BreakPlatform()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            // Calculate the lerped color between initialColor and black
            Color lerpedColor = Color.Lerp(initialColor, Color.black, elapsedTime / transitionDuration);

            // Assign the lerped color to the SpriteRenderer
            spriteRenderer.color = lerpedColor;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the final color is set to black
        spriteRenderer.color = Color.black;
        Destroy(gameObject, 0.5f);
    }
}
