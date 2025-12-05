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
        gridBlocks.TryGetValue(cell, out var block);    
        if (block != null) {
            return block.GetBlockType();
        }
        else {
            return EBlockType.NONE;
        }
    }

    public Grid GetGrid() {
        return grid;
    }
}
