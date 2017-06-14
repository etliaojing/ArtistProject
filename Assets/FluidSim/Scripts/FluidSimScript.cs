//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using UnityEditor;

[AddComponentMenu("FluidSim/Fluid Sim")]
public class FluidSimScript : MonoBehaviour
{
    public RenderTexture fluidRenderTextureSource;
    private RenderTexture fluidRenderTextureDestination;
    private RenderTexture velocityRenderTextureSource;
    private RenderTexture velocityRenderTextureDestination;
    private RenderTexture pressureRenderTextureSource;
    private RenderTexture pressureRenderTextureDestination;
    private RenderTexture divergenceRenderTextureSource;
    private RenderTexture divergenceRenderTextureDestination;
    private RenderTexture collisionDynamicRenderTextureSource;
    private RenderTexture collisionDynamicRenderTextureDestination;
    private RenderTexture collisionStaticRenderTextureSource;
    private RenderTexture collisionStaticRenderTextureDestination;

    private RenderTexture unityProTestRenderTexture;

    public Texture2D fluidRenderTextureSourceFree;
    private Color32[] fluidRenderTextureSourceFreeTempSA;
    private int fluidRenderTextureSourceFreeTempSCX = 0;
    private int fluidRenderTextureSourceFreeTempSCY = 0;

    private Texture2D fluidRenderTextureDestinationFree;
    private Color32[] fluidRenderTextureDestinationFreeTempSA;
    //private int fluidRenderTextureDestinationFreeTempSCX = 0;
    //private int fluidRenderTextureDestinationFreeTempSCY = 0;

    private Texture2D velocityRenderTextureSourceFree;
    private Vector2[] velocityRenderTextureSourceFreeTempSA;
    private int velocityRenderTextureSourceFreeTempSCX = 0;
    private int velocityRenderTextureSourceFreeTempSCY = 0;

    private Texture2D velocityRenderTextureDestinationFree;
    private Vector2[] velocityRenderTextureDestinationFreeTempSA;
    //private int velocityRenderTextureDestinationFreeTempSCX = 0;
    //private int velocityRenderTextureDestinationFreeTempSCY = 0;

    private Texture2D pressureRenderTextureSourceFree;
    private float[] pressureRenderTextureSourceFreeTempSA;
    private int pressureRenderTextureSourceFreeTempSCX = 0;
    private int pressureRenderTextureSourceFreeTempSCY = 0;

    private Texture2D divergenceRenderTextureSourceFree;
    private float[] divergenceRenderTextureSourceFreeTempSA;
    private int divergenceRenderTextureSourceFreeTempSCX = 0;
    private int divergenceRenderTextureSourceFreeTempSCY = 0;

    private Texture2D collisionDynamicRenderTextureSourceFree;
    private float[] collisionDynamicRenderTextureSourceFreeTempSA;
    private Vector2[] collisionDynamicVelocityRenderTextureSourceFreeTempSA;
    private int collisionDynamicRenderTextureSourceFreeTempSCX = 0;
    private int collisionDynamicRenderTextureSourceFreeTempSCY = 0;

    private Texture2D collisionStaticRenderTextureSourceFree;
    private float[] collisionStaticRenderTextureSourceFreeTempSA;
    private int collisionStaticRenderTextureSourceFreeTempSCX = 0;
    private int collisionStaticRenderTextureSourceFreeTempSCY = 0;

    private float[] floatZeroRenderTextureSourceFreeTempSA;
    private float[] floatOneRenderTextureSourceFreeTempSA;
    private Vector2[] vector2ZeroRenderTextureSourceFreeTempSA;

    //private Color32[] placeholderColor32Array;

    public Color startingColor = Color.black;
    public Texture2D startingColorTextureSource;

    public Vector2 startingVelocity = new Vector2(0.0f, 0.0f);
    public Texture2D startingVelocityTextureSource;

    public Texture2D startingCollisionTextureSource;

    public Texture2D colorDissipationTextureSource;
    public Texture2D velocityDissipationTextureSource;

    private Texture2D tempStorageBufferTexture;
    private Color32[] tempStorageArray = new Color32[64];
    private int tempVelStorageCounter = 0;
    private int tempColorStorageCounter = 0;
    private int tempCollisionStorageCounter = 0;

    private Material impulseLocationMat;
    private Material impulseLocation4Mat;
    private Material impulseLocation8Mat;
    private Material impulseLocationTexMat;
    private Material impulseLocationVelMat;
    private Material impulseLocationVel4Mat;
    private Material impulseLocationVel8Mat;
    private Material impulseLocationVelTexMat;
    private Material advectionColorMat;
    private Material advectionColorTexMat;
    private Material advectionVelocityMat;
    private Material advectionVelocityTexMat;
    private Material divergenceMat;
    private Material jacobiRelaxMat;
    private Material gradientMat;
    private Material boundaryOpMat;
    private Material initializeToValueMat;
    private Material initializeCollisionToTextureMat;
    private Material initializeToTextureMat;
    private Material initializeVelToValueMat;
    private Material initializeVelToTextureMat;
    private Material setCollisionMat;
    private Material setCollision4Mat;
    private Material setCollision8Mat;
    private Material setCollisionTexMat;

    public fluidInfluenceClass[] fluidActorStaticArray;
    public fluidInfluenceClass[] fluidActorDynamicArray;

    private int i;
    private int j;
    private int k;
    private int m;
    private int n;

    private float timeStep = 0.02f;  //this value produces a good visual result, updating it at runtime isnt effective.
    public int fluidUpdateFPS = 25;  //"realtime" isnt an option because it would require timeStep to update each frame and timeStep changes per frame produced ugly results.
    public int impulseUpdateFPS = 25;
    public int colorUpdateFPS = 25;
    public int collisionUpdateFPS = 25;
    public int resolution = 64;
    private int resolutionHidden = 64;
    public int collisionResolution = 64;
    public int velocityResolution = 64;
    public int resolutionIndex = 1;
    public int colorResolutionIndex = 1;
    public int collisionResolutionIndex = 1;
    public int velocityResolutionIndex = 1;
    private float fluidrdx;
    private float collisionrdx;
    public float simStrength = 1.0f;
    public bool useVelocityDissipation = true;
    public float velocityDissipation = 0.001f;
    public bool useVelocityDissipationTexture = false;
    public float oldVelocityDissipation = 0.001f;
    public bool useColorDissipation = true;
    public float colorDissipation = 0.001f;
    public bool useColorDissipationTexture = false;
    public float oldColorDissipation = 0.001f;
    public Color colorDissipateTo;

    private Vector3 tempLocation;
    private Vector3 tempInfluenceLocation;
    private Vector3 tempVector;
    private float tempVelocityStrength;
    private float tempVelocitySize;
    private float tempCollisionSize;
    private float tempColorSize;
    private Color tempImpulseColor;

    private Transform myTransform;
    private Vector3 myTransformPosition;
    private Vector3 myTransformLocalScale;

    public int dynamicInputArrayLength;

    private Vector3 myBoundsSize;
    private float myBoundsMagnitude;

    private Vector3 tempPositionOffset;

    private Vector3 textureForwardDirection;
    private float textureRotationAngle;
    private Quaternion textureRotationQuaternion;
    private Matrix4x4 textureRotationMatrix;

    public bool useStartColorTexture = false;
    public bool useStartVelocityTexture = false;
    public bool useBoundaryCollision = true;
    public bool useStartCollisionTexture = false;

    private GameObject fluidConnectorObject;
    private FluidSimConnector fluidConnectorScript;

    public int pressureIteration = 20;

    private bool isTerrain = true;

    public int outputTextureNum = 1;
    private int outputTextureNumHidden = 1;

    public Material materialTarget;
    public string materialTextureSlot = "_MainTex";

    //private float cachedInfluenceActorCollisionSize = 0.0f;
    //private float cachedInfluenceActorVelocitySize = 0.0f;
    //private float cachedInfluenceActorColorSize = 0.0f;

    private Vector3 collisionVelocity;
    private Vector2 freeCollisionVelocity;

    private Mesh myMesh;

    private bool collisionCopied;

    private int frameDelay = 0;

    private bool updateInfluenceArray = false;

    private bool blitVelShader = false;
    private bool blitColorShader = false;
    private bool blitCollisionShader = false;

    private float kEncodeBit = 1.0f / 255.0f;
    private Vector2 enc2;
    private Vector2 kEncodeMul2 = new Vector2(1.0f, 255.0f);
    private Vector2 kDecodeDot2 = new Vector2(1.0f, 1.0f / 255.0f);

    private Color tempColor1;
    private Color32 tempCompressedColor1;
    private Color32 tempCompressedColor2;

    private float freeColorPositionX = 0.0f;
    private float freeColorPositionY = 0.0f;
    private float freeColorSize = 0.0f;
    private float freeColorFalloff = 0.0f;

    private float freeVelocityPositionX = 0.0f;
    private float freeVelocityPositionY = 0.0f;
    private float freeVelocityStrength = 0.0f;
    private float freeVelocityStrengthX = 0.0f;
    private float freeVelocityStrengthY = 0.0f;
    private float freeVelocitySize = 0.0f;
    private float freeVelocityFalloff = 0.0f;
    private float freeTempFloat = 0.0f;
    private float freeColorDissipation = 0.0f;
    private float freeVelocityDissipation = 0.0f;

    private float freeCollisionPositionX = 0.0f;
    private float freeCollisionPositionY = 0.0f;
    private float freeCollisionSize = 0.0f;
    private float freeCollisionFalloff = 0.0f;
    private float freeCollisionStrength = 0.0f;

    private Vector4 tempShaderVectorData;

    public bool updateColorWithFluid = true;
    private bool updateColorWithFluidHidden = true;
    public bool updateImpulseWithFluid = true;
    private bool updateImpulseWithFluidHidden = true;
    public bool updateCollisionWithFluid = true;
    private bool updateCollisionWithFluidHidden = true;

    private float freeColorCalcAlpha = 0.0f;
    private int tempColorPixelCount = 0;

    private float freePosX = 0.0f;
    private float freePosY = 0.0f;
    private int freeTempInt = 0;

    private Color[] tempColorArray;

    private int resolutionMin1 = 0;
    private int resolution2Min1 = 0;
    private int resolution2 = 0;

    private float tempfloatX01 = 0.0f;
    private float tempfloatX02 = 0.0f;
    private float tempfloatX03 = 0.0f;
    private float tempfloatX04 = 0.0f;
    private float tempfloatY01 = 0.0f;
    private float tempfloatY02 = 0.0f;
    private float tempfloatY03 = 0.0f;
    private float tempfloatY04 = 0.0f;
    private float tempfloatZ01 = 0.0f;
    private float tempfloatZ02 = 0.0f;
    private float tempfloatZ03 = 0.0f;
    private float tempfloatZ04 = 0.0f;
    private float blendA = 0.0f;
    private float blendB = 0.0f;
    private float outputX = 0.0f;
    private float outputY = 0.0f;
    private float outputZ = 0.0f;

    private float freeUVx = 0.0f;
    private float freeUVy = 0.0f;

    private float uv1x = 0.0f;
    private float uv2x = 0.0f;
    private float uv3x = 0.0f;
    private float uv4x = 0.0f;

    private int targetPixelX = 0;
    private int targetPixelY = 0;

    private float tempFracX = 0.0f;
    private float tempFracY = 0.0f;

    private float vL;
    private float vR;
    private float vB;
    private float vT;

    private float vLx = 0.0f;
    private float vRx = 0.0f;
    private float vBx = 0.0f;
    private float vTx = 0.0f;

    //private float xC = 0.0f;
    private float xL = 0.0f;
    private float xR = 0.0f;
    private float xB = 0.0f;
    private float xT = 0.0f;
    private float bC = 0.0f;

    private float pL;
    private float pR;
    private float pB;
    private float pT;
    private float pC;

    private Vector3 vMask = Vector3.one;
    private Vector3 obstV = Vector3.zero;
    private Vector3 vMaskOne = Vector3.one;
    private Vector3 obstVZero = Vector3.zero;

    private Vector3 vector3Zero = new Vector3(0, 0, 0);
    private Vector2 vector2Zero = new Vector2(0, 0);

    private Vector3 tempVector3;
    private Vector2 tempVector2;
    private Vector2 freeTempVelocity;
    private float tempCurrentTime = 0.0f;

    private Color tempColor;

    private int freeMinX = 0;
    private int freeMinY = 0;
    private int freeMaxX = 0;
    private int freeMaxY = 0;

    private Color32 color32Zero = new Color32(0, 0, 0, 0);

    public bool useMyMaterial = true;

    public bool useUnityProMethod = true;

    public enum uniqueResOptionsData
    {
        _32x32 = 0,
        _64x64 = 1,
        _128x128 = 2,
        _256x256 = 3,
        _512x512 = 4,
        _1024x1024 = 5
    }

    public uniqueResOptionsData uniqueCollisionResOptions = uniqueResOptionsData._64x64;
    public uniqueResOptionsData uniqueColorResOptions = uniqueResOptionsData._64x64;
    public uniqueResOptionsData uniqueVelocityResOptions = uniqueResOptionsData._64x64;

    public enum resOptionsData
    {
        _32x32 = 0,
        _64x64 = 1,
        _128x128 = 2,
        _256x256 = 3,
        _512x512 = 4,
        _1024x1024 = 5,
        Unique_Per_Type = 6
    }

    public resOptionsData resOptions = resOptionsData._64x64;

    public enum outputTextureData
    {
        Collision_Buffer = 0,
        Color_Buffer = 1,
        Velocity_Buffer = 2
    }

    public outputTextureData outputTexture = outputTextureData.Color_Buffer;

    //========

    void Awake()
    {
        if (GameObject.Find("dynamiclyCreatedFluidSimConnector"))
        {
            fluidConnectorObject = GameObject.Find("dynamiclyCreatedFluidSimConnector");

            fluidConnectorScript = fluidConnectorObject.GetComponent<FluidSimConnector>();

            fluidConnectorScript.RegisterFluidActor(this);
        }
        else
        {
            fluidConnectorObject = new GameObject();

            fluidConnectorObject.name = "dynamiclyCreatedFluidSimConnector";

            fluidConnectorScript = fluidConnectorObject.AddComponent<FluidSimConnector>();

            fluidConnectorScript.RegisterFluidActor(this);
        }

        if (fluidConnectorObject == null)
        {
            Debug.LogError("FluidSimScript failed to find or create a FluidConnector object.  Make sure the FluidConnector script exists and can by found by the FluidSimScript.");
        }

        if (fluidConnectorScript == null)
        {
            Debug.LogError("FluidSimScript failed to find or create a FluidConnector script.  Make sure the FluidConnector script exists and can by found by the FluidSimScript.");
        }
    }

    //========

    void Start()
    {
        myTransform = transform;
        myTransformPosition = transform.position;

        myMesh = GetComponent<MeshFilter>().mesh;

        unityProTestRenderTexture = new RenderTexture(4, 4, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        unityProTestRenderTexture.Create();

        if (!unityProTestRenderTexture.IsCreated())
        {
            useUnityProMethod = false;

            Debug.LogWarning("RenderTextures failed to create.  RenderTextures are only available in Unity Pro and are required for FluidSim to work on the GPU.  FluidSim will use the non-RenderTexture method on the CPU instead.");
        }
        else
        {
            unityProTestRenderTexture.Release();
        }

        //Prevents bugs related to unexpected resolution changes and output texture changes.  Resolution and output texture can only be changed/set before a FluidSim is active.
        if (useUnityProMethod)
        {
            resolutionHidden = resolution;
        }
        else
        {
            //This is also done in the editor interface script.  We do it here incase usePro is turned on, but Unity couldnt create the render texture correctly.
            switch (uniqueColorResOptions)
            {
                case uniqueResOptionsData._32x32:
                    resolutionHidden = 32;
                    break;
                case uniqueResOptionsData._64x64:
                    resolutionHidden = 64;
                    break;
                case uniqueResOptionsData._128x128:
                    resolutionHidden = 128;
                    break;
                case uniqueResOptionsData._256x256:
                    resolutionHidden = 256;
                    break;
                case uniqueResOptionsData._512x512:
                    resolutionHidden = 512;
                    break;
                case uniqueResOptionsData._1024x1024:
                    resolutionHidden = 1024;
                    break;
            }
        }

        outputTextureNumHidden = outputTextureNum;

        if (useUnityProMethod)
        {
            //create a temp camera
            Camera tempCameraComp;
            GameObject tempCameraObject = Instantiate(Camera.main.gameObject, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
            tempCameraComp = tempCameraObject.GetComponent<Camera>();

            //render texture setup
            fluidRenderTextureSource = new RenderTexture(resolutionHidden, resolutionHidden, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            fluidRenderTextureSource.wrapMode = TextureWrapMode.Clamp;
            fluidRenderTextureSource.Create();
            tempCameraComp.targetTexture = fluidRenderTextureSource;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            fluidRenderTextureDestination = new RenderTexture(resolutionHidden, resolutionHidden, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            fluidRenderTextureDestination.wrapMode = TextureWrapMode.Clamp;
            fluidRenderTextureDestination.Create();
            tempCameraComp.targetTexture = fluidRenderTextureDestination;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            velocityRenderTextureSource = new RenderTexture(velocityResolution, velocityResolution, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            velocityRenderTextureSource.useMipMap = false;
            velocityRenderTextureSource.filterMode = FilterMode.Bilinear;
            velocityRenderTextureSource.wrapMode = TextureWrapMode.Clamp;
            velocityRenderTextureSource.Create();
            tempCameraComp.targetTexture = velocityRenderTextureSource;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            velocityRenderTextureDestination = new RenderTexture(velocityResolution, velocityResolution, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            velocityRenderTextureDestination.useMipMap = false;
            velocityRenderTextureDestination.filterMode = FilterMode.Bilinear;
            velocityRenderTextureDestination.wrapMode = TextureWrapMode.Clamp;
            velocityRenderTextureDestination.Create();
            tempCameraComp.targetTexture = velocityRenderTextureDestination;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            pressureRenderTextureSource = new RenderTexture(resolutionHidden, resolutionHidden, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            pressureRenderTextureSource.useMipMap = false;
            pressureRenderTextureSource.filterMode = FilterMode.Bilinear;
            pressureRenderTextureSource.wrapMode = TextureWrapMode.Clamp;
            pressureRenderTextureSource.Create();
            tempCameraComp.targetTexture = pressureRenderTextureSource;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            pressureRenderTextureDestination = new RenderTexture(resolutionHidden, resolutionHidden, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            pressureRenderTextureDestination.useMipMap = false;
            pressureRenderTextureDestination.filterMode = FilterMode.Bilinear;
            pressureRenderTextureDestination.wrapMode = TextureWrapMode.Clamp;
            pressureRenderTextureDestination.Create();
            tempCameraComp.targetTexture = pressureRenderTextureDestination;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            divergenceRenderTextureSource = new RenderTexture(resolutionHidden, resolutionHidden, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            divergenceRenderTextureSource.useMipMap = false;
            divergenceRenderTextureSource.filterMode = FilterMode.Bilinear;
            divergenceRenderTextureSource.wrapMode = TextureWrapMode.Clamp;
            divergenceRenderTextureSource.Create();
            tempCameraComp.targetTexture = divergenceRenderTextureSource;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            divergenceRenderTextureDestination = new RenderTexture(resolutionHidden, resolutionHidden, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            divergenceRenderTextureDestination.useMipMap = false;
            divergenceRenderTextureDestination.filterMode = FilterMode.Bilinear;
            divergenceRenderTextureDestination.wrapMode = TextureWrapMode.Clamp;
            divergenceRenderTextureDestination.Create();
            tempCameraComp.targetTexture = divergenceRenderTextureDestination;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            collisionDynamicRenderTextureSource = new RenderTexture(collisionResolution, collisionResolution, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            collisionDynamicRenderTextureSource.useMipMap = false;
            collisionDynamicRenderTextureSource.filterMode = FilterMode.Bilinear;
            collisionDynamicRenderTextureSource.wrapMode = TextureWrapMode.Clamp;
            collisionDynamicRenderTextureSource.Create();
            tempCameraComp.targetTexture = collisionDynamicRenderTextureSource;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            collisionDynamicRenderTextureDestination = new RenderTexture(collisionResolution, collisionResolution, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            collisionDynamicRenderTextureDestination.useMipMap = false;
            collisionDynamicRenderTextureDestination.filterMode = FilterMode.Bilinear;
            collisionDynamicRenderTextureDestination.wrapMode = TextureWrapMode.Clamp;
            collisionDynamicRenderTextureDestination.Create();
            tempCameraComp.targetTexture = collisionDynamicRenderTextureDestination;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            collisionStaticRenderTextureSource = new RenderTexture(collisionResolution, collisionResolution, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            collisionStaticRenderTextureSource.useMipMap = false;
            collisionStaticRenderTextureSource.filterMode = FilterMode.Bilinear;
            collisionStaticRenderTextureSource.wrapMode = TextureWrapMode.Clamp;
            collisionStaticRenderTextureSource.Create();
            tempCameraComp.targetTexture = collisionStaticRenderTextureSource;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            collisionStaticRenderTextureDestination = new RenderTexture(collisionResolution, collisionResolution, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            collisionStaticRenderTextureDestination.useMipMap = false;
            collisionStaticRenderTextureDestination.filterMode = FilterMode.Bilinear;
            collisionStaticRenderTextureDestination.wrapMode = TextureWrapMode.Clamp;
            collisionStaticRenderTextureDestination.Create();
            tempCameraComp.targetTexture = collisionStaticRenderTextureDestination;
            tempCameraComp.Render();
            tempCameraComp.targetTexture = null;

            //release temp files
            tempCameraComp = null;
            Destroy(tempCameraObject);
        }

        if (useUnityProMethod)
        {
            //material instance safety checks and setup
            if (!Shader.Find("FluidSim/impulseLocationShader"))
            {
                Debug.LogError("FluidSim/impulseLocationShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                impulseLocationMat = new Material(Shader.Find("FluidSim/impulseLocationShader"));
            }
            if (!Shader.Find("FluidSim/impulseLocation4Shader"))
            {
                Debug.LogError("FluidSim/impulseLocation4Shader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                impulseLocation4Mat = new Material(Shader.Find("FluidSim/impulseLocation4Shader"));
            }
            if (!Shader.Find("FluidSim/impulseLocation8Shader"))
            {
                Debug.LogError("FluidSim/impulseLocation8Shader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                impulseLocation8Mat = new Material(Shader.Find("FluidSim/impulseLocation8Shader"));
            }
            if (!Shader.Find("FluidSim/impulseLocationTexShader"))
            {
                Debug.LogError("FluidSim/impulseLocationTexShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                impulseLocationTexMat = new Material(Shader.Find("FluidSim/impulseLocationTexShader"));
            }
            if (!Shader.Find("FluidSim/impulseLocationVelShader"))
            {
                Debug.LogError("FluidSim/impulseLocationVelShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                impulseLocationVelMat = new Material(Shader.Find("FluidSim/impulseLocationVelShader"));
            }
            if (!Shader.Find("FluidSim/impulseLocationVel4Shader"))
            {
                Debug.LogError("FluidSim/impulseLocationVel4Shader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                impulseLocationVel4Mat = new Material(Shader.Find("FluidSim/impulseLocationVel4Shader"));
            }
            if (!Shader.Find("FluidSim/impulseLocationVel8Shader"))
            {
                Debug.LogError("FluidSim/impulseLocationVel8Shader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                impulseLocationVel8Mat = new Material(Shader.Find("FluidSim/impulseLocationVel8Shader"));
            }
            if (!Shader.Find("FluidSim/impulseLocationVelTexShader"))
            {
                Debug.LogError("FluidSim/impulseLocationVelTexShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                impulseLocationVelTexMat = new Material(Shader.Find("FluidSim/impulseLocationVelTexShader"));
            }
            if (!Shader.Find("FluidSim/advectionColorShader"))
            {
                Debug.LogError("FluidSim/advectionColorShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                advectionColorMat = new Material(Shader.Find("FluidSim/advectionColorShader"));
            }
            if (!Shader.Find("FluidSim/advectionColorTexShader"))
            {
                Debug.LogError("FluidSim/advectionColorTexShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                advectionColorTexMat = new Material(Shader.Find("FluidSim/advectionColorTexShader"));
            }
            if (!Shader.Find("FluidSim/advectionVelocityShader"))
            {
                Debug.LogError("FluidSim/advectionVelocityShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                advectionVelocityMat = new Material(Shader.Find("FluidSim/advectionVelocityShader"));
            }
            if (!Shader.Find("FluidSim/advectionVelocityTexShader"))
            {
                Debug.LogError("FluidSim/advectionVelocityTexShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                advectionVelocityTexMat = new Material(Shader.Find("FluidSim/advectionVelocityTexShader"));
            }
            if (!Shader.Find("FluidSim/divergenceShader"))
            {
                Debug.LogError("FluidSim/divergenceShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                divergenceMat = new Material(Shader.Find("FluidSim/divergenceShader"));
            }
            if (!Shader.Find("FluidSim/jacobiRelaxShader"))
            {
                Debug.LogError("FluidSim/jacobiRelaxShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                jacobiRelaxMat = new Material(Shader.Find("FluidSim/jacobiRelaxShader"));
            }
            if (!Shader.Find("FluidSim/gradientShader"))
            {
                Debug.LogError("FluidSim/gradientShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                gradientMat = new Material(Shader.Find("FluidSim/gradientShader"));
            }
            if (!Shader.Find("FluidSim/boundaryOpShader"))
            {
                Debug.LogError("FluidSim/boundaryOpShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                boundaryOpMat = new Material(Shader.Find("FluidSim/boundaryOpShader"));
            }
            if (!Shader.Find("FluidSim/initializeToValueShader"))
            {
                Debug.LogError("FluidSim/initializeToValueShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                initializeToValueMat = new Material(Shader.Find("FluidSim/initializeToValueShader"));
            }
            if (!Shader.Find("FluidSim/initializeCollisionToTextureShader"))
            {
                Debug.LogError("FluidSim/initializeCollisionToTextureShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                initializeCollisionToTextureMat = new Material(Shader.Find("FluidSim/initializeCollisionToTextureShader"));
            }
            if (!Shader.Find("FluidSim/initializeToTextureShader"))
            {
                Debug.LogError("FluidSim/initializeToTextureShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                initializeToTextureMat = new Material(Shader.Find("FluidSim/initializeToTextureShader"));
            }
            if (!Shader.Find("FluidSim/initializeVelToValueShader"))
            {
                Debug.LogError("FluidSim/initializeVelToValueShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                initializeVelToValueMat = new Material(Shader.Find("FluidSim/initializeVelToValueShader"));
            }
            if (!Shader.Find("FluidSim/initializeVelToTextureShader"))
            {
                Debug.LogError("FluidSim/initializeVelToTextureShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                initializeVelToTextureMat = new Material(Shader.Find("FluidSim/initializeVelToTextureShader"));
            }
            if (!Shader.Find("FluidSim/setCollisionShader"))
            {
                Debug.LogError("FluidSim/setCollisionShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                setCollisionMat = new Material(Shader.Find("FluidSim/setCollisionShader"));
            }
            if (!Shader.Find("FluidSim/setCollision4Shader"))
            {
                Debug.LogError("FluidSim/setCollision4Shader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                setCollision4Mat = new Material(Shader.Find("FluidSim/setCollision4Shader"));
            }
            if (!Shader.Find("FluidSim/setCollision8Shader"))
            {
                Debug.LogError("FluidSim/setCollision8Shader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                setCollision8Mat = new Material(Shader.Find("FluidSim/setCollision8Shader"));
            }
            if (!Shader.Find("FluidSim/setCollisionTexShader"))
            {
                Debug.LogError("FluidSim/setCollisionTexShader could not be found.  Fluids cannot be simulated without this shader.");
            }
            else
            {
                setCollisionTexMat = new Material(Shader.Find("FluidSim/setCollisionTexShader"));
            }
        }

        if (!useUnityProMethod)
        {
            resolutionMin1 = resolutionHidden - 1;
            resolution2Min1 = (resolutionHidden * resolutionHidden) - 1;
            resolution2 = resolutionHidden * resolutionHidden;

            fluidRenderTextureSourceFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
            fluidRenderTextureSourceFree.wrapMode = TextureWrapMode.Clamp;
            fluidRenderTextureSourceFreeTempSA = new Color32[resolution2];

            fluidRenderTextureDestinationFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
            fluidRenderTextureDestinationFree.wrapMode = TextureWrapMode.Clamp;
            fluidRenderTextureDestinationFreeTempSA = new Color32[resolution2];

            velocityRenderTextureSourceFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
            velocityRenderTextureSourceFree.wrapMode = TextureWrapMode.Clamp;
            velocityRenderTextureSourceFreeTempSA = new Vector2[resolution2];

            velocityRenderTextureDestinationFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
            velocityRenderTextureDestinationFree.wrapMode = TextureWrapMode.Clamp;
            velocityRenderTextureDestinationFreeTempSA = new Vector2[resolution2];

            pressureRenderTextureSourceFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
            pressureRenderTextureSourceFree.wrapMode = TextureWrapMode.Clamp;
            pressureRenderTextureSourceFreeTempSA = new float[resolution2];

            divergenceRenderTextureSourceFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
            divergenceRenderTextureSourceFree.wrapMode = TextureWrapMode.Clamp;
            divergenceRenderTextureSourceFreeTempSA = new float[resolution2];

            collisionDynamicRenderTextureSourceFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
            collisionDynamicRenderTextureSourceFree.wrapMode = TextureWrapMode.Clamp;
            collisionDynamicRenderTextureSourceFreeTempSA = new float[resolution2];
            collisionDynamicVelocityRenderTextureSourceFreeTempSA = new Vector2[resolution2];

            collisionStaticRenderTextureSourceFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
            collisionStaticRenderTextureSourceFree.wrapMode = TextureWrapMode.Clamp;
            collisionStaticRenderTextureSourceFreeTempSA = new float[resolution2];

            floatZeroRenderTextureSourceFreeTempSA = new float[resolution2];
            floatOneRenderTextureSourceFreeTempSA = new float[resolution2];
            vector2ZeroRenderTextureSourceFreeTempSA = new Vector2[resolution2];

            for (i = 0; i < resolution2; i++)
            {
                floatZeroRenderTextureSourceFreeTempSA[i] = 0.0f;
                floatOneRenderTextureSourceFreeTempSA[i] = 1.0f;
                vector2ZeroRenderTextureSourceFreeTempSA[i] = Vector2.zero;
            }

            //placeholderColor32Array = new Color32[resolutionHidden * resolutionHidden];

            if (tempColorArray == null)
            {
                tempColorArray = new Color[resolutionHidden * resolutionHidden];
            }
        }

        if (useUnityProMethod)
        {
            tempStorageBufferTexture = new Texture2D(16, 4, TextureFormat.ARGB32, false, true);
            tempStorageBufferTexture.filterMode = FilterMode.Point;
            tempStorageBufferTexture.wrapMode = TextureWrapMode.Clamp;
        }

        fluidrdx = 1.0f / resolutionHidden;
        collisionrdx = 1.0f / collisionResolution;

        if (useColorDissipation)
        {
            colorDissipation = oldColorDissipation;
        }
        else
        {
            colorDissipation = 0;
        }
        if (useVelocityDissipation)
        {
            velocityDissipation = oldVelocityDissipation;
        }
        else
        {
            velocityDissipation = 0;
        }

        if (gameObject.GetComponent<Terrain>())
        {
            isTerrain = true;
            Debug.LogError("Fluid Script doesn't support processing on Terrain.  Process Fluid on a GameObject with UVs and apply the results texture to the Terrain.");
        }
        else
        {
            isTerrain = false;

            if (materialTarget == null)
            {
                materialTarget = Resources.Load("FluidSimSimpleMat") as Material;

                if (materialTarget == null)
                {
                    Debug.LogError("Material Target is null and Unity failed to find \"FluidSimSimpleMat\".  Make sure FluidSimSimpleMat can be found, or assign a Material Target to the FluidSim.");
                }
            }

            if (useMyMaterial)
            {
                materialTarget = GetComponent<Renderer>().material;
            }

            if (materialTarget)
            {
                if (!materialTarget.HasProperty(materialTextureSlot))
                {
                    Debug.LogError("The Material Texture Slot entered in FluidSim could not be found in the material FluidSim is currently trying to use.  Make sure the texture slot name is correct and the material being used has that slot name.");
                }
            }
            else
            {
                Debug.LogError("Material Target is null on FluidSim.  Please make sure FluidSim has a material target set, or the material FluidSimSimpleMat is in the resources folder.");
            }

            if (useUnityProMethod)
            {
                switch (outputTextureNumHidden)
                {
                    case 0:
                        materialTarget.SetTexture(materialTextureSlot, collisionDynamicRenderTextureSource);
                        break;
                    case 1:
                        materialTarget.SetTexture(materialTextureSlot, fluidRenderTextureSource);
                        break;
                    case 2:
                        materialTarget.SetTexture(materialTextureSlot, velocityRenderTextureSource);
                        break;
                }
            }
            else
            {
                switch (outputTextureNumHidden)
                {
                    case 0:
                        materialTarget.SetTexture(materialTextureSlot, collisionDynamicRenderTextureSourceFree);
                        break;
                    case 1:
                        materialTarget.SetTexture(materialTextureSlot, fluidRenderTextureSourceFree);
                        break;
                    case 2:
                        materialTarget.SetTexture(materialTextureSlot, velocityRenderTextureSourceFree);
                        break;
                }
            }

            myBoundsSize = myMesh.bounds.size;
            myTransformLocalScale = myTransform.localScale;
            myBoundsSize = new Vector3(myBoundsSize.x * myTransformLocalScale.x, myBoundsSize.y * myTransformLocalScale.y, myBoundsSize.z * myTransformLocalScale.z);
            myBoundsMagnitude = myBoundsSize.magnitude;
        }

        if (useUnityProMethod)
        {
            if (startingColorTextureSource && useStartColorTexture)
            {
                initializeToTextureMat.SetTexture("initialTexture", startingColorTextureSource);
                Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, initializeToTextureMat);
                Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
            }
            else
            {
                initializeToValueMat.SetVector("initialValue", startingColor);
                Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, initializeToValueMat);
                Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
            }

            if (startingVelocityTextureSource && useStartVelocityTexture)
            {
                initializeVelToTextureMat.SetTexture("initialTexture", startingVelocityTextureSource);
                Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, initializeVelToTextureMat);
                Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
            }
            else
            {
                initializeVelToValueMat.SetVector("initialValue", startingVelocity);
                Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, initializeVelToValueMat);
                Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
            }

            if (startingCollisionTextureSource && useStartCollisionTexture)
            {
                initializeCollisionToTextureMat.SetTexture("initialTexture", startingCollisionTextureSource);
                Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, initializeCollisionToTextureMat);
                Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
                initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
                Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, initializeToValueMat);
                Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);
            }
            else
            {
                initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
                Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, initializeToValueMat);
                Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
                Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, initializeToValueMat);
                Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);
            }

            initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            Graphics.Blit(divergenceRenderTextureSource, divergenceRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(divergenceRenderTextureDestination, divergenceRenderTextureSource);

            initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            Graphics.Blit(pressureRenderTextureSource, pressureRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(pressureRenderTextureDestination, pressureRenderTextureSource);

            impulseLocationVelMat.SetTexture("velTex", velocityRenderTextureSource);
            impulseLocationVelMat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            impulseLocationVelMat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            impulseLocationVel4Mat.SetTexture("velTex", velocityRenderTextureSource);
            impulseLocationVel4Mat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            impulseLocationVel4Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            impulseLocationVel8Mat.SetTexture("velTex", velocityRenderTextureSource);
            impulseLocationVel8Mat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            impulseLocationVel8Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            impulseLocationVelTexMat.SetTexture("velTex", velocityRenderTextureSource);

            impulseLocationMat.SetTexture("fluidTex", fluidRenderTextureSource);
            impulseLocationMat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            impulseLocationMat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            impulseLocation4Mat.SetTexture("fluidTex", fluidRenderTextureSource);
            impulseLocation4Mat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            impulseLocation4Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            impulseLocation8Mat.SetTexture("fluidTex", fluidRenderTextureSource);
            impulseLocation8Mat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            impulseLocation8Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            impulseLocationTexMat.SetTexture("fluidTex", fluidRenderTextureSource);

            advectionColorMat.SetTexture("velocityTexSource", velocityRenderTextureSource);
            advectionColorMat.SetTexture("targetTex", fluidRenderTextureSource);
            advectionColorMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            advectionColorMat.SetFloat("timeStep", timeStep);
            advectionColorMat.SetFloat("dissipation", Mathf.Lerp(0.0f, 0.05f, colorDissipation));
            advectionColorMat.SetVector("colorDissipateTo", colorDissipateTo);
            advectionColorMat.SetFloat("simSpeed", simStrength);

            advectionColorTexMat.SetTexture("velocityTexSource", velocityRenderTextureSource);
            advectionColorTexMat.SetTexture("targetTex", fluidRenderTextureSource);
            advectionColorTexMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            advectionColorTexMat.SetFloat("timeStep", timeStep);
            advectionColorTexMat.SetTexture("dissipationTex", colorDissipationTextureSource);
            advectionColorTexMat.SetFloat("simSpeed", simStrength);

            advectionVelocityMat.SetTexture("velocityTexSource", velocityRenderTextureSource);
            advectionVelocityMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            advectionVelocityMat.SetTexture("targetTex", velocityRenderTextureSource);
            advectionVelocityMat.SetFloat("timeStep", timeStep);
            advectionVelocityMat.SetFloat("dissipation", Mathf.Lerp(1.0f, 0.5f, velocityDissipation));
            advectionVelocityMat.SetFloat("simSpeed", simStrength);

            advectionVelocityTexMat.SetTexture("velocityTexSource", velocityRenderTextureSource);
            advectionVelocityTexMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            advectionVelocityTexMat.SetTexture("targetTex", velocityRenderTextureSource);
            advectionVelocityTexMat.SetFloat("timeStep", timeStep);
            advectionVelocityTexMat.SetTexture("dissipationTex", velocityDissipationTextureSource);
            advectionVelocityTexMat.SetFloat("simSpeed", simStrength);

            divergenceMat.SetTexture("velocityTex", velocityRenderTextureSource);
            divergenceMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            divergenceMat.SetFloat("rdx", fluidrdx);

            jacobiRelaxMat.SetTexture("pressureTex", pressureRenderTextureSource);
            jacobiRelaxMat.SetTexture("divergenceTex", divergenceRenderTextureSource);
            jacobiRelaxMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            jacobiRelaxMat.SetFloat("rdx", fluidrdx);

            gradientMat.SetTexture("pressureTex", pressureRenderTextureSource);
            gradientMat.SetTexture("velocityTex", velocityRenderTextureSource);
            gradientMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            gradientMat.SetFloat("rdx", fluidrdx);

            boundaryOpMat.SetTexture("targetTex", collisionStaticRenderTextureSource);
            boundaryOpMat.SetVector("setColor", new Vector4(0, 0, 0, 0));
            boundaryOpMat.SetFloat("rdx", collisionrdx);

            setCollisionMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            setCollisionMat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            setCollisionMat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            setCollision4Mat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            setCollision4Mat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            setCollision4Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            setCollision8Mat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
            setCollision8Mat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
            setCollision8Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

            setCollisionTexMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
        }

        if (useBoundaryCollision)
        {
            if (useUnityProMethod)
            {
                Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, boundaryOpMat);
                Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
            }
            else
            {
                boundaryOpMatFree(true);
            }
        }
        else
        {
            if (!useUnityProMethod)
            {
                boundaryOpMatFree(false);
            }
        }

        if (!useUnityProMethod)
        {
            //clear and setup color and velocity fields
            if (useStartColorTexture)
            {
                initializeToTextureMatFree(startingColorTextureSource);
            }
            else
            {
                initializeToValueMatFree(startingColor);
            }

            if (useStartVelocityTexture)
            {
                if (startingVelocityTextureSource)
                {
                    initializeVelToTextureMatFree(startingVelocityTextureSource);
                }
                else
                {
                    Debug.LogError("A Starting Velocity Texture was not supplied for the Fluid Script to use.  Either turn off the Use Start Velocity boolean, or supply a texture to be used.");
                }
            }
            else
            {
                initializeVelToValueMatFree();
            }

            if (startingCollisionTextureSource && useStartCollisionTexture)
            {
                initializeCollisionToValueMatFree();
            }
        }

        updateColorWithFluidHidden = updateColorWithFluid;
        updateImpulseWithFluidHidden = updateImpulseWithFluid;
        updateCollisionWithFluidHidden = updateCollisionWithFluid;

        //A delayed call on static collision setup so that all of the collision actors can "report in" first.
        if (useUnityProMethod)
        {
            Invoke("UpdateStaticCollision", 0.5f);

            if (!updateCollisionWithFluidHidden)
            {
                if (collisionUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateDynamicCollision", 0.1f, 1f / collisionUpdateFPS);
                }
            }

            if (!updateImpulseWithFluidHidden)
            {
                if (impulseUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateImpulseActors", 0.11f, 1f / impulseUpdateFPS);
                }
            }

            if (!updateColorWithFluidHidden)
            {
                if (colorUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateColorActors", 0.12f, 1f / colorUpdateFPS);
                }
            }

            if (fluidUpdateFPS > 0)
            {
                InvokeRepeating("UpdateFluid", 0.13f, 1f / fluidUpdateFPS);
            }
        }
        else
        {
            Invoke("UpdateStaticCollisionFree", 0.5f);

            if (!updateCollisionWithFluidHidden)
            {
                if (collisionUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateDynamicCollisionFree", 0.1f, 1f / collisionUpdateFPS);
                }
            }

            if (!updateImpulseWithFluidHidden)
            {
                if (impulseUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateImpulseActorsFree", 0.11f, 1f / impulseUpdateFPS);
                }
            }

            if (!updateColorWithFluidHidden)
            {
                if (colorUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateColorActorsFree", 0.12f, 1f / colorUpdateFPS);
                }
            }

            if (fluidUpdateFPS > 0)
            {
                InvokeRepeating("UpdateFluidFree", 0.13f, 1f / fluidUpdateFPS);
            }
        }
    }

    //========

    void UpdateStaticCollision()
    {
        fluidConnectorScript.GetActorArrayUpdate(this);

        if (fluidActorStaticArray.Length > 0)
        {
            setCollisionTexMat.SetTexture("collisionTex", collisionStaticRenderTextureSource);

            if (fluidActorStaticArray.Length <= 4)
            {
                setCollision4Mat.SetTexture("collisionTex", collisionStaticRenderTextureSource);
            }
            if (fluidActorStaticArray.Length <= 8)
            {
                setCollision8Mat.SetTexture("collisionTex", collisionStaticRenderTextureSource);
            }
            else
            {
                setCollisionMat.SetTexture("collisionTex", collisionStaticRenderTextureSource);
            }

        }

        //set collision for each collision input object
        for (k = 0; k < fluidConnectorScript.staticArrayCount; k++)
        {
            if (fluidActorStaticArray[k] != null)
            {
                if (fluidActorStaticArray[k].staticCollision)
                {
                    if (fluidActorStaticArray[k].multiplySizeByScale)
                    {
                        tempCollisionSize = fluidActorStaticArray[k].collisionSize * (fluidActorStaticArray[k].myTransform.localScale.magnitude * 0.577f);
                    }
                    else
                    {
                        tempCollisionSize = fluidActorStaticArray[k].collisionSize;
                    }

                    if ((fluidActorStaticArray[k].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525) + tempCollisionSize)
                    {
                        if (fluidActorStaticArray[k].useCollisionMaskTexture && fluidActorStaticArray[k].collisionMaskTexture)
                        {
                            tempLocation = myTransform.InverseTransformPoint(fluidActorStaticArray[k].myTransform.position);
                            //locationX, locationY
                            tempShaderVectorData.x = ((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1) + 1.0f) * 0.5f;
                            tempShaderVectorData.y = ((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1) + 1.0f) * 0.5f;
                            tempShaderVectorData.z = 0;
                            tempShaderVectorData.w = 0;
                            setCollisionTexMat.SetVector("textureData", tempShaderVectorData);
                            setCollisionTexMat.SetFloat("collisionStrength", fluidActorStaticArray[k].collisionStrength);
                            setCollisionTexMat.SetTexture("collisionTexMask", fluidActorStaticArray[k].collisionMaskTexture);
                            textureRotationAngle = Vector3.Angle(fluidActorStaticArray[k].myTransform.forward, myTransform.forward);
                            if (Vector3.Dot(fluidActorStaticArray[k].myTransform.forward, myTransform.right) < 0)
                            {
                                textureRotationAngle *= -1;
                            }
                            textureRotationQuaternion = Quaternion.Euler(0, 0, textureRotationAngle);
                            textureRotationMatrix = Matrix4x4.TRS(vector2Zero, textureRotationQuaternion, new Vector2(1f / (1.8f * (tempCollisionSize / myBoundsSize.x)), 1f / (1.8f * (tempCollisionSize / myBoundsSize.z))));
                            setCollisionTexMat.SetMatrix("rotationMatrix", textureRotationMatrix);

                            Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, setCollisionTexMat);
                            Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
                        }
                        else
                        {
                            tempInfluenceLocation = fluidActorStaticArray[k].myTransform.position;
                            tempLocation = myTransform.InverseTransformPoint(tempInfluenceLocation);
                            //positionX positionY
                            tempCompressedColor1 = EncodeFloatRG(((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1) + 1.0f) * 0.5f);
                            tempCompressedColor2 = EncodeFloatRG(((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1) + 1.0f) * 0.5f);
                            tempStorageArray[tempCollisionStorageCounter] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, tempCompressedColor2.g);
                            //collision is static and should have no velocity.  127.5 is "zero" because the shader does (*50 - 25) and is calculated out of 255, but it wont take that as a value.
                            tempStorageArray[tempCollisionStorageCounter + 16] = new Color32(127, 127, 0, 0);
                            //collisionSize, collisionFalloff, collisionStrength
                            tempCompressedColor1 = EncodeFloatRG((tempCollisionSize / myBoundsSize.x) * 0.5f);
                            tempCompressedColor2.r = (byte)(fluidActorStaticArray[k].collisionFalloff * 255);
                            tempCompressedColor2.g = (byte)(fluidActorStaticArray[k].collisionStrength * 255);
                            tempStorageArray[tempCollisionStorageCounter + 32] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, tempCompressedColor2.g);

                            tempCollisionStorageCounter++;

                            if (tempCollisionStorageCounter > 15)
                            {
                                tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
                                tempStorageBufferTexture.Apply(false);

                                Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, setCollisionMat);
                                Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);

                                blitCollisionShader = false;
                                tempCollisionStorageCounter = 0;
                            }
                            else
                            {
                                blitCollisionShader = true;
                            }
                        }
                    }
                }
            }
            else
            {
                updateInfluenceArray = true;
            }
        }

        if (blitCollisionShader)
        {
            //zero out remaining pixels (if its not a full load) to prevent possible bugs with deleted actors
            for (k = tempCollisionStorageCounter; k < 16; k++)
            {
                tempStorageArray[k] = color32Zero;
                tempStorageArray[k + 16] = color32Zero;
                tempStorageArray[k + 32] = color32Zero;
            }

            tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
            tempStorageBufferTexture.Apply(false);

            if (tempCollisionStorageCounter <= 4)
            {
                Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, setCollision4Mat);
                Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
            }
            else if (tempCollisionStorageCounter <= 8)
            {
                Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, setCollision8Mat);
                Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
            }
            else
            {
                Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, setCollisionMat);
                Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
            }

            blitCollisionShader = false;
            tempCollisionStorageCounter = 0;
        }

        Graphics.Blit(collisionStaticRenderTextureSource, collisionDynamicRenderTextureSource);

        if (fluidActorStaticArray.Length > 0)
        {
            setCollisionTexMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
        }

        if (fluidActorStaticArray.Length <= 4)
        {
            setCollision4Mat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
        }
        else if (fluidActorStaticArray.Length <= 8)
        {
            setCollision8Mat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
        }
        else
        {
            setCollisionMat.SetTexture("collisionTex", collisionDynamicRenderTextureSource);
        }

        if (updateInfluenceArray)
        {
            fluidConnectorScript.SortActorArray();
            fluidConnectorScript.GetActorArrayUpdate(this);
            updateInfluenceArray = false;
        }
    }

    //========

    void UpdateDynamicCollision()
    {
        collisionCopied = false;

        tempCurrentTime = Time.time;

        //length is defined in the fluidConnector script
        for (k = 0; k < dynamicInputArrayLength; k++)
        {
            if (!collisionCopied)
            {
                //copy collision from static into dynamic
                Graphics.Blit(collisionStaticRenderTextureSource, collisionDynamicRenderTextureSource);

                collisionCopied = true;
            }

            if (fluidActorDynamicArray[k].myTransform)
            {
                if (fluidActorDynamicArray[k].dynamicCollision)
                {
                    if (fluidActorDynamicArray[k].multiplySizeByScale)
                    {
                        tempCollisionSize = fluidActorDynamicArray[k].collisionSize * (fluidActorDynamicArray[k].myTransform.localScale.magnitude * 0.577f);
                    }
                    else
                    {
                        tempCollisionSize = fluidActorDynamicArray[k].collisionSize;
                    }

                    if ((fluidActorDynamicArray[k].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempCollisionSize)
                    {
                        if (fluidActorDynamicArray[k].useCollisionMaskTexture && fluidActorDynamicArray[k].collisionMaskTexture)
                        {
                            tempInfluenceLocation = fluidActorDynamicArray[k].myTransform.position;
                            tempLocation = myTransform.InverseTransformPoint(tempInfluenceLocation);
                            //locationX, locationY
                            tempShaderVectorData.x = ((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1) + 1.0f) * 0.5f;
                            tempShaderVectorData.y = ((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1) + 1.0f) * 0.5f;

                            if (tempCurrentTime - fluidActorDynamicArray[k].lastFrameTime < 0.5f)
                            {
                                collisionVelocity = (myTransform.InverseTransformPoint(fluidActorDynamicArray[k].lastFramePosition) - tempLocation) * fluidActorDynamicArray[k].moveVelocityMultiplier;
                            }
                            else
                            {
                                collisionVelocity = vector3Zero;
                            }

                            //velocityX
                            tempShaderVectorData.z = collisionVelocity.x;
                            //velocityY
                            tempShaderVectorData.w = collisionVelocity.z;
                            setCollisionTexMat.SetVector("textureData", tempShaderVectorData);
                            setCollisionTexMat.SetFloat("collisionStrength", fluidActorDynamicArray[k].collisionStrength);
                            setCollisionTexMat.SetTexture("collisionTexMask", fluidActorDynamicArray[k].collisionMaskTexture);
                            textureRotationAngle = Vector3.Angle(fluidActorDynamicArray[k].myTransform.forward, myTransform.forward);

                            if (Vector3.Dot(fluidActorDynamicArray[k].myTransform.forward, myTransform.right) < 0)
                            {
                                textureRotationAngle *= -1;
                            }

                            textureRotationQuaternion = Quaternion.Euler(0, 0, textureRotationAngle);
                            textureRotationMatrix = Matrix4x4.TRS(vector2Zero, textureRotationQuaternion, new Vector2(1f / (1.8f * (tempCollisionSize / myBoundsSize.x)), 1f / (1.8f * (tempCollisionSize / myBoundsSize.z))));
                            setCollisionTexMat.SetMatrix("rotationMatrix", textureRotationMatrix);

                            Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, setCollisionTexMat);
                            Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);

                            //cache position for use next frame in velocity calculation
                            fluidActorDynamicArray[k].lastFramePosition = tempInfluenceLocation;
                            fluidActorDynamicArray[k].lastFrameTime = tempCurrentTime;
                        }
                        else
                        {
                            tempInfluenceLocation = fluidActorDynamicArray[k].myTransform.position;
                            tempLocation = myTransform.InverseTransformPoint(tempInfluenceLocation);
                            //positionX positionY
                            tempCompressedColor1 = EncodeFloatRG(((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1) + 1.0f) * 0.5f);
                            tempCompressedColor2 = EncodeFloatRG(((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1) + 1.0f) * 0.5f);
                            tempStorageArray[tempCollisionStorageCounter] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, tempCompressedColor2.g);
                            //calculate velocity value
                            if (tempCurrentTime - fluidActorDynamicArray[k].lastFrameTime < 0.5f)
                            {
                                collisionVelocity = myTransform.InverseTransformPoint(fluidActorDynamicArray[k].lastFramePosition);
                                collisionVelocity = new Vector3((collisionVelocity.x - tempLocation.x), (collisionVelocity.z - tempLocation.z), 0);
                            }
                            else
                            {
                                collisionVelocity = vector3Zero;
                            }
                            //collision movement velocity and multiplier
                            tempCompressedColor1.r = (byte)(Mathf.Clamp01(((collisionVelocity.x * fluidActorDynamicArray[k].moveVelocityMultiplier) + 5) / 10) * 255);
                            tempCompressedColor1.g = (byte)(Mathf.Clamp01(((collisionVelocity.y * fluidActorDynamicArray[k].moveVelocityMultiplier) + 5) / 10) * 255);
                            tempStorageArray[tempCollisionStorageCounter + 16] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, 0, 0);
                            //collisionSize, collisionFalloff, collisionStrength
                            tempCompressedColor1 = EncodeFloatRG((tempCollisionSize / myBoundsSize.x) * 0.5f);
                            tempCompressedColor2.r = (byte)(fluidActorDynamicArray[k].collisionFalloff * 255);
                            tempCompressedColor2.g = (byte)(fluidActorDynamicArray[k].collisionStrength * 255);
                            tempStorageArray[tempCollisionStorageCounter + 32] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, tempCompressedColor2.g);

                            tempCollisionStorageCounter++;

                            if (tempCollisionStorageCounter > 15)
                            {
                                tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
                                tempStorageBufferTexture.Apply(false);

                                Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, setCollisionMat);
                                Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);

                                blitCollisionShader = false;
                                tempCollisionStorageCounter = 0;
                            }
                            else
                            {
                                blitCollisionShader = true;
                            }

                            //cache position for use next frame in velocity calculation
                            fluidActorDynamicArray[k].lastFramePosition = tempInfluenceLocation;
                            fluidActorDynamicArray[k].lastFrameTime = tempCurrentTime;
                        }
                    }
                }
            }
            else
            {
                updateInfluenceArray = true;
            }
        }

        if (blitCollisionShader)
        {
            //zero out remaining pixels (if its not a full load) to prevent possible bugs with deleted actors
            for (k = tempCollisionStorageCounter; k < 16; k++)
            {
                tempStorageArray[k] = color32Zero;
                tempStorageArray[k + 16] = color32Zero;
                tempStorageArray[k + 32] = color32Zero;
            }

            tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
            tempStorageBufferTexture.Apply(false);

            if (tempCollisionStorageCounter <= 4)
            {
                Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, setCollision4Mat);
                Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);
            }
            else if (tempCollisionStorageCounter <= 8)
            {
                Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, setCollision8Mat);
                Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);
            }
            else
            {
                Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, setCollisionMat);
                Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);
            }

            blitCollisionShader = false;
            tempCollisionStorageCounter = 0;
        }

        if (updateInfluenceArray)
        {
            fluidConnectorScript.SortActorArray();
            fluidConnectorScript.GetActorArrayUpdate(this);
            updateInfluenceArray = false;
        }
    }

    //========

    void UpdateImpulseActors()
    {
        //set impulse for each input object
        //length is defined in the fluidConnector script
        for (m = 0; m < dynamicInputArrayLength; m++)
        {
            if (fluidActorDynamicArray[m] != null)
            {
                if (fluidActorDynamicArray[m].addVelocity)
                {
                    if (fluidActorDynamicArray[m].multiplySizeByScale)
                    {
                        tempVelocitySize = fluidActorDynamicArray[m].velocitySize * (fluidActorDynamicArray[m].myTransform.localScale.magnitude * 0.577f);
                    }
                    else
                    {
                        tempVelocitySize = fluidActorDynamicArray[m].velocitySize;
                    }

                    if ((fluidActorDynamicArray[m].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempVelocitySize)
                    {
                        if (fluidActorDynamicArray[m].useVelocityMaskTexture && fluidActorDynamicArray[m].velocityMaskTexture)
                        {
                            tempLocation = myTransform.InverseTransformPoint(fluidActorDynamicArray[m].myTransform.position);
                            //locationX, locationY
                            tempShaderVectorData.x = ((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1) + 1.0f) * 0.5f;
                            tempShaderVectorData.y = ((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1) + 1.0f) * 0.5f;
                            tempPositionOffset = myTransform.InverseTransformDirection(fluidActorDynamicArray[m].myTransform.TransformDirection(new Vector3(0, 0, -1)));
                            tempVelocityStrength = fluidActorDynamicArray[m].velocityStrength;
                            tempPositionOffset = new Vector3(tempPositionOffset.x * tempVelocityStrength, tempPositionOffset.z * tempVelocityStrength, 0);
                            //velocityDirection/Strength
                            tempShaderVectorData.z = tempPositionOffset.x;
                            tempShaderVectorData.w = tempPositionOffset.y;
                            impulseLocationVelTexMat.SetVector("textureData", tempShaderVectorData);
                            impulseLocationVelTexMat.SetTexture("velocityTexMask", fluidActorDynamicArray[m].velocityMaskTexture);
                            textureRotationAngle = Vector3.Angle(fluidActorDynamicArray[m].myTransform.forward, myTransform.forward);

                            if (Vector3.Dot(fluidActorDynamicArray[m].myTransform.forward, myTransform.right) < 0)
                            {
                                textureRotationAngle *= -1;
                            }

                            textureRotationQuaternion = Quaternion.Euler(0, 0, textureRotationAngle);
                            textureRotationMatrix = Matrix4x4.TRS(vector2Zero, textureRotationQuaternion, new Vector2(1f / (1.8f * (tempVelocitySize / myBoundsSize.x)), 1f / (1.8f * (tempVelocitySize / myBoundsSize.z))));
                            impulseLocationVelTexMat.SetMatrix("rotationMatrix", textureRotationMatrix);

                            Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, impulseLocationVelTexMat);
                            Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
                        }
                        else
                        {
                            tempLocation = myTransform.InverseTransformPoint(fluidActorDynamicArray[m].myTransform.position);
                            //positionX, positionY
                            tempCompressedColor1 = EncodeFloatRG(((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1) + 1.0f) * 0.5f);
                            tempCompressedColor2 = EncodeFloatRG(((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1) + 1.0f) * 0.5f);
                            tempStorageArray[tempVelStorageCounter] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, tempCompressedColor2.g);
                            tempPositionOffset = myTransform.InverseTransformDirection(fluidActorDynamicArray[m].myTransform.TransformDirection(new Vector3(0, 0, -1)));
                            tempCompressedColor1 = EncodeFloatRG(fluidActorDynamicArray[m].velocityStrength);
                            tempPositionOffset = new Vector3(tempPositionOffset.x, tempPositionOffset.z, 0);
                            //impulseVelocityX, impulseVelocityY
                            tempCompressedColor1.b = (byte)(((tempPositionOffset.x + 5.0) / 10.0f) * 255);
                            tempCompressedColor1.a = (byte)(((tempPositionOffset.y + 5.0) / 10.0f) * 255);
                            tempStorageArray[tempVelStorageCounter + 16] = tempCompressedColor1;
                            //velocitysize, fallOff
                            tempCompressedColor1 = EncodeFloatRG((tempVelocitySize / myBoundsSize.x) * 0.5f);
                            tempCompressedColor2.r = (byte)(fluidActorDynamicArray[m].velocityFalloff * 255);
                            tempStorageArray[tempVelStorageCounter + 32] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, 0);

                            tempVelStorageCounter++;
                            //set to 14 instead of 15 because the "16" impulse shader breaks the 512 instruction limit.  If 1 arithmatic instruction can be optimized out, it should be set back to the full 15 cycle.
                            if (tempVelStorageCounter > 14)
                            {
                                tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
                                tempStorageBufferTexture.Apply(false);

                                Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, impulseLocationVelMat);
                                Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);

                                blitVelShader = false;
                                tempVelStorageCounter = 0;
                            }
                            else
                            {
                                blitVelShader = true;
                            }
                        }
                    }
                }
            }
            else
            {
                //if we find a null entry for an objects transform
                updateInfluenceArray = true;
            }
        }

        if (blitVelShader)
        {
            //zero out remaining pixels (if its not a full load) to prevent possible bugs with deleted actors
            for (m = tempVelStorageCounter; m < 16; m++)
            {
                tempStorageArray[m] = color32Zero;
                tempStorageArray[m + 16] = color32Zero;
                tempStorageArray[m + 32] = color32Zero;
            }

            tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
            tempStorageBufferTexture.Apply(false);

            if (tempVelStorageCounter <= 4)
            {
                Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, impulseLocationVel4Mat);
                Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
            }
            else if (tempVelStorageCounter <= 8)
            {
                Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, impulseLocationVel8Mat);
                Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
            }
            else
            {
                Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, impulseLocationVelMat);
                Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
            }

            blitVelShader = false;
            tempVelStorageCounter = 0;
        }

        if (updateInfluenceArray)
        {
            fluidConnectorScript.SortActorArray();
            fluidConnectorScript.GetActorArrayUpdate(this);
            updateInfluenceArray = false;
        }
    }

    //========

    void UpdateColorActors()
    {
        //set color for each input object
        //length is defined in the fluidConnector script
        for (n = 0; n < dynamicInputArrayLength; n++)
        {
            if (fluidActorDynamicArray[n] != null)
            {
                if (fluidActorDynamicArray[n].addColor)
                {
                    if (fluidActorDynamicArray[n].multiplySizeByScale)
                    {
                        tempColorSize = fluidActorDynamicArray[n].colorSize * (fluidActorDynamicArray[n].myTransform.localScale.magnitude * 0.577f);
                    }
                    else
                    {
                        tempColorSize = fluidActorDynamicArray[n].colorSize;
                    }

                    if ((fluidActorDynamicArray[n].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempColorSize)
                    {
                        if (fluidActorDynamicArray[n].useColorMaskTexture && fluidActorDynamicArray[n].colorMaskTexture)
                        {
                            tempLocation = myTransform.InverseTransformPoint(fluidActorDynamicArray[n].myTransform.position);
                            //locationX, locationY
                            tempShaderVectorData.x = ((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1) + 1.0f) * 0.5f;
                            tempShaderVectorData.y = ((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1) + 1.0f) * 0.5f;
                            //Color alpha value
                            tempShaderVectorData.z = fluidActorDynamicArray[n].colorValue.a;
                            impulseLocationTexMat.SetVector("textureData", tempShaderVectorData);
                            impulseLocationTexMat.SetTexture("colorTexMask", fluidActorDynamicArray[n].colorMaskTexture);
                            textureRotationAngle = Vector3.Angle(fluidActorDynamicArray[n].myTransform.forward, myTransform.forward);
                            if (Vector3.Dot(fluidActorDynamicArray[n].myTransform.forward, myTransform.right) < 0)
                            {
                                textureRotationAngle *= -1;
                            }
                            textureRotationQuaternion = Quaternion.Euler(0, 0, textureRotationAngle);
                            textureRotationMatrix = Matrix4x4.TRS(vector2Zero, textureRotationQuaternion, new Vector2(1f / (1.8f * (tempColorSize / myBoundsSize.x)), 1f / (1.8f * (tempColorSize / myBoundsSize.z))));
                            impulseLocationTexMat.SetMatrix("rotationMatrix", textureRotationMatrix);

                            Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, impulseLocationTexMat);
                            Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
                        }
                        else
                        {
                            tempLocation = myTransform.InverseTransformPoint(fluidActorDynamicArray[n].myTransform.position);
                            //positionX, positionY
                            tempCompressedColor1 = EncodeFloatRG(((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1) + 1.0f) * 0.5f);
                            tempCompressedColor2 = EncodeFloatRG(((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1) + 1.0f) * 0.5f);
                            tempStorageArray[tempColorStorageCounter] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, tempCompressedColor2.g);
                            //color and alpha
                            tempColor1 = fluidActorDynamicArray[n].colorValue;
                            tempCompressedColor1 = new Color32((byte)(tempColor1.r * 255), (byte)(tempColor1.g * 255), (byte)(tempColor1.b * 255), (byte)(tempColor1.a * 255));
                            tempStorageArray[tempColorStorageCounter + 16] = tempCompressedColor1;
                            //color size, fallOff
                            tempCompressedColor1 = EncodeFloatRG((tempColorSize / myBoundsSize.x) * 0.5f);
                            tempCompressedColor2.r = (byte)(fluidActorDynamicArray[n].colorFalloff * 255);
                            tempStorageArray[tempColorStorageCounter + 32] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, 0);

                            tempColorStorageCounter++;

                            if (tempColorStorageCounter > 15)
                            {
                                tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
                                tempStorageBufferTexture.Apply(false);

                                Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, impulseLocationMat);
                                Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);

                                blitColorShader = false;
                                tempColorStorageCounter = 0;
                            }
                            else
                            {
                                blitColorShader = true;
                            }
                        }
                    }
                }
            }
            else
            {
                updateInfluenceArray = true;
            }
        }

        if (blitColorShader)
        {
            //zero out remaining pixels (if its not a full load) to prevent possible bugs with deleted actors
            for (n = tempColorStorageCounter; n < 16; n++)
            {
                tempStorageArray[n] = color32Zero;
                tempStorageArray[n + 16] = color32Zero;
                tempStorageArray[n + 32] = color32Zero;
            }

            tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
            tempStorageBufferTexture.Apply(false);

            if (tempColorStorageCounter <= 4)
            {
                Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, impulseLocation4Mat);
                Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
            }
            else if (tempColorStorageCounter <= 8)
            {
                Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, impulseLocation8Mat);
                Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
            }
            else
            {
                Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, impulseLocationMat);
                Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
            }

            blitColorShader = false;

            tempColorStorageCounter = 0;
        }

        if (updateInfluenceArray)
        {
            fluidConnectorScript.SortActorArray();
            fluidConnectorScript.GetActorArrayUpdate(this);
            updateInfluenceArray = false;
        }
    }

    //========

    void UpdateFluid()
    {
        frameDelay++;

        switch (frameDelay)
        {
            case 1:
                myTransformPosition = myTransform.position;
                break;
            case 2:
                myTransformLocalScale = myTransform.localScale;
                break;
            case 3:
                if (isTerrain == false)
                {
                    myBoundsSize = myMesh.bounds.size;
                    myBoundsSize = new Vector3(myBoundsSize.x * myTransformLocalScale.x, myBoundsSize.y * myTransformLocalScale.y, myBoundsSize.z * myTransformLocalScale.z);
                    myBoundsMagnitude = myBoundsSize.magnitude;
                }

                frameDelay = 0;
                break;
        }

        //advection color
        if (useColorDissipationTexture)
        {
            Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, advectionColorTexMat);
            Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
        }
        else
        {
            Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, advectionColorMat);
            Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
        }

        //advection velocity
        if (useVelocityDissipationTexture)
        {
            Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, advectionVelocityTexMat);
            Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
        }
        else
        {
            Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, advectionVelocityMat);
            Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
        }

        //run divergence with built in clear event
        Graphics.Blit(divergenceRenderTextureSource, divergenceRenderTextureDestination, divergenceMat);
        Graphics.Blit(divergenceRenderTextureDestination, divergenceRenderTextureSource);

        //clear pressure
        Graphics.Blit(pressureRenderTextureSource, pressureRenderTextureDestination, initializeToValueMat);
        Graphics.Blit(pressureRenderTextureDestination, pressureRenderTextureSource);

        //iterate pressure "jacobi relax"
        for (i = 0; i < pressureIteration; i++)
        {
            Graphics.Blit(pressureRenderTextureSource, pressureRenderTextureDestination, jacobiRelaxMat);
            Graphics.Blit(pressureRenderTextureDestination, pressureRenderTextureSource);
        }

        //subtract pressure gradient
        Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, gradientMat);
        Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);

        if (updateColorWithFluidHidden)
        {
            UpdateColorActors();
        }

        if (updateImpulseWithFluidHidden)
        {
            UpdateImpulseActors();
        }

        if (updateCollisionWithFluidHidden)
        {
            UpdateDynamicCollision();
        }
    }

    //===================================== Helper Functions Below This Line =====================================

    //========
    // Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly... so we clamp it.
    Color32 EncodeFloatRG(float v)
    {
        v = Mathf.Clamp(v, 0f, 0.9999f);
        enc2 = new Vector2(kEncodeMul2.x * v, kEncodeMul2.y * v);
        enc2 = new Vector2(enc2.x % 1, enc2.y % 1);   //same as doing frac()... but frac doesnt exist and % has precision loss of about 0.00000005
        enc2.x -= enc2.y * kEncodeBit;
        return new Color32((byte)(Mathf.RoundToInt(enc2.x * 255)), (byte)(enc2.y * 255), (byte)(0), (byte)(0));
    }

    //Decode is only used for testing and could be removed without breaking the simulation.
    float DecodeFloatRG(Color32 colorV)
    {
        return Vector2.Dot(new Vector2(colorV.r, colorV.g), kDecodeDot2);
    }

    //========

    public void ChangeSimStrength(float strength)
    {
        simStrength = strength;

        if (useUnityProMethod)
        {
            advectionColorMat.SetFloat("simSpeed", simStrength);

            advectionVelocityMat.SetFloat("simSpeed", simStrength);
        }
    }

    //========

    public void ChangeColorDissipationStrength(float dissipation)
    {
        if (useColorDissipation)
        {
            colorDissipation = dissipation;
            oldColorDissipation = dissipation;

            if (useUnityProMethod)
            {
                advectionColorMat.SetFloat("dissipation", Mathf.Lerp(0.0f, 0.05f, colorDissipation));
            }
        }
        else
        {
            Debug.LogWarning("Fluid Color dissipation is currently disabled. To see color dissipation, make sure it is enabled.");
        }
    }

    //========

    public void ChangeColorDissipateTo(Color dissipateColor)
    {
        colorDissipateTo = dissipateColor;

        if (useUnityProMethod)
        {
            advectionColorMat.SetVector("colorDissipateTo", colorDissipateTo);
        }
    }

    //========

    public void ChangeColorDissipationBool(bool value)
    {
        useColorDissipation = value;

        if (useColorDissipation)
        {
            colorDissipation = oldColorDissipation;

            if (useUnityProMethod)
            {
                advectionColorMat.SetFloat("dissipation", Mathf.Lerp(0.0f, 0.05f, colorDissipation));
            }
        }
        else
        {
            oldColorDissipation = colorDissipation;

            colorDissipation = 0.0f;

            if (useUnityProMethod)
            {
                advectionColorMat.SetFloat("dissipation", Mathf.Lerp(0.0f, 0.05f, colorDissipation));
            }
        }
    }

    //========

    public void ChangeVelocityDissipationStrength(float dissipation)
    {
        if (useVelocityDissipation)
        {
            velocityDissipation = dissipation;
            oldVelocityDissipation = dissipation;

            if (useUnityProMethod)
            {
                advectionVelocityMat.SetFloat("dissipation", Mathf.Lerp(1.0f, 0.5f, velocityDissipation));
            }
        }
        else
        {
            Debug.LogWarning("Fluid Velocity dissipation is currently disabled. To see velocity dissipation, make sure it is enabled.");
        }
    }

    //========

    public void ChangeVelocityDissipationBool(bool value)
    {
        useVelocityDissipation = value;

        if (useVelocityDissipation)
        {
            velocityDissipation = oldVelocityDissipation;

            if (useUnityProMethod)
            {
                advectionVelocityMat.SetFloat("dissipation", Mathf.Lerp(1.0f, 0.5f, velocityDissipation));
            }
        }
        else
        {
            oldVelocityDissipation = velocityDissipation;

            velocityDissipation = 0.0f;

            if (useUnityProMethod)
            {
                advectionVelocityMat.SetFloat("dissipation", Mathf.Lerp(1.0f, 0.5f, velocityDissipation));
            }
        }
    }

    //========

    public void SetFluidUpdateFPS(int fps)
    {
        fluidUpdateFPS = fps;

        if (useUnityProMethod)
        {
            CancelInvoke("UpdateFluid");

            if (fluidUpdateFPS > 0)
            {
                InvokeRepeating("UpdateFluid", 0, 1f / fluidUpdateFPS);
            }
        }
        else
        {
            CancelInvoke("UpdateFluidFree");

            if (fluidUpdateFPS > 0)
            {
                InvokeRepeating("UpdateFluidFree", 0, 1f / fluidUpdateFPS);
            }
        }
    }

    //========

    public void SetImpulseUpdateFPS(int fps)
    {
        impulseUpdateFPS = fps;

        if (useUnityProMethod)
        {
            if (!updateImpulseWithFluidHidden)
            {
                CancelInvoke("UpdateImpulseActors");

                if (impulseUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateImpulseActors", 0.1f, 1f / impulseUpdateFPS);
                }
            }
            else
            {
                Debug.LogWarning("Impulse Update is currently synced with Fluid update; to use SetImpulseUpdateFPS you need to turn off impulse sync with fluid first.  Try using SetSyncImpulseWithFluid(boolean) first.");
            }
        }
        else
        {
            if (!updateImpulseWithFluidHidden)
            {
                CancelInvoke("UpdateImpulseActorsFree");

                if (impulseUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateImpulseActorsFree", 0.1f, 1f / impulseUpdateFPS);
                }
            }
            else
            {
                Debug.LogWarning("Impulse Update is currently synced with Fluid update; to use SetImpulseUpdateFPS you need to turn off impulse sync with fluid first.  Try using SetSyncImpulseWithFluid(boolean) first.");
            }
        }
    }

    //========

    public void SetColorUpdateFPS(int fps)
    {
        colorUpdateFPS = fps;

        if (useUnityProMethod)
        {
            if (!updateColorWithFluidHidden)
            {
                CancelInvoke("UpdateColorActors");

                if (colorUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateColorActors", 0.1f, 1f / colorUpdateFPS);
                }
            }
            else
            {
                Debug.LogWarning("Color Update is currently synced with Fluid update; to use SetColorUpdateFPS you need to turn off color sync with fluid first.  Try using SetSyncColorWithFluid(boolean) first.");
            }
        }
        else
        {
            if (!updateColorWithFluidHidden)
            {
                CancelInvoke("UpdateColorActorsFree");

                if (colorUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateColorActorsFree", 0.1f, 1f / colorUpdateFPS);
                }
            }
            else
            {
                Debug.LogWarning("Color Update is currently synced with Fluid update; to use SetColorUpdateFPS you need to turn off color sync with fluid first.  Try using SetSyncColorWithFluid(boolean) first.");
            }
        }
    }

    //========

    public void SetSyncCollisionWithFluid(bool syncWithFluid)
    {
        updateCollisionWithFluidHidden = syncWithFluid;
        updateCollisionWithFluid = updateCollisionWithFluidHidden;

        if (useUnityProMethod)
        {
            if (updateCollisionWithFluidHidden)
            {
                CancelInvoke("UpdateDynamicCollision");
            }
            else
            {
                CancelInvoke("UpdateDynamicCollision");

                if (impulseUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateDynamicCollision", 0.1f, 1f / impulseUpdateFPS);
                }
            }
        }
        else
        {
            if (updateCollisionWithFluidHidden)
            {
                CancelInvoke("UpdateDynamicCollisionFree");
            }
            else
            {
                CancelInvoke("UpdateDynamicCollisionFree");

                if (impulseUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateDynamicCollisionFree", 0.1f, 1f / impulseUpdateFPS);
                }
            }
        }
    }

    //========

    public void SetSyncImpulseWithFluid(bool syncWithFluid)
    {
        updateImpulseWithFluidHidden = syncWithFluid;
        updateImpulseWithFluid = updateImpulseWithFluidHidden;

        if (useUnityProMethod)
        {
            if (updateImpulseWithFluidHidden)
            {
                CancelInvoke("UpdateImpulseActors");
            }
            else
            {
                CancelInvoke("UpdateImpulseActors");

                if (impulseUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateImpulseActors", 0.1f, 1f / impulseUpdateFPS);
                }
            }
        }
        else
        {
            if (updateImpulseWithFluidHidden)
            {
                CancelInvoke("UpdateImpulseActorsFree");
            }
            else
            {
                CancelInvoke("UpdateImpulseActorsFree");

                if (impulseUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateImpulseActorsFree", 0.1f, 1f / impulseUpdateFPS);
                }
            }
        }
    }

    //========

    public void SetSyncColorWithFluid(bool syncWithFluid)
    {
        updateColorWithFluidHidden = syncWithFluid;
        updateColorWithFluid = updateColorWithFluidHidden;

        if (useUnityProMethod)
        {
            if (updateColorWithFluidHidden)
            {
                CancelInvoke("UpdateColorActors");
            }
            else
            {
                CancelInvoke("UpdateColorActors");

                if (colorUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateColorActors", 0.1f, 1f / colorUpdateFPS);
                }
            }
        }
        else
        {
            if (updateColorWithFluidHidden)
            {
                CancelInvoke("UpdateColorActorsFree");
            }
            else
            {
                CancelInvoke("UpdateColorActorsFree");

                if (colorUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateColorActorsFree", 0.1f, 1f / colorUpdateFPS);
                }
            }
        }
    }

    //========

    public void SetCollisionUpdateFPS(int fps)
    {
        collisionUpdateFPS = fps;

        if (useUnityProMethod)
        {
            if (updateCollisionWithFluidHidden)
            {
                CancelInvoke("UpdateDynamicCollision");
            }
            else
            {
                CancelInvoke("UpdateDynamicCollision");

                if (collisionUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateDynamicCollision", 0.1f, 1f / collisionUpdateFPS);
                }
            }
        }
        else
        {
            if (updateCollisionWithFluidHidden)
            {
                CancelInvoke("UpdateDynamicCollisionFree");
            }
            else
            {
                CancelInvoke("UpdateDynamicCollisionFree");

                if (collisionUpdateFPS > 0)
                {
                    InvokeRepeating("UpdateDynamicCollisionFree", 0.1f, 1f / collisionUpdateFPS);
                }
            }
        }
    }

    //========

    public void ClearAllDynamicBuffers()
    {
        if (useUnityProMethod)
        {
            //clear color
            initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 0.0f));
            Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
            //clear velocity
            initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 0.0f));
            Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
            //clear dynamic collision
            initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 1.0f));
            Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);
            //clear divergence
            initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            Graphics.Blit(divergenceRenderTextureSource, divergenceRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(divergenceRenderTextureDestination, divergenceRenderTextureSource);
            //clear pressure
            initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            Graphics.Blit(pressureRenderTextureSource, pressureRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(pressureRenderTextureDestination, pressureRenderTextureSource);
        }
        else
        {
            //clear color
            ClearColorBuffer(new Color(0, 0, 0, 0));
            //clear velocity
            ClearVelocityBuffer(new Vector2(0, 0));
            //clear dynamic collision
            clearDynamicCollisionToValueMatFree();
            //clear divergence
            clearDivergenceToValueMatFree();
            //clear pressure
            clearPressureToValueMatFree();
        }
    }

    //========

    public void ClearColorBuffer(Color defaultColor)
    {
        if (useUnityProMethod)
        {
            initializeToValueMat.SetVector("initialValue", new Vector3(defaultColor.r, defaultColor.g, defaultColor.b));
            Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
        }
        else
        {

            clearColorToValueMatFree(defaultColor);
        }
    }

    //========

    public void ClearColorBufferToTexture(Texture2D colorTexture)
    {
        if (useUnityProMethod)
        {
            initializeToTextureMat.SetTexture("initialTexture", colorTexture);
            Graphics.Blit(fluidRenderTextureSource, fluidRenderTextureDestination, initializeToTextureMat);
            Graphics.Blit(fluidRenderTextureDestination, fluidRenderTextureSource);
        }
        else
        {
            initializeToTextureMatFree(colorTexture);
        }
    }

    //========

    public void ClearVelocityBuffer(Vector2 defaultVelocity)
    {
        if (useUnityProMethod)
        {
            initializeVelToValueMat.SetVector("initialValue", defaultVelocity);
            Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, initializeVelToValueMat);
            Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
        }
        else
        {
            clearVelocityToValueMatFree(defaultVelocity);
        }
    }

    //========

    public void ClearVelocityBufferToTexture(Texture2D velocityTexture)
    {
        if (useUnityProMethod)
        {
            if (velocityTexture != null)
            {
                initializeVelToTextureMat.SetTexture("initialTexture", velocityTexture);
                Graphics.Blit(velocityRenderTextureSource, velocityRenderTextureDestination, initializeVelToTextureMat);
                Graphics.Blit(velocityRenderTextureDestination, velocityRenderTextureSource);
            }
            else
            {
                Debug.LogError("ClearVelocityBufferToTexture() was called without supplying a velocityTexture to the function.  The Fluid Velocity buffer has not been cleared to the texture value.");
            }
        }
        else
        {
            if (velocityTexture != null)
            {
                initializeVelToTextureMatFree(velocityTexture);
            }
            else
            {
                Debug.LogError("ClearVelocityBufferToTexture() was called without supplying a velocityTexture to the function.  The Fluid Velocity buffer has not been cleared to the texture value.");
            }
        }
    }

    //========

    public void ClearDynamicCollisionBuffer()
    {
        if (useUnityProMethod)
        {
            initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 1.0f));
            Graphics.Blit(collisionDynamicRenderTextureSource, collisionDynamicRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(collisionDynamicRenderTextureDestination, collisionDynamicRenderTextureSource);
        }
        else
        {
            clearDynamicCollisionToValueMatFree();
        }
    }

    //========

    public void ClearStaticCollisionBuffer()
    {
        if (useUnityProMethod)
        {
            initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 1.0f));
            Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
        }
        else
        {
            clearStaticCollisionToValueMatFree();
        }
    }

    //========

    public void RecreateStaticCollisionBuffer(bool applyBoundaryCollision)
    {
        fluidConnectorScript.SortActorArray();
        fluidConnectorScript.GetActorArrayUpdate(this);

        if (useUnityProMethod)
        {
            initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 1.0f));
            Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, initializeToValueMat);
            Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);

            if (applyBoundaryCollision)
            {
                Graphics.Blit(collisionStaticRenderTextureSource, collisionStaticRenderTextureDestination, boundaryOpMat);
                Graphics.Blit(collisionStaticRenderTextureDestination, collisionStaticRenderTextureSource);
            }

            UpdateStaticCollision();
        }
        else
        {
            initializeCollisionToValueMatFree();

            if (applyBoundaryCollision)
            {
                boundaryOpMatFree(true);
            }

            UpdateStaticCollisionFree();
        }
    }

//====================================== UNITY FREE FLUIDSIM FUNCTIONS ONLY BELOW THIS LINE ======================================

    void UpdateStaticCollisionFree()
    {
        fluidConnectorScript.GetActorArrayUpdate(this);

        tempColorPixelCount = 0;

        //set collision for each collision input object
        for (k = 0; k < fluidActorStaticArray.Length; k++)
        {
            if (fluidActorStaticArray[k] != null)
            {
                if (fluidActorStaticArray[k].myTransform != null)
                {
                    if (fluidActorStaticArray[k].staticCollision)
                    {
                        if (fluidActorStaticArray[k].multiplySizeByScale)
                        {
                            tempCollisionSize = fluidActorStaticArray[k].collisionSize * (fluidActorStaticArray[k].myTransform.localScale.magnitude * 0.577f);
                        }
                        else
                        {
                            tempCollisionSize = fluidActorStaticArray[k].collisionSize;
                        }

                        if (fluidActorStaticArray[k].myTransform != null)
                        {
                            tempInfluenceLocation = fluidActorStaticArray[k].myTransform.position;

                            if ((tempInfluenceLocation - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempCollisionSize)
                            {
                                tempLocation = myTransform.InverseTransformPoint(tempInfluenceLocation);
                                //positionX positionY
                                freeCollisionPositionX = (((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) - 0.5f) * -1.0f;
                                freeCollisionPositionY = (((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) - 0.5f) * -1.0f;
                                //collisionSize, collisionFalloff, collisionStrength
                                freeCollisionSize = (tempCollisionSize / myBoundsSize.x);
                                freeCollisionFalloff = fluidActorStaticArray[k].collisionFalloff * 150;
                                freeCollisionStrength = fluidActorStaticArray[k].collisionStrength;

                                freeMinY = (int)((freeCollisionPositionY - (freeCollisionSize * 1.1f)) * resolutionHidden);
                                freeMinX = (int)((freeCollisionPositionX - (freeCollisionSize * 1.1f)) * resolutionHidden);

                                if (freeMinY < 0)
                                {
                                    freeMinY = 0;
                                }

                                if (freeMinX < 0)
                                {
                                    freeMinX = 0;
                                }

                                freeMaxY = (int)((freeCollisionPositionY + (freeCollisionSize * 1.1f)) * resolutionHidden);
                                freeMaxX = (int)((freeCollisionPositionX + (freeCollisionSize * 1.1f)) * resolutionHidden);

                                if (freeMaxY > resolutionHidden)
                                {
                                    freeMaxY = resolutionHidden;
                                }

                                if (freeMaxX > resolutionHidden)
                                {
                                    freeMaxX = resolutionHidden;
                                }

                                for (collisionStaticRenderTextureSourceFreeTempSCY = freeMinY; collisionStaticRenderTextureSourceFreeTempSCY < freeMaxY; collisionStaticRenderTextureSourceFreeTempSCY++)
                                {
                                    tempColorPixelCount = (collisionStaticRenderTextureSourceFreeTempSCY * resolutionHidden) + freeMinX;

                                    if (tempColorPixelCount >= 0)
                                    {
                                        for (collisionStaticRenderTextureSourceFreeTempSCX = freeMinX; collisionStaticRenderTextureSourceFreeTempSCX < freeMaxX; collisionStaticRenderTextureSourceFreeTempSCX++)
                                        {
                                            if (tempColorPixelCount < resolution2Min1)
                                            {
                                                freeTempFloat = ((Vector2.Distance(new Vector2(collisionStaticRenderTextureSourceFreeTempSCX / (float)resolutionHidden, collisionStaticRenderTextureSourceFreeTempSCY / (float)resolutionHidden), new Vector2(freeCollisionPositionX, freeCollisionPositionY)) * -1) + freeCollisionSize) * freeCollisionFalloff;

                                                if (freeTempFloat < 0)
                                                {
                                                    freeTempFloat = 0;
                                                }
                                                else if (freeTempFloat > 1)
                                                {
                                                    freeTempFloat = 1;
                                                }

                                                freeTempFloat *= freeCollisionStrength;

                                                freeTempFloat = 1 - freeTempFloat;

                                                collisionStaticRenderTextureSourceFreeTempSA[tempColorPixelCount] = collisionStaticRenderTextureSourceFreeTempSA[tempColorPixelCount] * freeTempFloat;

                                                collisionDynamicRenderTextureSourceFreeTempSA[tempColorPixelCount] = collisionStaticRenderTextureSourceFreeTempSA[tempColorPixelCount];

                                                tempColorPixelCount++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                updateInfluenceArray = true;
            }
        }

        if (updateInfluenceArray)
        {
            fluidConnectorScript.SortActorArray();
            fluidConnectorScript.GetActorArrayUpdate(this);
            updateInfluenceArray = false;
        }
    }

    //========

    void UpdateDynamicCollisionFree()
    {
        collisionCopied = false;

        tempColorPixelCount = 0;

        tempCurrentTime = Time.time;

        //set collision for each collision input object
        for (k = 0; k < fluidConnectorScript.dynamicArrayCount; k++)
        {
            if (!collisionCopied)
            {
                System.Array.Copy(collisionStaticRenderTextureSourceFreeTempSA, collisionDynamicRenderTextureSourceFreeTempSA, resolution2);

                System.Array.Copy(vector2ZeroRenderTextureSourceFreeTempSA, collisionDynamicVelocityRenderTextureSourceFreeTempSA, resolution2);

                collisionCopied = true;
            }

            if (fluidActorDynamicArray[k].myTransform != null)
            {
                if (fluidActorDynamicArray[k].dynamicCollision)
                {
                    if (fluidActorDynamicArray[k].multiplySizeByScale)
                    {
                        tempCollisionSize = fluidActorDynamicArray[k].collisionSize * (fluidActorDynamicArray[k].myTransform.localScale.magnitude * 0.577f);
                    }
                    else
                    {
                        tempCollisionSize = fluidActorDynamicArray[k].collisionSize;
                    }

                    if ((fluidActorDynamicArray[k].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempCollisionSize)
                    {
                        tempInfluenceLocation = fluidActorDynamicArray[k].myTransform.position;
                        tempLocation = myTransform.InverseTransformPoint(tempInfluenceLocation);
                        //positionX positionY
                        freeCollisionPositionX = (((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) - 0.5f) * -1.0f;
                        freeCollisionPositionY = (((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) - 0.5f) * -1.0f;
                        tempVector2 = new Vector2(freeCollisionPositionX, freeCollisionPositionY);
                        //collisionSize, collisionFalloff, collisionStrength
                        freeCollisionSize = (tempCollisionSize / myBoundsSize.x);
                        freeCollisionFalloff = fluidActorDynamicArray[k].collisionFalloff * 150;
                        freeCollisionStrength = fluidActorDynamicArray[k].collisionStrength;
                        //collision object velocity
                        if (tempCurrentTime - fluidActorDynamicArray[k].lastFrameTime < 0.5f)
                        {
                            collisionVelocity = (myTransform.InverseTransformPoint(fluidActorDynamicArray[k].lastFramePosition) - tempLocation) * fluidActorDynamicArray[k].moveVelocityMultiplier;
                            freeCollisionVelocity = new Vector2(collisionVelocity.x, collisionVelocity.z);
                        }
                        else
                        {
                            freeCollisionVelocity = vector2Zero;
                        }

                        freeMinY = (int)((freeCollisionPositionY - (freeCollisionSize * 0.975f)) * resolutionHidden);
                        freeMinX = (int)((freeCollisionPositionX - (freeCollisionSize * 0.975f)) * resolutionHidden);

                        if (freeMinY < 0)
                        {
                            freeMinY = 0;
                        }

                        if (freeMinX < 0)
                        {
                            freeMinX = 0;
                        }

                        freeMaxY = (int)((freeCollisionPositionY + (freeCollisionSize * 1.2f)) * resolutionHidden);
                        freeMaxX = (int)((freeCollisionPositionX + (freeCollisionSize * 1.2f)) * resolutionHidden);

                        if (freeMaxY > resolutionHidden)
                        {
                            freeMaxY = resolutionHidden;
                        }

                        if (freeMaxX > resolutionHidden)
                        {
                            freeMaxX = resolutionHidden;
                        }

                        for (collisionDynamicRenderTextureSourceFreeTempSCY = freeMinY; collisionDynamicRenderTextureSourceFreeTempSCY < freeMaxY; collisionDynamicRenderTextureSourceFreeTempSCY++)
                        {
                            tempColorPixelCount = (collisionDynamicRenderTextureSourceFreeTempSCY * resolutionHidden) + freeMinX;

                            if (tempColorPixelCount >= 0)
                            {
                                for (collisionDynamicRenderTextureSourceFreeTempSCX = freeMinX; collisionDynamicRenderTextureSourceFreeTempSCX < freeMaxX; collisionDynamicRenderTextureSourceFreeTempSCX++)
                                {
                                    if (tempColorPixelCount <= resolution2Min1)
                                    {
                                        freeTempFloat = ((Vector2.Distance(new Vector2(collisionDynamicRenderTextureSourceFreeTempSCX / (float)resolutionHidden, collisionDynamicRenderTextureSourceFreeTempSCY / (float)resolutionHidden), tempVector2) * -1) + freeCollisionSize) * freeCollisionFalloff;

                                        if (freeTempFloat < 0)
                                        {
                                            freeTempFloat = 0;
                                        }
                                        else if (freeTempFloat > 1)
                                        {
                                            freeTempFloat = 1;
                                        }

                                        freeTempFloat *= freeCollisionStrength;

                                        freeTempFloat = 1 - freeTempFloat;

                                        collisionDynamicRenderTextureSourceFreeTempSA[tempColorPixelCount] *= freeTempFloat;

                                        collisionDynamicVelocityRenderTextureSourceFreeTempSA[tempColorPixelCount] = Vector2.Lerp(freeCollisionVelocity, collisionDynamicVelocityRenderTextureSourceFreeTempSA[tempColorPixelCount], freeTempFloat);

                                        tempColorPixelCount++;
                                    }
                                }
                            }
                        }

                        fluidActorDynamicArray[k].lastFramePosition = tempInfluenceLocation;
                        fluidActorDynamicArray[k].lastFrameTime = tempCurrentTime;
                    }
                }
            }
            else
            {
                updateInfluenceArray = true;
            }
        }

        if (updateInfluenceArray)
        {
            fluidConnectorScript.SortActorArray();
            fluidConnectorScript.GetActorArrayUpdate(this);
            updateInfluenceArray = false;
        }
    }

    //========

    void UpdateImpulseActorsFree()
    {
        //set impulse for each input object
        //length is defined in the fluidConnector script
        for (m = 0; m < dynamicInputArrayLength; m++)
        {
            if (fluidActorDynamicArray[m].myTransform != null)
            {
                if (fluidActorDynamicArray[m].addVelocity)
                {
                    if (fluidActorDynamicArray[m].multiplySizeByScale)
                    {
                        tempVelocitySize = fluidActorDynamicArray[m].velocitySize * (fluidActorDynamicArray[m].myTransform.localScale.magnitude * 0.577f);
                    }
                    else
                    {
                        tempVelocitySize = fluidActorDynamicArray[m].velocitySize;
                    }

                    if ((fluidActorDynamicArray[m].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempVelocitySize)
                    {
                        tempLocation = myTransform.InverseTransformPoint(fluidActorDynamicArray[m].myTransform.position);
                        //positionX, positionY
                        freeVelocityPositionX = (((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) - 0.5f) * -1.0f;
                        freeVelocityPositionY = (((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) - 0.5f) * -1.0f;
                        tempVector2 = new Vector2(freeVelocityPositionX, freeVelocityPositionY);
                        tempPositionOffset = myTransform.InverseTransformDirection(fluidActorDynamicArray[m].myTransform.TransformDirection(new Vector3(0, 0, -1)));
                        freeVelocityStrength = fluidActorDynamicArray[m].velocityStrength * 10;
                        //impulseVelocityX, impulseVelocityY
                        freeVelocityStrengthX = tempPositionOffset.x * freeVelocityStrength;
                        freeVelocityStrengthY = tempPositionOffset.z * freeVelocityStrength;
                        freeTempVelocity = new Vector2(freeVelocityStrengthX, freeVelocityStrengthY);
                        //velocitysize, fallOff
                        freeVelocitySize = (tempVelocitySize / myBoundsSize.x);
                        freeVelocityFalloff = fluidActorDynamicArray[m].velocityFalloff * 150;

                        freeMinY = (int)((freeVelocityPositionY - (freeVelocitySize * 0.975f)) * resolutionHidden);
                        freeMinX = (int)((freeVelocityPositionX - (freeVelocitySize * 0.975f)) * resolutionHidden);

                        if (freeMinY < 0)
                        {
                            freeMinY = 0;
                        }

                        if (freeMinX < 0)
                        {
                            freeMinX = 0;
                        }

                        freeMaxY = (int)((freeVelocityPositionY + (freeVelocitySize * 1.2f)) * resolutionHidden);
                        freeMaxX = (int)((freeVelocityPositionX + (freeVelocitySize * 1.2f)) * resolutionHidden);

                        if (freeMaxY > resolutionHidden)
                        {
                            freeMaxY = resolutionHidden;
                        }

                        if (freeMaxX > resolutionHidden)
                        {
                            freeMaxX = resolutionHidden;
                        }

                        tempColorPixelCount = 0;

                        for (velocityRenderTextureSourceFreeTempSCY = freeMinY; velocityRenderTextureSourceFreeTempSCY < freeMaxY; velocityRenderTextureSourceFreeTempSCY++)
                        {
                            tempColorPixelCount = (velocityRenderTextureSourceFreeTempSCY * resolutionHidden) + freeMinX;

                            if (tempColorPixelCount >= 0)
                            {
                                for (velocityRenderTextureSourceFreeTempSCX = freeMinX; velocityRenderTextureSourceFreeTempSCX < freeMaxX; velocityRenderTextureSourceFreeTempSCX++)
                                {
                                    if (tempColorPixelCount <= resolution2Min1)
                                    {
                                        freeTempFloat = (Vector2.Distance(new Vector2(velocityRenderTextureSourceFreeTempSCX / (float)resolutionHidden, velocityRenderTextureSourceFreeTempSCY / (float)resolutionHidden), tempVector2) - freeVelocitySize) * -freeVelocityFalloff;

                                        if (freeTempFloat < 0)
                                        {
                                            freeTempFloat = 0;
                                        }
                                        else if (freeTempFloat > 1)
                                        {
                                            freeTempFloat = 1;
                                        }

                                        velocityRenderTextureSourceFreeTempSA[tempColorPixelCount] += freeTempVelocity * freeTempFloat;

                                        tempColorPixelCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                updateInfluenceArray = true;
            }
        }

        if (updateInfluenceArray)
        {
            fluidConnectorScript.SortActorArray();
            fluidConnectorScript.GetActorArrayUpdate(this);
            updateInfluenceArray = false;
        }
    }

    //========

    void UpdateColorActorsFree()
    {
        //set color for each input object
        //length is defined in the fluidConnector script
        for (n = 0; n < dynamicInputArrayLength; n++)
        {
            if (fluidActorDynamicArray[n].myTransform != null)
            {
                if (fluidActorDynamicArray[n].addColor)
                {
                    if (fluidActorDynamicArray[n].multiplySizeByScale)
                    {
                        tempColorSize = fluidActorDynamicArray[n].colorSize * (fluidActorDynamicArray[n].myTransform.localScale.magnitude * 0.577f);
                    }
                    else
                    {
                        tempColorSize = fluidActorDynamicArray[n].colorSize;
                    }

                    if ((fluidActorDynamicArray[n].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempColorSize)
                    {
                        tempLocation = myTransform.InverseTransformPoint(fluidActorDynamicArray[n].myTransform.position);
                        //positionX, positionY
                        freeColorPositionX = (((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) - 0.5f) * -1;
                        freeColorPositionY = (((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) - 0.5f) * -1;
                        tempVector2 = new Vector2(freeColorPositionX, freeColorPositionY);
                        //color and alpha
                        tempColor1 = fluidActorDynamicArray[n].colorValue;
                        //color size, fallOff
                        freeColorSize = (tempColorSize / myBoundsSize.x);
                        freeColorFalloff = fluidActorDynamicArray[n].colorFalloff * 150;

                        freeMinY = (int)((freeColorPositionY - (freeColorSize * 0.975f)) * resolutionHidden);
                        freeMinX = (int)((freeColorPositionX - (freeColorSize * 0.975f)) * resolutionHidden);

                        if (freeMinY < 0)
                        {
                            freeMinY = 0;
                        }

                        if (freeMinX < 0)
                        {
                            freeMinX = 0;
                        }

                        freeMaxY = (int)((freeColorPositionY + (freeColorSize * 1.2f)) * resolutionHidden);
                        freeMaxX = (int)((freeColorPositionX + (freeColorSize * 1.2f)) * resolutionHidden);

                        if (freeMaxY > resolutionHidden)
                        {
                            freeMaxY = resolutionHidden;
                        }

                        if (freeMaxX > resolutionHidden)
                        {
                            freeMaxX = resolutionHidden;
                        }

                        tempColorPixelCount = 0;

                        for (fluidRenderTextureSourceFreeTempSCY = freeMinY; fluidRenderTextureSourceFreeTempSCY < freeMaxY; fluidRenderTextureSourceFreeTempSCY++)
                        {
                            //fake clamp function
                            tempColorPixelCount = (fluidRenderTextureSourceFreeTempSCY * resolutionHidden) + freeMinX;

                            if (tempColorPixelCount >= 0)
                            {
                                for (fluidRenderTextureSourceFreeTempSCX = freeMinX; fluidRenderTextureSourceFreeTempSCX < freeMaxX; fluidRenderTextureSourceFreeTempSCX++)
                                {
                                    if (tempColorPixelCount <= resolution2Min1)
                                    {
                                        //fake clamp function
                                        freeTempFloat = (Vector2.Distance(new Vector2(fluidRenderTextureSourceFreeTempSCX / (float)resolutionHidden, fluidRenderTextureSourceFreeTempSCY / (float)resolutionHidden), tempVector2) - freeColorSize) * -freeColorFalloff;

                                        if (freeTempFloat < 0)
                                        {
                                            freeTempFloat = 0;
                                        }
                                        else if (freeTempFloat > 1)
                                        {
                                            freeTempFloat = 1;
                                        }

                                        freeColorCalcAlpha = tempColor1.a * freeTempFloat;

                                        fluidRenderTextureSourceFreeTempSA[tempColorPixelCount] = Color32.Lerp(fluidRenderTextureSourceFreeTempSA[tempColorPixelCount], tempColor1, freeColorCalcAlpha);

                                        tempColorPixelCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                updateInfluenceArray = true;
            }
        }

        if (updateInfluenceArray)
        {
            fluidConnectorScript.SortActorArray();
            fluidConnectorScript.GetActorArrayUpdate(this);
            updateInfluenceArray = false;
        }
    }

    //========

    void UpdateFluidFree()
    {
        frameDelay++;

        switch (frameDelay)
        {
            case 1:
                myTransformPosition = myTransform.position;
                break;
            case 2:
                myTransformLocalScale = myTransform.localScale;
                break;
            case 3:
                if (isTerrain == false)
                {
                    myBoundsSize = myMesh.bounds.size;
                    myBoundsSize = new Vector3(myBoundsSize.x * myTransformLocalScale.x, myBoundsSize.y * myTransformLocalScale.y, myBoundsSize.z * myTransformLocalScale.z);
                    myBoundsMagnitude = myBoundsSize.magnitude;
                }

                frameDelay = 0;
                break;
        }

        //advection color
        advectionColorMatFree();

        //advection velocity
        advectionVelocityMatFree();

        //run divergence with built in clear event
        divergenceMatFree();

        //clear pressure
        clearPressureToValueMatFree();

        //iterate pressure "jacobi relax"
        for (i = 0; i < pressureIteration; i++)
        {
            jacobiRelaxMatFree();
        }

        //subtract pressure gradient
        gradientMatFree();

        if (updateColorWithFluidHidden)
        {
            UpdateColorActorsFree();
        }

        if (updateImpulseWithFluidHidden)
        {
            UpdateImpulseActorsFree();
        }

        if (updateCollisionWithFluidHidden)
        {
            UpdateDynamicCollisionFree();
        }

        UpdateDisplayFree();
    }

    //========

    void UpdateDisplayFree()
    {
        //Output texture to screen
        if (outputTextureNumHidden == 0)
        {
            for (i = 0; i < resolution2; i++)
            {
                tempColorArray[i].r = collisionDynamicVelocityRenderTextureSourceFreeTempSA[i].x;
                tempColorArray[i].g = collisionDynamicVelocityRenderTextureSourceFreeTempSA[i].y;
                tempColorArray[i].b = collisionDynamicRenderTextureSourceFreeTempSA[i];
            }

            collisionDynamicRenderTextureSourceFree.SetPixels(tempColorArray);
            collisionDynamicRenderTextureSourceFree.Apply();
        }
        else if (outputTextureNumHidden == 1)
        {
            fluidRenderTextureSourceFree.SetPixels32(fluidRenderTextureSourceFreeTempSA);
            fluidRenderTextureSourceFree.Apply();
        }
        else if (outputTextureNumHidden == 2)
        {
            for (i = 0; i < resolution2; i++)
            {
                tempColorArray[i].r = velocityRenderTextureSourceFreeTempSA[i].x;
                tempColorArray[i].g = velocityRenderTextureSourceFreeTempSA[i].y;
            }

            velocityRenderTextureSourceFree.SetPixels(tempColorArray);
            velocityRenderTextureSourceFree.Apply();
        }
    }

    //========

    void advectionColorMatFree()
    {
        tempColorPixelCount = 0;

        freeColorDissipation = (0.0005f * colorDissipation);

        for (fluidRenderTextureSourceFreeTempSCY = 0; fluidRenderTextureSourceFreeTempSCY < resolutionHidden; fluidRenderTextureSourceFreeTempSCY++)
        {
            freeUVy = fluidRenderTextureSourceFreeTempSCY;

            for (fluidRenderTextureSourceFreeTempSCX = 0; fluidRenderTextureSourceFreeTempSCX < resolutionHidden; fluidRenderTextureSourceFreeTempSCX++)
            {
                freeUVx = fluidRenderTextureSourceFreeTempSCX;

                freePosX = 40 * timeStep * simStrength * velocityRenderTextureSourceFreeTempSA[tempColorPixelCount].x;
                freePosY = 40 * timeStep * simStrength * velocityRenderTextureSourceFreeTempSA[tempColorPixelCount].y;

                //fake floor function
                freeTempInt = (int)((freeUVx - freePosX));
                if (freeTempInt < 0)
                {
                    freeTempInt -= 1;
                }
                targetPixelX = freeTempInt;
                //fake floor function
                freeTempInt = (int)((freeUVy - freePosY));
                if (freeTempInt < 0)
                {
                    freeTempInt -= 1;
                }
                targetPixelY = freeTempInt * resolutionHidden;

                //fake clamp functions
                if (targetPixelX < 0)
                {
                    targetPixelX = 0;
                }
                else if (targetPixelX > resolutionMin1)
                {
                    targetPixelX = resolutionMin1;
                }

                if (targetPixelY < 0)
                {
                    targetPixelY = 0;
                }
                else if (targetPixelY > resolution2Min1)
                {
                    targetPixelY = resolution2Min1;
                }

                uv1x = targetPixelY + targetPixelX;
                uv2x = targetPixelX + 1;
                uv3x = targetPixelY + targetPixelX + resolutionHidden;
                uv4x = targetPixelX + 1;
                //fake clamp functions.
                if (uv1x < 0)
                {
                    uv1x = 0;
                }
                if (uv1x > resolution2Min1)
                {
                    uv1x = resolution2Min1;
                }

                if (uv2x < 0)
                {
                    uv2x = 0;
                }
                if (uv2x > resolutionMin1)
                {
                    uv2x = resolutionMin1;
                }

                uv2x += targetPixelY;

                if (uv2x > resolution2Min1)
                {
                    uv2x = resolution2Min1;
                }

                if (uv3x < 0)
                {
                    uv3x = 0;
                }
                if (uv3x > resolution2Min1)
                {
                    uv3x = resolution2Min1;
                }

                if (uv4x < 0)
                {
                    uv4x = 0;
                }
                if (uv4x > resolutionMin1)
                {
                    uv4x = resolutionMin1;
                }

                uv4x += (targetPixelY + resolutionHidden);

                if (uv4x > resolution2Min1)
                {
                    uv4x = resolution2Min1;
                }

                //now that we have the target location, we need to Bilinear Lerp the location target.
                tempfloatX01 = fluidRenderTextureSourceFreeTempSA[(int)uv1x].r;
                tempfloatX02 = fluidRenderTextureSourceFreeTempSA[(int)uv2x].r;
                tempfloatX03 = fluidRenderTextureSourceFreeTempSA[(int)uv3x].r;
                tempfloatX04 = fluidRenderTextureSourceFreeTempSA[(int)uv4x].r;

                tempfloatY01 = fluidRenderTextureSourceFreeTempSA[(int)uv1x].g;
                tempfloatY02 = fluidRenderTextureSourceFreeTempSA[(int)uv2x].g;
                tempfloatY03 = fluidRenderTextureSourceFreeTempSA[(int)uv3x].g;
                tempfloatY04 = fluidRenderTextureSourceFreeTempSA[(int)uv4x].g;

                tempfloatZ01 = fluidRenderTextureSourceFreeTempSA[(int)uv1x].b;
                tempfloatZ02 = fluidRenderTextureSourceFreeTempSA[(int)uv2x].b;
                tempfloatZ03 = fluidRenderTextureSourceFreeTempSA[(int)uv3x].b;
                tempfloatZ04 = fluidRenderTextureSourceFreeTempSA[(int)uv4x].b;

                //fake floor function
                freeTempInt = (int)freePosX;
                if (freeTempInt < 0)
                {
                    freeTempInt -= 1;
                }
                tempFracX = freePosX - freeTempInt;

                //fake floor function
                freeTempInt = (int)freePosY;
                if (freeTempInt < 0)
                {
                    freeTempInt -= 1;
                }
                tempFracY = freePosY - freeTempInt;

                //fake clamp functions
                if (tempFracX > 0)
                {
                    tempFracX = 1 - tempFracX;
                }
                else if (tempFracX < 0)
                {
                    tempFracX = tempFracX * -1;
                }

                if (tempFracY > 0)
                {
                    tempFracY = 1 - tempFracY;
                }
                else if (tempFracY < 0)
                {
                    tempFracY = tempFracY * -1;
                }

                //fake lerp function
                blendA = (tempfloatX01 * (1 - tempFracX)) + (tempfloatX02 * tempFracX);
                blendB = (tempfloatX03 * (1 - tempFracX)) + (tempfloatX04 * tempFracX);
                outputX = (blendA * (1 - tempFracY)) + (blendB * tempFracY);

                blendA = (tempfloatY01 * (1 - tempFracX)) + (tempfloatY02 * tempFracX);
                blendB = (tempfloatY03 * (1 - tempFracX)) + (tempfloatY04 * tempFracX);
                outputY = (blendA * (1 - tempFracY)) + (blendB * tempFracY);

                blendA = (tempfloatZ01 * (1 - tempFracX)) + (tempfloatZ02 * tempFracX);
                blendB = (tempfloatZ03 * (1 - tempFracX)) + (tempfloatZ04 * tempFracX);
                outputZ = (blendA * (1 - tempFracY)) + (blendB * tempFracY);

                freeTempFloat = ((1 - collisionDynamicRenderTextureSourceFreeTempSA[tempColorPixelCount]) * 0.01f) + freeColorDissipation;

                outputX = (outputX * (1 - freeTempFloat)) + ((colorDissipateTo.r * 255) * freeTempFloat);
                outputY = (outputY * (1 - freeTempFloat)) + ((colorDissipateTo.g * 255) * freeTempFloat);
                outputZ = (outputZ * (1 - freeTempFloat)) + ((colorDissipateTo.b * 255) * freeTempFloat);

                fluidRenderTextureDestinationFreeTempSA[tempColorPixelCount] = new Color32((byte)outputX, (byte)outputY, (byte)outputZ, 255);

                tempColorPixelCount++;
            }
        }

        for (i = 0; i <= resolution2Min1; i++)
        {
            fluidRenderTextureSourceFreeTempSA[i] = fluidRenderTextureDestinationFreeTempSA[i];
        }
    }

    //========

    void advectionVelocityMatFree()
    {
        tempColorPixelCount = 0;

        freeVelocityDissipation = (1 - velocityDissipation) + (0.5f * velocityDissipation);

        for (velocityRenderTextureSourceFreeTempSCY = 0; velocityRenderTextureSourceFreeTempSCY < resolutionHidden; velocityRenderTextureSourceFreeTempSCY++)
        {
            freeUVy = velocityRenderTextureSourceFreeTempSCY;

            for (velocityRenderTextureSourceFreeTempSCX = 0; velocityRenderTextureSourceFreeTempSCX < resolutionHidden; velocityRenderTextureSourceFreeTempSCX++)
            {
                freeUVx = velocityRenderTextureSourceFreeTempSCX;

                freePosX = 40 * timeStep * simStrength * velocityRenderTextureSourceFreeTempSA[tempColorPixelCount].x;
                freePosY = 40 * timeStep * simStrength * velocityRenderTextureSourceFreeTempSA[tempColorPixelCount].y;

                //fake floor function
                freeTempInt = (int)(freeUVx - freePosX);
                if (freeTempInt < 0)
                {
                    freeTempInt -= 1;
                }
                targetPixelX = freeTempInt;
                //fake floor function
                freeTempInt = (int)(freeUVy - freePosY);
                if (freeTempInt < 0)
                {
                    freeTempInt -= 1;
                }
                targetPixelY = freeTempInt * resolutionHidden;

                //fake clamp functions
                if (targetPixelX < 0)
                {
                    targetPixelX = 0;
                }
                else if (targetPixelX > resolutionMin1)
                {
                    targetPixelX = resolutionMin1;
                }

                if (targetPixelY < 0)
                {
                    targetPixelY = 0;
                }
                else if (targetPixelY > resolution2Min1)
                {
                    targetPixelY = resolution2Min1;
                }

                uv1x = targetPixelY + targetPixelX;
                uv2x = targetPixelX + 1;
                uv3x = targetPixelY + targetPixelX + resolutionHidden;
                uv4x = targetPixelX + 1;
                //fake clamp functions
                if (uv1x > resolution2Min1)
                {
                    uv1x = resolution2Min1;
                }

                if (uv2x > resolutionMin1)
                {
                    uv2x = resolutionMin1;
                }

                uv2x += targetPixelY;

                if (uv2x > resolution2Min1)
                {
                    uv2x = resolution2Min1;
                }

                if (uv3x > resolution2Min1)
                {
                    uv3x = resolution2Min1;
                }

                if (uv4x > resolutionMin1)
                {
                    uv4x = resolutionMin1;
                }

                uv4x += (targetPixelY + resolutionHidden);

                if (uv4x > resolution2Min1)
                {
                    uv4x = resolution2Min1;
                }

                //Now that we have the target location we need, get the bilinear lerp location of target location.
                tempfloatX01 = velocityRenderTextureSourceFreeTempSA[(int)uv1x].x;
                tempfloatX02 = velocityRenderTextureSourceFreeTempSA[(int)uv2x].x;
                tempfloatX03 = velocityRenderTextureSourceFreeTempSA[(int)uv3x].x;
                tempfloatX04 = velocityRenderTextureSourceFreeTempSA[(int)uv4x].x;

                tempfloatY01 = velocityRenderTextureSourceFreeTempSA[(int)uv1x].y;
                tempfloatY02 = velocityRenderTextureSourceFreeTempSA[(int)uv2x].y;
                tempfloatY03 = velocityRenderTextureSourceFreeTempSA[(int)uv3x].y;
                tempfloatY04 = velocityRenderTextureSourceFreeTempSA[(int)uv4x].y;

                //fake floor function
                freeTempInt = (int)freePosX;
                if (freeTempInt < 0)
                {
                    freeTempInt -= 1;
                }
                tempFracX = freePosX - freeTempInt;
                //fake floor function
                freeTempInt = (int)freePosY;
                if (freeTempInt < 0)
                {
                    freeTempInt -= 1;
                }
                tempFracY = freePosY - freeTempInt;

                //invert and ABS() checks
                if (tempFracX > 0)
                {
                    tempFracX = 1 - tempFracX;
                }
                else if (tempFracX < 0)
                {
                    tempFracX = tempFracX * -1;
                }

                if (tempFracY > 0)
                {
                    tempFracY = 1 - tempFracY;
                }
                else if (tempFracY < 0)
                {
                    tempFracY = tempFracY * -1;
                }

                //fake lerp functions
                blendA = (tempfloatX01 * (1 - tempFracX)) + (tempfloatX02 * tempFracX);
                blendB = (tempfloatX03 * (1 - tempFracX)) + (tempfloatX04 * tempFracX);
                outputX = (blendA * (1 - tempFracY)) + (blendB * tempFracY);

                blendA = (tempfloatY01 * (1 - tempFracX)) + (tempfloatY02 * tempFracX);
                blendB = (tempfloatY03 * (1 - tempFracX)) + (tempfloatY04 * tempFracX);
                outputY = (blendA * (1 - tempFracY)) + (blendB * tempFracY);

                freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[tempColorPixelCount];

                velocityRenderTextureDestinationFreeTempSA[tempColorPixelCount] = new Vector2((outputX * freeTempFloat * freeVelocityDissipation), (outputY * freeTempFloat * freeVelocityDissipation));

                tempColorPixelCount++;
            }
        }

        for (i = 0; i <= resolution2Min1; i++)
        {
            velocityRenderTextureSourceFreeTempSA[i] = velocityRenderTextureDestinationFreeTempSA[i];
        }
    }

    //========

    void divergenceMatFree()
    {
        tempColorPixelCount = 0;

        for (divergenceRenderTextureSourceFreeTempSCY = 0; divergenceRenderTextureSourceFreeTempSCY < resolutionHidden; divergenceRenderTextureSourceFreeTempSCY++)
        {
            freeUVy = divergenceRenderTextureSourceFreeTempSCY * resolutionHidden;

            for (divergenceRenderTextureSourceFreeTempSCX = 0; divergenceRenderTextureSourceFreeTempSCX < resolutionHidden; divergenceRenderTextureSourceFreeTempSCX++)
            {
                freeUVx = divergenceRenderTextureSourceFreeTempSCX;

                uv1x = freeUVy + freeUVx - resolutionHidden;
                uv2x = freeUVy + freeUVx + resolutionHidden;
                uv3x = freeUVx - 1;
                uv4x = freeUVx + 1;
                //fake clamp functions
                if (uv1x < 0)
                {
                    uv1x = 0;
                }

                if (uv2x > resolution2Min1)
                {
                    uv2x = resolution2Min1;
                }

                if (uv3x < 0)
                {
                    uv3x = 0;
                }

                uv3x += freeUVy;

                if (uv4x > resolutionMin1)
                {
                    uv4x = resolutionMin1;
                }

                uv4x += freeUVy;

                freeTempVelocity = velocityRenderTextureSourceFreeTempSA[(int)uv1x];
                freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[(int)uv1x];
                vB = freeTempVelocity.y * freeTempFloat;

                freeTempVelocity = velocityRenderTextureSourceFreeTempSA[(int)uv2x];
                freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[(int)uv2x];
                vT = freeTempVelocity.y * freeTempFloat;

                freeTempVelocity = velocityRenderTextureSourceFreeTempSA[(int)uv3x];
                freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[(int)uv3x];
                vL = freeTempVelocity.x * freeTempFloat;

                freeTempVelocity = velocityRenderTextureSourceFreeTempSA[(int)uv4x];
                freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[(int)uv4x];
                vR = freeTempVelocity.x * freeTempFloat;

                freeTempFloat = ((((vR - vL) + (vT - vB)) * 0.5f));

                divergenceRenderTextureSourceFreeTempSA[tempColorPixelCount] = freeTempFloat;

                tempColorPixelCount++;
            }
        }
    }

    //========

    void jacobiRelaxMatFree()
    {
        tempColorPixelCount = 0;

        for (pressureRenderTextureSourceFreeTempSCY = 0; pressureRenderTextureSourceFreeTempSCY < resolutionHidden; pressureRenderTextureSourceFreeTempSCY++)
        {
            freeUVy = pressureRenderTextureSourceFreeTempSCY * resolutionHidden;

            for (pressureRenderTextureSourceFreeTempSCX = 0; pressureRenderTextureSourceFreeTempSCX < resolutionHidden; pressureRenderTextureSourceFreeTempSCX++)
            {
                freeUVx = pressureRenderTextureSourceFreeTempSCX;

                uv1x = freeUVy + freeUVx - resolutionHidden;
                uv2x = freeUVy + freeUVx + resolutionHidden;
                uv3x = freeUVx - 1;
                uv4x = freeUVx + 1;
                //fake clamp functions
                if (uv1x < 0)
                {
                    uv1x = 0;
                }

                if (uv2x > resolution2Min1)
                {
                    uv2x = resolution2Min1;
                }

                if (uv3x < 0)
                {
                    uv3x = 0;
                }

                uv3x += freeUVy;

                if (uv4x > resolutionMin1)
                {
                    uv4x = resolutionMin1;
                }

                uv4x += freeUVy;

                //		xC = pressureRenderTextureSourceFreeTempSA[tempColorPixelCount];
                xB = pressureRenderTextureSourceFreeTempSA[(int)uv1x];
                xT = pressureRenderTextureSourceFreeTempSA[(int)uv2x];
                xL = pressureRenderTextureSourceFreeTempSA[(int)uv3x];
                xR = pressureRenderTextureSourceFreeTempSA[(int)uv4x];

                bC = divergenceRenderTextureSourceFreeTempSA[tempColorPixelCount];

                //collision checks using fake lerp function ... turned off because of performance impact vs visual payoff.  Left in for users to turn on.
                /*		freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[uv1x];
                        xB = (xC * (1 - freeTempFloat)) + (xB * freeTempFloat);
                        freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[uv2x];
                        xT = (xC * (1 - freeTempFloat)) + (xT * freeTempFloat);
                        freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[uv3x];
                        xL = (xC * (1 - freeTempFloat)) + (xL * freeTempFloat);
                        freeTempFloat = collisionDynamicRenderTextureSourceFreeTempSA[uv4x];
                        xR = (xC * (1 - freeTempFloat)) + (xR * freeTempFloat);					*/

                freeTempFloat = (((xL + xR + xB + xT - bC) / 4));

                pressureRenderTextureSourceFreeTempSA[tempColorPixelCount] = freeTempFloat;

                tempColorPixelCount++;
            }
        }
    }

    //========

    void gradientMatFree()
    {
        tempColorPixelCount = 0;

        for (velocityRenderTextureSourceFreeTempSCY = 0; velocityRenderTextureSourceFreeTempSCY < resolutionHidden; velocityRenderTextureSourceFreeTempSCY++)
        {
            freeUVy = velocityRenderTextureSourceFreeTempSCY * resolutionHidden;

            for (velocityRenderTextureSourceFreeTempSCX = 0; velocityRenderTextureSourceFreeTempSCX < resolutionHidden; velocityRenderTextureSourceFreeTempSCX++)
            {
                freeUVx = velocityRenderTextureSourceFreeTempSCX;

                vMask = vMaskOne;
                obstV = obstVZero;

                uv1x = freeUVy + freeUVx - resolutionHidden;
                uv2x = freeUVy + freeUVx + resolutionHidden;
                uv3x = freeUVx - 1;
                uv4x = freeUVx + 1;
                //fake clamp functions
                if (uv1x < 0)
                {
                    uv1x = 0;
                }

                if (uv2x > resolution2Min1)
                {
                    uv2x = resolution2Min1;
                }

                if (uv3x < 0)
                {
                    uv3x = 0;
                }

                uv3x += freeUVy;

                if (uv4x > resolutionMin1)
                {
                    uv4x = resolutionMin1;
                }

                uv4x += freeUVy;

                vBx = collisionDynamicVelocityRenderTextureSourceFreeTempSA[(int)uv1x].x;
                vTx = collisionDynamicVelocityRenderTextureSourceFreeTempSA[(int)uv2x].x;
                vLx = collisionDynamicVelocityRenderTextureSourceFreeTempSA[(int)uv3x].y;
                vRx = collisionDynamicVelocityRenderTextureSourceFreeTempSA[(int)uv4x].y;

                pC = pressureRenderTextureSourceFreeTempSA[tempColorPixelCount];
                pB = pressureRenderTextureSourceFreeTempSA[(int)uv1x];
                pT = pressureRenderTextureSourceFreeTempSA[(int)uv2x];
                pL = pressureRenderTextureSourceFreeTempSA[(int)uv3x];
                pR = pressureRenderTextureSourceFreeTempSA[(int)uv4x];

                if (collisionDynamicRenderTextureSourceFreeTempSA[(int)uv1x] < 0.5f)
                {
                    pB = pC;
                    obstV.x = vTx;
                    vMask.x = 0;
                }
                if (collisionDynamicRenderTextureSourceFreeTempSA[(int)uv2x] < 0.5f)
                {
                    pT = pC;
                    obstV.x = vBx;
                    vMask.x = 0;
                }
                if (collisionDynamicRenderTextureSourceFreeTempSA[(int)uv3x] < 0.5f)
                {
                    pL = pC;
                    obstV.y = vLx;
                    vMask.y = 0;
                }
                if (collisionDynamicRenderTextureSourceFreeTempSA[(int)uv4x] < 0.5f)
                {
                    pR = pC;
                    obstV.y = vRx;
                    vMask.y = 0;
                }

                tempVector2.x = ((velocityRenderTextureSourceFreeTempSA[tempColorPixelCount].x - (pR - pL)) * vMask.x) + obstV.x;
                tempVector2.y = ((velocityRenderTextureSourceFreeTempSA[tempColorPixelCount].y - (pT - pB)) * vMask.y) + obstV.y;

                velocityRenderTextureSourceFreeTempSA[tempColorPixelCount] = tempVector2;

                tempColorPixelCount++;
            }
        }
    }

    //========

    void boundaryOpMatFree(bool tempUseBoundary)
    {
        tempColorPixelCount = 0;

        if (tempUseBoundary)
        {
            freeTempFloat = 0.0f;
        }
        else
        {
            freeTempFloat = 1.0f;
        }

        for (collisionStaticRenderTextureSourceFreeTempSCY = 0; collisionStaticRenderTextureSourceFreeTempSCY < resolutionHidden; collisionStaticRenderTextureSourceFreeTempSCY++)
        {
            for (collisionStaticRenderTextureSourceFreeTempSCX = 0; collisionStaticRenderTextureSourceFreeTempSCX < resolutionHidden; collisionStaticRenderTextureSourceFreeTempSCX++)
            {
                if (collisionStaticRenderTextureSourceFreeTempSCX == 0 || collisionStaticRenderTextureSourceFreeTempSCX == resolutionMin1 || collisionStaticRenderTextureSourceFreeTempSCY == 0 || collisionStaticRenderTextureSourceFreeTempSCY == resolutionMin1)
                {
                    collisionStaticRenderTextureSourceFreeTempSA[tempColorPixelCount] = freeTempFloat;
                    collisionDynamicRenderTextureSourceFreeTempSA[tempColorPixelCount] = freeTempFloat;
                }
                else
                {
                    collisionStaticRenderTextureSourceFreeTempSA[tempColorPixelCount] = 1.0f;
                    collisionDynamicRenderTextureSourceFreeTempSA[tempColorPixelCount] = 1.0f;
                }

                tempColorPixelCount++;
            }
        }
    }

    //========

    void clearPressureToValueMatFree()
    {
        System.Array.Copy(floatZeroRenderTextureSourceFreeTempSA, pressureRenderTextureSourceFreeTempSA, resolution2);
    }

    //========

    void clearDivergenceToValueMatFree()
    {
        System.Array.Copy(floatZeroRenderTextureSourceFreeTempSA, divergenceRenderTextureSourceFreeTempSA, resolution2);
    }

    //========

    void clearStaticCollisionToValueMatFree()
    {
        System.Array.Copy(floatOneRenderTextureSourceFreeTempSA, collisionStaticRenderTextureSourceFreeTempSA, resolution2);
    }

    //========

    void clearDynamicCollisionToValueMatFree()
    {
        System.Array.Copy(floatOneRenderTextureSourceFreeTempSA, collisionDynamicRenderTextureSourceFreeTempSA, resolution2);
    }

    //========

    void clearColorToValueMatFree(Color tempValue)
    {
        tempColorPixelCount = 0;

        for (fluidRenderTextureSourceFreeTempSCY = 0; fluidRenderTextureSourceFreeTempSCY < resolutionHidden; fluidRenderTextureSourceFreeTempSCY++)
        {
            for (fluidRenderTextureSourceFreeTempSCX = 0; fluidRenderTextureSourceFreeTempSCX < resolutionHidden; fluidRenderTextureSourceFreeTempSCX++)
            {
                fluidRenderTextureSourceFreeTempSA[tempColorPixelCount] = tempValue;

                tempColorPixelCount++;
            }
        }
    }

    //========

    void clearVelocityToValueMatFree(Vector2 tempValue)
    {
        tempColorPixelCount = 0;

        if (tempValue == Vector2.zero)
        {
            System.Array.Copy(vector2ZeroRenderTextureSourceFreeTempSA, velocityRenderTextureSourceFreeTempSA, resolution2);
        }
        else
        {
            for (velocityRenderTextureSourceFreeTempSCY = 0; velocityRenderTextureSourceFreeTempSCY < resolutionHidden; velocityRenderTextureSourceFreeTempSCY++)
            {
                for (velocityRenderTextureSourceFreeTempSCX = 0; velocityRenderTextureSourceFreeTempSCX < resolutionHidden; velocityRenderTextureSourceFreeTempSCX++)
                {
                    velocityRenderTextureSourceFreeTempSA[tempColorPixelCount] = tempValue;

                    tempColorPixelCount++;
                }
            }
        }
    }

    //========

    void initializeToTextureMatFree(Texture2D tempTexture)
    {
        tempColorPixelCount = 0;

        if (tempTexture)
        {
            for (fluidRenderTextureSourceFreeTempSCY = 0; fluidRenderTextureSourceFreeTempSCY < resolutionHidden; fluidRenderTextureSourceFreeTempSCY++)
            {
                for (fluidRenderTextureSourceFreeTempSCX = 0; fluidRenderTextureSourceFreeTempSCX < resolutionHidden; fluidRenderTextureSourceFreeTempSCX++)
                {
                    tempColor = tempTexture.GetPixelBilinear((fluidRenderTextureSourceFreeTempSCX * 1.0f) / resolutionHidden, (fluidRenderTextureSourceFreeTempSCY * 1.0f) / resolutionHidden);

                    fluidRenderTextureSourceFreeTempSA[tempColorPixelCount] = tempColor;

                    tempColorPixelCount++;
                }
            }
        }
    }

    //========

    void initializeToValueMatFree(Color tempColor)
    {
        tempColorPixelCount = 0;

        for (fluidRenderTextureSourceFreeTempSCY = 0; fluidRenderTextureSourceFreeTempSCY < resolutionHidden; fluidRenderTextureSourceFreeTempSCY++)
        {
            for (fluidRenderTextureSourceFreeTempSCX = 0; fluidRenderTextureSourceFreeTempSCX < resolutionHidden; fluidRenderTextureSourceFreeTempSCX++)
            {
                fluidRenderTextureSourceFreeTempSA[tempColorPixelCount] = tempColor;

                tempColorPixelCount++;
            }
        }
    }

    //========

    void initializeVelToValueMatFree()
    {
        tempColorPixelCount = 0;

        for (velocityRenderTextureSourceFreeTempSCY = 0; velocityRenderTextureSourceFreeTempSCY < resolutionHidden; velocityRenderTextureSourceFreeTempSCY++)
        {
            for (velocityRenderTextureSourceFreeTempSCX = 0; velocityRenderTextureSourceFreeTempSCX < resolutionHidden; velocityRenderTextureSourceFreeTempSCX++)
            {
                velocityRenderTextureSourceFreeTempSA[tempColorPixelCount] = new Vector2(startingVelocity.x, startingVelocity.y);

                tempColorPixelCount++;
            }
        }
    }

    //========

    void initializeVelToTextureMatFree(Texture2D tempTexture)
    {
        tempColorPixelCount = 0;

        if (tempTexture)
        {
            for (velocityRenderTextureSourceFreeTempSCY = 0; velocityRenderTextureSourceFreeTempSCY < resolutionHidden; velocityRenderTextureSourceFreeTempSCY++)
            {
                for (velocityRenderTextureSourceFreeTempSCX = 0; velocityRenderTextureSourceFreeTempSCX < resolutionHidden; velocityRenderTextureSourceFreeTempSCX++)
                {
                    tempColor = tempTexture.GetPixelBilinear((velocityRenderTextureSourceFreeTempSCX * 1.0f) / resolutionHidden, (velocityRenderTextureSourceFreeTempSCY * 1.0f) / resolutionHidden);

                    velocityRenderTextureSourceFreeTempSA[tempColorPixelCount] = new Vector2((tempColor.r - 0.5f) * 2, (tempColor.g - 0.5f) * 2);

                    tempColorPixelCount++;
                }
            }
        }
    }

    //========

    void initializeCollisionToValueMatFree()
    {
        tempColorPixelCount = 0;

        if (startingCollisionTextureSource && useStartCollisionTexture)
        {
            for (collisionStaticRenderTextureSourceFreeTempSCY = 0; collisionStaticRenderTextureSourceFreeTempSCY < resolutionHidden; collisionStaticRenderTextureSourceFreeTempSCY++)
            {
                for (collisionStaticRenderTextureSourceFreeTempSCX = 0; collisionStaticRenderTextureSourceFreeTempSCX < resolutionHidden; collisionStaticRenderTextureSourceFreeTempSCX++)
                {
                    freeTempFloat = startingCollisionTextureSource.GetPixelBilinear((collisionStaticRenderTextureSourceFreeTempSCX * 1.0f) / resolutionHidden, (collisionStaticRenderTextureSourceFreeTempSCY * 1.0f) / resolutionHidden).b;

                    collisionStaticRenderTextureSourceFreeTempSA[tempColorPixelCount] *= freeTempFloat;
                    collisionDynamicRenderTextureSourceFreeTempSA[tempColorPixelCount] *= freeTempFloat;

                    tempColorPixelCount++;
                }
            }
        }
    }
}