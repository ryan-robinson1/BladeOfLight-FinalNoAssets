

using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    private static bool tap, swipeLeft, swipeRight, swipeUp, swipeDown,swipeLeftBig,swipeRightBig;
    private bool tapRequested;
    private bool isDraging = false;
    private Vector2 startTouch, swipeDelta;

    #region Instance
    private static SwipeInput instance;
    public static SwipeInput Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SwipeInput>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned SwipeInput", typeof(SwipeInput)).GetComponent<SwipeInput>();
                }
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion
    #region Public properties
    public bool Tap
    {
        get { return tap; }
    }
    public bool DoubleTap
    {
        get { return false; }
    }
    public Vector2 SwipeDelta
    {
        get { return swipeDelta; }
    }
    public bool SwipeLeft
    {
        get { return swipeLeft; }
    }
    public bool SwipeRight
    {
        get { return swipeRight; }
    }
    public bool SwipeLeftBig
    {
        get { return swipeLeftBig; }
    }
    public bool SwipeRightBig
    {
        get { return swipeRightBig; }
    }
    public bool SwipeUp
    {
        get { return swipeUp; }
    }
    public bool SwipeDown
    {
        get { return swipeDown; }
    }




    #endregion
    private void Update()
    {
        if (Time.timeScale == 1)
        {
            tap = swipeDown = swipeUp = swipeLeft = swipeRight = false;
            if (Input.GetMouseButtonDown(0))
            {
                tapRequested = true;
                isDraging = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (tapRequested)
                    tap = true;
                isDraging = false;
                Reset();
            }

            if (Input.touches.Length > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    tapRequested = true;
                    isDraging = true;
                    startTouch = Input.touches[0].position;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    if (tapRequested) { tap = true; }
                    isDraging = false;
                    Reset();
                }
            }

            //Calculate the distance
            swipeDelta = Vector2.zero;
            if (isDraging)
            {

                if (Input.touches.Length < 0)
                    swipeDelta = Input.touches[0].position - startTouch;
                else if (Input.GetMouseButton(0))
                    swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }

            //Did we cross the distance?
            if (swipeDelta.magnitude > 7.5f )
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    if (x < 0)
                        swipeLeft = true;
                    else
                        swipeRight = true;
                }
                else
                {
                    if (y < 0)
                        swipeDown = true;
                    else
                        swipeUp = true;
                }
                Reset();
            }
            
            
        }
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
        tapRequested = false;
    }
}