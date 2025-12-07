using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Transform playerLineCenter;

    [SerializeField] private Transform playerSpawnPoint;
    [Header("Swipe")]
    [SerializeField] private SwipeController swipeController;

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
        playerLineCenter.position = player.transform.position;
        playerController = player.GetComponent<PlayerController>();
        playerController.OnEnemyHit += RespawnPlayer;

        cameraFollow.Follow = playerLineCenter.transform;
        
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

        Vector3Int testCell = newCell;
        testCell.y = 0;
        AbsBlock block = gridController.GetBlock(testCell);


        switch (block?.GetBlockType() ?? EBlockType.NONE) {
            case EBlockType.NONE:
                AdjustPlayerLineCenter(grid.GetCellCenterWorld(newCell)); 
                playerController.Move(grid.GetCellCenterWorld(newCell));
                break;
            case EBlockType.WALL:
                playerController.NoMove();
                break;
            case EBlockType.INTERACTABLE:
                playerController.Interact(block);
                break;
            }

        
    }

    private void AdjustPlayerLineCenter(Vector3 newCell) {
        playerLineCenter.DOMove(new Vector3(0.5f, newCell.y, newCell.z), playerController.GetMoveDuration());
    }

    public void RespawnPlayer() {
        DisableSwipe();
        StopRoadSpawn();
        uiController.ShowBlackScreen(() => {
            player.transform.position = playerSpawnPoint.position;
            playerLineCenter.position = player.transform.position;
            roadController.ForEach(road => road.ClearEnemies());
            playerController.SetColliderState(true);
            uiController.HideBlackScreen(() => {
                playerController.SetLockedState(false);
                EnableSwipe();
                roadController.ForEach(road => road.StartRoadSpawn());
            });
        });
    }

    public void StartKeyPuzzle() {
        StopRoadSpawn();
        uiController.ShowKeyPuzzle();
        playerController.SetLockedState(true);
        DisableSwipe();
    }

    public void StopRoadSpawn() {
        roadController.ForEach(road => road.StopRoadSpawn());
    }

    private void DisableSwipe() {
        swipeController.enabled = false;
    }
    private void EnableSwipe() {
        swipeController.enabled = true;
    }

     private void OnDestroy() {
        playerController.OnEnemyHit -= RespawnPlayer;
        if (Instance == this) {
            Instance = null;
        }
    }
}
