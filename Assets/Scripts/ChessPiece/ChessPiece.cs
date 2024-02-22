using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6,
}

public class ChessPiece : MonoBehaviour
{
    public ChessTeam team;
    public ChessPieceType type;
    public int currentX;
    public int currentY;

    public Vector3 desiredPosition;
    public Vector3 desiredScale;
}
