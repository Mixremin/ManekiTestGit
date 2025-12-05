using UnityEngine;
using System;
using DG.Tweening;

public class PlayerController : MonoBehaviour, IEntity
{
    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.5f;
    
    [Header("Animation")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeRandomness = 90f;

    private Tween moveTween;
    public Action OnEnemyHit;
    public void Move(Vector3 position, bool canMove)
    {
        if (canMove) 
        {
            moveTween = transform.DOMove(position, moveDuration).SetEase(Ease.InOutSine);
        }
        else 
        {
            transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<EnemyController>(out var enemyController))
        {            
            moveTween.Kill();
            transform.DOPunchPosition(Vector3.up * 0.1f, 0.1f, 10, 0.1f).OnComplete(() => OnEnemyHit?.Invoke());
        }
    }
}
