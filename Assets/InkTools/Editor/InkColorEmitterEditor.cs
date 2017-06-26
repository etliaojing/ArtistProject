

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InkColorEmitter))]
[CanEditMultipleObjects]
class InkColorEmitterEditor : Editor
{

    [MenuItem("GameObject/Create Other/Inkling/Ink Color Emitter")]
    private static void Inkling_ColorEmitter()
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

        if (Resources.Load("InkColorEmitterObject") != null)
        {
            Instantiate( Resources.Load("InkColorEmitterObject")
                       , menuSpawnPosition
                       , Quaternion.identity
                       );
        }
        else
        {
            Debug.LogError("InkColorEmitterObject was not found in the Resources folder and could"
                          + " not be created in the Scene.");
        }
    }

    private string _showGizmosHelp = "";
    private string _actorPriorityHelp = "";

    private string _useColorMaskHelp = "";

    private string _colorFalloffHelp = "";

    private string _scaleBySizeHelp = "";

    private SerializedProperty _showGizmosTemp;

    private SerializedProperty _velocityFoldoutTemp;

    private SerializedProperty _addColorTemp;
    private SerializedProperty _useColorMaskTextureTemp;
    private SerializedProperty _colorMaskTextureTemp;
    private SerializedProperty _colorValueTemp;
    private SerializedProperty _colorSizeTemp;
    private SerializedProperty _colorFalloffTemp;

    private SerializedProperty _myTransformTemp;
    private SerializedProperty _lastFramePositionTemp;
    private SerializedProperty _actorPriorityTemp;
    private SerializedProperty _multiplySizeByScaleTemp;

    void OnEnable()
    {
        _showGizmosTemp              = serializedObject.FindProperty("showGizmos");

        _useColorMaskTextureTemp     = serializedObject.FindProperty("useColorMaskTexture");
        _colorMaskTextureTemp        = serializedObject.FindProperty("colorMaskTexture");
        _colorValueTemp              = serializedObject.FindProperty("colorValue");
        _colorSizeTemp               = serializedObject.FindProperty("colorSize");
        _colorFalloffTemp            = serializedObject.FindProperty("colorFalloff");

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

        EditorGUILayout.PropertyField( _showGizmosTemp
                                     , new GUIContent("Show Size Gizmos", _showGizmosHelp));

        EditorGUILayout.PropertyField( _multiplySizeByScaleTemp
                                     , new GUIContent("Scale Size by localScale", _scaleBySizeHelp));

        EditorGUILayout.IntSlider( _actorPriorityTemp
                                 , 1
                                 , 5
                                 , new GUIContent("Influence Priority", _actorPriorityHelp)
                                 );

        EditorGUILayout.LabelField("_______________________");
        EditorGUILayout.Space();


        EditorGUILayout.PropertyField( _useColorMaskTextureTemp
                                     , new GUIContent("Use Color Mask", _useColorMaskHelp));

        if (_useColorMaskTextureTemp.boolValue)
        {
            if (!_colorMaskTextureTemp.hasMultipleDifferentValues)
            {
                _colorMaskTextureTemp.objectReferenceValue =
                    EditorGUILayout.ObjectField( "Color Mask Texture"
                                               , _colorMaskTextureTemp.objectReferenceValue
                                               , typeof(Texture2D)
                                               , false
                                               );
            }
            else
            {
                EditorGUILayout.LabelField("Color Mask Texture");
                EditorGUILayout.LabelField("                                Multiple");
                EditorGUILayout.LabelField("                                Texture");
                EditorGUILayout.LabelField("                                Types");
            }
        }


        if (!_useColorMaskTextureTemp.boolValue)
        {
            EditorGUILayout.PropertyField(_colorValueTemp, new GUIContent("Color (Alpha = Strength)"));
        }

        EditorGUILayout.Slider(_colorSizeTemp, 0, 500, "Color Size");

        if (!_useColorMaskTextureTemp.boolValue)
        {
            EditorGUILayout.Slider( _colorFalloffTemp
                                  , 0.01f
                                  , 1.0f
                                  , new GUIContent("Color Falloff", _colorFalloffHelp));
        }

        EditorGUILayout.Space();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
