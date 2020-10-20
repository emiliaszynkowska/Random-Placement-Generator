using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class MazeGenerator : MonoBehaviour
{
    //Tiles
    public Tile groundTile;
    public Tile wallTile;
    public Tile edgeTile;
    public Tile leftEdgeTile;
    public Tile rightEdgeTile;
    public Tile leftCornerTile;
    public Tile rightCornerTile;
    public Tile leftPointTile;
    public Tile rightPointTile;
    public Tile leftWallTile;
    public Tile rightWallTile;
    //Tilemaps
    public Tilemap groundMap;
    // public Tilemap outerWallMap;
    // public Tilemap innerWallMap;
    // public Tilemap edgeMap;
    // Variables
    public int width;
    public int height;
    public int maxRooms;
    public int minRoomSize;
    public int maxRoomSize;
    private Room[] rooms;
    
    // Start is called before the first frame update
    void Start()
    {
        PlaceRooms();
    }

    // Update is called once per frame
    void PlaceRooms()
    {
        rooms = new Room[]{};
        (int, int) newCenter;
        Random random = new Random();

        for (int i = 0; i < maxRooms; i++)
        {
            int w = minRoomSize + random.Next(minRoomSize, maxRoomSize);
            int h = minRoomSize + random.Next(minRoomSize, maxRoomSize);
            int x = random.Next(width - w);
            int y = random.Next(height - h);

            Room newRoom = new Room(x, y, w, h);

            bool success = true;
            foreach (Room otherRoom in rooms)
            {
                if (newRoom.Intersects(otherRoom))
                    success = false;
            }

            if (success)
            {
                CreateRoom(newRoom);
                newCenter = newRoom.c;

                if (rooms.Length != 0)
                {
                    (int, int) prevCenter = rooms[rooms.Length - 1].c;
                    if (random.Next(2) == 1)
                    {
                        HorizontalCorridor(prevCenter.Item1, newCenter.Item1, prevCenter.Item2);
                        VerticalCorridor(prevCenter.Item2, newCenter.Item2, newCenter.Item1);
                    }
                    else
                    {
                        VerticalCorridor(prevCenter.Item2, newCenter.Item2, prevCenter.Item1);
                        HorizontalCorridor(prevCenter.Item1,newCenter.Item1,newCenter.Item2);
                    }
                }
                
                rooms.Append(newRoom);
            }
        }
    }

    void CreateRoom(Room room)
    {
        groundMap.SetTile(new Vector3Int(room.c.Item1,room.c.Item2,0), groundTile);
        groundMap.BoxFill(new Vector3Int(room.x1,room.y1,0), groundTile, room.x1, room.y1, room.x2, room.y2);
    }

    void VerticalCorridor(int y1, int y2, int x)
    {
        Random random = new Random();
        foreach (var y in Enumerable.Range(Math.Min(y1, y2), Math.Max(y1, y2)))
            groundMap.SetTile(new Vector3Int(x,y,0), groundTile);
    }
    
    void HorizontalCorridor(int x1, int x2, int y)
    {
        Random random = new Random();
        foreach (var x in Enumerable.Range(Math.Min(x1, x2), Math.Max(x1, x2)))
            groundMap.SetTile(new Vector3Int(x,y,0), groundTile);
    }

}
