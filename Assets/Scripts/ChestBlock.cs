using UnityEngine;

public class ChestBlock : AbsBlock, IInteractable
{
    public void Interact() {
        Debug.Log("Interact with ChestBlock");
        GameController.Instance.StartKeyPuzzle();
    }
}
