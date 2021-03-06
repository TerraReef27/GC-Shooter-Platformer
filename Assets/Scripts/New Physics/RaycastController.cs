﻿using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    #region Variables
    protected float skinSize = 0.0125f;
    const float rayDistance = .2f;
    protected int numHorizontalRays;
    protected int numVerticalRays;

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

    protected BoxCollider2D mainCollider;
    [SerializeField] protected LayerMask solidColisionMask;
    [SerializeField] protected LayerMask interactionMask;
    #endregion

    public virtual void Awake()
    {
        
    }

    public virtual void Start()
    {
        mainCollider = GetComponentInChildren<ColliderController>().ActiveCollider;
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

        float boundsX = bounds.size.x;
        float boundsY = bounds.size.y;
        
        numHorizontalRays = (int)Mathf.Ceil(boundsY / rayDistance); //Set the number of rays. Must be at least two
        numVerticalRays = (int)Mathf.Ceil(boundsX / rayDistance);

        horizontalRaySpace = boundsY / (numHorizontalRays - 1); //Get the distance between the individual rays
        verticalRaySpace = boundsX / (numVerticalRays - 1);
    }

    public void ChangeActiveCollider()
    {
        mainCollider = GetComponentInChildren<ColliderController>().ActiveCollider;
        CalculateSpacing();
        UpdateRayOrigin();
    }
}
