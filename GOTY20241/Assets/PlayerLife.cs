using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    int maxLife;
    [SerializeField] int life = 5;
    public bool canBeHit;
    ResetScene rs;
    [SerializeField] List<GameObject> lifeImg;
    private void Start()
    {
        maxLife = life;
        canBeHit = true;
    }
    public void TakeHit(int dmg)
    {
        if(canBeHit)
        {
            
            life = life - dmg;
            if (life <= 0)
            {
                Destroy(gameObject);
                StartCoroutine(ResetScene());
            }
            ShowLife(life);
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
    public int GetLife()
    {
        return life;
    }

    public void AddLife(int qtt)
    {
        if(life + qtt <= maxLife)
        {
            life += qtt;
        }
        else
        {
            life = maxLife;
        }
        ShowLife(life);
    }
    public void ShowLife(int life)
    {
        for (int i = maxLife; i > 0; i--)
        {
            if (i <= life)
            {
                lifeImg[i - 1].SetActive(true);
            }
            else
            {
                lifeImg[i - 1].SetActive(false);
            }
        }
    }
}
