using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public static event OnSwipeInput OnSwipeEvent;
    public static event OnSwipeInput OnFastSwipeEvent;
    public delegate void OnSwipeInput(Vector2 Dir, Vector2 Pos);

    private Coroutine FastSwipeCoroutine;

    [Header("Swipe")]
    public float DeadZone;
    private Vector2 TapPosition;
    private Vector2 SwipeDelta;
    private bool isSwiping;
    [Header("Fast Swipe")]
    public float FastDeadZone;
    public float FastSpeedZone;
    public float TimeZone;
    private float StartTime;
    private bool isFastSwiping;
    private float FastSwipeSpeed;
    private Vector2 PrevPos;
    private Vector2 FastTapPosition;
    private Vector2 FastSwipeDelta;
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
                StartTime = Time.time;
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
                    StartTime = Time.time;
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

        if (isSwiping)
        {
            if (!isMobile && Input.GetMouseButton(0))
            {
                SwipeDelta = (Vector2)Input.mousePosition - TapPosition;
            }
            else if (Input.touchCount > 0)
            {
                SwipeDelta = Input.GetTouch(0).position - TapPosition;
            }
        }
        if (SwipeDelta.magnitude > DeadZone)
        {
            OnSwipeEvent?.Invoke(SwipeDelta.normalized, TapPosition);
            ResetSwipe();
        }
    }
    private void ResetSwipe()
    {
        isSwiping = false;
        TapPosition = Vector2.zero;
        SwipeDelta = Vector2.zero;
    }

    private void FastSwipeStuff()
    {
        if (!isMobile)
        {
            FastSwipeSpeed = ((Vector2)Input.mousePosition - PrevPos).magnitude;
            PrevPos = (Vector2)Input.mousePosition;

            if (Input.GetMouseButtonUp(0))
            {
                ResetFastSwipe();
            }
            if(Input.GetMouseButton(0) && FastSwipeCoroutine == null && FastSwipeSpeed > FastSpeedZone)
            {
                FastSwipeCoroutine = StartCoroutine(FastSwipeCheck());
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                FastSwipeSpeed = (Input.GetTouch(0).position - PrevPos).magnitude;
                PrevPos = Input.GetTouch(0).position;
            }
            else
            {
                FastSwipeSpeed = 0;
            }

            if (Input.touchCount == 0)
            {
                ResetFastSwipe();
            }
            if (Input.touchCount > 0 && FastSwipeCoroutine == null && FastSwipeSpeed > FastSpeedZone)
            {
                FastSwipeCoroutine = StartCoroutine(FastSwipeCheck());
            }
        }
    }
    private IEnumerator FastSwipeCheck()
    {
        StartTime = Time.time;
        FastSwipeDelta = Vector2.zero;

        if (!isMobile)
        {
            FastTapPosition = Input.mousePosition;
            while (Time.time - StartTime < TimeZone)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    ResetFastSwipe();
                    yield break;
                }

                FastSwipeDelta = (Vector2)Input.mousePosition - FastTapPosition;
                if (FastSwipeDelta.magnitude > FastDeadZone)
                {
                    yield return new WaitForSeconds(0.1f);
                    OnFastSwipeEvent?.Invoke(FastSwipeDelta.normalized, TapPosition);
                    ResetFastSwipe();
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            FastTapPosition = Input.GetTouch(0).position;
            while (Time.time - StartTime < TimeZone)
            {
                if (Input.touchCount == 0)
                {
                    ResetFastSwipe();
                    yield break;
                }

                FastSwipeDelta = Input.GetTouch(0).position - FastTapPosition;
                if (FastSwipeDelta.magnitude > FastDeadZone)
                {
                    yield return new WaitForSeconds(0.1f);
                    OnFastSwipeEvent?.Invoke(FastSwipeDelta.normalized, TapPosition);
                    ResetFastSwipe();
                    yield break;
                }

                yield return new WaitForFixedUpdate();
            }
        }
        ResetFastSwipe();
        yield break;
    }
    private void ResetFastSwipe()
    {
        isSwiping = false;
        TapPosition = Vector2.zero;
        SwipeDelta = Vector2.zero;
        FastSwipeCoroutine = null;
    }

    private void Update()
    {
        SwipeStuff();
        FastSwipeStuff();
    }
    private void FixedUpdate()
    {
        
    }
}
