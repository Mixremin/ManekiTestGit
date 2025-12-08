using UnityEngine;
using System;
using DG.Tweening;

public class PlayerController : MonoBehaviour, IEntity
{
    public bool locked = false;
    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private GameObject view;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private int shakeVibrato = 5;
    [SerializeField] private float shakeRandomness = 50f;

    [Header("Attack")]
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float pushForce = 10f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private ParticleSystem attackVFX;

    private Tween moveTween;
    private float lastAttackTime = -999f;
    public Action OnEnemyHit;
    public void Move(Vector3 position)
    {
        if (locked) return;
        
        locked = true;
        moveTween = view.transform.DOLookAt(position, 0.1f).OnComplete(() => {

            animator.SetBool("Walking", true);
            animator.SetFloat("WalkSpeed", 1.15f / moveDuration);

            moveTween = transform.DOMove(position, moveDuration).SetEase(Ease.InOutSine).OnComplete(() => {
                locked = false;
                animator.SetBool("Walking", false);
            });
        });
    }

    public void NoMove() {
        if (locked) return;

        transform.DOShakeScale(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, ShakeRandomnessMode.Full);
    }

    public void Attack()
    {
        if (locked) return;
        if (Time.time - lastAttackTime < attackCooldown)
        {
            Debug.Log("Attack is on cooldown!");
            return;
        }
        lastAttackTime = Time.time;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        attackVFX.gameObject.SetActive(true);
        
        PlayAttackVFX();
        Invoke(nameof(StopAttackVFX), attackVFX.main.duration);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<BossEnemy>(out var bossEnemy))
            {
                bossEnemy.TakeDamage(attackDamage);
                Vector3 pushDirection = (hitCollider.transform.position - transform.position).normalized;
                bossEnemy.PushBack(pushDirection, pushForce);
                Debug.Log($"Attacked {hitCollider.name} for {attackDamage} damage!");
            }
        }

    }

    private void PlayAttackVFX() {
        attackVFX.gameObject.SetActive(true);
        attackVFX.Play();
    }

    private void StopAttackVFX() {
        attackVFX.gameObject.SetActive(false);
        attackVFX.Stop();
    }

    public void Interact(AbsBlock block) {
        block.TryGetComponent(out IInteractable interactable);
        if (interactable != null) {
            interactable.Interact();
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision with " + collision.transform.name);
        if (collision.transform.TryGetComponent<IEntity>(out var enemyController))
        {   
            locked = true;
            moveTween.Kill();
            OnEnemyHit?.Invoke();
            SetColliderState(false);
            
            transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true);
        }
    }

    public void SetColliderState(bool state) {
        playerCollider.enabled = state;
    }

    public void SetLockedState(bool state) {
        locked = state;
    }

    public float GetMoveDuration() {
        return moveDuration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
