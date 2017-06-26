using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[AddComponentMenu("Inkling/(Auto Spawned) InkFluid Connector")]
public class InkConnector : MonoBehaviour
{
    private int _inkActorArraySize = 128;
    private int _inkSimArraySize = 64;

    private InkActor[] _inkActorHistoryArray;
    public InkActor[] inkActorStaticArray;
    public InkActor[] inkActorDynamicArray;
    private InkActor[] _inkActorDynamicTempArray;
    private InkActor[] _inkActorEmptyArray;

    private InkSimulation[] _inkSimulations;
    private int _inkSimCount = 0;

    public int staticArrayCount = 0;
    public int dynamicArrayCount = 0;

    private int _actorIdSlot = 0;
    private bool _actorAssigned;

    private float _lastSortTime = -1.0f;

    //=============================================================================================

    public static GameObject GetObjInstance()
    {
        GameObject inkConnectorObject = GameObject.Find("InkFluidConnector");

        if (!inkConnectorObject)
        {
            inkConnectorObject = new GameObject();

            inkConnectorObject.name = "InkFluidConnector";

            inkConnectorObject.AddComponent<InkConnector>();
        }

        return inkConnectorObject;
    }

    public static InkConnector GetScriptInstance()
    {
        GameObject inkConnectorObject = GameObject.Find("InkFluidConnector");
        InkConnector inkConnector = null;

        if (inkConnectorObject)
        {

            inkConnector = inkConnectorObject.GetComponent<InkConnector>();
        }
        else
        {
            inkConnectorObject = new GameObject();

            inkConnectorObject.name = "InkFluidConnector";

            inkConnector = inkConnectorObject.AddComponent<InkConnector>();
        }

        return inkConnector;
    }

    //=============================================================================================

    private void Awake()
    {
        _inkActorHistoryArray = new InkActor[_inkActorArraySize];
        inkActorStaticArray = new InkActor[_inkActorArraySize];
        inkActorDynamicArray = new InkActor[_inkActorArraySize];
        _inkActorDynamicTempArray = new InkActor[_inkActorArraySize];
        _inkActorEmptyArray = new InkActor[_inkActorArraySize];

        _inkSimulations = new InkSimulation[_inkSimArraySize];

        for(int i = 0; i < _inkSimArraySize; ++i)
        {
            _inkSimulations[i] = null;
        }

        for(int i = 0; i < _inkActorArraySize; ++i)
        {
            _inkActorEmptyArray[i] = null;
        }

        System.Array.Copy(_inkActorEmptyArray, _inkActorHistoryArray, _inkActorArraySize);
        System.Array.Copy(_inkActorEmptyArray, inkActorStaticArray, _inkActorArraySize);
        System.Array.Copy(_inkActorEmptyArray, inkActorDynamicArray, _inkActorArraySize);
        System.Array.Copy(_inkActorEmptyArray, _inkActorDynamicTempArray, _inkActorArraySize);
    }

    //=============================================================================================

    public int AddActor(InkActor inkActor)
    {
        _actorAssigned = false;

        for(int i = 0; i < _inkActorArraySize; ++i)
        {
            if(_inkActorHistoryArray[i] == null)
            {
                _inkActorHistoryArray[i] = inkActor;
                _actorAssigned = true;
                _actorIdSlot = i;
                i = 128; //@TODO: Break?
            }
        }

        if(_actorAssigned == false)
        {
            Debug.LogError( "InkFluidConnector tried to assign an Actor but the array is full."
                          + "  Inkling defaults to a maximum of 128 actors cached in the array."
                          + "  If you need to use more than 128 actors, change the"
                          + " InkFluidConnector script variable inkActorArraySize to a larger value."
                          );
            _actorIdSlot = 127; //@TODO: Set to max -1?
        }

        SortActorArray();

        return Mathf.Clamp(_actorIdSlot, 0, _inkActorArraySize);
    }

    //=============================================================================================

    public void RemoveActor(int actorId)
    {
        _inkActorHistoryArray[actorId] = null;

        SortActorArray();
    }

    //=============================================================================================

    public void SortActorArray()
    {
        if((Time.time - _lastSortTime) > 0.245f)
        {
            Invoke("SortActorArrayHidden", 0.25f);

            _lastSortTime = Time.time;
        }
    }

    //=============================================================================================

    private void SortActorArrayHidden()
    {
        System.Array.Copy(_inkActorEmptyArray, inkActorStaticArray, _inkActorArraySize);
        System.Array.Copy(_inkActorEmptyArray, inkActorDynamicArray, _inkActorArraySize);
        System.Array.Copy(_inkActorEmptyArray, _inkActorDynamicTempArray, _inkActorArraySize);

        staticArrayCount = 0;
        dynamicArrayCount = 0;

        for(int i = 0; i < _inkActorHistoryArray.Length; ++i)
        {
            if(_inkActorHistoryArray[i] != null)
            {
                if(_inkActorHistoryArray[i] is InkStaticCollider)
                {
                    inkActorStaticArray[staticArrayCount] = _inkActorHistoryArray[i];
                    staticArrayCount++;

                    _inkActorDynamicTempArray[dynamicArrayCount] = _inkActorHistoryArray[i];
                    dynamicArrayCount++;
                }
                else
                {
                    _inkActorDynamicTempArray[dynamicArrayCount] = _inkActorHistoryArray[i];
                    dynamicArrayCount++;
                }
            }
        }

        if(dynamicArrayCount > 0)
        {
            dynamicArrayCount = 0;

            for(int i = 0; i < _inkActorDynamicTempArray.Length; ++i)
            {
                if(  _inkActorDynamicTempArray[i] != null
                  && _inkActorDynamicTempArray[i].actorPriority == 5)
                {
                    inkActorDynamicArray[dynamicArrayCount] = _inkActorDynamicTempArray[i];

                    dynamicArrayCount++;

                    _inkActorDynamicTempArray[i] = null;
                }
            }

            for (int i = 0; i < _inkActorDynamicTempArray.Length; ++i)
            {
                if (_inkActorDynamicTempArray[i] != null
                  && _inkActorDynamicTempArray[i].actorPriority == 4)
                {
                    inkActorDynamicArray[dynamicArrayCount] = _inkActorDynamicTempArray[i];

                    dynamicArrayCount++;

                    _inkActorDynamicTempArray[i] = null;
                }
            }

            for (int i = 0; i < _inkActorDynamicTempArray.Length; ++i)
            {
                if (_inkActorDynamicTempArray[i] != null
                  && _inkActorDynamicTempArray[i].actorPriority == 3)
                {
                    inkActorDynamicArray[dynamicArrayCount] = _inkActorDynamicTempArray[i];

                    dynamicArrayCount++;

                    _inkActorDynamicTempArray[i] = null;
                }
            }

            for (int i = 0; i < _inkActorDynamicTempArray.Length; ++i)
            {
                if (_inkActorDynamicTempArray[i] != null
                  && _inkActorDynamicTempArray[i].actorPriority == 2)
                {
                    inkActorDynamicArray[dynamicArrayCount] = _inkActorDynamicTempArray[i];

                    dynamicArrayCount++;

                    _inkActorDynamicTempArray[i] = null;
                }
            }

            for (int i = 0; i < _inkActorDynamicTempArray.Length; ++i)
            {
                if (_inkActorDynamicTempArray[i] != null
                  && _inkActorDynamicTempArray[i].actorPriority == 1)
                {
                    inkActorDynamicArray[dynamicArrayCount] = _inkActorDynamicTempArray[i];

                    dynamicArrayCount++;

                    _inkActorDynamicTempArray[i] = null;
                }
            }
        }

        for(int i = 0; i < _inkSimulations.Length; ++i)
        {
            if(_inkSimulations[i])
            {
                GetActorArrayUpdate(_inkSimulations[i]);
            }
        }
    }

    //=============================================================================================

    public void GetActorArrayUpdate(InkSimulation inkSim)
    {
        inkSim.actorStaticArray = inkActorStaticArray;

        inkSim.actorDynamicArray = inkActorDynamicArray;

        inkSim.dynamicInputArrayLength = dynamicArrayCount;
    }

    //=============================================================================================

    public void RegisterInkActor(InkSimulation inkSim)
    {
        _inkSimulations[_inkSimCount] = inkSim;

        _inkSimCount++;
    }

	//=============================================================================================

	private void Start ()
    {

	}

	//=============================================================================================

	private void Update ()
    {

	}

    //=============================================================================================
}
