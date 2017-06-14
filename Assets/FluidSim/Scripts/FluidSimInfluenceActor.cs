//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using UnityEditor;

[System.Serializable]
public class fluidInfluenceClass
{
    public bool staticCollision = false;
    public bool dynamicCollision = false;
    public bool useCollisionMaskTexture = false;
    public Texture2D collisionMaskTexture;
    public float collisionStrength = 1.0f;
    public float collisionSize = 0.5f;
    public float collisionFalloff = 0.8f;
    public float moveVelocityMultiplier = 1.0f;

    public bool addColor = true;
    public bool useColorMaskTexture = false;
    public Texture2D colorMaskTexture;
    public Color colorValue = Color.white;
    public float colorSize = 0.5f;
    public float colorFalloff = 0.5f;

    public bool addVelocity = true;
    public bool useVelocityMaskTexture = false;
    public Texture2D velocityMaskTexture;
    public float velocityStrength = 0.5f;
    public float velocitySize = 0.5f;
    public float velocityFalloff = 0.5f;

    public Transform myTransform;
    public Vector3 lastFramePosition;
    public float lastFrameTime = 0.0f;
    public int actorPriority = 3;
    public bool multiplySizeByScale = false;
}

[AddComponentMenu("FluidSim/FluidSim Influence Actor")]
public class FluidSimInfluenceActor : MonoBehaviour
{
public bool useGPUfeatures = true;

public bool showGizmos = true;
public fluidInfluenceClass fluidDetails;

private GameObject fluidConnectorObject;
private FluidSimConnector fluidConnectorScript;

private int fluidActorId;
		
private bool tempDynamicCollision;
private bool tempAddColor;
private bool tempAddVelocity;

private bool hasBeenDisabled = false;

public bool collisionFoldout = true;
public bool colorFoldout = true;
public bool velocityFoldout = true;

//=============

void OnDrawGizmos()
{
	if(showGizmos)
	{
		if(fluidDetails.staticCollision || fluidDetails.dynamicCollision)
		{
	    	Gizmos.color = Color.red;
	    	if(fluidDetails.multiplySizeByScale)
	    	{
	    		Gizmos.DrawWireSphere(transform.position, fluidDetails.collisionSize * (transform.localScale.magnitude * 0.577f));
	    	}
	    	else
	    	{
	    		Gizmos.DrawWireSphere(transform.position, fluidDetails.collisionSize);
	    	}
	    }
	    
	    if(fluidDetails.addColor)
	    {
	    	Gizmos.color = Color.yellow;
	    	if(fluidDetails.multiplySizeByScale)
	    	{
	    		Gizmos.DrawWireSphere(transform.position, fluidDetails.colorSize * (transform.localScale.magnitude * 0.577f));
			}
			else
			{
				Gizmos.DrawWireSphere(transform.position, fluidDetails.colorSize);
			}
		}
		
		if(fluidDetails.addVelocity)
		{
			Gizmos.color = Color.blue;
			if(fluidDetails.multiplySizeByScale)
			{
	    		Gizmos.DrawWireSphere(transform.position, fluidDetails.velocitySize * (transform.localScale.magnitude * 0.577f));
			}
			else
			{
				Gizmos.DrawWireSphere(transform.position, fluidDetails.velocitySize);
			}
		}
	}
}

//=============

void Start()
{
	fluidDetails.myTransform = transform;
	fluidDetails.lastFramePosition = transform.position;

	if(fluidConnectorScript == null)
		{
		if(GameObject.Find("dynamiclyCreatedFluidSimConnector"))
		{
			fluidConnectorObject = GameObject.Find("dynamiclyCreatedFluidSimConnector");

            fluidConnectorScript = fluidConnectorObject.GetComponent<FluidSimConnector>();
		}
		else
		{
			fluidConnectorObject = new GameObject();
			
			fluidConnectorObject.name = "dynamiclyCreatedFluidSimConnector";

            fluidConnectorScript = fluidConnectorObject.AddComponent<FluidSimConnector>();
		}
	}
	
	if(fluidConnectorObject == null)
	{
		Debug.LogError("FluidSimInfluenceActor failed to find or create a FluidConnector object.  Make sure the FluidConnector script exists and can by found by the FluidSimInfluenceActor.");
	}
	
	if(fluidConnectorScript == null)
	{
		Debug.LogError("FluidSimInfluenceActor failed to find or create a FluidConnector script.  Make sure the FluidConnector script exists and can by found by the FluidSimInfluenceActor.");
	}
	else
	{
		fluidActorId = fluidConnectorScript.AddInfluenceActor(fluidDetails);
	}
}

//==============

void ChangeActorPriority(int tempInt)
{
	tempInt = Mathf.Clamp(tempInt, 1, 5);
	
	fluidDetails.actorPriority = tempInt;
	
	fluidConnectorScript.SortActorArray();
}

//==============

void DestroyFluidInfluenceActor()
{
	fluidConnectorScript.RemoveInfluenceActor(fluidActorId);
	
	Destroy(gameObject, 0.1f);
}

//==============

void OnEnable()
{
	if(hasBeenDisabled)
	{
		fluidDetails.dynamicCollision = tempDynamicCollision;
		fluidDetails.addColor = tempAddColor;
		fluidDetails.addVelocity = tempAddVelocity;
	}
}

//==============

void OnDisable()
{
	tempDynamicCollision = fluidDetails.dynamicCollision;
	tempAddColor = fluidDetails.addColor;
	tempAddVelocity = fluidDetails.addVelocity;
	
	fluidDetails.dynamicCollision = false;
	fluidDetails.addColor = false;
	fluidDetails.addVelocity = false;
	
	hasBeenDisabled = true;
}
}