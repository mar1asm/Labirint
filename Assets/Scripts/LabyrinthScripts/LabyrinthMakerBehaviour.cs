using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthMakerBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject Wall;
    private GameObject ValidDestinationZoneOne;
    [SerializeField]
    private int size = 13;
    bool[,] map;

    private void Start()
    {
        if (size % 2 == 0)
        {
            size++;
        }

        map = new bool[size + 3, size + 3];
        map = GenerateMap(size);

        float lenghtofCube = Wall.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
        float startX = transform.position.x - lenghtofCube * ((size + 2) / 2);
        float startZ = transform.position.z - lenghtofCube * ((size + 2) / 2);

        for (int i = 0; i <= size + 1; i++)
        {
            float aux = startZ;
            for (int j = 0; j <= size + 1; j++)
            {
                if (!map[i, j])
                {
                    Instantiate(Wall, new Vector3(startX, transform.position.y + lenghtofCube / 2, aux), Quaternion.identity);
                }
                aux += lenghtofCube;
            }
            startX += lenghtofCube;
        }

    }




    bool[,] GenerateMap(int size)
    {
        bool[,] map = new bool[size + 3, size + 3];

        Wilson(map, size);


        return map;
    }
    void Wilson(bool[,] map, int size)
    {


        int s = size / 2 + 1;
        LabCell[,] miniLab = new LabCell[s + 2, s + 2];

        for (int i = 1; i <= s; i++)
        {
            for (int j = 1; j <= s; j++)
            {
                miniLab[i, j] = new LabCell();
            }
        }

        Vector2[] directions = { new Vector2(0, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };





        #region TheAlgorithm

        int Aux, x, y;
        short dir;

        #region ChoosingTheFirstCell

        Aux = ChooseCell(s);
        x = (int)Mathf.Ceil((float)Aux / s);
        y = Aux % s;
        if (y == 0) y = s;

        miniLab[x, y].MakeOpen();


        int visited = s * s - 1;

        #endregion



        while (visited > 0)
        {
            short[,] road = new short[s + 1, s + 1];
            #region ChoosingARandomEmptyCell

            List<Vector2> CloseCells = new List<Vector2>();

            for (int i = 1; i <= s; i++)
            {
                for (int j = 1; j <= s; j++)
                {
                    if (!miniLab[i, j].isOpen())
                        CloseCells.Add(new Vector2(i, j));
                }
            }

            //print(CloseCells.Count);

            int random = UnityEngine.Random.Range(0, CloseCells.Count - 1);
            //Mathf.Clamp(random, 0, CloseCells.Count);
            //print("Random number: " + random);
            Vector2 V = CloseCells[random];

            x = (int)V.x;
            y = (int)V.y;
            int initX = x;
            int initY = y;
            #endregion



            #region MakeARoad
            int debu = 0;

            while (!miniLab[x, y].isOpen())
            {

                float rand;


                do
                {
                    rand = UnityEngine.Random.Range(1, 5);

                    dir = (short)rand;
                    //print(dir);


                } while (!Inside(x + (int)directions[dir].x, y + (int)directions[dir].y, s));

                x += (int)directions[dir].x;
                y += (int)directions[dir].y;


                if (road[x, y] == 0)
                {
                    road[x, y] = dir;
                }


                if (miniLab[x, y].isOpen())
                {
                    //  print("am ajuns:" + visited);
                }

                debu++;
                if (debu >= 1000)
                    print("FF " + visited);
            }
            #endregion

            #region BackTrack
            debu = 0;
            while (x != initX || y != initY)
            {

                int d = road[x, y];
                if (!miniLab[x, y].isOpen()) visited--;
                miniLab[x, y].DestroyWall(Opposite(d));

                x += (int)directions[Opposite(d)].x;
                y += (int)directions[Opposite(d)].y;



                if (!miniLab[x, y].isOpen()) visited--;
                miniLab[x, y].DestroyWall(d);

            }
            #endregion



        }

        #endregion


        #region MakingTheMap
        for (int i = 1; i <= s; i++)
        {
            for (int j = 1; j <= s; j++)
            {
                int auxX, auxY;
                auxX = i * 2 - 1;
                auxY = j * 2 - 1;
                map[auxX, auxY] = true;
                for (int d = 1; d <= 4; d++)
                {
                    int newX, newY;
                    newX = auxX + (int)directions[d].x;
                    newY = auxY + (int)directions[d].y;
                    if (Inside(newX, newY, size))
                    {
                        if (!miniLab[i, j].walls[d])
                            map[newX, newY] = true;
                    }
                }
            }
        }
        #endregion




    }


    #region HelperFunctions

    int ChooseCell(int size)
    {
        return (int)Mathf.Ceil(UnityEngine.Random.Range(1, size * size));
    }
    int Opposite(int dir)
    {
        switch (dir)
        {
            case 1: return 3;
            case 2: return 4;
            case 3: return 1;
            case 4: return 2;
            default: return 0;
        }
    }

    bool Inside(int i, int j, int size)
    {
        return i >= 1 && i <= size && j >= 1 && j <= size;
    }


    #endregion
}
