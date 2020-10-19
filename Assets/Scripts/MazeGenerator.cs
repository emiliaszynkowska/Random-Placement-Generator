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
    private ArrayList rooms;
    
    // Start is called before the first frame update
    void Start()
    {
        PlaceRooms();
    }

    // Update is called once per frame
    void PlaceRooms()
    {
        rooms = new ArrayList();
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
                rooms.Add(newRoom);
            }
        }
    }

    void CreateRoom(Room room)
    {
        groundMap.SetTile(new Vector3Int(room.centerX,room.centerY,0), groundTile);
        groundMap.BoxFill(new Vector3Int(room.x1,room.y1,0), groundTile, room.x1, room.y1, room.x2, room.y2);
    }

}
