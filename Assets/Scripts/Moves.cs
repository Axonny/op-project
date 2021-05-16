using UnityEngine;

public class Moves
{
    private MoveType _moveType;
    public readonly Vector2Int move;
    
    public Moves(MoveType moveType, Vector2Int move)
    {
        _moveType = moveType;
        this.move = move;
    }
}