using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class KeyHole : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Sprite diamondSprite;
    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite emeraldSprite;

    [Header("Key Hole")]
    [SerializeField] private EColor requiredColor;
    [SerializeField] private Image keyHoleSprite;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private int needCount = 3;
    [SerializeField] private int currentCount = 0;
    

    public UnityEvent OnAllKeysPlaced;

    private void Start() {
        UpdateCountText();
    }

    private void UpdateCountText() {
        countText.text = currentCount.ToString() + "/" + needCount.ToString();
    }

    public void SetColor(EColor color) {
        requiredColor = color;
        keyHoleSprite.sprite = color switch {
            EColor.DIAMOND => diamondSprite,
            EColor.GOLD => goldSprite,
            EColor.EMERALD => emeraldSprite,
            _ => null
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
