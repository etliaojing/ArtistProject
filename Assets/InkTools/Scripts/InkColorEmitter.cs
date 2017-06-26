using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkColorEmitter : InkActor {

    public bool useColorMaskTexture = false;
    public Texture2D colorMaskTexture;
    public Color colorValue = Color.white;
    public float colorSize = 0.5f;
    public float colorFalloff = 0.5f;

    //=============================================================================================

    protected override void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.red;
            if (multiplySizeByScale)
            {
                Gizmos.DrawWireSphere(transform.position
                                     , colorSize * transform.localScale.magnitude * 0.577f
                                     );
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, colorSize);
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
