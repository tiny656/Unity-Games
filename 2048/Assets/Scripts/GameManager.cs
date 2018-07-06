using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int targetNumber = 2048;
    [Range(0, 2f)]
    public float Delay;
    public GameStatus gameStatus;
    public GameObject MainPanel;
    public GameObject GameOverPanel;

    private const int MatrixSize = 4;
    private readonly Tile[,] allTile = new Tile[MatrixSize, MatrixSize];
    private readonly List<Tile> emptyTiles = new List<Tile>();
    private bool isMoved;
    private bool[] lineCompleted;

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
        this.gameStatus = GameStatus.Playing;
        this.GameOverPanel.SetActive(false);
        Debug.Log("Game Manager Star Over");
    }

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CheckGameOver()
    {
        if (!this.CanMove() || this.IsReachGoal())
        {
            this.gameStatus = GameStatus.GameOver;
            this.GameOverPanel.SetActive(true);
            string gameOverText = (this.IsReachGoal() ? "Contratulations!" : "Game Over!")
                + string.Format("\nYou score\n{0}", ScoreManager.GetInstance().Score.ToString());
            this.GameOverPanel.transform.Find("GameOverText").GetComponent<Text>().text = gameOverText;
        }
        else
        {
            this.gameStatus = GameStatus.Playing;
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
    public void Move(MoveDirection moveDirection)
    {
        Debug.Log(string.Format("Try Move {0}", moveDirection.ToString()));
        this.isMoved = false;
        this.gameStatus = GameStatus.WaitingForMoveToEnd;
        if (this.Delay > 0f)
        {
            StartCoroutine(this.InternalMoveWithDelay(moveDirection));
        }
        else
        {
            this.InternalMove(moveDirection);
        }
    }

    private void InternalMove(MoveDirection moveDirection)
    {
        Tile[] tileInLine;
        int matrixSize = this.allTile.GetLength(0);
        for (int i = 0; i < matrixSize; i++)
        {
            if (moveDirection == MoveDirection.Up || moveDirection == MoveDirection.Down)
                tileInLine = GetTilesFromOneColumn(i, moveDirection);
            else
                tileInLine = GetTilesFromOneRow(i, moveDirection);
            this.UpdateTileOnOneLine(i, tileInLine);
        }
        if (this.isMoved)
        {
            this.TryGenerateNewTile();
            this.TryGenerateNewTile();
        }
        this.CheckGameOver();
    }

    private IEnumerator InternalMoveWithDelay(MoveDirection moveDirection)
    {
        Tile[] tileInLine;
        int matrixSize = this.allTile.GetLength(0);
        this.lineCompleted = new bool[matrixSize];
        for (int i = 0; i < matrixSize; i++)
        {
            this.lineCompleted[i] = false;
            if (moveDirection == MoveDirection.Up || moveDirection == MoveDirection.Down)
                tileInLine = this.GetTilesFromOneColumn(i, moveDirection);
            else
                tileInLine = this.GetTilesFromOneRow(i, moveDirection);
            StartCoroutine(this.UpdateTileOnOneLineWithDelay(i, tileInLine));
        }
        while (this.lineCompleted.Any(x => x == false))
        {
            Debug.Log("wait for coroutine");
            yield return null;
        }
        if (this.isMoved)
        {
            this.TryGenerateNewTile();
            this.TryGenerateNewTile();
        }
        this.CheckGameOver();
    }

    private void UpdateTileOnOneLine(int lineIndex, Tile[] tileInLine)
    {
        int size = tileInLine.Length;
        for (int i = 0; i < size; i++)
        {
            var nextNotZeroPos = FindNextNonZeroPos(tileInLine, i + 1);
            if (nextNotZeroPos == -1) continue;

            Tile curTile = tileInLine[i];
            Tile nextTile = tileInLine[nextNotZeroPos];
            if (curTile.Number == 0)
            {// Move tile
                curTile.Number = nextTile.Number;
                nextTile.Number = 0;
                i -= 1;
                this.emptyTiles.Remove(curTile);
                this.emptyTiles.Add(nextTile);
                this.isMoved = true;
            }
            else if (curTile.Number == nextTile.Number)
            {// Merge tile
                ScoreManager.GetInstance().Score += curTile.Number;
                curTile.Number *= 2;
                nextTile.Number = 0;
                this.emptyTiles.Add(nextTile);
                this.isMoved = true;
            }
        }
    }

    private IEnumerator UpdateTileOnOneLineWithDelay(int lineIndex, Tile[] tileInLine)
    {
        int size = tileInLine.Length;
        Tile curTile, nextTile;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size - 1; j++)
            {
                curTile = tileInLine[j];
                nextTile = tileInLine[j + 1];
                if (curTile.Number == 0 && nextTile.Number != 0)
                {
                    curTile.Number = nextTile.Number;
                    nextTile.Number = 0;
                    this.emptyTiles.Add(nextTile);
                    this.emptyTiles.Remove(curTile);
                    this.isMoved = true;
                    yield return new WaitForSeconds(this.Delay);
                }
            }
        }
        int pos = -1;
        for (int i = 0; i < size - 1; i++)
        {
            curTile = tileInLine[i];
            nextTile = tileInLine[i + 1];
            if (curTile.Number != 0 && curTile.Number == nextTile.Number)
            {
                ScoreManager.GetInstance().Score += curTile.Number;
                curTile.Number *= 2;
                nextTile.Number = 0;
                this.emptyTiles.Add(nextTile);
                pos = i + 1;
                this.isMoved = true;
                yield return new WaitForSeconds(this.Delay);
                break;
            }
        }
        for (int i = pos; pos != -1 && i < size - 1; i++)
        {
            curTile = tileInLine[i];
            nextTile = tileInLine[i + 1];
            if (curTile.Number == 0 && nextTile.Number != 0)
            {
                curTile.Number = nextTile.Number;
                nextTile.Number = 0;
                this.emptyTiles.Add(nextTile);
                this.emptyTiles.Remove(curTile);
                this.isMoved = true;
                yield return new WaitForSeconds(this.Delay);
            }
        }
        this.lineCompleted[lineIndex] = true;
    }


    private static int FindNextNonZeroPos(Tile[] tileInLine, int pos)
    {
        for (int i = pos; i < tileInLine.Length; i++)
        {
            if (tileInLine[i].Number != 0) return i;
        }
        return -1;
    }

    private Tile[] GetTilesFromOneColumn(int pos, MoveDirection moveDirection)
    {
        int matrixSize = this.allTile.GetLength(0);
        Tile[] result = new Tile[matrixSize];
        for (int lineIndex = 0; lineIndex < matrixSize; lineIndex++)
        {
            if (moveDirection == MoveDirection.Up)
                result[lineIndex] = this.allTile[lineIndex, pos];
            else
                result[lineIndex] = this.allTile[matrixSize - lineIndex - 1, pos];
        }
        return result;
    }

    private Tile[] GetTilesFromOneRow(int pos, MoveDirection moveDirection)
    {
        int matrixSize = this.allTile.GetLength(0);
        Tile[] result = new Tile[matrixSize];
        for (int lineIndex = 0; lineIndex < matrixSize; lineIndex++)
        {
            if (moveDirection == MoveDirection.Left)
                result[lineIndex] = this.allTile[pos, lineIndex];
            else
                result[lineIndex] = this.allTile[pos, matrixSize - lineIndex - 1];
        }
        return result;
    }
}
