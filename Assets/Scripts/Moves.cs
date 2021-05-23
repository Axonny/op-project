using UnityEngine;

public class Moves
{
    private MoveType moveType;
    public readonly Vector2Int Move;
    
    public Moves(MoveType moveType, Vector2Int move)
    {
        this.moveType = moveType;
        Move = move;
    }
}