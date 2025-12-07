using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AbsDraggableObject : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Drag Settings")]
    [SerializeField] protected bool isDraggable = true;
    [SerializeField] protected float dragAlpha = 0.6f;
    
    protected bool isDragging;
    protected Vector3 startPosition;
    protected Vector3 dragOffset;
    protected Canvas parentCanvas;
    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        startPosition = transform.position;
        OnDragStart();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!isDraggable) return;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = dragAlpha;
        
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector3 worldPoint);
        
        dragOffset = transform.position - worldPoint;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable || !isDragging) return;
        
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint))
        {
            // Direct position for responsive touch input on mobile
            transform.position = worldPoint + dragOffset;
        }
        
        OnDragUpdate(eventData);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        OnDragEnd(eventData);

        if (gameObject.activeInHierarchy)
        {
            ResetToStartPosition();
        }
    }

    public void ResetToStartPosition()
    {
        transform.position = startPosition;
    }

    public void SetDraggable(bool value)
    {
        isDraggable = value;
    }

    public bool IsDragging => isDragging;

    protected abstract void OnDragStart();
    
    protected abstract void OnDragUpdate(PointerEventData eventData);
    
    protected abstract void OnDragEnd(PointerEventData eventData);

}
