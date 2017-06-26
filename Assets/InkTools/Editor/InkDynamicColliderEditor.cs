

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InkDynamicCollider))]
[CanEditMultipleObjects]
class InkDynamicColliderEditor : Editor
{

    [MenuItem("GameObject/Create Other/Inkling/Ink Dynamic Collider")]
    private static void Inkling_DynamicCollider()
    {
        //Uses the current viewport camera for spawn location, or the world zero if the current
        //  camera isnt valid.
        Vector3 menuSpawnPosition = Vector3.zero;
        Camera menuSpawnCamera;

        if (Selection.activeGameObject != null)
        {
            menuSpawnPosition = Selection.activeGameObject.transform.position;
        }
        else
        {
            menuSpawnCamera = Camera.current;

            if (menuSpawnCamera != null)
            {
                menuSpawnPosition =
                    menuSpawnCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10.0f));
            }
            else
            {
                menuSpawnPosition = Vector3.zero;
            }
        }

        if (Resources.Load("InkDynamicColliderObject") != null)
        {
            Instantiate( Resources.Load("InkDynamicColliderObject")
                       , menuSpawnPosition
                       , Quaternion.identity
                       );
        }
        else
        {
            Debug.LogError( "InkDynamicColliderObject was not found in the Resources folder and"
                          + " could not be created in the Scene.");
        }
    }

    private string _showGizmosHelp = "";
    private string _actorPriorityHelp = "";

    private string _useCollisionMaskHelp = "";

    private string _collisionStrengthHelp = "";
    private string _moveVelocityMultiplierHelp = "";

    private string _collisionFalloffHelp = "";

    private string _scaleBySizeHelp = "";


    private SerializedProperty _showGizmosTemp;
    private SerializedProperty _useCollisionMaskTextureTemp;
    private SerializedProperty _collisionMaskTextureTemp;
    private SerializedProperty _collisionStrengthTemp;
    private SerializedProperty _collisionSizeTemp;
    private SerializedProperty _collisionFalloffTemp;
    private SerializedProperty _moveVelocityMultiplierTemp;
    private SerializedProperty _myTransformTemp;
    private SerializedProperty _lastFramePositionTemp;
    private SerializedProperty _actorPriorityTemp;
    private SerializedProperty _multiplySizeByScaleTemp;

    void OnEnable()
    {
        _showGizmosTemp              = serializedObject.FindProperty("showGizmos");
        _useCollisionMaskTextureTemp = serializedObject.FindProperty("useCollisionMaskTexture");
        _collisionMaskTextureTemp    = serializedObject.FindProperty("collisionMaskTexture");
        _collisionStrengthTemp       = serializedObject.FindProperty("collisionStrength");
        _collisionSizeTemp           = serializedObject.FindProperty("collisionSize");
        _collisionFalloffTemp        = serializedObject.FindProperty("collisionFalloff");
        _moveVelocityMultiplierTemp  = serializedObject.FindProperty("moveVelocityMultiplier");
        _actorPriorityTemp           = serializedObject.FindProperty("actorPriority");
        _multiplySizeByScaleTemp     = serializedObject.FindProperty("multiplySizeByScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Works with the if() at the bottom to tell unity this object has been changed and it
        //  should update the sphere gizmo rendering.
        GUI.changed = false;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_showGizmosTemp
                                     , new GUIContent("Show Size Gizmos", _showGizmosHelp));

        EditorGUILayout.PropertyField(_multiplySizeByScaleTemp
                                     , new GUIContent("Scale Size by localScale", _scaleBySizeHelp));

        EditorGUILayout.IntSlider(_actorPriorityTemp
                                 , 1
                                 , 5
                                 , new GUIContent("Influence Priority", _actorPriorityHelp)
                                 );

        EditorGUILayout.LabelField("_______________________");
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField( _useCollisionMaskTextureTemp
                                     , new GUIContent("Use Collision Mask", _useCollisionMaskHelp));

        if (_useCollisionMaskTextureTemp.boolValue)
        {
            if (!_collisionMaskTextureTemp.hasMultipleDifferentValues)
            {
                _collisionMaskTextureTemp.objectReferenceValue =
                    EditorGUILayout.ObjectField( "Collision Mask Texture"
                                               , _collisionMaskTextureTemp.objectReferenceValue
                                               , typeof(Texture2D)
                                               , false
                                               );
            }
            else
            {
                EditorGUILayout.LabelField("Collision Mask Texture");
                EditorGUILayout.LabelField("                                Multiple");
                EditorGUILayout.LabelField("                                Texture");
                EditorGUILayout.LabelField("                                Types");
            }
        }

        EditorGUILayout.Slider( _collisionStrengthTemp
                              , 0
                              , 1
                              , new GUIContent("Collision Strength", _collisionStrengthHelp)
                              );

        EditorGUILayout.Slider(_collisionSizeTemp, 0, 500, "Collision Size");


        EditorGUILayout.Slider( _moveVelocityMultiplierTemp
                                , 0
                                , 2
                                , new GUIContent( "Move Velocity Multiplier"
                                                , _moveVelocityMultiplierHelp)
                                );

        if (!_useCollisionMaskTextureTemp.boolValue)
        {
            EditorGUILayout.Slider ( _collisionFalloffTemp
                                   , 0.01f
                                   , 1.0f
                                   , new GUIContent("Collision Falloff", _collisionFalloffHelp)
                                   );
        }

        EditorGUILayout.Space();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        serializedObject.ApplyModifiedProperties();
    }
}