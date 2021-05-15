using System.Drawing;

public class Moves
{
    private MoveType _moveType;
    public readonly Size move;
    
    public Moves(MoveType moveType, Size move)
    {
        _moveType = moveType;
        this.move = move;
    }
}