using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public int targetNumber = 2048;

    [HideInInspector]
    public bool isGameOver;

    private const int MatrixSize = 4;
    private readonly Tile[,] allTile = new Tile[MatrixSize, MatrixSize];
    private readonly List<Tile> emptyTiles = new List<Tile>();
    private int currentScore;
    private Text scoreText;
    private Text highScoreText;
    private Text resultText;

    void Awake()
    {
        this.scoreText = GameObject.Find("ScoreNumber").GetComponent<Text>();
        this.highScoreText = GameObject.Find("HighScoreNumber").GetComponent<Text>();
        this.resultText = GameObject.Find("GameResultText").GetComponent<Text>();
    }


    void Start()
    {
        this.NewGame();
    }

    public void NewGame()
    {
        foreach (Tile tile in GameObject.FindObjectsOfType<Tile>())
        {
            tile.Number = 0;
            this.allTile[tile.rowIndex, tile.colIndex] = tile;
            this.emptyTiles.Add(tile);
        }
        TryGenerateNewTile();
        TryGenerateNewTile();
        this.isGameOver = false;
        this.scoreText.text = "0";
        this.highScoreText.text = this.LoadHighScore(); // read from local store
        this.currentScore = 0;
        this.resultText.text = "";
    }

    public void Move(MoveDirection moveDirection)
    {
        Position startPosition = new Position();
        for (int i = 0; i < MatrixSize; i++)
        {
            if (moveDirection == MoveDirection.Up || moveDirection == MoveDirection.Down)
            {
                startPosition.RowIndex = moveDirection == MoveDirection.Up ? 0 : MatrixSize - 1;
                startPosition.ColIndex = i;
            }
            else
            {
                startPosition.RowIndex = i;
                startPosition.ColIndex = moveDirection == MoveDirection.Left ? 0 : MatrixSize - 1;
            }
            this.UpdateTiles(startPosition, moveDirection);
        }

        if (!this.CanMove() || this.IsReachGoal())
        {
            this.isGameOver = true;
            this.SaveHighScore();
            this.resultText.text = this.IsReachGoal() ? "Congratulatons!" : "Ooops, you lose!";
        }
        else
        {
            TryGenerateNewTile();
            TryGenerateNewTile();
        }
    }

    private void SaveHighScore()
    {

    }

    private string LoadHighScore()
    {
        return "3084";
    }

    private void AddScore(int score)
    {
        this.currentScore += score;
        this.scoreText.text = this.currentScore.ToString();
    }

    private void TryGenerateNewTile()
    {
        if (this.emptyTiles.Count > 0)
        {
            int indexForNewNumber = Random.Range(0, emptyTiles.Count);
            this.emptyTiles[indexForNewNumber].Number = 2;
            this.emptyTiles.RemoveAt(indexForNewNumber);
        }
    }

    private bool IsReachGoal()
    {
        foreach (var tile in this.allTile)
        {
            if (tile.Number == targetNumber)
            {
                return true;
            }
        }
        return false;
    }

    private bool CanMove()
    {
        int[] dx = {0, 1, 0, -1}, dy = {1, 0, -1, 0};
        foreach (var tile in this.allTile)
        {
            for (int d = 0; d < 4; d++)
            {
                int newX = tile.rowIndex + dx[d];
                int newY = tile.colIndex + dy[d];
                if (newX >= 0 && newX < MatrixSize && newY >= 0 && newY < MatrixSize)
                {
                    Tile nearTile = this.allTile[newX, newY];
                    if (tile.Number == 0 || nearTile.Number == 0 || tile.Number == nearTile.Number)
                        return true;
                }
            }
        }
        return false;
    }

    private Position FindNextNotZeroPosWithDirection(int pos, Position startPosition, MoveDirection moveDirection)
    {
        Position curPosition = new Position();
        Position nextNotZeroPos = new Position
        {
            RowIndex = -1,
            ColIndex = -1
        };

        for (int i = pos + 1; i < MatrixSize; i += 1)
        {
            if (moveDirection == MoveDirection.Up || moveDirection == MoveDirection.Down)
            {
                curPosition.RowIndex = moveDirection == MoveDirection.Up ? i : MatrixSize -i - 1;
                curPosition.ColIndex = startPosition.ColIndex;
            }
            else 
            {
                curPosition.RowIndex = startPosition.RowIndex;
                curPosition.ColIndex = moveDirection == MoveDirection.Left ? i : MatrixSize - i - 1;
            }

            if (this.allTile[curPosition.RowIndex, curPosition.ColIndex].Number != 0)
            {
                nextNotZeroPos = curPosition;
                break;
            }
        }
        return nextNotZeroPos;
    }


    private void UpdateTiles(Position startPosition, MoveDirection moveDirection)
    {
        for (int i = 0; i < MatrixSize; i += 1)
        {
            var nextNotZeroPos = this.FindNextNotZeroPosWithDirection(i, startPosition, moveDirection);

            if (nextNotZeroPos.IsValidPosition())
            {
                Tile curTile, nextTile;
                if (moveDirection == MoveDirection.Up || moveDirection == MoveDirection.Down)
                {
                    curTile = this.allTile[moveDirection == MoveDirection.Up ? i : MatrixSize - i - 1, startPosition.ColIndex];
                    nextTile = this.allTile[nextNotZeroPos.RowIndex, startPosition.ColIndex];
                }
                else
                {
                    curTile = this.allTile[startPosition.RowIndex, moveDirection == MoveDirection.Left ? i : MatrixSize - i - 1];
                    nextTile = this.allTile[startPosition.RowIndex, nextNotZeroPos.ColIndex];
                }

                if (curTile.Number == 0)
                {
                    curTile.Number = nextTile.Number;
                    nextTile.Number = 0;
                    i -= 1;
                    this.emptyTiles.Remove(curTile);
                    this.emptyTiles.Add(nextTile);
                }
                else if (curTile.Number == nextTile.Number)
                {
                    curTile.Number *= 2;
                    nextTile.Number = 0;
                    this.emptyTiles.Add(nextTile);
                    this.AddScore(curTile.Number);   
                }
            }
        }
    }
}
