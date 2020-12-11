using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyContainer", menuName = "ScriptableObjects/EnemyContainer", order = 1)]

public class EnemyContainer : ScriptableObject
{
    [Tooltip("An array of Enemies.")]
    public Enemy[] enemyArray;
}
