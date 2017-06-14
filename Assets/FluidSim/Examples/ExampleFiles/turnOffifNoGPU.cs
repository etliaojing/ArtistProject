
using UnityEngine;
using UnityEditor;

[AddComponentMenu("FluidSim/FluidSim Influence Actor")]
class turnOffifNoGPU : MonoBehaviour
{
    private GameObject objectToDestroy;
		
    private FluidSimScript tempScript;

    void Start()
    {
        objectToDestroy = GameObject.Find("LargeFluidSimInfluenceActorObject");

        tempScript = GetComponent<FluidSimScript>();

	    if(!tempScript.useUnityProMethod)
	    {
		    Invoke("DestroyStatic", 0.3f);
		    Invoke("ClearStatic", 0.4f);
	    }
    }

    void DestroyStatic()
    {
	    //clear the static collision buffer after the static actor is destroyed.  We do this for GPU vs CPU examples.
	    Destroy(objectToDestroy);
    }

    void ClearStatic()
    {
	    //clear the static collision buffer after the static actor is destroyed.  We do this for GPU vs CPU examples.
	    gameObject.BroadcastMessage("RecreateStaticCollisionBuffer", true);
    }
}