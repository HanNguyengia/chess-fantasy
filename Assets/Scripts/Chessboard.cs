using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chessboard : MonoBehaviour
{
    [SerializeField] private Transform tilesContainer;
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize;
    [SerializeField] private float zOffset;
    [SerializeField] private Vector3 boardCenter;

    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
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
            }

            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else
        {
            info = Physics2D.Raycast(ray.origin, ray.direction, 100, LayerMask.GetMask("Hover"));
            if (!info && currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }

    // Generate Board
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
