using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataCaveGenerator : MonoBehaviour
{
    [Header("Cave Generation Settings")]
    public int width;
    public int height;
    public string seed;
    public bool useRandomSeed;
    public int smoothIterations;
    public int minRoomSize;
    [Range(0, 100)]
    public int fillPercent;

    [Header("Prefabs")]
    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject torchPrefab;
    public GameObject gemPrefab;

    [Header("Torch and Gem Settings")]
    public int numberOfTorches;
    public int numberOfGems;

    public int[,] map;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }

        List<List<Vector2Int>> regions = GetRegions(0);
        List<List<Vector2Int>> filteredRegions = new List<List<Vector2Int>>();

        foreach (List<Vector2Int> region in regions)
        {
            if (region.Count >= minRoomSize)
            {
                filteredRegions.Add(region);
            }
        }

        ConnectRegions(filteredRegions);

        CreateTiles();

        //PlaceTorchesAndGems(filteredRegions);
        PlaceTorchesAndGems();
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = DateTime.Now.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < fillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    List<List<Vector2Int>> GetRegions(int tileType)
    {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        int[,] flags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (flags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Vector2Int> region = GetRegionTiles(x, y);
                    regions.Add(region);

                    foreach (Vector2Int tile in region)
                    {
                        flags[tile.x, tile.y] = 1;
                    }
                }
            }
        }

        return regions;
    }

    List<Vector2Int> GetRegionTiles(int startX, int startY)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        int[,] flags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        flags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.x; x <= tile.x + 1; x++)
            {
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.y || x == tile.x))
                    {
                        if (flags[x, y] == 0 && map[x, y] == tileType)
                        {
                            flags[x, y] = 1;
                            queue.Enqueue(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void ConnectRegions(List<List<Vector2Int>> regions)
    {
        for (int i = 0; i < regions.Count - 1; i++)
        {
            Vector2Int regionA_Center = GetRegionCenter(regions[i]);
            Vector2Int regionB_Center = GetRegionCenter(regions[i + 1]);
            List<Vector2Int> path = GetPath(regionA_Center, regionB_Center);
            foreach (Vector2Int point in path)
            {
                map[point.x, point.y] = 0;
            }
        }
    }

    Vector2Int GetRegionCenter(List<Vector2Int> region)
    {
        int xSum = 0;
        int ySum = 0;
        foreach (Vector2Int tile in region)
        {
            xSum += tile.x;
            ySum += tile.y;
        }

        return new Vector2Int(xSum / region.Count, ySum / region.Count);
    }

    List<Vector2Int> GetPath(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = from;
        while (current != to)
        {
            path.Add(current);

            if (Mathf.Abs(to.x - current.x) > Mathf.Abs(to.y - current.y))
            {
                current = new Vector2Int(current.x + (int)Mathf.Sign(to.x - current.x), current.y);
            }
            else
            {
                current = new Vector2Int(current.x, current.y + (int)Mathf.Sign(to.y - current.y));
            }
        }

        return path;
    }
    void CreateTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, y, 0);
                if (map[x, y] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(groundPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }


    void PlaceTorchesAndGems()
    {
        System.Random random = new System.Random(seed.GetHashCode());

        for (int i = 0; i < numberOfTorches; i++)
        {
            int x, y;
            do
            {
                x = random.Next(1, width - 1);
                y = random.Next(1, height - 1);
            } while (map[x, y] == 1);

            Instantiate(torchPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
        }

        for (int i = 0; i < numberOfGems; i++)
        {
            int x, y;
            do
            {
                x = random.Next(1, width - 1);
                y = random.Next(1, height - 1);
            } while (map[x, y] == 1);

            Instantiate(gemPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
        }
    }
}