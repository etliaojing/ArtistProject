//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using UnityEditor;

[AddComponentMenu("FluidSim/(Auto Spawned) FluidSim Connector")]
class FluidSimConnector : MonoBehaviour
{
private int fluidActorArraySize = 128;
private int fluidSimArraySize = 64;

private fluidInfluenceClass[] fluidActorHistoryArray;
public fluidInfluenceClass[] fluidActorStaticArray;
public fluidInfluenceClass[] fluidActorDynamicArray;
private fluidInfluenceClass[] fluidActorDynamicTempArray;
private fluidInfluenceClass[] fluidActorEmptyArray;
		
private FluidSimScript[] fluidSimScripts;
private int fluidSimCount = 0;

public int staticArrayCount = 0;
public int dynamicArrayCount = 0;

private int actorIdSlot = 0;
private bool actorAssigned;

//private int totalFluidActorId;

private int i = 0;

private float lastSortTime = -1.0f;

//==========

void Awake()
{
    fluidActorHistoryArray = new fluidInfluenceClass[fluidActorArraySize];
    fluidActorStaticArray = new fluidInfluenceClass[fluidActorArraySize];
    fluidActorDynamicArray = new fluidInfluenceClass[fluidActorArraySize];
    fluidActorDynamicTempArray = new fluidInfluenceClass[fluidActorArraySize];
    fluidActorEmptyArray = new fluidInfluenceClass[fluidActorArraySize];

    fluidSimScripts = new FluidSimScript[fluidSimArraySize];

	//totalFluidActorId = 0;
	
//creates a cleared array and clears all arrays to make sure there is zero junk data floating around.
	for(i = 0; i < fluidSimArraySize; i++)
	{
		fluidSimScripts[i] = null;
	}

	for(i = 0; i < fluidActorArraySize; i++)
	{
		fluidActorEmptyArray[i] = null;
	}
	
	System.Array.Copy(fluidActorEmptyArray, fluidActorHistoryArray, fluidActorArraySize);
	System.Array.Copy(fluidActorEmptyArray, fluidActorStaticArray, fluidActorArraySize);
	System.Array.Copy(fluidActorEmptyArray, fluidActorDynamicArray, fluidActorArraySize);
	System.Array.Copy(fluidActorEmptyArray, fluidActorDynamicTempArray, fluidActorArraySize);
}

//==========

public int AddInfluenceActor(fluidInfluenceClass fluidDetails)
{
    actorAssigned = false;

    for (i = 0; i < fluidActorArraySize; i++)
    {
        if (fluidActorHistoryArray[i] == null)
        {
            fluidActorHistoryArray[i] = fluidDetails;
            actorAssigned = true;
            actorIdSlot = i;
            i = 128;
        }
    }
	
	if(actorAssigned == false)
	{
		Debug.LogError("FluidSimConnector tried to assign an Influence Actor but the array is full.  Fluid Sim defaults to a maximum of 128 actors cached in the array.  If you need to use more than 128 actors, change the FluidSimConnector script variable fluidActorArraySize to a larger value.");
        actorIdSlot = 127;
    }

    SortActorArray();

	return Mathf.Clamp(actorIdSlot, 0, fluidActorArraySize);
}

//==========

public void RemoveInfluenceActor(int fluidActorId)
{
	fluidActorHistoryArray[fluidActorId] = null;
	
	SortActorArray();
}

//==========

public void SortActorArray()
{
	if((Time.time - lastSortTime) > 0.245f)
	{
		Invoke("SortActorArrayHidden", 0.25f);
		
		lastSortTime = Time.time;
	}
}

//==========

void SortActorArrayHidden()
{
	System.Array.Copy(fluidActorEmptyArray, fluidActorStaticArray, fluidActorArraySize);
	System.Array.Copy(fluidActorEmptyArray, fluidActorDynamicArray, fluidActorArraySize);
	System.Array.Copy(fluidActorEmptyArray, fluidActorDynamicTempArray, fluidActorArraySize);
	
	staticArrayCount = 0;
	dynamicArrayCount = 0;
	
//Sort actors to arrays
	for(i = 0; i <  fluidActorHistoryArray.Length; i++)
	{
		if(fluidActorHistoryArray[i] != null)
		{
			if(fluidActorHistoryArray[i].staticCollision == true)
			{
				fluidActorStaticArray[staticArrayCount] = fluidActorHistoryArray[i];
				staticArrayCount++;

				fluidActorDynamicTempArray[dynamicArrayCount] = fluidActorHistoryArray[i];
				dynamicArrayCount++;
			}
			else
			{
				fluidActorDynamicTempArray[dynamicArrayCount] = fluidActorHistoryArray[i];
				dynamicArrayCount++;
			}
		}
	}
	
//This cascade of for statements seems redundant, but it sorts the actors correctly in the array for drawing later.
	if(dynamicArrayCount > 0)
	{
		dynamicArrayCount = 0;
		
		for(i = 0; i < fluidActorDynamicTempArray.Length; i++)
		{
			if(fluidActorDynamicTempArray[i] != null && fluidActorDynamicTempArray[i].actorPriority == 5)
			{
				fluidActorDynamicArray[dynamicArrayCount] = fluidActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				fluidActorDynamicTempArray[i] = null;
			}
			
		}
		for(i = 0; i < fluidActorDynamicTempArray.Length; i++)
		{
            if (fluidActorDynamicTempArray[i] != null && fluidActorDynamicTempArray[i].actorPriority == 4)
			{
				fluidActorDynamicArray[dynamicArrayCount] = fluidActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				fluidActorDynamicTempArray[i] = null;
			}
		}
		for(i = 0; i < fluidActorDynamicTempArray.Length; i++)
		{
            if (fluidActorDynamicTempArray[i] != null && fluidActorDynamicTempArray[i].actorPriority == 3)
			{
				fluidActorDynamicArray[dynamicArrayCount] = fluidActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				fluidActorDynamicTempArray[i] = null;
			}
		}
		for(i = 0; i < fluidActorDynamicTempArray.Length; i++)
		{
            if (fluidActorDynamicTempArray[i] != null && fluidActorDynamicTempArray[i].actorPriority == 2)
			{
				fluidActorDynamicArray[dynamicArrayCount] = fluidActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				fluidActorDynamicTempArray[i] = null;
			}
		}
		for(i = 0; i < fluidActorDynamicTempArray.Length; i++)
		{
            if (fluidActorDynamicTempArray[i] != null && fluidActorDynamicTempArray[i].actorPriority == 1)
			{
				fluidActorDynamicArray[dynamicArrayCount] = fluidActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				fluidActorDynamicTempArray[i] = null;
			}
		}
	}

//Tell fluid actors about actor updates
	for(i = 0; i < fluidSimScripts.Length; i++)
	{
		if(fluidSimScripts[i])
		{
			GetActorArrayUpdate(fluidSimScripts[i]);
		}
	}
}

//==========

public void GetActorArrayUpdate(FluidSimScript fluidScriptRef)
{
	fluidScriptRef.fluidActorStaticArray = fluidActorStaticArray;
	
	fluidScriptRef.fluidActorDynamicArray = fluidActorDynamicArray;
	fluidScriptRef.dynamicInputArrayLength = dynamicArrayCount;
}

//==========

public void RegisterFluidActor(FluidSimScript passedFluidScript)
{
	fluidSimScripts[fluidSimCount] = passedFluidScript;
		
	fluidSimCount++;
}
}