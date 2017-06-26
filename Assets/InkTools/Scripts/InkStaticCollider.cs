using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkStaticCollider : InkActor
{

    public bool useCollisionMaskTexture = false;
    public Texture2D collisionMaskTexture;
    public float collisionStrength = 1.0f;
    public float collisionSize = 0.5f;
    public float collisionFalloff = 0.8f;

    //=============================================================================================

    protected override void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.red;
            if (multiplySizeByScale)
            {
                Gizmos.DrawWireSphere(transform.position
                                     , collisionSize * transform.localScale.magnitude * 0.577f
                                     );
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, collisionSize);
            }
        }
    }

    //=============================================================================================

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    //=============================================================================================

    protected override void ChangeActorPriority(int tempInt)
    {
        base.ChangeActorPriority(tempInt);
    }

    //=============================================================================================

    protected override void DestroyActor()
    {
        base.DestroyActor();
    }

    //=============================================================================================

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    //=============================================================================================

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    //=============================================================================================

}
