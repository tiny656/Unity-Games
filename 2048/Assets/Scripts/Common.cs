
public enum MoveDirection
{
    Idle,
    Left,
    Right,
    Up,
    Down
}

public class Position
{
    public int RowIndex;
    public int ColIndex;

    public bool IsValidPosition()
    {
        return this.RowIndex >= 0 && this.ColIndex >= 0;
    }
}