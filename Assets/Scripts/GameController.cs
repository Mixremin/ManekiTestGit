using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Transform playerSpawnPoint;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cameraFollow;

    [Header("Grid")]
    public GridController gridController;

    [Header("Road")]
    [SerializeField] private List<RoadController> roadController;

    [Header("UI")]
    [SerializeField] private UIController uiController;

    private PlayerController playerController;
    private GameObject player;
    private Grid grid;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void Start() {
        player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        playerController = player.GetComponent<PlayerController>();
        playerController.OnEnemyHit += RespawnPlayer;

        cameraFollow.Follow = player.transform;
        
        grid = gridController.GetGrid();
        
        roadController = FindObjectsByType<RoadController>(FindObjectsSortMode.None).ToList();
        roadController.ForEach(road => road.StartRoadSpawn());
    }

    public void MovePlayer(EDirection direction) {
        Vector3Int currentCell = grid.WorldToCell(player.transform.position);
        Vector3Int newCell = currentCell + direction switch {
            EDirection.LEFT => Vector3Int.left,
            EDirection.RIGHT => Vector3Int.right,
            EDirection.FORWARD => Vector3Int.forward,
            _ => Vector3Int.zero
        };

        bool canMove = true;

        //Костыль для проверки
        Vector3Int testCell = newCell;
        testCell.y = 0;

        if (gridController.GetBlockType(testCell) == EBlockType.WALL) {
            canMove = false;
        }

        playerController.Move(grid.GetCellCenterWorld(newCell), canMove);
    }

    public void RespawnPlayer() {
        StopRoadSpawn();
        uiController.ShowBlackScreen(() => {
            player.transform.position = playerSpawnPoint.position;
            roadController.ForEach(road => road.ClearEnemies());
            playerController.SetColliderState(true);
            uiController.HideBlackScreen(() => {
                playerController.SetLockedState(false);
                roadController.ForEach(road => road.StartRoadSpawn());
            });
        });
    }

    public void StopRoadSpawn() {
        roadController.ForEach(road => road.StopRoadSpawn());
    }

     private void OnDestroy() {
        playerController.OnEnemyHit -= RespawnPlayer;
        if (Instance == this) {
            Instance = null;
        }
    }
}
