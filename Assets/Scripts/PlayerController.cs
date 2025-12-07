using UnityEngine;
using System;
using DG.Tweening;

public class PlayerController : MonoBehaviour, IEntity
{
    public bool locked = false;
    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Collider playerCollider;
    [Header("Animation")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private int shakeVibrato = 5;
    [SerializeField] private float shakeRandomness = 50f;

    private Tween moveTween;
    public Action OnEnemyHit;
    public void Move(Vector3 position)
    {
        if (locked) return;

        locked = true;
        moveTween = transform.DOMove(position, moveDuration).SetEase(Ease.InOutSine).OnComplete(() => locked = false);
    }

    public void NoMove() {
        if (locked) return;

        transform.DOShakeScale(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, ShakeRandomnessMode.Full);
    }

    public void Interact(AbsBlock block) {
        if (locked) return;

        block.TryGetComponent(out IInteractable interactable);
        if (interactable != null) {
            interactable.Interact();
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision with " + collision.transform.name);
        if (collision.transform.TryGetComponent<EnemyController>(out var enemyController))
        {   
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
}
