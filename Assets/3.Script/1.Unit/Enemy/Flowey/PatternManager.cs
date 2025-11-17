using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager : MonoBehaviour
{
    private BattleManager battleManager;
    private EnemyData currentEnemyData;
    private Coroutine currentPatternCoroutine;

    private List<GameObject> activeBullet = new List<GameObject>();

    private void Start()
    {
        TryGetComponent(out battleManager);
    }

    public void StartPattern(EnemyData enemyData, Vector3 bulletSpawnPosition)
    {
        this.currentEnemyData = enemyData;

        if (currentPatternCoroutine != null)
        {
            StopCoroutine(currentPatternCoroutine);
        }

        if (enemyData.EnemyPatterns.Count > 0)
        {
            int rnd = Random.Range(0, enemyData.EnemyPatterns.Count);

            EnemyPattern selectedPattern = enemyData.EnemyPatterns[rnd];

            currentPatternCoroutine = StartCoroutine(ExecutePattern(selectedPattern, bulletSpawnPosition));
        }
    }

    public void StopMonsterAttack()
    {
        if (currentPatternCoroutine != null)
        {
            StopCoroutine(currentPatternCoroutine);

            currentPatternCoroutine = null;
        }

        foreach (GameObject bullets in activeBullet)
        {
            Destroy(bullets);
        }

        activeBullet.Clear();
    }

    private IEnumerator ExecutePattern(EnemyPattern patternData, Vector3 bulletSpawnPosition)
    {
        float startTime = Time.time;

        while (Time.time < startTime + patternData.Duration)
        {
            for (int i = 0; i < patternData.BulletCount; i++)
            {
                GameObject newBullet = Instantiate(patternData.BulletPrefab, bulletSpawnPosition, Quaternion.identity);

                if(newBullet.TryGetComponent(out EnemyBulletController bulletController))
                {
                    bulletController.BulletDamage = currentEnemyData.RealEnemyAttack;
                }   

                activeBullet.Add(newBullet);

                Movement2D movementScript = newBullet.GetComponent<Movement2D>();

                if (movementScript != null)
                {
                    Vector3 direction;

                    float totalAngle = (patternData.BulletCount - 1) * patternData.AngleOffset;

                    float startAngle = -totalAngle / 2f;

                    float angle = startAngle + (i * patternData.AngleOffset);

                    direction = Quaternion.Euler(0, 0, angle) * Vector3.down;

                    movementScript.Move_Speed = patternData.Speed;

                    movementScript.MoveTo(direction);
                }
            }

            yield return new WaitForSeconds(patternData.FireRate);
        }
        
        if(battleManager != null)
        {
            battleManager.EndEnemyTurn();
        }
    }
}