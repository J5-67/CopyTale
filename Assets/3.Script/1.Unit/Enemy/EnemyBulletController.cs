using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    [SerializeField]
    private int bulletDamage = 3;

    public int BulletDamage { get; set; }

    private void Start()
    {
        if(BulletDamage == 0)
        {
            BulletDamage = bulletDamage;
        }
    }
}
