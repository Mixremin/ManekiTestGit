using UnityEngine;
using UnityEngine.AI;
using System;

public class BossEnemy : MonoBehaviour, IEntity
{
    [SerializeField] private int healthAmount = 3;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private GameObject view;
    [SerializeField] private float stopDistance = 0f;
    
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
    }


    private void Update()
    {
        if (player != null && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            MoveTowardsPlayer();
        }
    }

    public void MoveTowardsPlayer()
    {
        if (GameController.Instance != null && player == null)
        {
            player = GameController.Instance.GetPlayerController();
        }

        navMeshAgent.SetDestination(player.transform.position);
 
        if (view != null && navMeshAgent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = navMeshAgent.velocity.normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                view.transform.rotation = Quaternion.Slerp(
                    view.transform.rotation, 
                    Quaternion.LookRotation(direction), 
                    Time.deltaTime * 10f
                );
            }
        }
    }
    
    public void Move(Vector3 position)
    {
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(position);
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

            Invoke(nameof(ReEnableNavMesh), 0.3f);
        }
    }

    private void ReEnableNavMesh()
    {
        if (navMeshAgent != null && this != null)
        {
            navMeshAgent.enabled = true;
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
