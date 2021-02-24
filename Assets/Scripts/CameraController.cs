using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("What the camera is trying to follow")]
    [SerializeField] private GameObject objectToFollow = null;

    private float distanceFromScene = -10; //The Z coordinate of the camera from the scene. Should always be set above 0
    [Tooltip("How fast the camera will attempt to follow the target")]
    [SerializeField] private float cameraSpeed = 5;
    [Tooltip("The distance the objctToFollow can get from the camera before the camera begins to follow it")]
    [SerializeField] private Vector2 cameraGap = new Vector2(2, 2);

    [SerializeField] private int pixelCount = 16;

    [SerializeField] private Vector2 moveTo;

    private void Start()
    {
        moveTo = objectToFollow.transform.position;
        transform.position = new Vector3(moveTo.x, moveTo.y, distanceFromScene); //Starts the camera at the target position
    }

    void Update()
    {
        if (objectToFollow != null)
        {
            moveTo = objectToFollow.transform.position - transform.position; //Gets the direction that the camera should move towards
            
            if (moveTo.x > cameraGap.x || moveTo.x < -cameraGap.x || moveTo.y > cameraGap.y || moveTo.y < -cameraGap.y)  //Checks if the objectToFollow is outside of the follow range. If it is, the camera will begin to follow the object relative to the cameraSpeed
            {
                transform.Translate(moveTo.x * cameraSpeed * Time.deltaTime, moveTo.y * cameraSpeed * Time.deltaTime, distanceFromScene);

                int newX = Mathf.FloorToInt(transform.position.x * pixelCount);
                int newY = Mathf.FloorToInt(transform.position.y * pixelCount);
                
                transform.position = new Vector3(newX/pixelCount, newY/pixelCount, distanceFromScene);
            }
        }
    }
}

