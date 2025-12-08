using UnityEngine;
using DG.Tweening;
using System;

public class KeyPuzzleController : MonoBehaviour
{
    [SerializeField] private KeyGridController keyGridController;
    [SerializeField] private KeyHole keyHoleController;
    [SerializeField] private RectTransform rectTransform;

    public Action OnPuzzleCompleted;

    private void OnEnable() {
        keyHoleController.OnAllKeysPlaced.AddListener(OnPuzzleCompletedHandle);
    }

    public void ShowPuzzle() {
        rectTransform.gameObject.SetActive(true);
        InitRandomKeyHoleColor();
        rectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete(() => keyGridController.StartPuzzle());
    }

    public void HidePuzzle(Action onHideComplete = null) {
        rectTransform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            rectTransform.gameObject.SetActive(false);
            onHideComplete?.Invoke();
        });
    }

    private void OnPuzzleCompletedHandle() {
        OnPuzzleCompleted?.Invoke();
    }

    private void InitRandomKeyHoleColor() {
        int randomIndex = UnityEngine.Random.Range(0, 3);
        EColor keyFrameColor = (EColor)randomIndex;

        keyGridController.SetTargetKeyColor(keyFrameColor);
        keyHoleController.SetColor(keyFrameColor);
    }

    void OnDisable() {
        keyHoleController.OnAllKeysPlaced.RemoveListener(OnPuzzleCompletedHandle);
    }
}
