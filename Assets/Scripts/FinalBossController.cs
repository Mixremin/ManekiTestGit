using UnityEngine;
using System.Collections.Generic;
using System;

public class FinalBossController : MonoBehaviour
{
    [Header("Boss prefabs")]
    [SerializeField] private GameObject boss1Prefab;
    [SerializeField] private GameObject boss2Prefab;

    [Header("Boss spawn point")]
    [SerializeField] private Transform boss1SpawnPoint;
    [SerializeField] private Transform boss2SpawnPoint;

    [Header("Blocks")]
    [SerializeField] private StepOnBlock activatingBlock;
    [SerializeField] private ChestBlock chestBlock;

    private BossEnemy boss1;
    private BossEnemy boss2;

    public Action OnBossFightWon;
    public Action OnBossFightLost;
    public Action OnBossFightStarted;

    private void Start()
    {
        if (activatingBlock != null)
        {
            activatingBlock.OnSteppedOn += StartBossFight;
        }
    }

    private void StartBossFight()
    {
        OnBossFightStarted?.Invoke();
        var boss1Obj = Instantiate(boss1Prefab, boss1SpawnPoint.position, Quaternion.identity);
        var boss2Obj = Instantiate(boss2Prefab, boss2SpawnPoint.position, Quaternion.identity);
        
        boss1 = boss1Obj.GetComponent<BossEnemy>();
        boss2 = boss2Obj.GetComponent<BossEnemy>();
        
        GameController.Instance?.ChangeSwipeMode(ESwipeMode.ATTACK);

        boss1.OnBossEnemyDied += RemoveBossEnemy;
        boss2.OnBossEnemyDied += RemoveBossEnemy;

        boss1.MoveTowardsPlayer();
        boss2.MoveTowardsPlayer();
    }

    public void StopBossFight()
    {
        if (boss1 != null) {
            boss1.NoMove();
            boss1.OnBossEnemyDied -= RemoveBossEnemy;
            boss1 = null;
        }
        if (boss2 != null) {
            boss2.NoMove();
            boss2.OnBossEnemyDied -= RemoveBossEnemy;
            boss2 = null;
        }
    }

    private void RemoveBossEnemy(BossEnemy boss) {
        if (boss == boss1) {
            boss1.OnBossEnemyDied -= RemoveBossEnemy;
            boss1 = null;
        } else if (boss == boss2) {
            boss2.OnBossEnemyDied -= RemoveBossEnemy;
            boss2 = null;
        }
        CheckForBossFightEnd();
    }

    private void CheckForBossFightEnd() {
        if (boss1 == null && boss2 == null) {
            WinBossFight();
        }
    }

    private void WinBossFight()
    {
        StopBossFight();
        chestBlock.SpawnChestObject();
        OnBossFightWon?.Invoke();
    }

    private void OnDestroy()
    {
        if (activatingBlock != null)
        {
            activatingBlock.OnSteppedOn -= StartBossFight;
        }
    }
}