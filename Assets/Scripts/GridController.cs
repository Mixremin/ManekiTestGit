using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid grid;

    private Dictionary<Vector3Int, AbsBlock> gridBlocks = new Dictionary<Vector3Int, AbsBlock>();

    private void Start()
    {
        grid = gameObject.GetComponent<Grid>();
        InitGridBlocks();
    }

    public void InitGridBlocks() {
       var blocks = FindObjectsByType<AbsBlock>(FindObjectsSortMode.None);
       foreach (var block in blocks)
       {
            Vector3Int cell = grid.WorldToCell(block.transform.position);
            gridBlocks.Add(cell, block);
       }
    }

    public EBlockType GetBlockType(Vector3Int cell) {
        return GetBlock(cell)?.GetBlockType() ?? EBlockType.NONE;
    }

    public AbsBlock GetBlock(Vector3Int cell) {
        return gridBlocks.TryGetValue(cell, out AbsBlock block) ? block : null;
    }

    public Grid GetGrid() {
        return grid;
    }
}
