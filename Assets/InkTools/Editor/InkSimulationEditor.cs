//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other ,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InkSimulation))]
[CanEditMultipleObjects]
class InkSimulationEditor : Editor
{
    bool fpsFoldout = false;
    bool collisionFoldout = true;
    bool colorFoldout = true;
    bool velocityFoldout = true;

    private string _fpsFluidReport = "";
    //private string _fpsColorReport = "";
    //private string _fpsImpulseReport = "";
    //private string _fpsCollisionReport = "";

    private string _colorUpdateText = "";
    private string _velocityUpdateText = "";
    private string _collisionUpdateText = "";

    private bool _showFpsZeroWarning = false;

    private bool _cautionDisplayed = false;

    private string _fluidUpdateFPSHelp = "";
    private string _impulseUpdateFPSHelp = "";
    private string _colorUpdateFPSHelp = "";
    private string _collisionUpdateFPSHelp = "";
    private string _simStrengthHelp = "";
    private string _pressureIterationHelp = "";
    //private string _startingVelocityTextureHelp = "";
    private string _startingCollisionTextureHelp = "";
    private string _useColorDissipationHelp = "";
    private string _colorDissipationHelp = "";
    private string _colorDissipationToHelp = "";
    private string _useVelocityDissipationHelp = "";
    private string _velocityDissipationHelp = "";
    private string _useBoundaryCollisionHelp = "";
    private string _syncColorHelp = "";
    private string _syncImpulseHelp = "";
    private string _syncCollisionHelp = "";

    [MenuItem("GameObject/Create Other/Inkling/Ink Simulation")]
    private static void Inkling_Simulation()
    {
        Vector3 menuSpawnPosition = Vector3.zero;
        Camera menuSpawnCamera;

        if (Selection.activeGameObject != null)
        {
            menuSpawnPosition = Selection.activeGameObject.transform.position;
        }
        else
        {
            menuSpawnCamera = Camera.current;

            if (menuSpawnCamera)
            {
                menuSpawnPosition =
                        menuSpawnCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10.0f));
            }
            else
            {
                menuSpawnPosition = Vector3.zero;
            }
        }

        if (Resources.Load("InkSimulationObject"))
        {
            Instantiate(Resources.Load("InkSimulationObject"), menuSpawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("InkSimulationObject was not found in the Res folder and could not"
                          + " be created in the Scene."
                          );
        }
    }

    private SerializedProperty _fluidUpdateFPSTemp;
    private SerializedProperty _colorUpdateFPSTemp;
    private SerializedProperty _impulseUpdateFPSTemp;
    private SerializedProperty _collisionUpdateFPSTemp;
    private SerializedProperty _simStrengthTemp;
    private SerializedProperty _pressureIterationTemp;
    //private SerializedProperty _resolutionIndexTemp;
    private SerializedProperty _resolutionTemp;
    private SerializedProperty _collisionResolutionTemp;
    private SerializedProperty _velocityResolutionTemp;
    //private SerializedProperty _colorResolutionIndexTemp;
    //private SerializedProperty _collisionResolutionIndexTemp;
    //private SerializedProperty _velocityResolutionIndexTemp;
    private SerializedProperty _outputTextureNumTemp;
    private SerializedProperty _materialTextureSlotTemp;
    private SerializedProperty _useBoundaryCollisionTemp;
    private SerializedProperty _useColorDissipationTemp;
    private SerializedProperty _useColorDissipationTextureTemp;
    private SerializedProperty _colorDissipationTextureTemp;
    private SerializedProperty _colorDissipationTemp;
    private SerializedProperty _oldColorDissipationTemp;
    private SerializedProperty _colorDissipateToTemp;
    private SerializedProperty _useStartColorTextureTemp;
    private SerializedProperty _startingColorTextureTemp;
    private SerializedProperty _startingColorTemp;
    private SerializedProperty _useVelocityDissipationTemp;
    private SerializedProperty _useVelocityDissipationTextureTemp;
    private SerializedProperty _velocityDissipationTextureTemp;
    private SerializedProperty _velocityDissipationTemp;
    private SerializedProperty _oldVelocityDissipationTemp;
    private SerializedProperty _useStartVelocityTextureTemp;
    private SerializedProperty _startingVelocityTextureTemp;
    private SerializedProperty _startingVelocityTemp;
    private SerializedProperty _useStartCollisionTextureTemp;
    private SerializedProperty _startingCollisionTextureTemp;
    private SerializedProperty _resOptionsTemp;
    private SerializedProperty _uniqueCollisionResOptionsTemp;
    private SerializedProperty _uniqueColorResOptionsTemp;
    private SerializedProperty _uniqueVelocityResOptionsTemp;
    private SerializedProperty _outputTextureTemp;
    private SerializedProperty _updateColorWithFluidTemp;
    private SerializedProperty _updateImpulseWithFluidTemp;
    private SerializedProperty _updateCollisionWithFluidTemp;
    private SerializedProperty _materialTargetTemp;
    private SerializedProperty _useMyMaterialTemp;

    void OnEnable()
    {
        _fluidUpdateFPSTemp                   = serializedObject.FindProperty("fluidUpdateFPS");
        _colorUpdateFPSTemp                   = serializedObject.FindProperty("colorUpdateFPS");
        _impulseUpdateFPSTemp                 = serializedObject.FindProperty("impulseUpdateFPS");
        _collisionUpdateFPSTemp               = serializedObject.FindProperty("collisionUpdateFPS");
        _simStrengthTemp                      = serializedObject.FindProperty("simStrength");
        _pressureIterationTemp                = serializedObject.FindProperty("pressureIteration");
        //resolutionIndexTemp                 = serializedObject.FindProperty("resolutionIndex");
        _resolutionTemp                       = serializedObject.FindProperty("resolution");
        _collisionResolutionTemp              = serializedObject.FindProperty("collisionResolution");
        _velocityResolutionTemp               = serializedObject.FindProperty("velocityResolution");
        //colorResolutionIndexTemp            = serializedObject.FindProperty("colorResolutionIndex");
        //collisionResolutionIndexTemp        = serializedObject.FindProperty("collisionResolutionIndex");
        //velocityResolutionIndexTemp         = serializedObject.FindProperty("velocityResolutionIndex");
        _outputTextureNumTemp                 = serializedObject.FindProperty("outputTextureNum");
        _materialTextureSlotTemp              = serializedObject.FindProperty("materialTextureSlot");
        _useBoundaryCollisionTemp             = serializedObject.FindProperty("useBoundaryCollision");
        _useColorDissipationTemp              = serializedObject.FindProperty("useColorDissipation");
        _useColorDissipationTextureTemp       = serializedObject.FindProperty("useColorDissipationTexture");
        _colorDissipationTextureTemp          = serializedObject.FindProperty("colorDissipationTexture");
        _colorDissipationTemp                 = serializedObject.FindProperty("colorDissipation");
        _oldColorDissipationTemp              = serializedObject.FindProperty("oldColorDissipation");
        _colorDissipateToTemp                 = serializedObject.FindProperty("colorDissipateTo");
        _useStartColorTextureTemp             = serializedObject.FindProperty("useStartColorTexture");
        _startingColorTextureTemp             = serializedObject.FindProperty("startingColorTexture");
        _startingColorTemp                    = serializedObject.FindProperty("startingColor");
        _useVelocityDissipationTemp           = serializedObject.FindProperty("useVelocityDissipation");
        _useVelocityDissipationTextureTemp    = serializedObject.FindProperty("useVelocityDissipationTexture");
        _velocityDissipationTextureTemp       = serializedObject.FindProperty("velocityDissipationTexture");
        _velocityDissipationTemp              = serializedObject.FindProperty("velocityDissipation");
        _oldVelocityDissipationTemp           = serializedObject.FindProperty("oldVelocityDissipation");
        _useStartVelocityTextureTemp          = serializedObject.FindProperty("useStartVelocityTexture");
        _startingVelocityTextureTemp          = serializedObject.FindProperty("startingVelocityTexture");
        _startingVelocityTemp                 = serializedObject.FindProperty("startingVelocity");
        _useStartCollisionTextureTemp         = serializedObject.FindProperty("useStartCollisionTexture");
        _startingCollisionTextureTemp         = serializedObject.FindProperty("startingCollisionTexture");
        _resOptionsTemp                       = serializedObject.FindProperty("resOptions");
        _uniqueCollisionResOptionsTemp        = serializedObject.FindProperty("uniqueCollisionResOptions");
        _uniqueColorResOptionsTemp            = serializedObject.FindProperty("uniqueColorResOptions");
        _uniqueVelocityResOptionsTemp         = serializedObject.FindProperty("uniqueVelocityResOptions");
        _outputTextureTemp                    = serializedObject.FindProperty("outputTexture");
        _updateColorWithFluidTemp             = serializedObject.FindProperty("shouldUpdateColorWithFluid");
        _updateImpulseWithFluidTemp           = serializedObject.FindProperty("shouldUpdateImpulseWithFluid");
        _updateCollisionWithFluidTemp         = serializedObject.FindProperty("shouldUpdateCollisionWithFluid");
        _materialTargetTemp                   = serializedObject.FindProperty("materialTarget");
        _useMyMaterialTemp                    = serializedObject.FindProperty("useMyMaterial");

        if (!Shader.Find("Inkling/impulsePositionShader"))
        {
            Debug.LogError("Inkling/impulsePositionShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/impulsePosition4Shader"))
        {
            Debug.LogError("Inkling/impulsePosition4Shader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/impulsePosition8Shader"))
        {
            Debug.LogError("Inkling/impulsePosition8Shader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/impulsePositionVelShader"))
        {
            Debug.LogError("Inkling/impulsePositionVelShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/impulsePositionVel4Shader"))
        {
            Debug.LogError("Inkling/impulsePositionVel4Shader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/impulsePositionVel8Shader"))
        {
            Debug.LogError("Inkling/impulsePositionVel8Shader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/advectionColorShader"))
        {
            Debug.LogError("Inkling/advectionColorShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/advectionColorTexShader"))
        {
            Debug.LogError("Inkling/advectionColorTexShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/advectionVelocityShader"))
        {
            Debug.LogError("Inkling/advectionVelocityShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/advectionVelocityTexShader"))
        {
            Debug.LogError("Inkling/advectionVelocityTexShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/divergenceShader"))
        {
            Debug.LogError("Inkling/divergenceShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/jacobiRelaxShader"))
        {
            Debug.LogError("Inkling/jacobiRelaxShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/gradientShader"))
        {
            Debug.LogError("Inkling/gradientShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/boundaryOpShader"))
        {
            Debug.LogError("Inkling/boundaryOpShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/initializeToValueShader"))
        {
            Debug.LogError("Inkling/initializeToValueShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/initializeToTextureShader"))
        {
            Debug.LogError("Inkling/initializeToTextureShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/initializeCollisionToTextureShader"))
        {
            Debug.LogError("Inkling/initializeCollisionToTextureShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/setCollisionShader"))
        {
            Debug.LogError("Inkling/setCollisionShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/setCollision4Shader"))
        {
            Debug.LogError("Inkling/setCollision4Shader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/setCollision8Shader"))
        {
            Debug.LogError("Inkling/setCollision8Shader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
        if (!Shader.Find("Inkling/setCollisionTexShader"))
        {
            Debug.LogError("Inkling/setCollisionTexShader could not be found.  "
                          + "Fluids cannot be simulated without this shader.");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Works with the if() at the bottom to tell unity this object has been changed and it
        //  should update the sphere gizmo rendering.
        GUI.changed = false;

        //reset of the value check used below.
        _cautionDisplayed = false;

        EditorGUILayout.Space();

        if (!_updateColorWithFluidTemp.boolValue)
        {
            _colorUpdateText = _colorUpdateFPSTemp.intValue + "";
        }
        else
        {
            _colorUpdateText = _fluidUpdateFPSTemp.intValue + "*";
        }

        if (!_updateImpulseWithFluidTemp.boolValue)
        {
            _velocityUpdateText = _impulseUpdateFPSTemp.intValue + "";
        }
        else
        {
            _velocityUpdateText = _fluidUpdateFPSTemp.intValue + "*";
        }

        if (!_updateCollisionWithFluidTemp.boolValue)
        {
            _collisionUpdateText = _collisionUpdateFPSTemp.intValue + "";
        }
        else
        {
            _collisionUpdateText = _fluidUpdateFPSTemp.intValue + "*";
        }

        fpsFoldout =
            EditorGUILayout.Foldout(fpsFoldout
                                   , ("Sim FPS ("
                                     + _fluidUpdateFPSTemp.intValue
                                     + ", "
                                     + _collisionUpdateText
                                     + ", "
                                     + _colorUpdateText
                                     + ", "
                                     + _velocityUpdateText
                                     + ")"
                                     )
                                   );

        if (fpsFoldout)
        {
            EditorGUILayout.LabelField("_Sim:");

            EditorGUILayout.IntSlider(_fluidUpdateFPSTemp
                                     , 0
                                     , 120
                                     , new GUIContent("Fluid Update FPS", _fluidUpdateFPSHelp)
                                     );

            EditorGUILayout.LabelField("_Actors:");

            EditorGUILayout.PropertyField(_updateCollisionWithFluidTemp
                                         , new GUIContent("Sync Collision to Fluid FPS"
                                                         , _syncCollisionHelp)
                                         );

            if (!_updateCollisionWithFluidTemp.boolValue)
            {
                EditorGUILayout.IntSlider(_collisionUpdateFPSTemp
                                         , 0
                                         , 120
                                         , new GUIContent("Collision Update FPS"
                                                         , _collisionUpdateFPSHelp)
                                         );
            }
            else
            {
                EditorGUILayout.LabelField("Collision Update FPS      (*Synced with Fluid Update)");
            }

            EditorGUILayout.PropertyField(_updateColorWithFluidTemp
                                         , new GUIContent("Sync Color to Fluid FPS"
                                                         , _syncColorHelp)
                                         );

            if (!_updateColorWithFluidTemp.boolValue)
            {
                EditorGUILayout.IntSlider(_colorUpdateFPSTemp
                                         , 0
                                         , 120
                                         , new GUIContent("Color Update FPS"
                                                         , _colorUpdateFPSHelp)
                                         );
            }
            else
            {
                EditorGUILayout.LabelField("Color Update FPS          (*Synced with Fluid Update)");
            }

            EditorGUILayout.PropertyField(_updateImpulseWithFluidTemp
                                         , new GUIContent("Sync Impulse to Fluid FPS"
                                                         , _syncImpulseHelp)
                                         );

            if (!_updateImpulseWithFluidTemp.boolValue)
            {
                EditorGUILayout.IntSlider(_impulseUpdateFPSTemp
                                         , 0
                                         , 120
                                         , new GUIContent("Impulse Update FPS"
                                                         , _impulseUpdateFPSHelp)
                                         );
            }
            else
            {
                EditorGUILayout.LabelField("Impulse Update FPS      (*Synced with Fluid Update)");
            }

            EditorGUILayout.LabelField("_______________________");
            EditorGUILayout.Space();
        }

        if (_fluidUpdateFPSTemp.intValue == 0)
        {
            _fpsFluidReport = "An FPS is set to zero!";

            _showFpsZeroWarning = true;
        }
        else
        {
            _fpsFluidReport = "";

            _showFpsZeroWarning = false;
        }
        if (_colorUpdateFPSTemp.intValue == 0
          && !_showFpsZeroWarning
          && !_updateColorWithFluidTemp.boolValue
          )
        {
            _fpsFluidReport = "An FPS is set to zero!";

            _showFpsZeroWarning = true;
        }
        else
        {
            if (_fpsFluidReport == "")
            {
                _fpsFluidReport = "";

                _showFpsZeroWarning = false;
            }
        }
        if (_collisionUpdateFPSTemp.intValue == 0 && !_showFpsZeroWarning)
        {
            _fpsFluidReport = "An FPS is set to zero!";

            _showFpsZeroWarning = true;
        }
        else
        {
            if (_fpsFluidReport == "")
            {
                _fpsFluidReport = "";

                _showFpsZeroWarning = false;
            }
        }
        if (_impulseUpdateFPSTemp.intValue == 0 && !_showFpsZeroWarning)
        {
            _fpsFluidReport = "An FPS is set to zero!";

            _showFpsZeroWarning = true;
        }
        else
        {
            if (_fpsFluidReport == "")
            {
                _fpsFluidReport = "";

                _showFpsZeroWarning = false;
            }
        }

        if (_fpsFluidReport != "")
        {
            EditorGUILayout.HelpBox(_fpsFluidReport, MessageType.Info);
        }

        EditorGUILayout.Slider(_simStrengthTemp
                              , 0.0f
                              , 10.0f
                              , new GUIContent("Sim Strength", _simStrengthHelp)
                              );

        EditorGUILayout.IntSlider(_pressureIterationTemp
                                 , 1
                                 , 50
                                 , new GUIContent("Pressure Iterations", _pressureIterationHelp)
                                 );


        EditorGUILayout.PropertyField(_resOptionsTemp, new GUIContent("Sim Resolution"));

        switch (_resOptionsTemp.intValue)
        {
            case 0:
                _resolutionTemp.intValue = 32;
                _collisionResolutionTemp.intValue = 32;
                _velocityResolutionTemp.intValue = 32;
                break;
            case 1:
                _resolutionTemp.intValue = 64;
                _collisionResolutionTemp.intValue = 64;
                _velocityResolutionTemp.intValue = 64;
                break;
            case 2:
                _resolutionTemp.intValue = 128;
                _collisionResolutionTemp.intValue = 128;
                _velocityResolutionTemp.intValue = 128;
                break;
            case 3:
                _resolutionTemp.intValue = 256;
                _collisionResolutionTemp.intValue = 256;
                _velocityResolutionTemp.intValue = 256;
                break;
            case 4:
                _resolutionTemp.intValue = 512;
                _collisionResolutionTemp.intValue = 512;
                _velocityResolutionTemp.intValue = 512;
                break;
            case 5:
                _resolutionTemp.intValue = 1024;
                _collisionResolutionTemp.intValue = 1024;
                _velocityResolutionTemp.intValue = 1024;
                EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive and may"
                                       + " run slow on some machines", MessageType.Warning);
                break;
            case 6:
                EditorGUILayout.PropertyField(_uniqueCollisionResOptionsTemp
                                             , new GUIContent("Collision Resolution"));

                EditorGUILayout.PropertyField(_uniqueColorResOptionsTemp
                                             , new GUIContent("Color/Sim Resolution"));

                EditorGUILayout.PropertyField(_uniqueVelocityResOptionsTemp
                                             , new GUIContent("Velocity Resolution"));
                break;
        }

        if (_resOptionsTemp.intValue == 6)
        {
            switch (_uniqueColorResOptionsTemp.intValue)
            {
                case 0:
                    _resolutionTemp.intValue = 32;
                    break;
                case 1:
                    _resolutionTemp.intValue = 64;
                    break;
                case 2:
                    _resolutionTemp.intValue = 128;
                    break;
                case 3:
                    _resolutionTemp.intValue = 256;
                    break;
                case 4:
                    _resolutionTemp.intValue = 512;
                    break;
                case 5:
                    _resolutionTemp.intValue = 1024;
                    if (!_cautionDisplayed)
                    {
                        EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive"
                                               + " and may run slow on some machines"
                                               , MessageType.Warning
                                               );

                        _cautionDisplayed = true;
                    }
                    break;
            }

            switch (_uniqueCollisionResOptionsTemp.intValue)
            {
                case 0:
                    _collisionResolutionTemp.intValue = 32;
                    break;
                case 1:
                    _collisionResolutionTemp.intValue = 64;
                    break;
                case 2:
                    _collisionResolutionTemp.intValue = 128;
                    break;
                case 3:
                    _collisionResolutionTemp.intValue = 256;
                    break;
                case 4:
                    _collisionResolutionTemp.intValue = 512;
                    break;
                case 5:
                    _collisionResolutionTemp.intValue = 1024;
                    if (!_cautionDisplayed)
                    {
                        EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive"
                                               + " and may run slow on some machines"
                                               , MessageType.Warning
                                               );
                        _cautionDisplayed = true;
                    }
                    break;
            }

            switch (_uniqueVelocityResOptionsTemp.intValue)
            {
                case 0:
                    _velocityResolutionTemp.intValue = 32;
                    break;
                case 1:
                    _velocityResolutionTemp.intValue = 64;
                    break;
                case 2:
                    _velocityResolutionTemp.intValue = 128;
                    break;
                case 3:
                    _velocityResolutionTemp.intValue = 256;
                    break;
                case 4:
                    _velocityResolutionTemp.intValue = 512;
                    break;
                case 5:
                    _velocityResolutionTemp.intValue = 1024;
                    if (!_cautionDisplayed)
                    {
                        EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive"
                                               + " and may run slow on some machines"
                                               , MessageType.Warning
                                               );

                        _cautionDisplayed = true;
                    }
                    break;
            }
        }

        EditorGUILayout.PropertyField(_outputTextureTemp, new GUIContent("Output Texture"));

        switch (_outputTextureTemp.intValue)
        {
            case 0:
                _outputTextureNumTemp.intValue = 0;
                break;
            case 1:
                _outputTextureNumTemp.intValue = 1;
                break;
            case 2:
                _outputTextureNumTemp.intValue = 2;
                break;
        }

        EditorGUILayout.PropertyField(_useMyMaterialTemp, new GUIContent("Use My Material"));

        if (!_useMyMaterialTemp.boolValue)
        {
            EditorGUILayout.PropertyField(_materialTargetTemp, new GUIContent("Material Target"));
        }

        EditorGUILayout.PropertyField(_materialTextureSlotTemp
                                     , new GUIContent("Material Texture Slot"));

        EditorGUILayout.PropertyField(_useBoundaryCollisionTemp
                                     , new GUIContent("Use Boundary Collision"
                                                     , _useBoundaryCollisionHelp)
                                     );

        if (!_useBoundaryCollisionTemp.boolValue)
        {
            EditorGUILayout.HelpBox("Turning off Boundary Collision can cause artifacts in the"
                                   + " Sim velocity border.  Try turning on Boundary Collision if"
                                   + " you are seeing problems in the simulation."
                                   , MessageType.Info
                                   );
        }

        EditorGUILayout.LabelField("_______________________");
        EditorGUILayout.Space();

        collisionFoldout = EditorGUILayout.Foldout(collisionFoldout, "Fluid Collision:");

        if (collisionFoldout)
        {
            EditorGUILayout.PropertyField(_useStartCollisionTextureTemp
                                         , new GUIContent("Use Start Collision Texture"));

            if (_useStartCollisionTextureTemp.boolValue)
            {
                if (!_startingCollisionTextureTemp.hasMultipleDifferentValues)
                {
                    _startingCollisionTextureTemp.objectReferenceValue =
                        EditorGUILayout.ObjectField(new GUIContent("Starting Collision Texture"
                                                                   , _startingCollisionTextureHelp)
                                                   , _startingCollisionTextureTemp.objectReferenceValue
                                                   , typeof(Texture2D)
                                                   , false
                                                   );
                }
                else
                {
                    EditorGUILayout.LabelField("Starting Collision Texture");
                    EditorGUILayout.LabelField("                                Multiple");
                    EditorGUILayout.LabelField("                                Texture");
                    EditorGUILayout.LabelField("                                Types");
                }
            }
        }

        EditorGUILayout.LabelField("_______________________");
        EditorGUILayout.Space();

        colorFoldout = EditorGUILayout.Foldout(colorFoldout, "Fluid Color:");

        if (colorFoldout)
        {
            EditorGUILayout.PropertyField(_useColorDissipationTemp
                                         , new GUIContent("Use Color Dissipation"
                                                         , _useColorDissipationHelp)
                                         );


            EditorGUILayout.PropertyField(_useColorDissipationTextureTemp
                                            , new GUIContent("Color Dissipation Mask"));

            if (_useColorDissipationTextureTemp.boolValue)
            {
                if (!_colorDissipationTextureTemp.hasMultipleDifferentValues)
                {
                    _colorDissipationTextureTemp.objectReferenceValue =
                        EditorGUILayout.ObjectField("Color Dissp. Texture"
                                                   , _colorDissipationTextureTemp.objectReferenceValue
                                                   , typeof(Texture2D)
                                                   , false
                                                   );
                }
                else
                {
                    EditorGUILayout.LabelField("Color Dissp. Texture");
                    EditorGUILayout.LabelField("                                Multiple");
                    EditorGUILayout.LabelField("                                Texture");
                    EditorGUILayout.LabelField("                                Types");
                }
            }
            else
            {
                EditorGUILayout.Slider(_oldColorDissipationTemp
                                      , 0.001f
                                      , 1.0f
                                      , new GUIContent("Color Dissipation Rate"
                                                      , _colorDissipationHelp)
                                      );
            }

            if (!_useColorDissipationTextureTemp.boolValue)
            {
                EditorGUILayout.PropertyField(_colorDissipateToTemp
                                             , new GUIContent("Dissipate To Color"
                                                             , _colorDissipationToHelp)
                                             );
            }

            if (!_useColorDissipationTemp.boolValue)
            {
                _colorDissipationTemp.floatValue = 0.0f;
            }

            EditorGUILayout.PropertyField(_useStartColorTextureTemp
                                         , new GUIContent("Use Start Color Texture"));

            if (_useStartColorTextureTemp.boolValue)
            {
                if (!_startingColorTextureTemp.hasMultipleDifferentValues)
                {
                    _startingColorTextureTemp.objectReferenceValue =
                        EditorGUILayout.ObjectField("Starting Color Texture"
                                                   , _startingColorTextureTemp.objectReferenceValue
                                                   , typeof(Texture2D)
                                                   , false
                                                   );
                }
                else
                {
                    EditorGUILayout.LabelField("Starting Color Texture");
                    EditorGUILayout.LabelField("                                Multiple");
                    EditorGUILayout.LabelField("                                Texture");
                    EditorGUILayout.LabelField("                                Types");
                }
            }
            else
            {
                EditorGUILayout.PropertyField(_startingColorTemp, new GUIContent("Starting Color"));
            }
        }

        EditorGUILayout.LabelField("_______________________");
        EditorGUILayout.Space();

        velocityFoldout = EditorGUILayout.Foldout(velocityFoldout, "Fluid Velocity:");

        if (velocityFoldout)
        {
            EditorGUILayout.PropertyField(_useVelocityDissipationTemp
                                         , new GUIContent("Use Velocity Dissipation"
                                                         , _useVelocityDissipationHelp)
                                         );

            EditorGUILayout.PropertyField(_useVelocityDissipationTextureTemp
                                            , new GUIContent("Velocity Dissipation Mask"));

            if (_useVelocityDissipationTextureTemp.boolValue)
            {
                if (!_velocityDissipationTextureTemp.hasMultipleDifferentValues)
                {
                    _velocityDissipationTextureTemp.objectReferenceValue =
                        EditorGUILayout.ObjectField("Velocity Dissp. Texture"
                                                   , _velocityDissipationTextureTemp.objectReferenceValue
                                                   , typeof(Texture2D)
                                                   , false
                                                   );
                }
                else
                {
                    EditorGUILayout.LabelField("Velocity Dissp. Texture");
                    EditorGUILayout.LabelField("                                Multiple");
                    EditorGUILayout.LabelField("                                Texture");
                    EditorGUILayout.LabelField("                                Types");
                }
            }
            else
            {
                EditorGUILayout.Slider(_oldVelocityDissipationTemp
                                      , 0.001f
                                      , 1.0f
                                      , new GUIContent("Velocity Dissipation Rate"
                                                      , _velocityDissipationHelp)
                                      );
            }

            if (!_useVelocityDissipationTemp.boolValue)
            {
                _velocityDissipationTemp.floatValue = 0.0f;
            }

            EditorGUILayout.PropertyField(_useStartVelocityTextureTemp
                                         , new GUIContent("Use Start Velocity Texture"));

            if (_useStartVelocityTextureTemp.boolValue)
            {
                if (!_startingVelocityTextureTemp.hasMultipleDifferentValues)
                {
                    _startingVelocityTextureTemp.objectReferenceValue =
                        EditorGUILayout.ObjectField("Starting Velocity Texture"
                                                   , _startingVelocityTextureTemp.objectReferenceValue
                                                   , typeof(Texture2D)
                                                   , false
                                                   );
                }
                else
                {
                    EditorGUILayout.LabelField("Starting Velocity Texture");
                    EditorGUILayout.LabelField("                                Multiple");
                    EditorGUILayout.LabelField("                                Texture");
                    EditorGUILayout.LabelField("                                Types");
                }
            }
            else
            {
                _startingVelocityTemp.vector2Value =
                    EditorGUILayout.Vector2Field("Starting Velocity"
                                                , _startingVelocityTemp.vector2Value);
            }
        }

        EditorGUILayout.Space();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
