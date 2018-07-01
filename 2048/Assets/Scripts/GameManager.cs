using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public int targetNumber = 2048;

    [HideInInspector]
    public bool isGameOver;

    private const int MatrixSize = 4;
    private readonly Tile[,] allTile = new Tile[MatrixSize, MatrixSize];
    private readonly List<Tile> emptyTiles = new List<Tile>();

    void Start()
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
    }

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        }
        else
        {
            TryGenerateNewTile();
            TryGenerateNewTile();
        }
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
        return this.allTile.Cast<Tile>().Any(tile => tile.Number == this.targetNumber);
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
                // Get current tile and next not zero tile
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

                // Update current tile and next not zero tile
                if (curTile.Number == 0)
                {// Move tile
                    curTile.Number = nextTile.Number;
                    nextTile.Number = 0;
                    i -= 1;
                    this.emptyTiles.Remove(curTile);
                    this.emptyTiles.Add(nextTile);
                }
                else if (curTile.Number == nextTile.Number)
                {// Merge tile
                    ScoreManager.GetInstance().Score += curTile.Number;   
                    curTile.Number *= 2;
                    nextTile.Number = 0;
                    this.emptyTiles.Add(nextTile);
                }
            }
        }
    }
}
