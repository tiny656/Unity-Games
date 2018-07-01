using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;

    void Awake()
    {
        this.gameManager = GameObject.FindObjectOfType<GameManager>();       
    }

    void Update()
    {
        if (this.gameManager.isGameOver) return;
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
    }
}
