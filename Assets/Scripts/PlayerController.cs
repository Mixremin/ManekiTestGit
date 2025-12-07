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
    public void Move(Vector3 position, bool canMove)
    {
        if (locked) return;
        if (canMove) 
        {
            locked = true;
            moveTween = transform.DOMove(position, moveDuration).SetEase(Ease.InOutSine).OnComplete(() => locked = false);
        }
        else 
        {
            transform.DOShakeScale(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, ShakeRandomnessMode.Full);
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
}
