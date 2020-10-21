using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class MazeGenerator : MonoBehaviour
{
    // Tiles
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
    // Tilemaps
    public Tilemap groundMap;
    public Tilemap outerWallMap;
    public Tilemap innerWallMap;
    public Tilemap edgeMap;
    // Variables
    public int width;
    public int height;
    public int maxRooms;
    public int minRoomSize;
    public int maxRoomSize;
    private List<Room> rooms;
    
    // Start is called before the first frame update
    void Start()
    {
        PlaceRooms();
        FillTiles();
    }

    // Update is called once per frame
    void PlaceRooms()
    {
        rooms = new List<Room>();
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

                if (rooms.Count != 0)
                {
                    (int, int) prevCenter = rooms[rooms.Count - 1].c;
                    Debug.Log("Joining rooms " + newCenter + " " + prevCenter);
                    if (random.Next(2) == 1)
                    {
                        HorizontalCorridor(prevCenter.Item1, newCenter.Item1, prevCenter.Item2);
                        VerticalCorridor(prevCenter.Item2, newCenter.Item2, newCenter.Item1);
                    } 
                    else 
                    {
                        VerticalCorridor(prevCenter.Item2, newCenter.Item2, prevCenter.Item1);
                        HorizontalCorridor(prevCenter.Item1, newCenter.Item1, newCenter.Item2);
                    }
                }
                
                rooms.Add(newRoom);
                Debug.Log($"Room created: position ({newRoom.c.Item1},{newRoom.c.Item2}), width {newRoom.w}, height {newRoom.h}, center ({newRoom.c.Item1},{newRoom.c.Item2})");
            }
        }
    }

    private void CreateRoom(Room room)
    {
        for (int x = room.x1; x < room.x2; x++)
        {
            for (int y = room.y1; y < room.y2; y++)
            {
                groundMap.SetTile(new Vector3Int(x,y,0), groundTile);
            }
        }
    }

    private void HorizontalCorridor(int x1, int x2, int y)
    {
        for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2) + 1; x++)
        {
            groundMap.SetTile(new Vector3Int(x, y, 0), groundTile);
            groundMap.SetTile(new Vector3Int(x, y + 1, 0), groundTile);
        }
    }

    private void VerticalCorridor(int y1, int y2, int x)
    {
        for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2) + 1; y++)
        {
            groundMap.SetTile(new Vector3Int(x,y,0), groundTile);
            groundMap.SetTile(new Vector3Int(x + 1, y, 0), groundTile);
        }
    }
    
    private void FillTiles()
    {
        BoundsInt bounds = groundMap.cellBounds;
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                Vector3Int pos = new Vector3Int(xMap, yMap, 0);
                Vector3Int posLower = new Vector3Int(xMap, yMap - 1, 0);
                Vector3Int posUpper = new Vector3Int(xMap, yMap + 1, 0);

                if (groundMap.GetTile(pos) == null)
                {
                    //If it is a top wall
                    if (groundMap.GetTile(posLower) != null)
                    {
                        //Fill in a wall and edge
                        outerWallMap.SetTile(pos,wallTile);
                        edgeMap.SetTile(posUpper,edgeTile);
                        
                    }
                    //If it is a bottom wall
                    else if (groundMap.GetTile(posUpper) != null)
                    {
                        //Fill in a wall and edge
                        outerWallMap.SetTile(pos,wallTile);
                        edgeMap.SetTile(posUpper,edgeTile);
                    }
                }
            }
        }
        
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                Vector3Int pos = new Vector3Int(xMap, yMap, 0);
                Vector3Int posLower = new Vector3Int(xMap, yMap - 1, 0);
                Vector3Int posUpper = new Vector3Int(xMap, yMap + 1, 0);
                Vector3Int posLeft = new Vector3Int(xMap - 1, yMap, 0);
                Vector3Int posRight = new Vector3Int(xMap + 1, yMap, 0);
                Vector3Int posUpperLeft = new Vector3Int(xMap - 1, yMap + 1, 0);
                Vector3Int posUpperRight = new Vector3Int(xMap + 1, yMap + 1, 0);
                Vector3Int posLowerLeft = new Vector3Int(xMap - 1, yMap - 1, 0);
                Vector3Int posLowerRight = new Vector3Int(xMap + 1, yMap - 1, 0);

                if (groundMap.GetTile(pos) != null)
                {
                    //It it is a left edge
                    if (groundMap.GetTile(posLeft) == null && outerWallMap.GetTile(posLeft) == null)
                    {
                        //If it is a top left corner
                        if (outerWallMap.GetTile(posUpper) != null)
                        {
                            //Fill in a top left corner
                            edgeMap.SetTile(pos,leftEdgeTile);
                            edgeMap.SetTile(posUpper,leftEdgeTile);
                        }
                        //If it is a bottom left corner
                        else if (outerWallMap.GetTile(posLower) != null)
                        {
                            //Fill in a bottom left corner
                            edgeMap.SetTile(pos,leftCornerTile);
                        }
                        else
                        {
                            //Fill in a left edge
                            edgeMap.SetTile(pos,leftEdgeTile);
                        }
                    }
                    
                    //If it is a right edge
                    if (groundMap.GetTile(posRight) == null && outerWallMap.GetTile(posRight) == null)
                    {
                        //If it is a top right corner
                        if (outerWallMap.GetTile(posUpper) != null)
                        {
                            //Fill in a top right corner
                            edgeMap.SetTile(pos,rightEdgeTile);
                            edgeMap.SetTile(posUpper,rightEdgeTile);
                        }
                        //If it is a bottom right corner
                        else if (outerWallMap.GetTile(posLower) != null)
                        {
                            //Fill in a bottom right corner
                            edgeMap.SetTile(pos,rightCornerTile);
                        }
                        else
                        {
                            //Fill in a right edge
                            edgeMap.SetTile(pos,rightEdgeTile);
                        }
                    }
                    
                    //If a left edge appears by a wall
                    if (outerWallMap.GetTile(posLeft) != null && groundMap.GetTile(posRight) != null && groundMap.GetTile(posUpper) != null && groundMap.GetTile(posLowerLeft) == null)
                        edgeMap.SetTile(pos,leftEdgeTile);
                    //If a right edge appears by a wall
                    if (outerWallMap.GetTile(posRight) != null && groundMap.GetTile(posLeft) != null && groundMap.GetTile(posUpper) != null && groundMap.GetTile(posLowerRight) == null)
                        edgeMap.SetTile(pos,rightEdgeTile);
                    
                    //If two walls appear diagonally
                    if (outerWallMap.GetTile(posLeft) != null && outerWallMap.GetTile(posUpper) != null)
                    {
                        edgeMap.SetTile(posUpperLeft, rightCornerTile);
                        edgeMap.SetTile(new Vector3Int(xMap - 1, yMap + 2, 0),leftPointTile);
                    }

                    if (outerWallMap.GetTile(posRight) != null && outerWallMap.GetTile(posUpper) != null)
                    {
                        edgeMap.SetTile(posUpperRight, leftCornerTile);
                        edgeMap.SetTile(new Vector3Int(xMap + 1, yMap + 2, 0),rightPointTile);
                    }

                    if (outerWallMap.GetTile(posLeft) != null && outerWallMap.GetTile(posLower) != null)
                    {
                        edgeMap.SetTile(pos,leftCornerTile);
                        edgeMap.SetTile(posUpper,rightPointTile);
                    }
                    if (outerWallMap.GetTile(posRight) != null && outerWallMap.GetTile(posLower) != null)
                    {
                        edgeMap.SetTile(pos,rightCornerTile);
                        edgeMap.SetTile(posUpper,leftPointTile);
                    }
                }

                if (outerWallMap.GetTile(pos) != null)
                {
                    //If a wall comes to a point
                    if (groundMap.GetTile(posLeft) != null && groundMap.GetTile(posUpper) != null)
                        edgeMap.SetTile(posUpperLeft,leftPointTile);
                    if (groundMap.GetTile(posRight) != null && groundMap.GetTile(posUpper) != null)
                        edgeMap.SetTile(posUpperRight,rightPointTile);
                    
                    //If a wall extends with the edge
                    if (groundMap.GetTile(posLeft) != null && edgeMap.GetTile(posLeft) == null && edgeMap.GetTile(posUpper) != null && outerWallMap.GetTile(posUpperLeft) == null)
                    {
                        innerWallMap.SetTile(posLeft,leftWallTile);
                    }
                    if (groundMap.GetTile(posRight) != null && edgeMap.GetTile(posRight) == null && edgeMap.GetTile(posUpper) != null && outerWallMap.GetTile(posUpperRight) == null)
                    {
                        innerWallMap.SetTile(posRight,rightWallTile);
                    }
                }
            }
        }
    }

}
