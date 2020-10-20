using System;
using UnityEngine;

public class Room 
{
    // Coordinates
    public int x1;
    public int x2;
    public int y1;
    public int y2;
    // Dimensions
    public int w;
    public int h;
    public (int, int) c;

    public Room(int x, int y, int w, int h)
    {
        x1 = x;
        x2 = x + w;
        y1 = y;
        y2 = y + h;
        this.w = w;
        this.h = h;
        c = ((int) Math.Round((double) (x1 + x2) / 2),(int) Math.Round((double) (y1 + y2) / 2));
    }

    public bool Intersects(Room room)
    {
        return (x1 <= room.x2 && x2 >= room.x1 && y1 <= room.y2 && room.y2 >= room.y1);
    }

}
