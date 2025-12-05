using UnityEngine;


public class GridDebugLayout : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private int cellCountX = 5;
    [SerializeField] private int cellCountZ = 7;
    [SerializeField] private GameObject debugPrefab;

    void Start()
    {
        PrintGrid();
    }

    private void PrintGrid()
    {
        for(int x  = 0; x < cellCountX; x++)
        {
            for(int z = 0; z < cellCountZ; z++)
            {
                Instantiate(debugPrefab, grid.GetCellCenterWorld(new Vector3Int(x, 0, z)), Quaternion.identity);
            }
        }
    }
}
