using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    #region Variables
    protected float skinSize = 0.0125f;
    [SerializeField] protected int numHorizontalRays = 5;
    [SerializeField] protected int numVerticalRays = 3;

    [HideInInspector]
    protected float horizontalRaySpace, verticalRaySpace;

    protected struct RayOrigins { public Vector2 topLeft, topRight, bottomLeft, bottomRight; }
    protected RayOrigins rayOrigins;
    public struct CollisionInfo
    {
        public bool isAbove, isBelow, isLeft, isRight;
        public bool isClimbingSlope, isDecendingSlope;
        public float slopeAngle, oldSlopeAngle;

        public Collider2D fallThrough;
        public bool isFallingThrough;

        public void ResetInfo()
        {
            isAbove = isBelow = isLeft = isRight = false;
            isClimbingSlope = isDecendingSlope = false;
            oldSlopeAngle = slopeAngle;
            slopeAngle = 0;
        }
    }

    [HideInInspector]
    protected BoxCollider2D mainCollider = new BoxCollider2D();
    [SerializeField] protected LayerMask collisionMask;
    #endregion

    void Awake()
    {
        mainCollider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateSpacing();
    }

    public void UpdateRayOrigin()
    {
        Bounds bounds = mainCollider.bounds;
        bounds.Expand(-skinSize * 2); //Add the skin to offset the bounds inwards

        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y); //Set the bounds of the collider
        rayOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        rayOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    public void CalculateSpacing()
    {
        Bounds bounds = mainCollider.bounds;
        bounds.Expand(-skinSize * 2); //Add the skin to offset the bounds

        numHorizontalRays = Mathf.Clamp(numHorizontalRays, 2, int.MaxValue); //Set the number of rays. Must be at least two
        numVerticalRays = Mathf.Clamp(numVerticalRays, 2, int.MaxValue);

        horizontalRaySpace = bounds.size.y / (numHorizontalRays - 1); //Get the distance between the individual rays
        verticalRaySpace = bounds.size.x / (numVerticalRays - 1);
    }
}
