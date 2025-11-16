using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDamageCal : MonoBehaviour
{
    private float width;
    private Animator weaponanimator;
    private Movement2D movement2D;
    private SpriteRenderer spriteRenderer;
    private WaitForSeconds wfs = new WaitForSeconds(0.1f);
    private Vector3 targetBarPos = new Vector3(-5.7f, -1.3f);

    [Header("참조")]
    [SerializeField] private GameObject weaponObj;
    [SerializeField] private GameObject target_bar;
    [SerializeField] private BoxCollider2D target_border;
    [SerializeField] private BattleManager battleManager;

    [Header("스프라이트 설정")]
    [SerializeField]
    private Sprite originalSprite;
    [SerializeField]
    private Sprite highlightSprite;

    private void Start()
    {
        TryGetComponent(out movement2D);
        TryGetComponent(out spriteRenderer);
        weaponObj.TryGetComponent(out weaponanimator);
        target_bar.transform.position = targetBarPos;
        width = target_border.size.x * 240;
    }

    private void Update()
    {
        if (target_bar.transform.position.x <= -5.8f)
        {
            movement2D.MoveTo(new Vector3(1f, 0f, 0f));
        }

        if (target_bar.transform.position.x >= 5.8f)
        {
            movement2D.MoveTo(new Vector3(-1f, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteAttack();
        }
    }

    private void OnDisable()
    {
        if (movement2D.Move_Speed <= 0)
        {
            movement2D.Move_Speed = 10;
        }

        target_bar.transform.position = targetBarPos;
    }

    private void ExecuteAttack()
    {
        movement2D.Move_Speed = 0;

        StartCoroutine(SetSprite_co(originalSprite, highlightSprite));

        StartCoroutine(WeaponAnimator_co());

        float damage = TotalDamage();

        if (battleManager != null)
        {
            battleManager.AttackDamage(TotalDamage());
        }
    }

    private IEnumerator SetSprite_co(Sprite originalSprite, Sprite highlightSprite)
    {
        for (int i = 0; i < 8; i++)
        {
            spriteRenderer.sprite = highlightSprite;
            yield return wfs;

            spriteRenderer.sprite = originalSprite;
            yield return wfs;
        }
    }

    private IEnumerator WeaponAnimator_co()
    {
        Vector3 enemyPos = GameObject.FindGameObjectWithTag("Flowey").transform.position;

        weaponObj.transform.position = enemyPos;

        weaponObj.SetActive(true);

        weaponanimator.SetTrigger("PlayerAttack");

        yield return new WaitForSeconds(1f);

        weaponObj.SetActive(false);

        battleManager.StartEnemyTurn();

        this.enabled = false;
    }

    public float TotalDamage()
    {
        float PlayerAttack = GameManager.Instance.PlayerAttack;

        float maxDistanceFromCenter = width / 2.0f;

        float distanceToCenter = Mathf.Abs(target_bar.transform.position.x - 0.0f);

        float clampedDistance = Mathf.Min(distanceToCenter, maxDistanceFromCenter);

        float distanceRatio = clampedDistance / maxDistanceFromCenter;

        float closenessRatio = 1.0f - distanceRatio;

        float damagePercent = closenessRatio * 100f;

        float damageRatio = damagePercent / 100f;

        float totalDamage = Mathf.RoundToInt(PlayerAttack * damageRatio);

        return totalDamage;
    }

    
}
