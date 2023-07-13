using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] int life = 5;
    public bool canBeHit;
    ResetScene rs;
    [SerializeField] List<GameObject> lifeImg;
    private void Start()
    {
        canBeHit = true;
    }
    public void TakeHit(int dmg)
    {
        if(canBeHit)
        {
            for(int i = 5; i > life - dmg; i--)
            {
                if(i>=1)
                {
                    lifeImg[i-1].SetActive(false);
                }
            }
            life = life - dmg;
            if (life <= 0)
            {
                Destroy(gameObject);
                StartCoroutine(ResetScene());
            }
            StartCoroutine(Invencibility());
        }


    }
    IEnumerator Invencibility()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.6f);
        canBeHit = true;
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
    IEnumerator ResetScene()
    {

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
