using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkVelocityEmitter : InkActor {

    public bool useVelocityMaskTexture = false;
    public Texture2D velocityMaskTexture;
    public float velocityStrength = 0.5f;
    public float velocitySize = 0.5f;
    public float velocityFalloff = 0.5f;

    //=============================================================================================

    protected override void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.red;
            if (multiplySizeByScale)
            {
                Gizmos.DrawWireSphere(transform.position
                                     , velocitySize * transform.localScale.magnitude * 0.577f
                                     );
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, velocitySize);
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
