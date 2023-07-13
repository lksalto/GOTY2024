using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] int life = 3;

    public void TakeHit(int dmg)
    {
        life -= dmg;
        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }

}
