using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEntity
{
    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private GameObject view;

    private Tween moveTween;

    public void Move(Vector3 position)
    {
        moveTween = view.transform.DOLookAt(position, 0.1f).OnComplete(() => {
            moveTween = transform.DOMove(position, moveDuration).SetEase(Ease.Linear).OnComplete(() => {
                NoMove();
            });
        });
    }

    public void NoMove() {
        Destroy(gameObject);
    }

    public void StopMoving() {
        moveTween.Kill();
    }

    public void SetMoveDuration(float moveDuration)
    {
        this.moveDuration = moveDuration;
    }
}
