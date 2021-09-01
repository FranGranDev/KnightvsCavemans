using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public static event OnSwipeInput OnSwipeEvent;
    //public static event OnSwipeInput OnLongSwipeEvent;
    public delegate void OnSwipeInput(Vector2 Dir);

    public float DeadZone;
    //public float LongDeadZone;
    private Vector2 TapPosition;
    private Vector2 SwipeDelta;
    private bool isSwiping;
    private bool isMobile;

    private void Start()
    {
        isMobile = Application.isMobilePlatform;
    }

    private void SwipeStuff()
    {
        if(!isMobile)
        {
            if(Input.GetMouseButtonDown(0))
            {
                isSwiping = true;
                TapPosition = Input.mousePosition;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                ResetSwipe();
            }
        }
        else
        {
            if(Input.touchCount > 0)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    isSwiping = true;
                    TapPosition = Input.GetTouch(0).position;
                }
                else if(Input.GetTouch(0).phase == TouchPhase.Ended ||
                    Input.GetTouch(0).phase == TouchPhase.Canceled)
                {
                    ResetSwipe();
                }
            }
        }

        CheckSwipe();
    }
    private void CheckSwipe()
    {
        SwipeDelta = Vector2.zero;

        if(isSwiping)
        {
            if(!isMobile && Input.GetMouseButton(0))
            {
                SwipeDelta = (Vector2)Input.mousePosition - TapPosition;
            }
            else if(Input.touchCount > 0)
            {
                SwipeDelta = Input.GetTouch(0).position - TapPosition;
            }
        }
        if (SwipeDelta.magnitude > DeadZone)
        {
            if(OnSwipeEvent != null)
            {
                OnSwipeEvent(SwipeDelta.normalized);
            }
            ResetSwipe();
        }
    }
    private void ResetSwipe()
    {
        isSwiping = false;
        TapPosition = Vector2.zero;
        SwipeDelta = Vector2.zero;
    }

    private void Update()
    {
        SwipeStuff();
    }
}
