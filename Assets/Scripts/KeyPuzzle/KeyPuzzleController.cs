using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class KeyPuzzleController : MonoBehaviour
{
    [SerializeField] private KeyGridController keyGridController;
    [SerializeField] private KeyHole keyHoleController;
    [SerializeField] private RectTransform rectTransform;

    // private void Start() {
    //     rectTransform = GetComponent<RectTransform>();
    //     rectTransform.localScale = Vector3.zero;
    //     rectTransform.gameObject.SetActive(false);

    //     keyHoleController.OnAllKeysPlaced.AddListener(HidePuzzle);
    // }

    private void OnEnable() {
        keyHoleController.OnAllKeysPlaced.AddListener(HidePuzzle);
    }

    public void ShowPuzzle() {
        rectTransform.gameObject.SetActive(true);
        InitRandomKeyHoleColor();
        rectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete(() => keyGridController.StartPuzzle());
    }

    public void HidePuzzle() {
        rectTransform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => rectTransform.gameObject.SetActive(false));
    }

    private void InitRandomKeyHoleColor() {
        int randomIndex = Random.Range(0, 3);
        EColor keyFrameColor = (EColor)randomIndex;

        keyGridController.SetTargetKeyColor(keyFrameColor);
        keyHoleController.SetColor(keyFrameColor);
    }

    void OnDisable() {
        keyHoleController.OnAllKeysPlaced.RemoveListener(HidePuzzle);
    }
}
