//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FluidSimInfluenceActor))]
[CanEditMultipleObjects]
class FluidSimInfluenceActorInterface : Editor
{

[MenuItem("GameObject/Create Other/FluidSim/FluidSim InfluenceActor")]
private static void FluidSim_InfluenceActor()
{
    //Uses the current viewport camera for spawn location, or the world zero if the current camera isnt valid.
 	Vector3 menuSpawnPosition = Vector3.zero;
	Camera menuSpawnCamera;

	if(Selection.activeGameObject != null)
	{
		menuSpawnPosition = Selection.activeGameObject.transform.position;
	}
	else
	{
 		menuSpawnCamera = Camera.current;
		
		if(menuSpawnCamera != null)
		{
			menuSpawnPosition = menuSpawnCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10.0f));
		}
		else
		{
			menuSpawnPosition = Vector3.zero;
		}
	}
	
	if(Resources.Load("FluidSimInfluenceActorObject") != null)
	{
		Instantiate(Resources.Load("FluidSimInfluenceActorObject"), menuSpawnPosition, Quaternion.identity);
	}
	else
	{
		Debug.LogError("FluidSimInfluenceActorObject was not found in the Resources folder and could not be created in the Scene.");
	}
}

private string showGPUHelp = "";
private string showGizmosHelp = "";
private string actorPriorityHelp = "";

private string staticCollisionHelp = "";
private string dynamicCollisionHelp = "";
private string useCollisionMaskHelp = "";
private string useColorMaskHelp = "";
private string useVelocityMaskHelp = "";

private string collisionStrengthHelp = "";
private string moveVelocityMultiplierHelp = "";

private string collisionFalloffHelp = "";
private string colorFalloffHelp = "";
private string velocityFalloffHelp = "";

private string scaleBySizeHelp = "";

private string isCollisionOn = "";
private string isColorOn = "";
private string isVelocityOn = "";

private SerializedProperty useGPUfeaturesTemp;
private SerializedProperty showGizmosTemp;
private SerializedProperty collisionFoldoutTemp;
private SerializedProperty colorFoldoutTemp;
private SerializedProperty velocityFoldoutTemp;
private SerializedProperty staticCollisionTemp;
private SerializedProperty dynamicCollisionTemp;
private SerializedProperty useCollisionMaskTextureTemp;
private SerializedProperty collisionMaskTextureTemp;
private SerializedProperty collisionStrengthTemp;
private SerializedProperty collisionSizeTemp;
private SerializedProperty collisionFalloffTemp;
private SerializedProperty moveVelocityMultiplierTemp;
private SerializedProperty addColorTemp;
private SerializedProperty useColorMaskTextureTemp;
private SerializedProperty colorMaskTextureTemp;
private SerializedProperty colorValueTemp;
private SerializedProperty colorSizeTemp;
private SerializedProperty colorFalloffTemp;
private SerializedProperty addVelocityTemp;
private SerializedProperty useVelocityMaskTextureTemp;
private SerializedProperty velocityMaskTextureTemp;
private SerializedProperty velocityStrengthTemp;
private SerializedProperty velocitySizeTemp;
private SerializedProperty velocityFalloffTemp;
private SerializedProperty myTransformTemp;
private SerializedProperty lastFramePositionTemp;
private SerializedProperty actorPriorityTemp;
private SerializedProperty multiplySizeByScaleTemp;
	
    void OnEnable()
    {
    	useGPUfeaturesTemp = serializedObject.FindProperty("useGPUfeatures");
    	showGizmosTemp = serializedObject.FindProperty("showGizmos");
    	collisionFoldoutTemp = serializedObject.FindProperty("collisionFoldout");
		colorFoldoutTemp = serializedObject.FindProperty("colorFoldout");
		velocityFoldoutTemp = serializedObject.FindProperty("velocityFoldout");
		staticCollisionTemp = serializedObject.FindProperty("fluidDetails.staticCollision");
		dynamicCollisionTemp = serializedObject.FindProperty("fluidDetails.dynamicCollision");
		useCollisionMaskTextureTemp = serializedObject.FindProperty("fluidDetails.useCollisionMaskTexture");
		collisionMaskTextureTemp = serializedObject.FindProperty("fluidDetails.collisionMaskTexture");
		collisionStrengthTemp = serializedObject.FindProperty("fluidDetails.collisionStrength");
		collisionSizeTemp = serializedObject.FindProperty("fluidDetails.collisionSize");
		collisionFalloffTemp = serializedObject.FindProperty("fluidDetails.collisionFalloff");
		moveVelocityMultiplierTemp = serializedObject.FindProperty("fluidDetails.moveVelocityMultiplier");
		addColorTemp = serializedObject.FindProperty("fluidDetails.addColor");
		useColorMaskTextureTemp = serializedObject.FindProperty("fluidDetails.useColorMaskTexture");
		colorMaskTextureTemp = serializedObject.FindProperty("fluidDetails.colorMaskTexture");
		colorValueTemp = serializedObject.FindProperty("fluidDetails.colorValue");
		colorSizeTemp = serializedObject.FindProperty("fluidDetails.colorSize");
		colorFalloffTemp = serializedObject.FindProperty("fluidDetails.colorFalloff");
		addVelocityTemp = serializedObject.FindProperty("fluidDetails.addVelocity");
		useVelocityMaskTextureTemp = serializedObject.FindProperty("fluidDetails.useVelocityMaskTexture");
		velocityMaskTextureTemp = serializedObject.FindProperty("fluidDetails.velocityMaskTexture");
		velocityStrengthTemp = serializedObject.FindProperty("fluidDetails.velocityStrength");
		velocitySizeTemp = serializedObject.FindProperty("fluidDetails.velocitySize");
		velocityFalloffTemp = serializedObject.FindProperty("fluidDetails.velocityFalloff");
		actorPriorityTemp = serializedObject.FindProperty("fluidDetails.actorPriority");
		multiplySizeByScaleTemp = serializedObject.FindProperty("fluidDetails.multiplySizeByScale");
    }

    public override void OnInspectorGUI()
    {
    	serializedObject.Update();
    	
        //Works with the if() at the bottom to tell unity this object has been changed and it should update the sphere gizmo rendering.
    	GUI.changed = false;
    	
    	EditorGUILayout.Space();

    	EditorGUILayout.PropertyField(useGPUfeaturesTemp, new GUIContent("Use GPU Features", showGPUHelp));

		if(useGPUfeaturesTemp.boolValue)
		{
			EditorGUILayout.HelpBox("GPU Features only work in Unity Pro.  GPU features must also be turned on in the Fluid Object.", MessageType.Info);
    	}
    	EditorGUILayout.LabelField("_______________________");
    	EditorGUILayout.Space();
    	
    	EditorGUILayout.PropertyField(showGizmosTemp, new GUIContent("Show Size Gizmos", showGizmosHelp));
    	
    	EditorGUILayout.PropertyField(multiplySizeByScaleTemp, new GUIContent("Scale Size by localScale", scaleBySizeHelp));

		EditorGUILayout.IntSlider(actorPriorityTemp, 1, 5, new GUIContent("Influence Priority", actorPriorityHelp));

    	EditorGUILayout.LabelField("_______________________");
    	EditorGUILayout.Space();

		if(staticCollisionTemp.boolValue || dynamicCollisionTemp.boolValue)
		{
			if(staticCollisionTemp.boolValue && dynamicCollisionTemp.boolValue)
			{
				isCollisionOn = "(Static & Dynamic)";
			}
			else
			{
				if(staticCollisionTemp.boolValue)
				{
					isCollisionOn = "(Static)";
				}
				
				if(dynamicCollisionTemp.boolValue)
				{
					isCollisionOn = "(Dynamic)";
				}
			}
		}
		else
		{
			isCollisionOn = "(Off)";
		}

		collisionFoldoutTemp.boolValue = EditorGUILayout.Foldout(collisionFoldoutTemp.boolValue, "Influence Collision " + isCollisionOn);
    	
    	if(collisionFoldoutTemp.boolValue)
    	{
			EditorGUILayout.PropertyField(staticCollisionTemp, new GUIContent("Static Collision", staticCollisionHelp));
			EditorGUILayout.PropertyField(dynamicCollisionTemp, new GUIContent("Dynamic Collision", dynamicCollisionHelp));

			if(useGPUfeaturesTemp.boolValue)
			{
				EditorGUILayout.PropertyField(useCollisionMaskTextureTemp, new GUIContent("Use Collision Mask", useCollisionMaskHelp));

			    if(useCollisionMaskTextureTemp.boolValue)
		    	{
		    		if(!collisionMaskTextureTemp.hasMultipleDifferentValues)
		    		{
			    		collisionMaskTextureTemp.objectReferenceValue = EditorGUILayout.ObjectField("Collision Mask Texture", collisionMaskTextureTemp.objectReferenceValue, typeof(Texture2D), false);
					}
					else
					{
						EditorGUILayout.LabelField("Collision Mask Texture");
						EditorGUILayout.LabelField("                                Multiple");
						EditorGUILayout.LabelField("                                Texture");
						EditorGUILayout.LabelField("                                Types");
					}
				}
			}

			EditorGUILayout.Slider(collisionStrengthTemp, 0, 1, new GUIContent("Collision Strength", collisionStrengthHelp));
			EditorGUILayout.Slider(collisionSizeTemp, 0, 500, "Collision Size");
			
			if(dynamicCollisionTemp.boolValue)
			{
				EditorGUILayout.Slider(moveVelocityMultiplierTemp, 0, 2, new GUIContent("Move Velocity Multiplier", moveVelocityMultiplierHelp));
			}
			
			if(!useCollisionMaskTextureTemp.boolValue)
			{
				EditorGUILayout.Slider(collisionFalloffTemp, 0.01f, 1.0f, new GUIContent("Collision Falloff", collisionFalloffHelp));
			}
		}

	    EditorGUILayout.LabelField("_______________________");
	    EditorGUILayout.Space();

		if(addColorTemp.boolValue)
		{
			isColorOn = "(On)";
		}
		else
		{
			isColorOn = "(Off)";
		}

		colorFoldoutTemp.boolValue = EditorGUILayout.Foldout(colorFoldoutTemp.boolValue, "Influence Color " + isColorOn);
    	
    	if(colorFoldoutTemp.boolValue)
    	{
	    	EditorGUILayout.PropertyField(addColorTemp, new GUIContent("Add Color"));

			if(useGPUfeaturesTemp.boolValue)
			{
			    EditorGUILayout.PropertyField(useColorMaskTextureTemp, new GUIContent("Use Color Mask", useColorMaskHelp));
			    
			    if(useColorMaskTextureTemp.boolValue)
		    	{
		    		if(!colorMaskTextureTemp.hasMultipleDifferentValues)
		    		{
						colorMaskTextureTemp.objectReferenceValue = EditorGUILayout.ObjectField("Color Mask Texture", colorMaskTextureTemp.objectReferenceValue, typeof(Texture2D), false);
					}
					else
					{
						EditorGUILayout.LabelField("Color Mask Texture");
						EditorGUILayout.LabelField("                                Multiple");
						EditorGUILayout.LabelField("                                Texture");
						EditorGUILayout.LabelField("                                Types");
					}
				}
			}

			if(!useColorMaskTextureTemp.boolValue || !useGPUfeaturesTemp.boolValue)
			{
				EditorGUILayout.PropertyField(colorValueTemp, new GUIContent("Color (Alpha = Strength)"));
			}

	    	EditorGUILayout.Slider(colorSizeTemp, 0, 500, "Color Size");
			
			if(!useColorMaskTextureTemp.boolValue)
			{
				EditorGUILayout.Slider(colorFalloffTemp, 0.01f, 1.0f, new GUIContent("Color Falloff", colorFalloffHelp));
			}
		}

	    EditorGUILayout.LabelField("_______________________");
	    EditorGUILayout.Space();

		if(addVelocityTemp.boolValue)
		{
			isVelocityOn = "(On)";
		}
		else
		{
			isVelocityOn = "(Off)";
		}

		velocityFoldoutTemp.boolValue = EditorGUILayout.Foldout(velocityFoldoutTemp.boolValue, "Influence Velocity " + isVelocityOn);

    	if(velocityFoldoutTemp.boolValue)
    	{
	    	EditorGUILayout.PropertyField(addVelocityTemp, new GUIContent("Add Velocity"));

			if(useGPUfeaturesTemp.boolValue)
			{
			    EditorGUILayout.PropertyField(useVelocityMaskTextureTemp, new GUIContent("Use Velocity Mask", useVelocityMaskHelp));
			    
			    if(useVelocityMaskTextureTemp.boolValue)
		    	{
		    		if(!velocityMaskTextureTemp.hasMultipleDifferentValues)
		    		{
						velocityMaskTextureTemp.objectReferenceValue = EditorGUILayout.ObjectField("Velocity Mask Texture", velocityMaskTextureTemp.objectReferenceValue, typeof(Texture2D), false);
					}
					else
					{
						EditorGUILayout.LabelField("Velocity Mask Texture");
						EditorGUILayout.LabelField("                                Multiple");
						EditorGUILayout.LabelField("                                Texture");
						EditorGUILayout.LabelField("                                Types");
					}
				}
			}
			
			EditorGUILayout.Slider(velocityStrengthTemp, 0, 5, "Velocity Strength");
	    	EditorGUILayout.Slider(velocitySizeTemp, 0, 500, "Velocity Size");
			
			if(!useVelocityMaskTextureTemp.boolValue)
			{
				EditorGUILayout.Slider(velocityFalloffTemp, 0.01f, 1, new GUIContent("Velocity Falloff", velocityFalloffHelp));
			}
		}

		EditorGUILayout.Space();
		
    	if(GUI.changed)
		{
            EditorUtility.SetDirty(target);
    	}

    	serializedObject.ApplyModifiedProperties();
    }
}
