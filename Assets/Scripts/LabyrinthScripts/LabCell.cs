using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LabCell
{
    public bool[] walls = new bool[5];

    bool Open = false;

    public LabCell()
    {
        for (int i = 1; i <= 4; i++)
        {
            walls[i] = true;
        }
    }

    public void DestroyWalls()
    {
        for (int i = 1; i <= 4; i++)
        {
            walls[i] = false;
        }
        Open = true;
    }

    public void DestroyWall(int dir)
    {
        walls[dir] = false;
        Open = true;
    }

    public void MakeOpen()
    {
        Open = true;
    }


    public bool isOpen()
    {
        return Open;
    }

}

