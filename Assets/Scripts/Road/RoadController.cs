using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class RoadController : MonoBehaviour
{
    [Header("Road settings")]
    [SerializeField] private bool isRoadActive = false;

    [Header("Enemy prefabs")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private Transform leftSideSpawnPoint;
    [SerializeField] private Transform rightSideSpawnPoint;
    [Header("Cycle settings")]
    [SerializeField] private float cycleInterval = 0.8f;
    [SerializeField] private EDirection direction = EDirection.LEFT;

    [Header("Random enemies in line")]
    [Range(1, 3)]
    [SerializeField] private int enemiesInLine = 3;

    private List<EnemyController> enemies = new List<EnemyController>();

    private float cycleTimer = 0;
    private Vector3Int directionVector;
    private int enemiesInLineCounter = 0;
    private int waveWaitingCounter = 0;
    private Transform currentSpawnPoint;
    private Transform endSpawnPoint;
    private float distanceBetweenSpawnPoints;
    private float enemyMoveDuration;

    void Awake() {
        if(direction == EDirection.LEFT) {
            currentSpawnPoint = rightSideSpawnPoint;
            endSpawnPoint = leftSideSpawnPoint;
            //directionVector = Vector3Int.left;
        } else {
            currentSpawnPoint = leftSideSpawnPoint;
            endSpawnPoint = rightSideSpawnPoint;
            //directionVector = Vector3Int.right;
        }

        distanceBetweenSpawnPoints = Vector3.Distance(currentSpawnPoint.position, endSpawnPoint.position);
        enemyMoveDuration = distanceBetweenSpawnPoints * cycleInterval;
    }

    public void StartRoadSpawn() {
        isRoadActive = true;

        CheckForSpawn();
        //MoveEnemies();
    }

    public void StopRoadSpawn() {
        isRoadActive = false;
        for (int i = enemies.Count - 1; i >= 0; i--) {
            if (enemies[i] != null) {
                enemies[i].StopMoving();
            }
        }
    }

    public void ClearEnemies() {
        waveWaitingCounter = 0;
        enemiesInLineCounter = 0;
        cycleTimer = 0;
        var enemiesToClear = new List<EnemyController>(enemies);
        enemies.Clear();
        foreach (var enemy in enemiesToClear) {
            if (enemy != null) {
                enemy.OnEnemyDestroyed -= DestroyEnemy;
                Destroy(enemy.gameObject);
            }
        }
    }

    void Update() {
        if(isRoadActive) {
            cycleTimer += Time.deltaTime;
            if (cycleTimer >= cycleInterval) {
                    CheckForSpawn();
                    //MoveEnemies();
                    cycleTimer = 0;
                }
        }
    }

    private void SpawnEnemy() {
        GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], currentSpawnPoint.position, UnityEngine.Quaternion.identity);
        enemy.transform.parent = transform;
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.SetMoveDuration(enemyMoveDuration);
        enemyController.Move(endSpawnPoint.position);

        enemyController.OnEnemyDestroyed += DestroyEnemy;
        enemies.Add(enemyController);
    }

    private void DestroyEnemy(EnemyController enemyController) {
        enemies.Remove(enemyController);
        enemyController.OnEnemyDestroyed -= DestroyEnemy;
        Destroy(enemyController.gameObject);
    }

    // private void MoveEnemies() {
    //     for (int i = enemies.Count - 1; i >= 0; i--) {
    //         var enemy = enemies[i];
    //         Vector3Int currentCell = GameController.Instance.gridController.GetGrid().WorldToCell(enemy.gameObject.transform.position);
    //         Vector3Int newCell = currentCell + directionVector;
    //         if (GameController.Instance.gridController.GetBlockType(new Vector3Int(newCell.x, 0, newCell.z)) == EBlockType.WALL) {
    //             enemy.NoMove();
    //             enemies.RemoveAt(i);
    //         }
    //         else {
    //             enemy.Move(GameController.Instance.gridController.GetGrid().GetCellCenterWorld(newCell));
    //         }
    //     }
    // }

    private void CheckForSpawn() {
        if(enemiesInLineCounter > 0) {
                enemiesInLineCounter--;
                SpawnEnemy();
        } 
        else if (waveWaitingCounter > 0) {
            waveWaitingCounter--;
        } 
        else {
            RefreshEnemies();
        }
    }

    private void RefreshEnemies() {
        enemiesInLineCounter = Random.Range(1, enemiesInLine + 1);
        switch(enemiesInLineCounter) {
            case 1:
                waveWaitingCounter = 3;
                break;
            case 2:
                waveWaitingCounter = 2;
                break;
            case 3:waveWaitingCounter = 1;
                break;
        }
        enemiesInLineCounter--;
        SpawnEnemy();
    }
}
