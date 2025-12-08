using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Unity.VisualScripting;

public class UIController : MonoBehaviour
{
    [Header("Screen fade")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Key Puzzle")]
    [SerializeField] private KeyPuzzleController keyPuzzleController;

    [Header("Game Over")]
    [SerializeField] private GameEndController gameEndController;

    private void OnEnable() {
        keyPuzzleController.OnPuzzleCompleted += HideKeyPuzzle;
    }

    public void ShowBlackScreen(Action onComplete = null) {
        blackScreen.gameObject.SetActive(true);
        blackScreen.DOFade(1, fadeDuration).SetEase(Ease.InOutSine).OnComplete(() => onComplete?.Invoke());
    }

    public void HideBlackScreen(Action onHideComplete = null) {
        blackScreen.DOFade(0, fadeDuration).SetEase(Ease.InOutSine).OnComplete(() => {
            blackScreen.gameObject.SetActive(false);
            onHideComplete?.Invoke();
        });
    }

    public void ShowKeyPuzzle() {
        keyPuzzleController.ShowPuzzle();
    }

    public void HideKeyPuzzle() {
        keyPuzzleController.HidePuzzle(() => {
            ShowGameOver();
        });
    }

    public void ShowGameOver() {
        gameEndController.gameObject.SetActive(true);
        gameEndController.ShowGameOver();
    }

        private void OnDisable() {
        keyPuzzleController.OnPuzzleCompleted -= HideKeyPuzzle;
    }
}
