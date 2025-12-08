using System;
using UnityEngine;

public class StepOnBlock : AbsBlock, IInteractable
{
    public Action OnSteppedOn;

    public void Interact()
    {
        OnSteppedOn?.Invoke();
    }
}
