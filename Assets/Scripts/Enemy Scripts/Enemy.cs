using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]

public class Enemy : ScriptableObject
{
    [Header("Enemy characteristics.")]
    [Tooltip("Name of enemy.")]
    public string enemyName;
    [Tooltip("Speed of the enemy.")]
    public float movementSpeed;
    [Tooltip("How many of these enemies will spawn? Must have a spawn location for each.")]
    public int spawnDuplicates = 1;
    [Tooltip("Is the enemy grounded? Automatic Variable.")]
    public bool isGrounded;
    [Tooltip("Ability to fly?")]
    public bool canFly;
    [Tooltip("Ability to jump?")]
    public bool canJump;
    [Tooltip("Ability to hold a weapon?")]
    public bool canHoldWeapon;
    [Tooltip("Does this enemy patrol between two points?")]
    public bool patrols;
    [Tooltip("Does this enemy chase the player?")]
    public bool chases;
    [Tooltip("Dont spawn at compile time. Check this box if the enemy should spawn during gameplay. Based on some timer or player position.")]
    public bool spawnDuringRunTime;
    
    [Header("References.")]
    [Tooltip("Prefab of the enemy.")]
    public GameObject enemyPrefab;
    [Tooltip("Prefab of held weapon, if any.")]
    public GameObject weapon;
    [Tooltip("Where does the enemy spawn.")]
    public Vector3[] spawnLocation = new Vector3[1];
    [Tooltip("The amount of time to pass before the enemy spawns.")]
    public float spawnTime;
}
//[CustomEditor(typeof(Enemy))]
//public class EnemyScriptEditor : Editor
//{
//    void OnInspectorGUI()
//    {
//        var enemy = target as Enemy;

//        enemy.patrols = GUILayout.Toggle(enemy.patrols, "Can Fly");

//        if(enemy.patrols)
//        {
//            enemy.chases = true;
//        }
//    }
//}
