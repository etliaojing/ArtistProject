using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///
/// </summary>
[AddComponentMenu("Inkling/Ink Actor")]
public abstract class InkActor : MonoBehaviour
{
    //=============================================================================================

    public    Vector3      lastFramePosition;
    public    float        lastFrameTime       = 0.0f;
    public    int          actorPriority       = 3;
    public    bool         multiplySizeByScale = false;

    public    bool         showGizmos          = true;

    protected GameObject   _inkConnectorObject;
    protected InkConnector _inkConnectorScript;

    protected int          _inkActorID;

    protected bool         _hasBeenDisabled    = false;

    //=============================================================================================

    protected abstract void OnDrawGizmos();

    //=============================================================================================

    protected virtual void Start()
    {
        lastFramePosition = transform.position;

        _inkConnectorObject = InkConnector.GetObjInstance();
        _inkConnectorScript = InkConnector.GetScriptInstance();

        if (_inkConnectorObject == null)
        {
            Debug.LogError("InkActor failed to find or create a InkConnector object.  Make sure"
                          + " the InkConnector script exists and can by found by the InkActor."
                          );
        }
        else if (_inkConnectorScript == null)
        {
            Debug.LogError("InkActor failed to find or create a InkConnector script.  Make sure"
                          + " the InkConnector script exists and can by found by the InkActor."
                          );
        }
        else
        {
            _inkActorID = _inkConnectorScript.AddActor(this);
        }
    }

    //=============================================================================================

    protected virtual void ChangeActorPriority(int tempInt)
    {
        tempInt = Mathf.Clamp(tempInt, 1, 5);

        actorPriority = tempInt;

        _inkConnectorScript.SortActorArray();
    }

    //=============================================================================================

    protected virtual void DestroyActor()
    {
        _inkConnectorScript.RemoveActor(_inkActorID);

        Destroy(gameObject, 0.1f);
    }

    //=============================================================================================

    protected virtual void OnEnable()
    {
        _hasBeenDisabled = false;
    }

    //=============================================================================================

    protected virtual void OnDisable()
    {
        _hasBeenDisabled = true;
    }

    //=============================================================================================

} // InkActor
