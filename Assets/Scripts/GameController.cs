using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerLineCenter;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Road")]
    [SerializeField] private List<RoadController> roadController;

    [Header("Final Boss")]
    [SerializeField] private FinalBossController finalBossController;

    [Header("Swipe")]
    [SerializeField] private SwipeController swipeController;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cameraFollow;

    [Header("Grid")]
    public GridController gridController;


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

        swipeController.SetSwipeMode(ESwipeMode.MOVE);
    }

    void OnEnable() {
        finalBossController.OnBossFightStarted += StartFinalBossFight;
        finalBossController.OnBossFightWon += WinBossFight;
        finalBossController.OnBossFightLost += RespawnPlayer;
    }

    public void PlayerMove(EDirection direction) {
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
            case EBlockType.INTERACTABLE_ON_MOVE:
                AdjustPlayerLineCenter(grid.GetCellCenterWorld(newCell)); 
                playerController.Move(grid.GetCellCenterWorld(newCell));
                playerController.Interact(block);
                break;
        }
    }

    public void PlayerAttack() {
        playerController.Attack();
    }

    private void AdjustPlayerLineCenter(Vector3 newCell) {
        playerLineCenter.DOMove(new Vector3(0.5f, newCell.y, newCell.z), playerController.GetMoveDuration());
    }

    public void RespawnPlayer() {
        ChangeSwipeMode(ESwipeMode.DISABLED);
        StopRoadSpawn();
        finalBossController.StopBossFight();
        uiController.ShowBlackScreen(() => {
            player.transform.position = playerSpawnPoint.position;
            playerLineCenter.position = player.transform.position;
            roadController.ForEach(road => road.ClearEnemies());
            playerController.SetColliderState(true);
            uiController.HideBlackScreen(() => {
                playerController.SetLockedState(false);
                ChangeSwipeMode(ESwipeMode.MOVE);
                roadController.ForEach(road => road.StartRoadSpawn());
            });
        });
    }

    public void StartKeyPuzzle() {
        StopRoadSpawn();
        uiController.ShowKeyPuzzle();
        playerController.SetLockedState(true);
        ChangeSwipeMode(ESwipeMode.DISABLED);
    }

    public void StopRoadSpawn() {
        roadController.ForEach(road => road.StopRoadSpawn());
    }

    public void ChangeSwipeMode(ESwipeMode swipeMode) {
        swipeController.SetSwipeMode(swipeMode);
    }

    public GameObject GetPlayer() {
        return player;
    }

    public PlayerController GetPlayerController() {
        return playerController;
    }

    private void StartFinalBossFight() {
        ChangeSwipeMode(ESwipeMode.ATTACK);
        StopRoadSpawn();
        roadController.ForEach(road => road.ClearEnemies());
    }

    private void WinBossFight() {
        ChangeSwipeMode(ESwipeMode.MOVE);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDisable() {
        finalBossController.OnBossFightStarted -= StartFinalBossFight;
        finalBossController.OnBossFightWon -= WinBossFight;
        finalBossController.OnBossFightLost -= RespawnPlayer;
    }
    
     private void OnDestroy() {
        playerController.OnEnemyHit -= RespawnPlayer;
        if (Instance == this) {
            Instance = null;
        }
    }
}
