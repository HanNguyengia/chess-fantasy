using UnityEngine;

public enum ChessTeam
{
    White = 0,
    Black = 1,
}

public class Chessboard : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private Transform tilesContainer;
    [SerializeField] private Transform chessPiecesContainer;

    [Header("Tiles")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material hoverTileMaterial;
    [SerializeField] private float tileSize;
    [SerializeField] private float zOffset;
    [SerializeField] private Vector3 boardCenter;

    [Header("ChessPieces")]
    [SerializeField] private GameObject prefabs;
    [SerializeField] private ChessTeamSpritesListSO chessTeamSpritesListSO;

    private ChessPiece[,] chessPieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit2D info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        info = Physics2D.Raycast(ray.origin, ray.direction, 100, LayerMask.GetMask("Tile"));
        if (info)
        {
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Hover");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = hoverTileMaterial;
            }

            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                currentHover = hitPosition;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Hover");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = hoverTileMaterial;
            }
        }
        else
        {
            info = Physics2D.Raycast(ray.origin, ray.direction, 100, LayerMask.GetMask("Hover"));
            if (!info && currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                currentHover = -Vector2Int.one;
            }
        }
    }

    // Generate the Board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        zOffset += transform.position.z;
        bounds = new Vector3((tileCountX / 2) * tileSize, (tileCountY / 2) * tileSize, 0) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject($"X:{x}, Y:{y}");
        tileObject.transform.parent = tilesContainer;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, y * tileSize, zOffset) - bounds;
        vertices[1] = new Vector3(x * tileSize, (y + 1) * tileSize, zOffset) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, y * tileSize, zOffset) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, (y + 1) * tileSize, zOffset) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();
        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider2D>();

        return tileObject;
    }

    // Spawn of the pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        // white team
        chessPieces[0, 0] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.Rook);
        chessPieces[1, 0] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.Knight);
        chessPieces[2, 0] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.Bishop);
        chessPieces[3, 0] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.Queen);
        chessPieces[4, 0] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.King);
        chessPieces[5, 0] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.Bishop);
        chessPieces[6, 0] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.Knight);
        chessPieces[7, 0] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.Rook);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 1] = SpawnSinglePiece(ChessTeam.White, ChessPieceType.Pawn);
        }

        // black team
        chessPieces[0, 7] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.Rook);
        chessPieces[1, 7] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.Knight);
        chessPieces[2, 7] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.Bishop);
        chessPieces[3, 7] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.Queen);
        chessPieces[4, 7] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.King);
        chessPieces[5, 7] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.Bishop);
        chessPieces[6, 7] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.Knight);
        chessPieces[7, 7] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.Rook);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessTeam.Black, ChessPieceType.Pawn);
        }
    }
    private ChessPiece SpawnSinglePiece(ChessTeam team, ChessPieceType type)
    {
        GameObject chessPieceGO = Instantiate(prefabs, chessPiecesContainer);
        chessPieceGO.name = $"{team}-{type}";
        chessPieceGO.AddComponent(System.Type.GetType($"{type}"));

        ChessPiece cp = chessPieceGO.GetComponent<ChessPiece>();
        cp.team = team;
        cp.type = type;

        chessPieceGO.GetComponent<SpriteRenderer>().sprite = chessTeamSpritesListSO.ctSpritesSOList[(int)team].ctSprites[(int)type - 1];

        return cp;
    }

    // Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null) PositionSinglePiece(x, y, true);
            }
        }
    }
    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].transform.position = GetTileCenter(x, y);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, y * tileSize, zOffset) - bounds + new Vector3(tileSize / 2, tileSize / 2, 0);
    }

    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x, y] == hitInfo) return new Vector2Int(x, y);
            }
        }

        return -Vector2Int.one; // Invalid value
    }
}
