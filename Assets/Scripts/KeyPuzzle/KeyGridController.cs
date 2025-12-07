using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class KeyGridController : MonoBehaviour
{
    [Header("Key grid")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GridLayoutGroup keyGridLayoutGroup;

    [SerializeField] private EColor keyTargetColor;
    [SerializeField] private int cellCountX = 6;
    [SerializeField] private int cellCountZ = 6;

    private List<GameObject> keyList = new List<GameObject>();

    public void StartPuzzle() {
        InitKeyGrid();
        StartCoroutine(DisableGridLayoutAfterFrame());
    }

    private IEnumerator DisableGridLayoutAfterFrame() {
        yield return new WaitForEndOfFrame();
        keyGridLayoutGroup.enabled = false;
    }

    void InitKeyGrid() {
        int totalKeys = cellCountX * cellCountZ;
        int colorCount = System.Enum.GetValues(typeof(EColor)).Length;
        
        List<EColor> randomColorsList = new List<EColor>();

        for (int i = 0; i < 3; i++) {
            randomColorsList.Add(keyTargetColor);
        }

        for (int i = 3; i < totalKeys; i++) {
            randomColorsList.Add((EColor)Random.Range(0, colorCount));
        }

        for (int i = randomColorsList.Count - 1; i > 0; i--) {
            int randomIndex = Random.Range(0, i + 1);
            (randomColorsList[i], randomColorsList[randomIndex]) = (randomColorsList[randomIndex], randomColorsList[i]);
        }
        
        int colorIndex = 0;
        for (int x = 0; x < cellCountX; x++) {
            for (int z = 0; z < cellCountZ; z++) {
                GameObject key = Instantiate(keyPrefab, new Vector3(x, 0, z), Quaternion.identity, keyGridLayoutGroup.transform);
                
                KeyElement keyElement = key.GetComponent<KeyElement>();
                if (keyElement != null) {
                    keyElement.SetColor(randomColorsList[colorIndex]);
                }
                
                keyList.Add(key);
                colorIndex++;
            }
        }
    }

    public void SetTargetKeyColor(EColor color) {
        keyTargetColor = color;
    }
}
