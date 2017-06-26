

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InkVelocityEmitter))]
[CanEditMultipleObjects]
class InkVelocityEmitterEditor : Editor
{

    [MenuItem("GameObject/Create Other/Inkling/Ink Velocity Emitter")]
    private static void Inkling_VelocityEmitter()
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

        if (Resources.Load("InkVelocityEmitterObject") != null)
        {
            Instantiate( Resources.Load("InkVelocityEmitterObject")
                       , menuSpawnPosition
                       , Quaternion.identity
                       );
        }
        else
        {
            Debug.LogError( "InkVelocityEmitterObject was not found in the Resources folder and"
                          + " could not be created in the Scene.");
        }
    }

    private string _showGizmosHelp = "";
    private string _actorPriorityHelp = "";

    private string _useVelocityMaskHelp = "";

    private string _velocityFalloffHelp = "";

    private string _scaleBySizeHelp = "";


    private SerializedProperty _showGizmosTemp;
    private SerializedProperty _addVelocityTemp;
    private SerializedProperty _useVelocityMaskTextureTemp;
    private SerializedProperty _velocityMaskTextureTemp;
    private SerializedProperty _velocityStrengthTemp;
    private SerializedProperty _velocitySizeTemp;
    private SerializedProperty _velocityFalloffTemp;
    private SerializedProperty _myTransformTemp;
    private SerializedProperty _lastFramePositionTemp;
    private SerializedProperty _actorPriorityTemp;
    private SerializedProperty _multiplySizeByScaleTemp;

    void OnEnable()
    {
        _showGizmosTemp              = serializedObject.FindProperty("showGizmos");
        _useVelocityMaskTextureTemp  = serializedObject.FindProperty("useVelocityMaskTexture");
        _velocityMaskTextureTemp     = serializedObject.FindProperty("velocityMaskTexture");
        _velocityStrengthTemp        = serializedObject.FindProperty("velocityStrength");
        _velocitySizeTemp            = serializedObject.FindProperty("velocitySize");
        _velocityFalloffTemp         = serializedObject.FindProperty("velocityFalloff");
        _actorPriorityTemp           = serializedObject.FindProperty("actorPriority");
        _multiplySizeByScaleTemp     = serializedObject.FindProperty("multiplySizeByScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Works with the if() at the bottom to tell unity this object has been changed and it should update the sphere gizmo rendering.
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

        EditorGUILayout.PropertyField( _useVelocityMaskTextureTemp
                                        , new GUIContent("Use Velocity Mask", _useVelocityMaskHelp));

        if (_useVelocityMaskTextureTemp.boolValue)
        {
            if (!_velocityMaskTextureTemp.hasMultipleDifferentValues)
            {
                _velocityMaskTextureTemp.objectReferenceValue =
                    EditorGUILayout.ObjectField( "Velocity Mask Texture"
                                               , _velocityMaskTextureTemp.objectReferenceValue
                                               , typeof(Texture2D)
                                               , false
                                               );
            }
            else
            {
                EditorGUILayout.LabelField("Velocity Mask Texture");
                EditorGUILayout.LabelField("                                Multiple");
                EditorGUILayout.LabelField("                                Texture");
                EditorGUILayout.LabelField("                                Types");
            }
        }

        EditorGUILayout.Slider(_velocityStrengthTemp, 0, 5, "Velocity Strength");
        EditorGUILayout.Slider(_velocitySizeTemp, 0, 500, "Velocity Size");

        if (!_useVelocityMaskTextureTemp.boolValue)
        {
            EditorGUILayout.Slider( _velocityFalloffTemp
                                  , 0.01f
                                  , 1
                                  , new GUIContent("Velocity Falloff", _velocityFalloffHelp)
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
