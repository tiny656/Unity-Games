using UnityEngine;

public class InputManager : MonoBehaviour
{
    public int MinTouchMoveDis;

    private Vector3 beginPosition;
    private Vector3 endPosition;

    private GameManager gameManager;

    void Awake()
    {
        this.gameManager = GameObject.FindObjectOfType<GameManager>();       
    }

    void Update()
    {
        if (this.gameManager.gameStatus != GameStatus.Playing) return;
        KeyboardInput();
        TouchInput();
    }

    void KeyboardInput()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            this.gameManager.Move(MoveDirection.Left);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            this.gameManager.Move(MoveDirection.Right);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            this.gameManager.Move(MoveDirection.Up);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            this.gameManager.Move(MoveDirection.Down);
        }
    }

    void TouchInput()
    {
        InputPhase inputPhase = GetInputPhase();
        MoveDirection moveDirection = MoveDirection.Idle;
        if (inputPhase.isValid)
        {
            if (inputPhase.phase == TouchPhase.Began)
            {
                this.beginPosition = inputPhase.position;
            }
            else if (inputPhase.phase == TouchPhase.Ended)
            {
                this.endPosition = inputPhase.position;
                Vector2 deltaDir = this.endPosition - this.beginPosition;
                if (deltaDir.sqrMagnitude > this.MinTouchMoveDis)
                {
                    if (Mathf.Abs(deltaDir.x) > Mathf.Abs(deltaDir.y))
                    {
                        moveDirection = deltaDir.x > 0 ? MoveDirection.Right : MoveDirection.Left;
                    }
                    if (Mathf.Abs(deltaDir.y) > Mathf.Abs(deltaDir.x))
                    {
                        moveDirection = deltaDir.y > 0 ? MoveDirection.Up : MoveDirection.Down;
                    }
                }
                if (moveDirection != MoveDirection.Idle)
                {
                    this.gameManager.Move(moveDirection);
                }
            }
        }
    }

    private struct InputPhase
    {
        public bool isValid;
        public TouchPhase phase;
        public Vector3 position;
    }

    private InputPhase GetInputPhase()
    {
        InputPhase inputPhase = new InputPhase { phase = TouchPhase.Canceled, position = new Vector3(), isValid = false };
        #if UNITY_EDITOR || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            inputPhase.isValid = true;
            inputPhase.phase = TouchPhase.Began;
            inputPhase.position = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            inputPhase.isValid = true;
            inputPhase.phase = TouchPhase.Ended;
            inputPhase.position = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            inputPhase.isValid = true;
            inputPhase.phase = TouchPhase.Moved;
            inputPhase.position = Input.mousePosition;
        }
        #elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0) {
            Touch touch = Input.touches [0];
            inputPhase.phase = touch.phase;
            inputPhase.position = touch.position;
            inputPhase.isValid = true;
        }
        #else
        Debug.Log("Unable to identify the game running environment");
        #endif
        return inputPhase;
    }
}
