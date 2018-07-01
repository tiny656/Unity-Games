using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MatrixSize = 4;
    private Tile[,] allTile = new Tile[MatrixSize, MatrixSize];
    private List<Tile> emptyTiles = new List<Tile>();

    void Start()
    {
        Debug.Log("Get all tiles");
        foreach (Tile tile in GameObject.FindObjectsOfType<Tile>())
        {
            tile.Number = 0;
            this.allTile[tile.rowIndex, tile.colIndex] = tile;
            this.emptyTiles.Add(tile);
        }
        GenerateNewTile();
        GenerateNewTile();
    }

    public void Move(MoveDirection moveDirection)
    {
        Debug.Log(moveDirection.ToString() + " move.");

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

        // Check Game is Over

        //Generate 2 New Tiles;
        GenerateNewTile();
        GenerateNewTile();
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
                }
                else if (curTile.Number == nextTile.Number)
                {
                    curTile.Number *= 2;
                    nextTile.Number = 0;
                }
            }
        }
    }

    private void GenerateNewTile()
    {
        if (this.emptyTiles.Count > 0)
        {
            int indexForNewNumber = Random.Range(0, emptyTiles.Count);
            this.emptyTiles[indexForNewNumber].Number = 2;
            this.emptyTiles.RemoveAt(indexForNewNumber);
        }
    }
}
