using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIController : MonoBehaviour
{
    [Header("Screen fade")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Key Puzzle")]
    [SerializeField] private KeyPuzzleController keyPuzzleController;

    public void ShowBlackScreen(Action onComplete = null) {
        blackScreen.gameObject.SetActive(true);
        blackScreen.DOFade(1, fadeDuration).SetEase(Ease.InOutSine).OnComplete(() => onComplete?.Invoke());
    }

    public void HideBlackScreen(Action onComplete = null) {
        blackScreen.DOFade(0, fadeDuration).SetEase(Ease.InOutSine).OnComplete(() => {
            blackScreen.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void ShowKeyPuzzle() {
        keyPuzzleController.ShowPuzzle();
    }

    public void HideKeyPuzzle() {
        keyPuzzleController.HidePuzzle();
    }
}
