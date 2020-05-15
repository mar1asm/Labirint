using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    private Animator animator;
    bool[,] map;
    int mapSize;
    int yPosition = -1;  // nu avem podea? :))
    int mapDifference;  // pozitia map[1,1] === transform.position (1- mapDifference, yPosition, 1-mapDifference) 
    public GameObject gm;

    List<KeyValuePair<int, int>> navPositions = new List<KeyValuePair<int, int>>(); // pozitiile navigabile
    int currentPathIndex;
    List<Vector3> pathVectorList;
    PathFinding pathFinder;
    List<PathNode> path;
    public float speed = 0.1f;
    

    int startX = -1, startZ, endX, endZ;
    private float rotationSpeed=5f;

    enum enemyStates { roaming, chasing, gameOver, attack };
    enemyStates state;

    void OnEnable()
    {
        map = GameObject.Find("BaseFloor").GetComponent<LabyrinthMakerBehaviour>().map;
        mapSize = (int) Mathf.Sqrt(map.Length);
        mapDifference = mapSize/2-1;
        getNavigablePositions();
        pathFinder=new PathFinding(map);
        SetTarget();
        animator = GetComponent<Animator>();

        state = enemyStates.roaming;

    }

    void SetTarget()
    {
        currentPathIndex = 0;
        pathVectorList = null;

        if (state==enemyStates.chasing)
        {
            startX = (int)(transform.position.x + mapDifference)*2;
            startZ = (int)(transform.position.z + mapDifference)*2;

            transform.position = new Vector3((int)transform.position.x, transform.position.y, (int)transform.position.z);
            //endX = navPositions[40].Key; //TODO inlocuit cu pozitia playerului (int)
            //endZ = navPositions[40].Value;

           // endX = (int) GameObject.Find("Cube").transform.position.x+mapDifference;
            //endZ = (int) GameObject.Find("Cube").transform.position.z+mapDifference;

        }
        else
        {

            if (startX == -1)
            {
                startX = navPositions[0].Key;    //nu am stiut unde sa spawnez asa ca am spawnat o pe prima pozitie 'libera'
                startZ = navPositions[0].Value;
                Vector3 startingPosition = new Vector3((startX - mapDifference)*2, yPosition, (startZ - mapDifference)*2);
                transform.position = startingPosition;
            }
            else
            {
                startX = endX;  //continua de unde a ramas
                startZ = endZ;
            }
            int randomPosition = Random.Range(0, navPositions.Count);
            endX = navPositions[randomPosition].Key;    //alege o pozitie random din cele disponibile
            endZ = navPositions[randomPosition].Value;
        }
        path = pathFinder.FindPath(startX, startZ, endX, endZ);
        pathVectorList = FindPath();
    }

    public List<Vector3> FindPath()
    {
        List<Vector3> vectorPath = new List<Vector3>();
        foreach(PathNode pathNode in path)
            vectorPath.Add(new Vector3((pathNode.xValue - mapDifference)*2, yPosition, (pathNode.zValue - mapDifference)*2));

        vectorPath.RemoveAt(0);
     
        return vectorPath;
    }



    private void getNavigablePositions()
    {
        for (int i=0; i<mapSize; i++)
        {
            for (int j=0; j<mapSize; j++)
            {
                if (map[i, j])
                    navPositions.Add(new KeyValuePair<int, int>(i,j));
            }
        }

    }

    private void FindTarget()
    {
        float targetRange = 5f;  //distanta minima la care incepe urmarirea

        int newX= (int)GameObject.Find("Cube").transform.position.x;
        int newZ = (int)GameObject.Find("Cube").transform.position.z;
        if (Vector3.Distance(transform.position, new Vector3(newX, 1, newZ)) <= 2f) //te-o prins
        { 
            gameOver();
            return;
        }

            if (Vector3.Distance(transform.position, new Vector3(newX, 1, newZ)) < targetRange) //pozitia playerului
        {
            //chase that mf
            if (state!=enemyStates.chasing || currentPathIndex >= pathVectorList.Count)
            {
                state = enemyStates.chasing;
                animator.SetBool(state.ToString(), true);
                SetTarget(); 
            }
        }
        else if (state==enemyStates.chasing)
        {
            animator.SetBool(state.ToString(), false);
            state = enemyStates.roaming;
            endX = (int)transform.position.x+mapDifference;
            endZ = (int)transform.position.z+mapDifference;
            SetTarget();
        }
    }

    void gameOver()
    {
        state = enemyStates.gameOver;
        animator.SetBool(enemyStates.chasing.ToString(), false);
        animator.SetBool(enemyStates.attack.ToString(), true);
    }



    void Update()
    {
        if (state!=enemyStates.gameOver)
        {
                FindTarget();
                if (pathVectorList != null)
            {
                Vector3 targetPosition = pathVectorList[currentPathIndex];
                if (Vector3.Distance(transform.position, targetPosition) > 0.05f)
                {
                    Vector3 moveDir = (targetPosition - transform.position).normalized;
                    if (moveDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            Quaternion.LookRotation(moveDir),
                            Time.deltaTime * rotationSpeed
                        );
                    }
                    transform.position = transform.position + moveDir * speed * Time.deltaTime;
                }
                else
                {
                    transform.position = targetPosition;
                    currentPathIndex++;
                    if (currentPathIndex >= pathVectorList.Count && state!=enemyStates.chasing)
                        SetTarget();
                }
            }
            
        }
        
    }
}
