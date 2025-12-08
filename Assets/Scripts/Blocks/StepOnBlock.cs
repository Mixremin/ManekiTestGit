using System;
using UnityEngine;

public class StepOnBlock : AbsBlock, IInteractable
{
    public Action OnSteppedOn;

    public void Interact()
    {
        Debug.Log("StepOnBlock interacted!");
        OnSteppedOn?.Invoke();
    }
}
