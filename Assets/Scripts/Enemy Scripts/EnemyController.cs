using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    #region Variables
    private Enemy enemy;
    private GameObject clone;

    [Tooltip("Refernce to the player. To set the target of the enemies.")]
    [SerializeField] private GameObject player;
    [Tooltip("ScriptableOject EnemyContainer.")]
    [SerializeField] private EnemyContainer enemyContainer;

    [Tooltip("Array of spawn points for each enemy. Name the game object the same as the enemy or the enemy will spawn at a default location.")]
     private Transform[] spawnLocations;

    [SerializeField] private GameObject spawnLocationContainer;

    [Tooltip("Game time. How long the game has been running. Currently used for enemy spawn times.")]
    [SerializeField] private double gameTime;
    #endregion
    private void Start()
    {
        foreach(Enemy enemy in enemyContainer.enemyArray)
        {
            //Spawn all enemies at their respective location
            #region Spawn Enemies
            //if (spawnLocationContainer.transform.GetChild(spawnLocalIterator).name.Equals(enemy.enemyName) || true)
            //{
            //    enemy.spawnLocation = spawnLocationContainer.transform.GetChild(spawnLocalIterator).transform;
            //    spawnLocalIterator++;
            //}
            //else
            //    enemy.spawnLocation.position.Set(0f, 0f, 0f);

            #endregion
            for (int i = 1; i <= enemy.spawnDuplicates; i++)
            {
                clone = Instantiate(enemy.enemyPrefab, enemy.spawnLocation[i-1], Quaternion.identity);
                clone.transform.parent = spawnLocationContainer.transform;
                //clone.GetComponent<AIDestinationSetter>().target = player.transform;
                clone.GetComponent<EnemyAI>().target = player.transform;
                clone.name = enemy.enemyName + " " + i;
            }



            //If the enemy can fly, turn off gravity for that enemy
            if (enemy.canFly)
            {
                clone.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
            
        }
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
    }
}
