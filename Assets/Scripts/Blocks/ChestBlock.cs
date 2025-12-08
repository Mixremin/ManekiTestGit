using UnityEngine;
using DG.Tweening;

public class ChestBlock : AbsBlock, IInteractable
{
    [SerializeField] private Transform chestTransform;
    [SerializeField] private Transform chestObjectPoint;

    public void Interact() {
        Debug.Log("Interact with ChestBlock");
        GameController.Instance?.StartKeyPuzzle();
    }

    public void SpawnChestObject() {
        chestTransform.DOMove(chestObjectPoint.position, 0.5f);
    }
}
