using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SwipeController : MonoBehaviour
{
    [Header("Swipe Settings")]
    [SerializeField] private float minSwipeDistance = 50f;
    [SerializeField] private float maxSwipeTime = 0.5f;
    [SerializeField] private float tapMaxDistance = 20f;
    [SerializeField] private float tapMaxTime = 0.2f;

    private Vector2 touchStartPos;
    private float touchStartTime;
    private bool isTouching;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerUp -= OnFingerUp;
        EnhancedTouchSupport.Disable();
    }

    private void OnFingerDown(Finger finger)
    {
        if (finger.index != 0) return;

        Vector2 screenPos = finger.currentTouch.screenPosition;
        if (!IsWithinScreenBounds(screenPos)) return;

        touchStartPos = screenPos;
        touchStartTime = Time.time;
        isTouching = true;
    }

    private void OnFingerUp(Finger finger)
    {
        if (finger.index != 0 || !isTouching) return;

        Vector2 screenPos = finger.currentTouch.screenPosition;
        if (!IsWithinScreenBounds(screenPos)) return;

        ProcessTouchEnd(screenPos);
        isTouching = false;
    }

    private bool IsWithinScreenBounds(Vector2 position)
    {
        return position.x >= 0 && position.x <= Screen.width &&
               position.y >= 0 && position.y <= Screen.height;
    }

    private void ProcessTouchEnd(Vector2 endPos)
    {
        float touchDuration = Time.time - touchStartTime;
        Vector2 swipeDelta = endPos - touchStartPos;
        float swipeDistance = swipeDelta.magnitude;

        // Check for tap
        if (swipeDistance < tapMaxDistance && touchDuration < tapMaxTime)
        {
            OnTap();
            return;
        }

        // Check for swipe
        if (swipeDistance >= minSwipeDistance && touchDuration <= maxSwipeTime)
        {
            // Determine swipe direction (horizontal vs vertical)
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                // Horizontal swipe
                if (swipeDelta.x > 0)
                {
                    OnSwipeRight();
                }
                else
                {
                    OnSwipeLeft();
                }
            }
        }
    }

    private void OnSwipeLeft()
    {
        GameController.Instance.MovePlayer(EDirection.LEFT);
    }

    private void OnSwipeRight()
    {
        GameController.Instance.MovePlayer(EDirection.RIGHT);
    }

    private void OnTap()
    {
        GameController.Instance.MovePlayer(EDirection.FORWARD);
    }
}
