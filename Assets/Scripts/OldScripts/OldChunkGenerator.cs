using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class chunkGenerator : MonoBehaviour
{
    [SerializeField]
    public GameObject[] chunks;
    [SerializeField]
    private float chunkSize = 300;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    public GameObject enemy;
    [SerializeField]
    public GameObject healthPack
        ;
    private int relativePosition;
    private int distanceTraveled;

    private int lastPositionSpawned = 150;
    private Queue<GameObject> renderedObjects = new Queue<GameObject>();
    private ArrayList enemySpawnPoints = new ArrayList();

    private float lastZCoor = 10;
    private void Update()
    {
        spawnChunks();

    }
    /**
    *   Takes the spawn position of the player and spawns a new chunk if that position is at x=150
    */
    void spawnChunks()
    {
        if (relativePosition >= chunkSize)
        {
            relativePosition = 0;
            distanceTraveled = Mathf.RoundToInt(player.transform.position.x);
        }
        else
        {
            relativePosition = Mathf.RoundToInt(player.transform.position.x) - distanceTraveled;
        }
        if (relativePosition == 150)

        {
            int nextSpawnPosition = Mathf.RoundToInt(distanceTraveled) + 300 + Mathf.RoundToInt(relativePosition);
            if (lastPositionSpawned != nextSpawnPosition)
            {
                renderedObjects.Enqueue(Instantiate(chunks[Random.Range(0, chunks.Length)], new Vector3(nextSpawnPosition, 0, 0), Quaternion.Euler(0, 0, 0)));
        
                for (int i = 0; i < Random.Range(8, 14); i++){
                  
                    spawnEnemies();
                }

                if (renderedObjects.Count > 2)
                {
                    Destroy(renderedObjects.Dequeue());
                    enemySpawnPoints.Clear(); 
                }
            }

            lastPositionSpawned = nextSpawnPosition;
        }
        /* if(distanceTraveled == 300)
         {
             Debug.Log(distanceTraveled);
             player.transform.position = new Vector3(2, player.transform.position.y, player.transform.position.z);
             distanceTraveled = Mathf.RoundToInt(player.transform.position.x);
             relativePosition = Mathf.RoundToInt(player.transform.position.x) - distanceTraveled;
             camera.transform.position = new Vector3(-2, camera.transform.position.y, camera.transform.position.z);
         }*/
    }
    void spawnEnemies()
    {
    
        float[] possibleZCoor = { -3.5f,-1.75f, 0, 1.75f,3.5f };
        int xCoor = Random.Range(0, 30) * 10;
        /*while (enemySpawnPoints.Contains(xCoor))
        {
            xCoor = Random.Range(0, 30) * 10;
        }
        */
        enemySpawnPoints.Add(xCoor);
        
        float zCoor = possibleZCoor[Random.Range(0, 5)];

        if (Random.Range(0,25) == 1)
        {
            Instantiate(healthPack, new Vector3(Mathf.RoundToInt(player.transform.position.x) + 150 + xCoor, 0.933f, zCoor), Quaternion.identity);
        }
        else
        {
            Instantiate(enemy, new Vector3(Mathf.RoundToInt(player.transform.position.x) + 150 + xCoor, 0.72f, zCoor), Quaternion.Euler(0, -90, 0));
        }
       


    }

}
