//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FluidSimScript))]
[CanEditMultipleObjects]
class FluidSimScriptInterface : Editor
{
    bool fpsFoldout = false;
    bool collisionFoldout = true;
    bool colorFoldout = true;
    bool velocityFoldout = true;

    private string fpsFluidReport = "";
    //private string fpsColorReport = "";
    //private string fpsImpulseReport = "";
    //private string fpsCollisionReport = "";

    private string colorUpdateText = "";
    private string velocityUpdateText = "";
    private string collisionUpdateText = "";

    private bool showFpsZeroWarning = false;

    private bool cautionDisplayed = false;

    private string fluidUpdateFPSHelp = "";
    private string impulseUpdateFPSHelp = "";
    private string colorUpdateFPSHelp = "";
    private string collisionUpdateFPSHelp = "";
    private string simStrengthHelp = "";
    private string pressureIterationHelp = "";
    //private string startingVelocityTextureHelp = "";
    private string startingCollisionTextureHelp = "";
    private string useColorDissipationHelp = "";
    private string colorDissipationHelp = "";
    private string colorDissipationToHelp = "";
    private string useVelocityDissipationHelp = "";
    private string velocityDissipationHelp = "";
    private string useBoundaryCollisionHelp = "";
    private string syncColorHelp = "";
    private string syncImpulseHelp = "";
    private string syncCollisionHelp = "";
    private string useUnityProMethodHelp = "";

[MenuItem("GameObject/Create Other/FluidSim/FluidSim SimGrid")]
private static void FluidSim_SimGrid()
{
    Vector3 menuSpawnPosition = Vector3.zero;
	Camera menuSpawnCamera;

	if(Selection.activeGameObject != null)
	{
		menuSpawnPosition = Selection.activeGameObject.transform.position;
	}
	else
	{
 		menuSpawnCamera = Camera.current;
 		
 		if(menuSpawnCamera)
 		{
			menuSpawnPosition = menuSpawnCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10.0f));
		}
		else
		{
			menuSpawnPosition = Vector3.zero;
		}
	}
	
	if(Resources.Load("FluidSimObject"))
	{
		Instantiate(Resources.Load("FluidSimObject"), menuSpawnPosition, Quaternion.identity);
	}
	else
	{
		Debug.LogError("FluidSimObject was not found in the Resources folder and could not be created in the Scene.");
	}
}

	private SerializedProperty fluidUpdateFPSTemp;
    private SerializedProperty colorUpdateFPSTemp;
    private SerializedProperty impulseUpdateFPSTemp;
   	private SerializedProperty collisionUpdateFPSTemp;
    private SerializedProperty simStrengthTemp;
    private SerializedProperty pressureIterationTemp;
    //private SerializedProperty resolutionIndexTemp;
    private SerializedProperty resolutionTemp;
    private SerializedProperty collisionResolutionTemp;
    private SerializedProperty velocityResolutionTemp;
    //private SerializedProperty colorResolutionIndexTemp;
    //private SerializedProperty collisionResolutionIndexTemp;
    //private SerializedProperty velocityResolutionIndexTemp;
    private SerializedProperty outputTextureNumTemp;
    private SerializedProperty materialTextureSlotTemp;
    private SerializedProperty useBoundaryCollisionTemp;
    private SerializedProperty useColorDissipationTemp;
    private SerializedProperty useColorDissipationTextureTemp;
    private SerializedProperty colorDissipationTextureSourceTemp;
    private SerializedProperty colorDissipationTemp;
    private SerializedProperty oldColorDissipationTemp;
    private SerializedProperty colorDissipateToTemp;
    private SerializedProperty useStartColorTextureTemp;
    private SerializedProperty startingColorTextureSourceTemp;
    private SerializedProperty startingColorTemp;
    private SerializedProperty useVelocityDissipationTemp;
    private SerializedProperty useVelocityDissipationTextureTemp;
    private SerializedProperty velocityDissipationTextureSourceTemp;
    private SerializedProperty velocityDissipationTemp;
    private SerializedProperty oldVelocityDissipationTemp;
    private SerializedProperty useStartVelocityTextureTemp;
    private SerializedProperty startingVelocityTextureSourceTemp;
    private SerializedProperty startingVelocityTemp;
    private SerializedProperty useStartCollisionTextureTemp;
    private SerializedProperty startingCollisionTextureSourceTemp;
	private SerializedProperty resOptionsTemp;
	private SerializedProperty uniqueCollisionResOptionsTemp;
	private SerializedProperty uniqueColorResOptionsTemp;
	private SerializedProperty uniqueVelocityResOptionsTemp;
	private SerializedProperty outputTextureTemp;
	private SerializedProperty updateColorWithFluidTemp;
	private SerializedProperty updateImpulseWithFluidTemp;
	private SerializedProperty updateCollisionWithFluidTemp;
	private SerializedProperty materialTargetTemp;
	private SerializedProperty useMyMaterialTemp;
    private SerializedProperty useUnityProMethodTemp;
	
	void OnEnable()
    {
		fluidUpdateFPSTemp = serializedObject.FindProperty("fluidUpdateFPS");
	    colorUpdateFPSTemp = serializedObject.FindProperty("colorUpdateFPS");
	    impulseUpdateFPSTemp = serializedObject.FindProperty("impulseUpdateFPS");
	   	collisionUpdateFPSTemp = serializedObject.FindProperty("collisionUpdateFPS");
	    simStrengthTemp = serializedObject.FindProperty("simStrength");
	    pressureIterationTemp = serializedObject.FindProperty("pressureIteration");
	    //resolutionIndexTemp = serializedObject.FindProperty("resolutionIndex");
	    resolutionTemp = serializedObject.FindProperty("resolution");
	    collisionResolutionTemp = serializedObject.FindProperty("collisionResolution");
	    velocityResolutionTemp = serializedObject.FindProperty("velocityResolution");
	    //colorResolutionIndexTemp = serializedObject.FindProperty("colorResolutionIndex");
	    //collisionResolutionIndexTemp = serializedObject.FindProperty("collisionResolutionIndex");
	    //velocityResolutionIndexTemp = serializedObject.FindProperty("velocityResolutionIndex");
	    outputTextureNumTemp = serializedObject.FindProperty("outputTextureNum");
	    materialTextureSlotTemp = serializedObject.FindProperty("materialTextureSlot");
	    useBoundaryCollisionTemp = serializedObject.FindProperty("useBoundaryCollision");
	    useColorDissipationTemp = serializedObject.FindProperty("useColorDissipation");
	    useColorDissipationTextureTemp = serializedObject.FindProperty("useColorDissipationTexture");
	    colorDissipationTextureSourceTemp = serializedObject.FindProperty("colorDissipationTextureSource");
	    colorDissipationTemp = serializedObject.FindProperty("colorDissipation");
	    oldColorDissipationTemp = serializedObject.FindProperty("oldColorDissipation");
	    colorDissipateToTemp = serializedObject.FindProperty("colorDissipateTo");
	    useStartColorTextureTemp = serializedObject.FindProperty("useStartColorTexture");
	    startingColorTextureSourceTemp = serializedObject.FindProperty("startingColorTextureSource");
	    startingColorTemp = serializedObject.FindProperty("startingColor");
	    useVelocityDissipationTemp = serializedObject.FindProperty("useVelocityDissipation");
	    useVelocityDissipationTextureTemp = serializedObject.FindProperty("useVelocityDissipationTexture");
	    velocityDissipationTextureSourceTemp = serializedObject.FindProperty("velocityDissipationTextureSource");
	    velocityDissipationTemp = serializedObject.FindProperty("velocityDissipation");
	    oldVelocityDissipationTemp = serializedObject.FindProperty("oldVelocityDissipation");
	    useStartVelocityTextureTemp = serializedObject.FindProperty("useStartVelocityTexture");
	    startingVelocityTextureSourceTemp = serializedObject.FindProperty("startingVelocityTextureSource");
	    startingVelocityTemp = serializedObject.FindProperty("startingVelocity");
	    useStartCollisionTextureTemp = serializedObject.FindProperty("useStartCollisionTexture");
	    startingCollisionTextureSourceTemp = serializedObject.FindProperty("startingCollisionTextureSource");
	    resOptionsTemp = serializedObject.FindProperty("resOptions");
	    uniqueCollisionResOptionsTemp = serializedObject.FindProperty("uniqueCollisionResOptions");
		uniqueColorResOptionsTemp = serializedObject.FindProperty("uniqueColorResOptions");
		uniqueVelocityResOptionsTemp = serializedObject.FindProperty("uniqueVelocityResOptions");
		outputTextureTemp = serializedObject.FindProperty("outputTexture");
		updateColorWithFluidTemp = serializedObject.FindProperty("updateColorWithFluid");
		updateImpulseWithFluidTemp = serializedObject.FindProperty("updateImpulseWithFluid");
		updateCollisionWithFluidTemp = serializedObject.FindProperty("updateCollisionWithFluid");
		materialTargetTemp = serializedObject.FindProperty("materialTarget");
		useMyMaterialTemp = serializedObject.FindProperty("useMyMaterial");
		useUnityProMethodTemp = serializedObject.FindProperty("useUnityProMethod");

		if(!Shader.Find("FluidSim/impulseLocationShader"))
		{
			Debug.LogError("FluidSim/impulseLocationShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/impulseLocation4Shader"))
		{
			Debug.LogError("FluidSim/impulseLocation4Shader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/impulseLocation8Shader"))
		{
			Debug.LogError("FluidSim/impulseLocation8Shader could not be found.  Fluids cannot be simulated without this shader.");
		}
    	if(!Shader.Find("FluidSim/impulseLocationVelShader"))
		{
			Debug.LogError("FluidSim/impulseLocationVelShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/impulseLocationVel4Shader"))
		{
			Debug.LogError("FluidSim/impulseLocationVel4Shader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/impulseLocationVel8Shader"))
		{
			Debug.LogError("FluidSim/impulseLocationVel8Shader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/advectionColorShader"))
		{
			Debug.LogError("FluidSim/advectionColorShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/advectionColorTexShader"))
		{
			Debug.LogError("FluidSim/advectionColorTexShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/advectionVelocityShader"))
		{
			Debug.LogError("FluidSim/advectionVelocityShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/advectionVelocityTexShader"))
		{
			Debug.LogError("FluidSim/advectionVelocityTexShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/divergenceShader"))
		{
			Debug.LogError("FluidSim/divergenceShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/jacobiRelaxShader"))
		{
			Debug.LogError("FluidSim/jacobiRelaxShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/gradientShader"))
		{
			Debug.LogError("FluidSim/gradientShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/boundaryOpShader"))
		{
			Debug.LogError("FluidSim/boundaryOpShader could not be found.  Fluids cannot be simulated without this shader.");
		}
    	if(!Shader.Find("FluidSim/initializeToValueShader"))
		{
			Debug.LogError("FluidSim/initializeToValueShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/initializeToTextureShader"))
		{
			Debug.LogError("FluidSim/initializeToTextureShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/initializeCollisionToTextureShader"))
		{
			Debug.LogError("FluidSim/initializeCollisionToTextureShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/setCollisionShader"))
		{
			Debug.LogError("FluidSim/setCollisionShader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/setCollision4Shader"))
		{
			Debug.LogError("FluidSim/setCollision4Shader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/setCollision8Shader"))
		{
			Debug.LogError("FluidSim/setCollision8Shader could not be found.  Fluids cannot be simulated without this shader.");
		}
		if(!Shader.Find("FluidSim/setCollisionTexShader"))
		{
			Debug.LogError("FluidSim/setCollisionTexShader could not be found.  Fluids cannot be simulated without this shader.");
		}
	}
	
    public override void OnInspectorGUI()
    {
    	serializedObject.Update();
		
		//Works with the if() at the bottom to tell unity this object has been changed and it should update the sphere gizmo rendering.
    	GUI.changed = false;
    	
		//reset of the value check used below.
		cautionDisplayed = false;
		
    	EditorGUILayout.Space();

    	EditorGUILayout.PropertyField(useUnityProMethodTemp, new GUIContent("Use GPU Features", useUnityProMethodHelp));

		EditorGUILayout.HelpBox("GPU Features only work in Unity Pro.  If \"Use GPU Features\" is false, FluidSim will only use the CPU.", MessageType.Info);

    	EditorGUILayout.LabelField("_______________________");
    	EditorGUILayout.Space();
    	
    	if(!updateColorWithFluidTemp.boolValue)
    	{
    		colorUpdateText = colorUpdateFPSTemp.intValue + "";
    	}
    	else
    	{
    		colorUpdateText = fluidUpdateFPSTemp.intValue + "*";
    	}

		if(!updateImpulseWithFluidTemp.boolValue)
    	{
    		velocityUpdateText = impulseUpdateFPSTemp.intValue + "";
    	}
    	else
    	{
    		velocityUpdateText = fluidUpdateFPSTemp.intValue + "*";
    	}
    	
    	if(!updateCollisionWithFluidTemp.boolValue)
    	{
    		collisionUpdateText = collisionUpdateFPSTemp.intValue + "";
    	}
    	else
    	{
    		collisionUpdateText = fluidUpdateFPSTemp.intValue + "*";
    	}
    	
		fpsFoldout = EditorGUILayout.Foldout(fpsFoldout, "Sim FPS (" + fluidUpdateFPSTemp.intValue + ", " + collisionUpdateText + ", " + colorUpdateText + ", " + velocityUpdateText + ")");

    	if(fpsFoldout)
    	{
            EditorGUILayout.LabelField("_Sim:");
	    	EditorGUILayout.IntSlider(fluidUpdateFPSTemp, 0, 120, new GUIContent("Fluid Update FPS", fluidUpdateFPSHelp));
            EditorGUILayout.LabelField("_Actors:");
            EditorGUILayout.PropertyField(updateCollisionWithFluidTemp, new GUIContent("Sync Collision to Fluid FPS", syncCollisionHelp));
	    	if(!updateCollisionWithFluidTemp.boolValue)
	    	{
				EditorGUILayout.IntSlider(collisionUpdateFPSTemp, 0, 120, new GUIContent("Collision Update FPS", collisionUpdateFPSHelp));
	    	}
	    	else
	    	{
	    		EditorGUILayout.LabelField("Collision Update FPS      (*Synced with Fluid Update)");
	    	}
	    	EditorGUILayout.PropertyField(updateColorWithFluidTemp, new GUIContent("Sync Color to Fluid FPS", syncColorHelp));
	    	if(!updateColorWithFluidTemp.boolValue)
	    	{
				EditorGUILayout.IntSlider(colorUpdateFPSTemp, 0, 120, new GUIContent("Color Update FPS", colorUpdateFPSHelp));
	    	}
	    	else
	    	{
	    		EditorGUILayout.LabelField("Color Update FPS          (*Synced with Fluid Update)");
	    	}
	    	EditorGUILayout.PropertyField(updateImpulseWithFluidTemp, new GUIContent("Sync Impulse to Fluid FPS", syncImpulseHelp));
	    	if(!updateImpulseWithFluidTemp.boolValue)
	    	{
				EditorGUILayout.IntSlider(impulseUpdateFPSTemp, 0, 120, new GUIContent("Impulse Update FPS", impulseUpdateFPSHelp));
	    	}
	    	else
	    	{
	    		EditorGUILayout.LabelField("Impulse Update FPS      (*Synced with Fluid Update)");
	    	}

	    	EditorGUILayout.LabelField("_______________________");
    		EditorGUILayout.Space();
	    }
    	
    	if(fluidUpdateFPSTemp.intValue == 0)
		{
			fpsFluidReport = "An FPS is set to zero!";
				
			showFpsZeroWarning = true;
		}
		else
		{
			fpsFluidReport = "";
				
			showFpsZeroWarning = false;
		}
		if(colorUpdateFPSTemp.intValue == 0 && !showFpsZeroWarning && !updateColorWithFluidTemp.boolValue)
		{
			fpsFluidReport = "An FPS is set to zero!";
				
			showFpsZeroWarning = true;
		}
		else
		{
			if(fpsFluidReport == "")
			{
				fpsFluidReport = "";
					
				showFpsZeroWarning = false;
			}
		}
		if(collisionUpdateFPSTemp.intValue == 0 && !showFpsZeroWarning)
		{
			fpsFluidReport = "An FPS is set to zero!";
				
			showFpsZeroWarning = true;
		}
		else
		{
			if(fpsFluidReport == "")
			{
				fpsFluidReport = "";
					
				showFpsZeroWarning = false;
			}
		}
		if(impulseUpdateFPSTemp.intValue == 0 && !showFpsZeroWarning)
		{
			fpsFluidReport = "An FPS is set to zero!";
				
			showFpsZeroWarning = true;
		}
		else
		{
			if(fpsFluidReport == "")
			{
				fpsFluidReport = "";
					
				showFpsZeroWarning = false;
			}
		}

		if(fpsFluidReport != "")
		{
    		EditorGUILayout.HelpBox(fpsFluidReport, MessageType.Info);
    	}
    	
    	EditorGUILayout.Slider(simStrengthTemp, 0.0f, 10.0f, new GUIContent("Sim Strength", simStrengthHelp));
   		
    	EditorGUILayout.IntSlider(pressureIterationTemp, 1, 50, new GUIContent("Pressure Iterations", pressureIterationHelp));
    	
    	if(useUnityProMethodTemp.boolValue)
		{
    		EditorGUILayout.PropertyField(resOptionsTemp, new GUIContent("Sim Resolution"));
    		
    		switch(resOptionsTemp.intValue)
			{
				case 0 :
					resolutionTemp.intValue = 32;
					collisionResolutionTemp.intValue = 32;
					velocityResolutionTemp.intValue = 32;
					break;
				case 1 :
					resolutionTemp.intValue = 64;
					collisionResolutionTemp.intValue = 64;
					velocityResolutionTemp.intValue = 64;
					break;
				case 2 :
					resolutionTemp.intValue = 128;
					collisionResolutionTemp.intValue = 128;
					velocityResolutionTemp.intValue = 128;
					break;
				case 3 :
					resolutionTemp.intValue = 256;
					collisionResolutionTemp.intValue = 256;
					velocityResolutionTemp.intValue = 256;
					break;
				case 4 :
					resolutionTemp.intValue = 512;
					collisionResolutionTemp.intValue = 512;
					velocityResolutionTemp.intValue = 512;
					break;
				case 5 :
					resolutionTemp.intValue = 1024;
					collisionResolutionTemp.intValue = 1024;
					velocityResolutionTemp.intValue = 1024;
					EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive and may run slow on some machines", MessageType.Warning);
					break;
				case 6 :
					EditorGUILayout.PropertyField(uniqueCollisionResOptionsTemp, new GUIContent("Collision Resolution"));
					EditorGUILayout.PropertyField(uniqueColorResOptionsTemp, new GUIContent("Color/Sim Resolution"));
					EditorGUILayout.PropertyField(uniqueVelocityResOptionsTemp, new GUIContent("Velocity Resolution"));
                    break;
			}
		}
		else
		{
			EditorGUILayout.PropertyField(uniqueColorResOptionsTemp, new GUIContent("Sim Resolution"));
			
			switch(uniqueColorResOptionsTemp.intValue)
			{
				case 0 :
					resolutionTemp.intValue = 32;
					break;
				case 1 :
					resolutionTemp.intValue = 64;
					break;
				case 2 :
					resolutionTemp.intValue = 128;
					break;
				case 3 :
					resolutionTemp.intValue = 256;
					break;
				case 4 :
					resolutionTemp.intValue = 512;
					EditorGUILayout.HelpBox("Caution, a Fluid resolution of 512 is expensive and may run slow on some machines", MessageType.Warning);
					break;
				case 5 :
					resolutionTemp.intValue = 1024;
					EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive and may run slow on some machines", MessageType.Warning);
					break;
			}
		}
    	
		if(resOptionsTemp.intValue == 6)
		{
			switch(uniqueColorResOptionsTemp.intValue)
			{
				case 0 :
					resolutionTemp.intValue = 32;
					break;
				case 1 :
					resolutionTemp.intValue = 64;
					break;
				case 2 :
					resolutionTemp.intValue = 128;
					break;
				case 3 :
					resolutionTemp.intValue = 256;
					break;
				case 4 :
					resolutionTemp.intValue = 512;
					break;
				case 5 :
					resolutionTemp.intValue = 1024;
					if(!cautionDisplayed && useUnityProMethodTemp.boolValue)
					{
						EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive and may run slow on some machines", MessageType.Warning);
						cautionDisplayed = true;
					}
					break;
			}
			
			switch(uniqueCollisionResOptionsTemp.intValue)
			{
				case 0 :
					collisionResolutionTemp.intValue = 32;
					break;
				case 1 :
					collisionResolutionTemp.intValue = 64;
					break;
				case 2 :
					collisionResolutionTemp.intValue = 128;
					break;
				case 3 :
					collisionResolutionTemp.intValue = 256;
					break;
				case 4 :
					collisionResolutionTemp.intValue = 512;
					break;
				case 5 :
					collisionResolutionTemp.intValue = 1024;
					if(!cautionDisplayed)
					{
						EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive and may run slow on some machines", MessageType.Warning);
						cautionDisplayed = true;
					}
					break;
			}
			
			switch(uniqueVelocityResOptionsTemp.intValue)
			{
				case 0 :
					velocityResolutionTemp.intValue = 32;
					break;
				case 1 :
					velocityResolutionTemp.intValue = 64;
					break;
				case 2 :
					velocityResolutionTemp.intValue = 128;
					break;
				case 3 :
					velocityResolutionTemp.intValue = 256;
					break;
				case 4 :
					velocityResolutionTemp.intValue = 512;
					break;
				case 5 :
					velocityResolutionTemp.intValue = 1024;
					if(!cautionDisplayed)
					{
						EditorGUILayout.HelpBox("Caution, a Fluid resolution of 1024 is expensive and may run slow on some machines", MessageType.Warning);
						cautionDisplayed = true;
					}
					break;
			}
		}

		EditorGUILayout.PropertyField(outputTextureTemp, new GUIContent("Output Texture"));

    	switch(outputTextureTemp.intValue)
		{
			case 0 :
				outputTextureNumTemp.intValue = 0;
				break;
			case 1 :
				outputTextureNumTemp.intValue = 1;
				break;
			case 2 :
				outputTextureNumTemp.intValue = 2;
				break; 	
		}

    	EditorGUILayout.PropertyField(useMyMaterialTemp, new GUIContent("Use My Material"));
    	
    	if(!useMyMaterialTemp.boolValue)
    	{
    		EditorGUILayout.PropertyField(materialTargetTemp, new GUIContent("Material Target"));
    	}
    	
    	EditorGUILayout.PropertyField(materialTextureSlotTemp, new GUIContent("Material Texture Slot"));

    	EditorGUILayout.PropertyField(useBoundaryCollisionTemp, new GUIContent("Use Boundary Collision", useBoundaryCollisionHelp));
    	
    	if(!useBoundaryCollisionTemp.boolValue)
    	{
    		EditorGUILayout.HelpBox("Turning off Boundary Collision can cause artifacts in the Sim velocity border.  Try turning on Boundary Collision if you are seeing problems in the simulation.", MessageType.Info);
		}
		
    	EditorGUILayout.LabelField("_______________________");
    	EditorGUILayout.Space();

		collisionFoldout = EditorGUILayout.Foldout(collisionFoldout, "Fluid Collision:");
    	
    	if(collisionFoldout)
    	{
			EditorGUILayout.PropertyField(useStartCollisionTextureTemp, new GUIContent("Use Start Collision Texture"));
			
			if(useStartCollisionTextureTemp.boolValue)
			{
				if(!startingCollisionTextureSourceTemp.hasMultipleDifferentValues)
		    	{
					startingCollisionTextureSourceTemp.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Starting Collision Texture", startingCollisionTextureHelp), startingCollisionTextureSourceTemp.objectReferenceValue, typeof(Texture2D), false);
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
    	
    	if(colorFoldout)
    	{
	    	EditorGUILayout.PropertyField(useColorDissipationTemp, new GUIContent("Use Color Dissipation", useColorDissipationHelp));
	
			if(useUnityProMethodTemp.boolValue)
			{
				EditorGUILayout.PropertyField(useColorDissipationTextureTemp, new GUIContent("Color Dissipation Mask"));
			}
			
			if(useColorDissipationTextureTemp.boolValue && useUnityProMethodTemp.boolValue)
			{
				if(!colorDissipationTextureSourceTemp.hasMultipleDifferentValues)
	    		{
					colorDissipationTextureSourceTemp.objectReferenceValue = EditorGUILayout.ObjectField("Color Dissp. Texture", colorDissipationTextureSourceTemp.objectReferenceValue, typeof(Texture2D), false);
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
				EditorGUILayout.Slider(oldColorDissipationTemp, 0.001f, 1.0f, new GUIContent("Color Dissipation Rate", colorDissipationHelp));
			}
			
			if(!useColorDissipationTextureTemp.boolValue || !useUnityProMethodTemp.boolValue)
			{
				EditorGUILayout.PropertyField(colorDissipateToTemp, new GUIContent("Dissipate To Color", colorDissipationToHelp));
			}
			
			if(!useColorDissipationTemp.boolValue)
			{
				colorDissipationTemp.floatValue = 0.0f;
			}
	    	
			EditorGUILayout.PropertyField(useStartColorTextureTemp, new GUIContent("Use Start Color Texture"));
			
	    	if(useStartColorTextureTemp.boolValue)
	    	{
	    		if(!startingColorTextureSourceTemp.hasMultipleDifferentValues)
	    		{
					startingColorTextureSourceTemp.objectReferenceValue = EditorGUILayout.ObjectField("Starting Color Texture", startingColorTextureSourceTemp.objectReferenceValue, typeof(Texture2D), false);;
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
				EditorGUILayout.PropertyField(startingColorTemp, new GUIContent("Starting Color"));
			}
		}
		
		EditorGUILayout.LabelField("_______________________");
    	EditorGUILayout.Space();
		
		velocityFoldout = EditorGUILayout.Foldout(velocityFoldout, "Fluid Velocity:");
    	
    	if(velocityFoldout)
    	{
			EditorGUILayout.PropertyField(useVelocityDissipationTemp, new GUIContent("Use Velocity Dissipation", useVelocityDissipationHelp));

			if(useUnityProMethodTemp.boolValue)
			{
				EditorGUILayout.PropertyField(useVelocityDissipationTextureTemp, new GUIContent("Velocity Dissipation Mask"));
			}
			if(useVelocityDissipationTextureTemp.boolValue && useUnityProMethodTemp.boolValue)
			{
				if(!velocityDissipationTextureSourceTemp.hasMultipleDifferentValues)
	    		{
					velocityDissipationTextureSourceTemp.objectReferenceValue = EditorGUILayout.ObjectField("Velocity Dissp. Texture", velocityDissipationTextureSourceTemp.objectReferenceValue, typeof(Texture2D), false);
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
				EditorGUILayout.Slider(oldVelocityDissipationTemp, 0.001f, 1.0f, new GUIContent("Velocity Dissipation Rate", velocityDissipationHelp));
			}
			
			if(!useVelocityDissipationTemp.boolValue)
			{
				velocityDissipationTemp.floatValue = 0.0f;
			}
			
			EditorGUILayout.PropertyField(useStartVelocityTextureTemp, new GUIContent("Use Start Velocity Texture"));
			
			if(useStartVelocityTextureTemp.boolValue)
			{
				if(!startingVelocityTextureSourceTemp.hasMultipleDifferentValues)
		    	{
					startingVelocityTextureSourceTemp.objectReferenceValue = EditorGUILayout.ObjectField("Starting Velocity Texture", startingVelocityTextureSourceTemp.objectReferenceValue, typeof(Texture2D), false);
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
				startingVelocityTemp.vector2Value = EditorGUILayout.Vector2Field("Starting Velocity", startingVelocityTemp.vector2Value);
			}
		}
		
		EditorGUILayout.Space();
		
		if(GUI.changed)
		{
            EditorUtility.SetDirty(target);
    	}
    	
    	serializedObject.ApplyModifiedProperties ();
    }
}
