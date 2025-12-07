using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEntity
{
    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.5f;

    private Tween moveTween;

    public void Move(Vector3 position, bool canMove)
    {
        if (canMove)
        {
            moveTween = transform.DOMove(position, moveDuration).SetEase(Ease.InOutSine);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopMoving() {
        moveTween.Kill();
    }

    public void SetMoveDuration(float moveDuration)
    {
        this.moveDuration = moveDuration;
    }
}
