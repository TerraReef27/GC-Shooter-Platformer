using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    private Vector3 baseRespawnPoint;
    [Tooltip("The object to respawn when called")]
    [SerializeField] GameObject targetObject;

    [SerializeField] private Gun objectGun; //TODO implement a better system for this

    void Start()
    {
        baseRespawnPoint = targetObject.transform.position;
    }

    //Respawns the object at the base position
    public void Respawn()
    {
        targetObject.transform.position = baseRespawnPoint;
        objectGun.RefillAmmo(); //TODO implement a better system for this
    }
    //Respawns the object at the designated position
    public void Respawn(Vector3 respawnPosition)
    {
        targetObject.transform.position = respawnPosition;
    }
}
