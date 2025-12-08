using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameEndController : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameOverPanel;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Button restartButton;

    private void OnEnable() {
        restartButton.onClick.AddListener(OnRestartHandle);
    }

    public void ShowGameOver() {
        gameOverPanel.gameObject.SetActive(true);
        gameOverPanel.DOFade(1, fadeDuration).SetEase(Ease.InOutSine);
    }

    private void OnRestartHandle() {
        GameController.Instance?.RestartGame();
    }
}
