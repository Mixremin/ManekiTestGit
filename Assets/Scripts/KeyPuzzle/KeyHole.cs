using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class KeyHole : MonoBehaviour
{
    [SerializeField] private EColor requiredColor;
    [SerializeField] private Image keyHoleBackground;

    [SerializeField] private int needCount = 3;
    [SerializeField] private int currentCount = 0;
    [SerializeField] private TextMeshProUGUI countText;

    public UnityEvent OnAllKeysPlaced;

    private void Start() {
        UpdateCountText();
    }

    private void UpdateCountText() {
        countText.text = currentCount.ToString() + "/" + needCount.ToString();
    }

    public void SetColor(EColor color) {
        requiredColor = color;
        keyHoleBackground.color = color switch {
            EColor.RED => Color.red,
            EColor.BLUE => Color.blue,
            EColor.GREEN => Color.green,
            _ => Color.white
        };
    }

    public bool checkColor(EColor color) {
        Debug.Log("Result: " + (requiredColor == color));
        return requiredColor == color;
    }

    public void AddCount() {
        Debug.Log("AddCount: " + currentCount);
        currentCount++;
        UpdateCountText();
        
        if (currentCount >= needCount) {
            Debug.Log("OnAllKeysPlaced triggered");
            OnAllKeysPlaced?.Invoke();
        }
    }
}
