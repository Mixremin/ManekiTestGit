using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyElement : AbsDraggableObject
{

    [SerializeField] private EColor color;
    [SerializeField] private Image keyImage;
    [SerializeField] private Sprite diamondSprite;
    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite emeraldSprite;
    
    //public EColor Color => color;
    
    public void SetColor(EColor color) {
        this.color = color;
        keyImage.sprite = color switch {
            EColor.DIAMOND => diamondSprite,
            EColor.GOLD => goldSprite,
            EColor.EMERALD => emeraldSprite,
            _ => null
        };
    }

    protected override void OnDragStart() {
        Debug.Log("OnDragStart");
    }

    protected override void OnDragUpdate(PointerEventData eventData) {
    }

    protected override void OnDragEnd(PointerEventData eventData) {
        if (eventData.pointerEnter != null) {
            Debug.Log("eventData.pointerEnter: " + eventData.pointerEnter.name);
            if (eventData.pointerEnter.TryGetComponent(out KeyHole keyHole)) {
                if (keyHole.checkColor(color)) {
                    gameObject.SetActive(false);
                    keyHole.AddCount();
                    return;
                }
            }
        }
    }


}
