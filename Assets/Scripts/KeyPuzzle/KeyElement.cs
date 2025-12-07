using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyElement : AbsDraggableObject
{

    [SerializeField] private EColor color;
    [SerializeField] private Image keyImage;
    
    //public EColor Color => color;
    
    public void SetColor(EColor color) {
        this.color = color;
        keyImage.color = color switch {
            EColor.RED => Color.red,
            EColor.BLUE => Color.blue,
            EColor.GREEN => Color.green,
            _ => Color.white
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
