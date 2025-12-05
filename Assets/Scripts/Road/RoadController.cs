using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    [Header("Road settings")]
    [SerializeField] private bool isRoadActive = false;

    [Header("Enemy prefabs")]
    [SerializeField] private GameObject enemyPrefab;
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

    public void StartRoadSpawn() {
        isRoadActive = true;
        if(direction == EDirection.LEFT) {
            currentSpawnPoint = rightSideSpawnPoint;
            directionVector = Vector3Int.left;
        } else {
            currentSpawnPoint = leftSideSpawnPoint;
            directionVector = Vector3Int.right;
        }
        CheckForSpawn();
        MoveEnemies();
    }

    public void StopRoadSpawn() {
        isRoadActive = false;
    }

    void Update() {
        if(isRoadActive) {
            cycleTimer += Time.deltaTime;
            if (cycleTimer >= cycleInterval) {
                    CheckForSpawn();
                    MoveEnemies();
                    cycleTimer = 0;
                }
        }
    }

    private void SpawnEnemy() {
        GameObject enemy = Instantiate(enemyPrefab, currentSpawnPoint.position, UnityEngine.Quaternion.identity);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.SetMoveDuration(cycleInterval);
        enemies.Add(enemyController);
    }

    private void MoveEnemies() {
        for (int i = enemies.Count - 1; i >= 0; i--) {
            var enemy = enemies[i];
            Vector3Int currentCell = GameController.Instance.gridController.GetGrid().WorldToCell(enemy.gameObject.transform.position);
            Vector3Int newCell = currentCell + directionVector;
            if (GameController.Instance.gridController.GetBlockType(new Vector3Int(newCell.x, 0, newCell.z)) == EBlockType.WALL) {
                enemy.Move(enemy.gameObject.transform.position, false);
                enemies.RemoveAt(i);
            }
            else {
                enemy.Move(GameController.Instance.gridController.GetGrid().GetCellCenterWorld(newCell), true);
            }
        }
    }

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
