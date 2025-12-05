using UnityEngine;

public class AbsBlock : MonoBehaviour
{
    [SerializeField] protected EBlockType blockType;

    public EBlockType GetBlockType()
    {
        return blockType;
    }
}
