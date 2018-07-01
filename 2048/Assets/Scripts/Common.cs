
public enum MoveDirection
{
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