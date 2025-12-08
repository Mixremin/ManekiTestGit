using DG.Tweening;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour, IEntity
{
    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private GameObject view;

    private Tween moveTween;
    private bool isBeingDestroyed = false;

    public Action<EnemyController> OnEnemyDestroyed;

    public void Move(Vector3 position)
    {
        moveTween = view.transform.DOLookAt(position, 0.1f)
            .SetLink(gameObject)
            .OnComplete(() => {
                if (!isBeingDestroyed)
                {
                    moveTween = transform.DOMove(position, moveDuration)
                        .SetEase(Ease.Linear)
                        .SetLink(gameObject)
                        .OnComplete(() => {
                            if (!isBeingDestroyed)
                            {
                                NoMove();
                            }
                        });
                }
            });
    }

    public void NoMove() {
        if (!isBeingDestroyed)
        {
            OnEnemyDestroyed?.Invoke(this);
            isBeingDestroyed = true;
        }
    }

    public void StopMoving() {
        if (!isBeingDestroyed && moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }

    public void SetMoveDuration(float moveDuration)
    {
        this.moveDuration = moveDuration;
    }

    private void OnDestroy()
    {
        isBeingDestroyed = true;
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }
}
