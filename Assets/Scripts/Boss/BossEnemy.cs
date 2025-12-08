using UnityEngine;
using UnityEngine.AI;
using System;
using DG.Tweening;

public class BossEnemy : MonoBehaviour, IEntity
{
    [SerializeField] private int healthAmount = 3;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private GameObject view;
    [SerializeField] private float stopDistance = 0f;
    
    private Vector3 lastDestination = Vector3.zero;
    private Tween rotationTween;

    private PlayerController player;
    public Action<BossEnemy> OnBossEnemyDied;


    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        // Configure NavMeshAgent
        if (navMeshAgent != null)
        {
            navMeshAgent.stoppingDistance = stopDistance;
            navMeshAgent.updateRotation = false; // We'll handle rotation manually if needed
        }

        if (GameController.Instance != null && player == null)
        {
            player = GameController.Instance.GetPlayerController();
        }
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            lastDestination = player.transform.position;
            navMeshAgent.SetDestination(lastDestination);
            RotateTowardsDestination();
        }
    }
    
    public void Move(Vector3 position)
    {
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            lastDestination = position;
            navMeshAgent.SetDestination(lastDestination);
            RotateTowardsDestination();
        }
    }

    public void NoMove()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        healthAmount -= damage;
        if (healthAmount <= 0) {
            Die();
        }
    }

    public void PushBack(Vector3 direction, float force)
    {
        if (rb != null)
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }

            rb.AddForce(direction * force, ForceMode.Impulse);

            Invoke(nameof(ReEnableNavMesh), 1.0f);
        }
    }

    private void ReEnableNavMesh()
    {
        if (navMeshAgent != null && this != null)
        {
            navMeshAgent.enabled = true;
            Move(lastDestination);
        }
    }

    private void RotateTowardsDestination() {
        if (view != null)
        {
            Vector3 direction = lastDestination - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                rotationTween?.Kill();
                rotationTween = view.transform.DOLookAt(lastDestination, 0.5f)
                    .SetLink(gameObject)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => {
                        rotationTween = null;
                    });
            }
        }
    }

    private void Die()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
        OnBossEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
