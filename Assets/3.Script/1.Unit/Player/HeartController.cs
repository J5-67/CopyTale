using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    [SerializeField] private GameObject Heart;
    private BattleManager battleManager;
    private Movement2D movement2D;

    private void Awake()
    {
        TryGetComponent(out movement2D);
    }

    private void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        movement2D.MoveTo(new Vector3(inputX, inputY, 0f));
    }

    public void SetBattleManager(BattleManager manager)
    {
        battleManager = manager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBulletController bullet = collision.GetComponent<EnemyBulletController>();

        if(bullet != null)
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage(bullet.BulletDamage);

                Destroy(collision.gameObject);
            }
        }
    }
}
