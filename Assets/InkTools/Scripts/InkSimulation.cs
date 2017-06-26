using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[AddComponentMenu("Inkling/Ink Simulation")]
public class InkSimulation : MonoBehaviour
{

    private InkSwapBuffer _fluidBuffer;
    private InkSwapBuffer _velocityBuffer;
    private InkSwapBuffer _pressureBuffer;
    private InkSwapBuffer _divergenceBuffer;
    private InkSwapBuffer _collisionDynamicBuffer;
    private InkSwapBuffer _collisionStaticBuffer;

    public Color startingColor = Color.black;
    public Texture2D startingColorTexture;

    public Vector2 startingVelocity = new Vector2(0.0f, 0.0f);
    public Texture2D startingVelocityTexture;

    public Texture2D startingCollisionTexture;

    public Texture2D colorDissipationTexture;
    public Texture2D velocityDissipationTexture;

    private Texture2D _tempStorageBufferTexture;
    private Color32[] _tempStorageArray = new Color32[64];
    private int _tempVelStorageCounter = 0;
    private int _tempColorStorageCounter = 0;
    private int _tempCollisionStorageCounter = 0;

    private Material _impulsePositionMat;
    private Material _impulsePosition4Mat;
    private Material _impulsePosition8Mat;
    private Material _impulsePositionTexMat;
    private Material _impulsePositionVelMat;
    private Material _impulsePositionVel4Mat;
    private Material _impulsePositionVel8Mat;
    private Material _impulsePositionVelTexMat;
    private Material _advectionColorMat;
    private Material _advectionColorTexMat;
    private Material _advectionVelocityMat;
    private Material _advectionVelocityTexMat;
    private Material _divergenceMat;
    private Material _jacobiRelaxMat;
    private Material _gradientMat;
    private Material _boundaryOpMat;
    private Material _initializeToValueMat;
    private Material _initializeCollisionToTextureMat;
    private Material _initializeToTextureMat;
    private Material _initializeVelToValueMat;
    private Material _initializeVelToTextureMat;
    private Material _setCollisionMat;
    private Material _setCollision4Mat;
    private Material _setCollision8Mat;
    private Material _setCollisionTexMat;

    public InkActor[] actorStaticArray;
    public InkActor[] actorDynamicArray;

    private float _timeStep = 0.02f;
    public int fluidUpdateFPS = 25;
    public int impulseUpdateFPS = 25;
    public int colorUpdateFPS = 25;
    public int collisionUpdateFPS = 25;
    public int resolution = 64;
    public int collisionResolution = 64;
    public int velocityResolution = 64;
    public int resolutionIndex = 1;
    public int colorResolutionIndex = 1;
    public int collisionResolutionIndex = 1;
    public int velocityResolutionIndex = 1;
    private float _fluidrdx;
    private float _collisionrdx;
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

    private Vector3 _tempPosition;
    private Vector3 _tempActorPosition;
    private Vector3 _tempVector;
    private float _tempVelocityStrength;
    private float _tempVelocitySize;
    private float _tempCollisionSize;
    private float _tempColorSize;
    private Color _tempImpulseColor;

    //private Transform _transform;
    //private Vector3 _transformPosition;
    //private Vector3 _transformLocalScale;

    public int dynamicInputArrayLength;

    private Vector3 _boundsSize;
    private float _boundsMagnitude;

    private Vector3 _tempPositionOffset;

    private Vector3 _textureForwardDirection;
    private float _textureRotationAngle;
    private Quaternion _textureRotationQuaternion;
    private Matrix4x4 _textureRotationMatrix;

    public bool useStartColorTexture = false;
    public bool useStartVelocityTexture = false;
    public bool useBoundaryCollision = true;
    public bool useStartCollisionTexture = false;

    private GameObject _inkConnectorObject;
    private InkConnector _inkConnectorScript;

    public int pressureIteration = 20;

    public int outputTextureNum = 1;

    public Material materialTarget;
    public string materialTextureSlot = "_MainTex";

    private Vector3 _collisionVelocity;

    private Mesh _mesh;

    private bool _collisionCopied;

    private int _frameDelay = 0;

    private bool _shouldUpdateActorArray = false;

    private bool _blitVelShader = false;
    private bool _blitColorShader = false;
    private bool _blitCollisionShader = false;

    private float _kEncodeBit = 1.0f / 255.0f;
    private Vector2 _enc2;
    private Vector2 _kEncodeMul2 = new Vector2(1.0f, 255.0f);
    private Vector2 _kDecodeDot2 = new Vector2(1.0f, 1.0f / 255.0f);

    private Color _tempColor1;
    private Color32 _tempCompressedColor1;
    private Color32 _tempCompressedColor2;

    private Vector4 _tempShaderVectorData;

    public bool shouldUpdateColorWithFluid = true;
    private bool _shouldUpdateColorWithFluid = true;
    public bool shouldUpdateImpulseWithFluid = true;
    private bool _shouldUpdateImpulseWithFluid = true;
    public bool shouldUpdateCollisionWithFluid = true;
    private bool _shouldUpdateCollisionWithFluid = true;

    private Color[] _tempColorArray;

    private float _vL;
    private float _vR;
    private float _vB;
    private float _vT;

    private float _pL;
    private float _pR;
    private float _pB;
    private float _pT;
    private float _pC;

    private Vector3 _vector3Zero = new Vector3(0, 0, 0);
    private Vector2 _vector2Zero = new Vector2(0, 0);

    private Vector3 _tempVector3;
    private Vector2 _tempVector2;

    private float _tempCurrentTime = 0.0f;

    private Color _tempColor;

    private Color32 _color32Zero = new Color32(0, 0, 0, 0);

    public bool useMyMaterial = true;

    private const float _gamma = 0.577f; // Euler-Mascheroni constant

    public enum uniqueResOptionsData
    {
        _32x32 = 0,
        _64x64 = 1,
        _128x128 = 2,
        _256x256 = 3,
        _512x512 = 4,
        _1024x1024 = 5
    }

    public uniqueResOptionsData uniqueCollisionResOptions = uniqueResOptionsData._1024x1024;
    public uniqueResOptionsData uniqueColorResOptions = uniqueResOptionsData._1024x1024;
    public uniqueResOptionsData uniqueVelocityResOptions = uniqueResOptionsData._1024x1024;

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

    public resOptionsData resOptions = resOptionsData._1024x1024;

    public enum outputTextureData
    {
        Collision_Buffer = 0,
        Color_Buffer = 1,
        Velocity_Buffer = 2
    }

    public outputTextureData outputTexture = outputTextureData.Color_Buffer;

    //=============================================================================================

    private void Awake()
    {
        _inkConnectorObject = InkConnector.GetObjInstance();
        _inkConnectorScript = InkConnector.GetScriptInstance();
        _inkConnectorScript.RegisterInkActor(this);

        if(_inkConnectorObject == null)
        {
            Debug.LogError( "InkSimulation failed to find or create a InkFluidConnector object."
                          + "  Make sure the InkFluidConnector script exists and can be found by"
                          + " the InkSimulation."
                          );
        }

        if (_inkConnectorScript == null)
        {
            Debug.LogError("InkSimulation failed to find or create a InkFluidConnector script."
                          + "  Make sure the InkFluidConnector script exists and can be found by"
                          + " the InkSimulation."
                          );
        }
    }

    //=============================================================================================

    // Initialization
    void Start ()
    {
        //_transform = transform;
        //_transformPosition = transform.position;

        _mesh = GetComponent<MeshFilter>().mesh;

        _fluidBuffer = new InkSwapBuffer(resolution, resolution, true);
        _velocityBuffer = new InkSwapBuffer(velocityResolution, velocityResolution, false);
        _pressureBuffer = new InkSwapBuffer(resolution, resolution, false);
        _divergenceBuffer = new InkSwapBuffer(resolution, resolution, false);
        _collisionDynamicBuffer = new InkSwapBuffer(collisionResolution, collisionResolution, false);
        _collisionStaticBuffer = new InkSwapBuffer(collisionResolution, collisionResolution, false);

        Shader shader = null;

        shader = Shader.Find("Inkling/impulsePositionShader");
        if (!shader)
            Debug.LogError("Inkling/impulsePositionShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePositionMat = new Material(shader);

        shader = Shader.Find("Inkling/impulsePosition4Shader");
        if (!shader)
            Debug.LogError("Inkling/impulsePosition4Shader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePosition4Mat = new Material(shader);

        shader = Shader.Find("Inkling/impulsePosition8Shader");
        if (!shader)
            Debug.LogError("Inkling/impulsePosition8Shader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePosition8Mat = new Material(shader);

        shader = Shader.Find("Inkling/impulsePositionTexShader");
        if (!shader)
            Debug.LogError("Inkling/impulsePositionTexShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePositionTexMat = new Material(shader);

        shader = Shader.Find("Inkling/impulsePositionVelShader");
        if (!shader)
            Debug.LogError("Inkling/impulsePositionVelShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePositionVelMat = new Material(shader);

        shader = Shader.Find("Inkling/impulsePositionVel4Shader");
        if (!shader)
            Debug.LogError("Inkling/impulsePositionVel4Shader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePositionVel4Mat = new Material(shader);

        shader = Shader.Find("Inkling/impulsePositionVel8Shader");
        if (!shader)
            Debug.LogError("Inkling/impulsePositionVel8Shader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePositionVel8Mat = new Material(shader);

        shader = Shader.Find("Inkling/impulsePositionVelTexShader");
        if (!shader)
            Debug.LogError("Inkling/impulsePositionVelTexShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePositionVelTexMat = new Material(shader);

        shader = Shader.Find("Inkling/impulsePositionVelTexShader");
        if (!shader)
            Debug.LogError("Inkling/impulsePositionVelTexShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _impulsePositionVelTexMat = new Material(shader);

        shader = Shader.Find("Inkling/advectionColorShader");
        if (!shader)
            Debug.LogError("Inkling/advectionColorShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _advectionColorMat = new Material(shader);

        shader = Shader.Find("Inkling/advectionColorTexShader");
        if (!shader)
            Debug.LogError("Inkling/advectionColorTexShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _advectionColorTexMat = new Material(shader);

        shader = Shader.Find("Inkling/advectionVelocityShader");
        if (!shader)
            Debug.LogError("Inkling/advectionVelocityShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _advectionVelocityMat = new Material(shader);

        shader = Shader.Find("Inkling/advectionVelocityTexShader");
        if (!shader)
            Debug.LogError("Inkling/advectionVelocityTexShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _advectionVelocityTexMat = new Material(shader);

        shader = Shader.Find("Inkling/divergenceShader");
        if (!shader)
            Debug.LogError("Inkling/divergenceShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _divergenceMat = new Material(shader);

        shader = Shader.Find("Inkling/jacobiRelaxShader");
        if (!shader)
            Debug.LogError("Inkling/jacobiRelaxShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _jacobiRelaxMat = new Material(shader);

        shader = Shader.Find("Inkling/gradientShader");
        if (!shader)
            Debug.LogError("Inkling/gradientShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _gradientMat = new Material(shader);

        shader = Shader.Find("Inkling/boundaryOpShader");
        if (!shader)
            Debug.LogError("Inkling/boundaryOpShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _boundaryOpMat = new Material(shader);

        shader = Shader.Find("Inkling/initializeToValueShader");
        if (!shader)
            Debug.LogError("Inkling/initializeToValueShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _initializeToValueMat = new Material(shader);

        shader = Shader.Find("Inkling/initializeCollisionToTextureShader");
        if (!shader)
            Debug.LogError("Inkling/initializeCollisionToTextureShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _initializeCollisionToTextureMat = new Material(shader);

        shader = Shader.Find("Inkling/initializeToTextureShader");
        if (!shader)
            Debug.LogError("Inkling/initializeToTextureShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _initializeToTextureMat = new Material(shader);

        shader = Shader.Find("Inkling/initializeVelToValueShader");
        if (!shader)
            Debug.LogError("Inkling/initializeVelToValueShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _initializeVelToValueMat = new Material(shader);

        shader = Shader.Find("Inkling/initializeVelToTextureShader");
        if (!shader)
            Debug.LogError("Inkling/initializeVelToTextureShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _initializeVelToTextureMat = new Material(shader);

        shader = Shader.Find("Inkling/setCollisionShader");
        if (!shader)
            Debug.LogError("Inkling/setCollisionShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _setCollisionMat = new Material(shader);

        shader = Shader.Find("Inkling/setCollision4Shader");
        if (!shader)
            Debug.LogError("Inkling/setCollision4Shader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _setCollision4Mat = new Material(shader);

        shader = Shader.Find("Inkling/setCollision8Shader");
        if (!shader)
            Debug.LogError("Inkling/setCollision8Shader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _setCollision8Mat = new Material(shader);

        shader = Shader.Find("Inkling/setCollisionTexShader");
        if (!shader)
            Debug.LogError("Inkling/setCollisionTexShader could not be found."
                          + "Fluids cannot be simulated without this shader."
                          );
        else
            _setCollisionTexMat = new Material(shader);

        _tempStorageBufferTexture = new Texture2D(16, 4, TextureFormat.ARGB32, false, true);
        _tempStorageBufferTexture.filterMode = FilterMode.Point;
        _tempStorageBufferTexture.wrapMode = TextureWrapMode.Clamp;

        _fluidrdx = 1.0f / resolution;
        _collisionrdx = 1.0f / collisionResolution;

        if (useColorDissipation)
            colorDissipation = oldColorDissipation;
        else
            colorDissipation = 0;

        if (useVelocityDissipation)
            velocityDissipation = oldVelocityDissipation;
        else
            velocityDissipation = 0;

        if (materialTarget == null)
        {
            materialTarget = Resources.Load("InklingSimpleMat") as Material;

            if (materialTarget == null)
                Debug.LogError( "Material Target is null and Unity failed to find"
                              + " \'InklingSimpleMat\" Make sure InklingSimpleMat can be found, or"
                              + " assign a Material Target to the InkSimulation."
                              );
        }

        if(useMyMaterial)
            materialTarget = GetComponent<Renderer>().material;

        if (materialTarget)
        {
            if (!materialTarget.HasProperty(materialTextureSlot))
                Debug.LogError( "The Material Texture Slot entered in InkSimulation could not"
                              + " be found in the material InkSimulation is currently trying"
                              + " to use.  Make sure the texture slot name is correct and the"
                              + " material being used has that slot name."
                              );
        }
        else
        {
            Debug.LogError( "Material Target is null on InkSimulation."
                          + "Please make sure InkSimulation has a material target set,"
                          + "or the material InklingSimpleMat is in the resources folder."
                          );
        }

        switch(outputTextureNum)
        {
            case 0:
                materialTarget.SetTexture(materialTextureSlot, _collisionDynamicBuffer.GetFrontBuffer());
                break;
            case 1:
                materialTarget.SetTexture(materialTextureSlot, _fluidBuffer.GetFrontBuffer());
                break;
            case 2:
                materialTarget.SetTexture(materialTextureSlot, _velocityBuffer.GetFrontBuffer());
                break;
        }

        if(startingColorTexture && useStartColorTexture)
        {
            _initializeToTextureMat.SetTexture("initialTexture", startingColorTexture);

            Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                         , _fluidBuffer.GetBackBuffer()
                         , _initializeToTextureMat
                         );

            //_fluidBuffer.Swap();

            Graphics.Blit( _fluidBuffer.GetBackBuffer()
                         , _fluidBuffer.GetFrontBuffer());
        }
        else
        {
            _initializeToValueMat.SetVector("initialValue", startingColor);

            Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                         , _fluidBuffer.GetBackBuffer()
                         , _initializeToValueMat
                         );

            //_fluidBuffer.Swap();

            Graphics.Blit( _fluidBuffer.GetBackBuffer()
                         , _fluidBuffer.GetFrontBuffer());
        }

        if (startingVelocityTexture && useStartVelocityTexture)
        {
            _initializeVelToTextureMat.SetTexture("initialTexture", startingVelocityTexture);

            Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                         , _velocityBuffer.GetBackBuffer()
                         , _initializeVelToTextureMat
                         );

            //_velocityBuffer.Swap();

            Graphics.Blit( _velocityBuffer.GetBackBuffer()
                         , _velocityBuffer.GetFrontBuffer());
        }
        else
        {
            _initializeVelToValueMat.SetVector("initialValue", startingVelocity);

            Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                         , _velocityBuffer.GetBackBuffer()
                         , _initializeVelToValueMat
                         );

            //_velocityBuffer.Swap();

            Graphics.Blit( _velocityBuffer.GetBackBuffer()
                         , _velocityBuffer.GetFrontBuffer());
        }

        if (startingCollisionTexture && useStartCollisionTexture)
        {
            _initializeCollisionToTextureMat.SetTexture("initialTexture", startingCollisionTexture);

            Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                         , _collisionStaticBuffer.GetBackBuffer()
                         , _initializeCollisionToTextureMat
                         );

            //_collisionStaticBuffer.Swap();

            Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                         , _collisionStaticBuffer.GetFrontBuffer());

            _initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 1.0f, 0.0f));

            Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                         , _collisionDynamicBuffer.GetBackBuffer()
                         , _initializeToValueMat
                         );

            //_collisionDynamicBuffer.Swap();

            Graphics.Blit( _collisionDynamicBuffer.GetBackBuffer()
                         , _collisionDynamicBuffer.GetFrontBuffer());
        }
        else
        {
            _initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 1.0f, 0.0f));

            Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                         , _collisionStaticBuffer.GetBackBuffer()
                         , _initializeToValueMat
                         );

            //_collisionStaticBuffer.Swap();

            Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                         , _collisionStaticBuffer.GetFrontBuffer());

            Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                         , _collisionDynamicBuffer.GetBackBuffer()
                         , _initializeToValueMat
                         );

            //_collisionDynamicBuffer.Swap();

            Graphics.Blit( _collisionDynamicBuffer.GetBackBuffer()
                         , _collisionDynamicBuffer.GetFrontBuffer());
        }

        _initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));

        Graphics.Blit( _divergenceBuffer.GetFrontBuffer()
                     , _divergenceBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_divergenceBuffer.Swap();

        Graphics.Blit( _divergenceBuffer.GetBackBuffer()
                     , _divergenceBuffer.GetFrontBuffer());

        _initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));

        Graphics.Blit( _pressureBuffer.GetFrontBuffer()
                     , _pressureBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_pressureBuffer.Swap();

        Graphics.Blit( _pressureBuffer.GetBackBuffer()
                     , _pressureBuffer.GetFrontBuffer());

        _impulsePositionVelMat.SetTexture("velTex", _velocityBuffer.GetFrontBuffer());
        _impulsePositionVelMat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _impulsePositionVelMat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _impulsePositionVel4Mat.SetTexture("velTex", _velocityBuffer.GetFrontBuffer());
        _impulsePositionVel4Mat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _impulsePositionVel4Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _impulsePositionVel8Mat.SetTexture("velTex", _velocityBuffer.GetFrontBuffer());
        _impulsePositionVel8Mat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _impulsePositionVel8Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _impulsePositionVelTexMat.SetTexture("velTex", _velocityBuffer.GetFrontBuffer());

        _impulsePositionMat.SetTexture("fluidTex", _fluidBuffer.GetFrontBuffer());
        _impulsePositionMat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _impulsePositionMat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _impulsePosition4Mat.SetTexture("fluidTex", _fluidBuffer.GetFrontBuffer());
        _impulsePosition4Mat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _impulsePosition4Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _impulsePosition8Mat.SetTexture("fluidTex", _fluidBuffer.GetFrontBuffer());
        _impulsePosition8Mat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _impulsePosition8Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _impulsePositionTexMat.SetTexture("fluidTex", _fluidBuffer.GetFrontBuffer());

        _advectionColorMat.SetTexture("velocityTexSource", _velocityBuffer.GetFrontBuffer());
        _advectionColorMat.SetTexture("targetTex", _fluidBuffer.GetFrontBuffer());
        _advectionColorMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _advectionColorMat.SetFloat("timeStep", _timeStep);
        _advectionColorMat.SetFloat("dissipation", Mathf.Lerp(0.0f, 0.5f, colorDissipation));
        _advectionColorMat.SetVector("colorDissipateTo", colorDissipateTo);
        _advectionColorMat.SetFloat("simSpeed", simStrength);

        _advectionColorTexMat.SetTexture("velocityTexSource", _velocityBuffer.GetFrontBuffer());
        _advectionColorTexMat.SetTexture("targetTex", _fluidBuffer.GetFrontBuffer());
        _advectionColorTexMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _advectionColorTexMat.SetFloat("timeStep", _timeStep);
        _advectionColorTexMat.SetTexture("dissipationTex", colorDissipationTexture);
        _advectionColorTexMat.SetFloat("simSpeed", simStrength);

        _advectionVelocityMat.SetTexture("velocityTexSource", _velocityBuffer.GetFrontBuffer());
        _advectionVelocityMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _advectionVelocityMat.SetTexture("targetTex", _velocityBuffer.GetFrontBuffer());
        _advectionVelocityMat.SetFloat("timeStep", _timeStep);
        _advectionVelocityMat.SetFloat("dissipation", Mathf.Lerp(1.0f, 0.5f, velocityDissipation));
        _advectionVelocityMat.SetFloat("simSpeed", simStrength);

        _advectionVelocityTexMat.SetTexture("velocityTexSource", _velocityBuffer.GetFrontBuffer());
        _advectionVelocityTexMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _advectionVelocityTexMat.SetTexture("targetTex", _velocityBuffer.GetFrontBuffer());
        _advectionVelocityTexMat.SetFloat("timeStep", _timeStep);
        _advectionVelocityTexMat.SetTexture("dissipationTex", velocityDissipationTexture);
        _advectionVelocityTexMat.SetFloat("simSpeed", simStrength);

        _divergenceMat.SetTexture("veloctyTex", _velocityBuffer.GetFrontBuffer());
        _divergenceMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _divergenceMat.SetFloat("rdx", _fluidrdx);

        _jacobiRelaxMat.SetTexture("pressureTex", _pressureBuffer.GetFrontBuffer());
        _jacobiRelaxMat.SetTexture("divergenceTex", _divergenceBuffer.GetFrontBuffer());
        _jacobiRelaxMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _jacobiRelaxMat.SetFloat("rdx", _fluidrdx);

        _gradientMat.SetTexture("pressureTex", _pressureBuffer.GetFrontBuffer());
        _gradientMat.SetTexture("velocityTex", _velocityBuffer.GetFrontBuffer());
        _gradientMat.SetTexture("collsionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _gradientMat.SetFloat("rdx", _fluidrdx);

        _boundaryOpMat.SetTexture("targetTex", _collisionStaticBuffer.GetFrontBuffer());
        _boundaryOpMat.SetVector("setColor", new Vector4(0, 0, 0, 0));
        _boundaryOpMat.SetFloat("rdx", _collisionrdx);

        _setCollisionMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _setCollisionMat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _setCollisionMat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _setCollision4Mat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _setCollision4Mat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _setCollision4Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _setCollision8Mat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        _setCollision8Mat.SetTexture("tempStorageBuffer", _tempStorageBufferTexture);
        _setCollision8Mat.SetFloat("halfStorageRDX", (1f / 16f) * 0.5f);

        _setCollisionTexMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());


        if(useBoundaryCollision)
        {
            Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                         , _collisionStaticBuffer.GetBackBuffer()
                         , _boundaryOpMat
                         );

            //_collisionStaticBuffer.Swap();

            Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                         , _collisionStaticBuffer.GetFrontBuffer());
        }

        _shouldUpdateColorWithFluid = shouldUpdateColorWithFluid;
        _shouldUpdateImpulseWithFluid = shouldUpdateImpulseWithFluid;
        _shouldUpdateCollisionWithFluid = shouldUpdateCollisionWithFluid;

        Invoke("UpdateStaticCollision", 0.5f);

        if(!_shouldUpdateCollisionWithFluid)
        {
            if (collisionUpdateFPS > 0)
            {
                InvokeRepeating("UpdateDynamicCollision", 0.1f, 1f / collisionUpdateFPS);
            }
        }

        if(!_shouldUpdateImpulseWithFluid)
        {
            if(impulseUpdateFPS > 0)
            {
                InvokeRepeating("UpdateImpulseActors", 0.11f, 1f / impulseUpdateFPS);
            }
        }

        if(!_shouldUpdateColorWithFluid)
        {
            if(colorUpdateFPS > 0)
            {
                InvokeRepeating("UpdateColorActors", 0.12f, 1f / colorUpdateFPS);
            }
        }

        if(fluidUpdateFPS > 0)
        {
            InvokeRepeating("UpdateFluid", 0.13f, 1f / fluidUpdateFPS);
        }
    }

    //=============================================================================================

    void UpdateStaticCollision()
    {
        _inkConnectorScript.GetActorArrayUpdate(this);

        if(actorStaticArray.Length > 0)
        {
            _setCollisionTexMat.SetTexture("collisionTex", _collisionStaticBuffer.GetFrontBuffer());

            if(actorStaticArray.Length <= 4)
            {
                _setCollision4Mat.SetTexture("collisionTex", _collisionStaticBuffer.GetFrontBuffer());
            }
            else if(actorStaticArray.Length <= 8)
            {
                _setCollision8Mat.SetTexture("collisionTex", _collisionStaticBuffer.GetFrontBuffer());
            }
            else
            {
                _setCollisionMat.SetTexture("collisionTex", _collisionStaticBuffer.GetFrontBuffer());
            }
        }

        for(int i = 0; i < _inkConnectorScript.staticArrayCount; ++i)
        {
            if (actorStaticArray[i] != null && actorStaticArray[i] is InkStaticCollider)
            {
                InkStaticCollider collider = actorStaticArray[i] as InkStaticCollider;
                if(collider.multiplySizeByScale)
                {
                    _tempCollisionSize = collider.collisionSize
                                       * collider.transform.localScale.magnitude
                                       * _gamma;
                }
                else
                {
                    _tempCollisionSize = collider.collisionSize;
                }

                if( (collider.transform.position - transform.position).magnitude
                  < (_boundsMagnitude * 0.525) + _tempCollisionSize
                  )
                {
                    if(collider.useCollisionMaskTexture && collider.collisionMaskTexture)
                    {
                        _tempPosition =
                            transform.InverseTransformPoint(collider.transform.position);

                        _tempShaderVectorData.x = ((( ( _tempPosition.x * transform.localScale.x)
                                                    / _boundsSize.x) * -1) + 1.0f) * 0.5f;

                        _tempShaderVectorData.y = ((( (_tempPosition.z * transform.localScale.z)
                                                    / _boundsSize.z) * -1) + 1.0f) * 0.5f;

                        _tempShaderVectorData.z = 0;
                        _tempShaderVectorData.w = 0;

                        _setCollisionTexMat.SetVector("textureData", _tempShaderVectorData);

                        _setCollisionTexMat.SetFloat( "collisionStrength"
                                                    , collider.collisionStrength
                                                    );

                        _setCollisionTexMat.SetTexture( "collisionTexMask"
                                                      , collider.collisionMaskTexture
                                                      );

                        _textureRotationAngle =
                            Vector3.Angle(collider.transform.forward, transform.forward);

                        if(Vector3.Dot(collider.transform.forward, transform.right) < 0)
                        {
                            _textureRotationAngle *= -1;
                        }

                        _textureRotationQuaternion = Quaternion.Euler(0, 0, _textureRotationAngle);
                        _textureRotationMatrix =
                            Matrix4x4.TRS( _vector2Zero
                                         , _textureRotationQuaternion
                                         , new Vector2
                                            ( 1f / ( 1.8f * ( _tempCollisionSize / _boundsSize.x))
                                            , 1f / ( 1.8f * ( _tempCollisionSize / _boundsSize.z))
                                            )
                                         );

                        _setCollisionTexMat.SetMatrix("rotationMatrix", _textureRotationMatrix);

                        Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                                     , _collisionStaticBuffer.GetBackBuffer()
                                     , _setCollisionTexMat
                                     );

                        //_collisionStaticBuffer.Swap();

                        Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                                     , _collisionStaticBuffer.GetFrontBuffer());
                    }
                    else
                    {
                        _tempActorPosition = collider.transform.position;
                        _tempPosition = transform.InverseTransformPoint(_tempActorPosition);

                        //positionX positionY
                        _tempCompressedColor1 = EncodeFloatRG(((((_tempPosition.x * transform.localScale.x) / _boundsSize.x) * -1) + 1.0f) * 0.5f);
                        _tempCompressedColor2 = EncodeFloatRG(((((_tempPosition.z * transform.localScale.z) / _boundsSize.z) * -1) + 1.0f) * 0.5f);
                        _tempStorageArray[_tempCollisionStorageCounter] = new Color32(_tempCompressedColor1.r, _tempCompressedColor1.g, _tempCompressedColor2.r, _tempCompressedColor2.g);
                        //collision is static and should have no velocity.  127.5 is "zero" because the shader does (*50 - 25) and is calculated out of 255, but it wont take that as a value.
                        _tempStorageArray[_tempCollisionStorageCounter + 16] = new Color32(127, 127, 0, 0);
                        //collisionSize, collisionFalloff, collisionStrength
                        _tempCompressedColor1 = EncodeFloatRG((_tempCollisionSize / _boundsSize.x) * 0.5f);
                        _tempCompressedColor2.r = (byte)(collider.collisionFalloff * 255);
                        _tempCompressedColor2.g = (byte)(collider.collisionStrength * 255);
                        _tempStorageArray[_tempCollisionStorageCounter + 32] = new Color32(_tempCompressedColor1.r, _tempCompressedColor1.g, _tempCompressedColor2.r, _tempCompressedColor2.g);

                        _tempCollisionStorageCounter++;

                        if (_tempCollisionStorageCounter > 15)
                        {
                            _tempStorageBufferTexture.SetPixels32(_tempStorageArray, 0);
                            _tempStorageBufferTexture.Apply(false);

                            Graphics.Blit(_collisionStaticBuffer.GetFrontBuffer(), _collisionStaticBuffer.GetBackBuffer(), _setCollisionMat);

                            //_collisionStaticBuffer.Swap();

                            Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                                         , _collisionStaticBuffer.GetFrontBuffer());

                            _blitCollisionShader = false;
                            _tempCollisionStorageCounter = 0;
                        }
                        else
                        {
                            _blitCollisionShader = true;
                        }
                    }
                }
            }
            else if(actorStaticArray[i] == null)
            {
                _shouldUpdateActorArray = true;
            }
        }

        if(_blitCollisionShader)
        {
            //zero out remaining pixels (if its not a full load) to prevent possible bugs with deleted actors
            for (int k = _tempCollisionStorageCounter; k < 16; k++)
            {
                _tempStorageArray[k] = _color32Zero;
                _tempStorageArray[k + 16] = _color32Zero;
                _tempStorageArray[k + 32] = _color32Zero;
            }

            _tempStorageBufferTexture.SetPixels32(_tempStorageArray, 0);
            _tempStorageBufferTexture.Apply(false);

            if (_tempCollisionStorageCounter <= 4)
            {
                Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                             , _collisionStaticBuffer.GetBackBuffer()
                             , _setCollision4Mat
                             );

                //_collisionStaticBuffer.Swap();

                Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                             , _collisionStaticBuffer.GetFrontBuffer());
            }
            else if (_tempCollisionStorageCounter <= 8)
            {
                Graphics.Blit(_collisionStaticBuffer.GetFrontBuffer()
                             , _collisionStaticBuffer.GetBackBuffer()
                             , _setCollision8Mat
                             );

                //_collisionStaticBuffer.Swap();

                Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                             , _collisionStaticBuffer.GetFrontBuffer());
            }
            else
            {
                Graphics.Blit(_collisionStaticBuffer.GetFrontBuffer()
                             , _collisionStaticBuffer.GetBackBuffer()
                             , _setCollisionMat
                             );

                //_collisionStaticBuffer.Swap();

                Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                             , _collisionStaticBuffer.GetFrontBuffer());
            }

            _blitCollisionShader = false;
            _tempCollisionStorageCounter = 0;
        }

        Graphics.Blit(_collisionStaticBuffer.GetFrontBuffer(), _collisionDynamicBuffer.GetFrontBuffer());

        if (actorStaticArray.Length > 0)
        {
            _setCollisionTexMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        }

        if (actorStaticArray.Length <= 4)
        {
            _setCollision4Mat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        }
        else if (actorStaticArray.Length <= 8)
        {
            _setCollision8Mat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        }
        else
        {
            _setCollisionMat.SetTexture("collisionTex", _collisionDynamicBuffer.GetFrontBuffer());
        }

        if (_shouldUpdateActorArray)
        {
            _inkConnectorScript.SortActorArray();
            _inkConnectorScript.GetActorArrayUpdate(this);
            _shouldUpdateActorArray = false;
        }
    }

    //=============================================================================================

    void UpdateDynamicCollision()
    {
        _collisionCopied = false;

        _tempCurrentTime = Time.time;

        //length is defined in the fluidConnector script
        for (int k = 0; k < dynamicInputArrayLength; k++)
        {
            if (!_collisionCopied)
            {
                //copy collision from static into dynamic
                Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                             , _collisionDynamicBuffer.GetFrontBuffer()
                             );

                _collisionCopied = true;
            }

            if (actorDynamicArray[k] != null && actorDynamicArray[k] is InkDynamicCollider)
            {
                InkDynamicCollider collider = actorDynamicArray[k] as InkDynamicCollider;

                if (collider.multiplySizeByScale)
                {
                    _tempCollisionSize =
                        collider.collisionSize * collider.transform.localScale.magnitude * _gamma;
                }
                else
                {
                    _tempCollisionSize = collider.collisionSize;
                }

                if (( collider.transform.position - transform.position).magnitude
                    < (_boundsMagnitude * 0.525f) + _tempCollisionSize
                   )
                {
                    if (collider.useCollisionMaskTexture && collider.collisionMaskTexture)
                    {
                        _tempActorPosition = collider.transform.position;
                        _tempPosition = transform.InverseTransformPoint(_tempActorPosition);
                        //locationX, locationY
                        _tempShaderVectorData.x =
                            ((((_tempPosition.x * transform.localScale.x)
                              / _boundsSize.x) * -1) + 1.0f) * 0.5f;

                        _tempShaderVectorData.y =
                            ((((_tempPosition.z * transform.localScale.z)
                              / _boundsSize.z) * -1) + 1.0f) * 0.5f;

                        if (_tempCurrentTime - collider.lastFrameTime < 0.5f)
                        {
                            _collisionVelocity =
                                ( transform.InverseTransformPoint(collider.lastFramePosition)
                                - _tempPosition) * collider.moveVelocityMultiplier;
                        }
                        else
                        {
                            _collisionVelocity = _vector3Zero;
                        }

                        //velocityX
                        _tempShaderVectorData.z = _collisionVelocity.x;
                        //velocityY
                        _tempShaderVectorData.w = _collisionVelocity.z;
                        _setCollisionTexMat.SetVector("textureData", _tempShaderVectorData);

                        _setCollisionTexMat.SetFloat( "collisionStrength"
                                                    , collider.collisionStrength
                                                    );

                        _setCollisionTexMat.SetTexture( "collisionTexMask"
                                                      , collider.collisionMaskTexture
                                                      );

                        _textureRotationAngle =
                            Vector3.Angle(collider.transform.forward, transform.forward);

                        if (Vector3.Dot(collider.transform.forward, transform.right) < 0)
                        {
                            _textureRotationAngle *= -1;
                        }

                        _textureRotationQuaternion = Quaternion.Euler(0, 0, _textureRotationAngle);
                        _textureRotationMatrix =
                            Matrix4x4.TRS( _vector2Zero
                                         , _textureRotationQuaternion
                                         , new Vector2
                                            ( 1f / (1.8f * (_tempCollisionSize / _boundsSize.x))
                                            , 1f / (1.8f * (_tempCollisionSize / _boundsSize.z))
                                            )
                                         );

                        _setCollisionTexMat.SetMatrix("rotationMatrix", _textureRotationMatrix);

                        Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                                     , _collisionDynamicBuffer.GetBackBuffer()
                                     , _setCollisionTexMat
                                     );

                        //_collisionDynamicBuffer.Swap();

                        Graphics.Blit(_collisionDynamicBuffer.GetBackBuffer()
                                     , _collisionDynamicBuffer.GetFrontBuffer());

                        //cache position for use next frame in velocity calculation
                        collider.lastFramePosition = _tempActorPosition;
                        collider.lastFrameTime = _tempCurrentTime;
                    }
                    else
                    {
                        _tempActorPosition = collider.transform.position;
                        _tempPosition = transform.InverseTransformPoint(_tempActorPosition);
                        //positionX positionY
                        _tempCompressedColor1 =
                            EncodeFloatRG(((( (_tempPosition.x * transform.localScale.x)
                                            / _boundsSize.x) * -1) + 1.0f) * 0.5f);

                        _tempCompressedColor2 =
                            EncodeFloatRG(((( (_tempPosition.z * transform.localScale.z)
                                            / _boundsSize.z) * -1) + 1.0f) * 0.5f);

                        _tempStorageArray[_tempCollisionStorageCounter] =
                            new Color32( _tempCompressedColor1.r
                                       , _tempCompressedColor1.g
                                       , _tempCompressedColor2.r
                                       , _tempCompressedColor2.g
                                       );

                        //calculate velocity value
                        if (_tempCurrentTime - collider.lastFrameTime < 0.5f)
                        {
                            _collisionVelocity =
                                transform.InverseTransformPoint(collider.lastFramePosition);

                            _collisionVelocity =
                                new Vector3( (_collisionVelocity.x - _tempPosition.x)
                                           , (_collisionVelocity.z - _tempPosition.z)
                                           , 0
                                           );
                        }
                        else
                        {
                            _collisionVelocity = _vector3Zero;
                        }
                        //collision movement velocity and multiplier
                        _tempCompressedColor1.r =
                            (byte)(Mathf.Clamp01
                                ((( _collisionVelocity.x * collider.moveVelocityMultiplier)
                                 + 5) / 10) * 255);

                        _tempCompressedColor1.g =
                            (byte)(Mathf.Clamp01
                                (((_collisionVelocity.y * collider.moveVelocityMultiplier)
                                 + 5) / 10) * 255);

                        _tempStorageArray[_tempCollisionStorageCounter + 16] =
                            new Color32(_tempCompressedColor1.r, _tempCompressedColor1.g, 0, 0);

                        //collisionSize, collisionFalloff, collisionStrength
                        _tempCompressedColor1 =
                            EncodeFloatRG((_tempCollisionSize / _boundsSize.x) * 0.5f);

                        _tempCompressedColor2.r = (byte)(collider.collisionFalloff * 255);
                        _tempCompressedColor2.g = (byte)(collider.collisionStrength * 255);

                        _tempStorageArray[_tempCollisionStorageCounter + 32] =
                            new Color32( _tempCompressedColor1.r
                                       , _tempCompressedColor1.g
                                       , _tempCompressedColor2.r
                                       , _tempCompressedColor2.g
                                       );

                        _tempCollisionStorageCounter++;

                        if (_tempCollisionStorageCounter > 15)
                        {
                            _tempStorageBufferTexture.SetPixels32(_tempStorageArray, 0);
                            _tempStorageBufferTexture.Apply(false);

                            Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                                         , _collisionDynamicBuffer.GetBackBuffer()
                                         , _setCollisionMat
                                         );

                            //_collisionDynamicBuffer.Swap();

                            Graphics.Blit( _collisionDynamicBuffer.GetBackBuffer()
                                         , _collisionDynamicBuffer.GetFrontBuffer());

                            _blitCollisionShader = false;
                            _tempCollisionStorageCounter = 0;
                        }
                        else
                        {
                            _blitCollisionShader = true;
                        }

                        //cache position for use next frame in velocity calculation
                        collider.lastFramePosition = _tempActorPosition;
                        collider.lastFrameTime = _tempCurrentTime;
                    }
                }
            }
            else if(actorDynamicArray[k] == null)
            {
                _shouldUpdateActorArray = true;
            }
        }

        if (_blitCollisionShader)
        {
            //zero out remaining pixels (if its not a full load) to prevent possible bugs with
            //  deleted actors
            for (int k = _tempCollisionStorageCounter; k < 16; k++)
            {
                _tempStorageArray[k] = _color32Zero;
                _tempStorageArray[k + 16] = _color32Zero;
                _tempStorageArray[k + 32] = _color32Zero;
            }

            _tempStorageBufferTexture.SetPixels32(_tempStorageArray, 0);
            _tempStorageBufferTexture.Apply(false);

            if (_tempCollisionStorageCounter <= 4)
            {
                Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                             , _collisionDynamicBuffer.GetBackBuffer()
                             , _setCollision4Mat
                             );

                //_collisionDynamicBuffer.Swap();

                Graphics.Blit( _collisionDynamicBuffer.GetBackBuffer()
                             , _collisionDynamicBuffer.GetFrontBuffer());
            }
            else if (_tempCollisionStorageCounter <= 8)
            {
                Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                             , _collisionDynamicBuffer.GetBackBuffer()
                             , _setCollision8Mat
                             );

                //_collisionDynamicBuffer.Swap();

                Graphics.Blit( _collisionDynamicBuffer.GetBackBuffer()
                             , _collisionDynamicBuffer.GetFrontBuffer());
            }
            else
            {
                Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                             , _collisionDynamicBuffer.GetBackBuffer()
                             , _setCollisionMat
                             );

                //_collisionDynamicBuffer.Swap();

                Graphics.Blit( _collisionDynamicBuffer.GetBackBuffer()
                             , _collisionDynamicBuffer.GetFrontBuffer());
            }

            _blitCollisionShader = false;
            _tempCollisionStorageCounter = 0;
        }

        if (_shouldUpdateActorArray)
        {
            _inkConnectorScript.SortActorArray();
            _inkConnectorScript.GetActorArrayUpdate(this);
            _shouldUpdateActorArray = false;
        }
    }

    //=============================================================================================

    void UpdateImpulseActors()
    {
        //set impulse for each input object
        //length is defined in the fluidConnector script
        for (int m = 0; m < dynamicInputArrayLength; m++)
        {
            if (actorDynamicArray[m] != null && actorDynamicArray[m] is InkVelocityEmitter)
            {
                InkVelocityEmitter emitter = actorDynamicArray[m] as InkVelocityEmitter;
                if (emitter.multiplySizeByScale)
                {
                    _tempVelocitySize =
                        emitter.velocitySize * emitter.transform.localScale.magnitude * _gamma;
                }
                else
                {
                    _tempVelocitySize = emitter.velocitySize;
                }

                if( ( emitter.transform.position - transform.position).magnitude
                    < (_boundsMagnitude * 0.525f) + _tempVelocitySize)
                {
                    if (emitter.useVelocityMaskTexture && emitter.velocityMaskTexture)
                    {
                        _tempPosition = transform.InverseTransformPoint(emitter.transform.position);
                        //locationX, locationY
                        _tempShaderVectorData.x =
                            ((( (_tempPosition.x * transform.localScale.x)
                              / _boundsSize.x) * -1) + 1.0f) * 0.5f;

                        _tempShaderVectorData.y =
                            ((( (_tempPosition.z * transform.localScale.z)
                              / _boundsSize.z) * -1) + 1.0f) * 0.5f;

                        _tempPositionOffset =
                            transform.InverseTransformDirection
                                (emitter.transform.TransformDirection(new Vector3(0, 0, -1)));
                        _tempVelocityStrength = emitter.velocityStrength;

                        _tempPositionOffset =
                            new Vector3( _tempPositionOffset.x * _tempVelocityStrength
                                       , _tempPositionOffset.z * _tempVelocityStrength
                                       , 0
                                       );

                        //velocityDirection/Strength
                        _tempShaderVectorData.z = _tempPositionOffset.x;
                        _tempShaderVectorData.w = _tempPositionOffset.y;
                        _impulsePositionVelTexMat.SetVector("textureData", _tempShaderVectorData);
                        _impulsePositionVelTexMat.
                            SetTexture( "velocityTexMask"
                                      , emitter.velocityMaskTexture
                                      );

                        _textureRotationAngle =
                            Vector3.Angle(emitter.transform.forward, transform.forward);

                        if (Vector3.Dot(emitter.transform.forward, transform.right) < 0)
                        {
                            _textureRotationAngle *= -1;
                        }

                        _textureRotationQuaternion = Quaternion.Euler(0, 0, _textureRotationAngle);

                        _textureRotationMatrix =
                            Matrix4x4.TRS
                                ( _vector2Zero
                                , _textureRotationQuaternion
                                , new Vector2( 1f / (1.8f * (_tempVelocitySize / _boundsSize.x))
                                             , 1f / (1.8f * (_tempVelocitySize / _boundsSize.z))
                                             )
                                );

                        _impulsePositionVelTexMat.
                            SetMatrix( "rotationMatrix", _textureRotationMatrix);

                        Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                                     , _velocityBuffer.GetBackBuffer()
                                     , _impulsePositionVelTexMat
                                     );

                        //_velocityBuffer.Swap();

                        Graphics.Blit( _velocityBuffer.GetBackBuffer()
                                     , _velocityBuffer.GetFrontBuffer());
                    }
                    else
                    {
                        _tempPosition = transform.InverseTransformPoint(emitter.transform.position);
                        //positionX, positionY
                        _tempCompressedColor1 =
                            EncodeFloatRG(((( (_tempPosition.x * transform.localScale.x)
                                            / _boundsSize.x) * -1) + 1.0f) * 0.5f);

                        _tempCompressedColor2 =
                            EncodeFloatRG(((( (_tempPosition.z * transform.localScale.z)
                                            / _boundsSize.z) * -1) + 1.0f) * 0.5f);

                        _tempStorageArray[_tempVelStorageCounter] =
                            new Color32( _tempCompressedColor1.r
                                       , _tempCompressedColor1.g
                                       , _tempCompressedColor2.r
                                       , _tempCompressedColor2.g
                                       );

                        _tempPositionOffset =
                            transform.
                                InverseTransformDirection
                                    (emitter.transform.TransformDirection(new Vector3(0, 0, -1)));

                        _tempCompressedColor1 = EncodeFloatRG(emitter.velocityStrength);

                        _tempPositionOffset =
                            new Vector3( _tempPositionOffset.x, _tempPositionOffset.z, 0);

                        //impulseVelocityX, impulseVelocityY
                        _tempCompressedColor1.b =
                            (byte)(((_tempPositionOffset.x + 5.0) / 10.0f) * 255);

                        _tempCompressedColor1.a =
                            (byte)(((_tempPositionOffset.y + 5.0) / 10.0f) * 255);

                        _tempStorageArray[_tempVelStorageCounter + 16] = _tempCompressedColor1;
                        //velocitysize, fallOff
                        _tempCompressedColor1 =
                            EncodeFloatRG((_tempVelocitySize / _boundsSize.x) * 0.5f);
                        _tempCompressedColor2.r = (byte)(emitter.velocityFalloff * 255);
                        _tempStorageArray[_tempVelStorageCounter + 32] =
                            new Color32( _tempCompressedColor1.r
                                       , _tempCompressedColor1.g
                                       , _tempCompressedColor2.r
                                       , 0
                                       );

                        _tempVelStorageCounter++;
                        //set to 14 instead of 15 because the "16" impulse shader breaks the 512
                        //  instruction limit.  If 1 arithmatic instruction can be optimized out,
                        //  it should be set back to the full 15 cycle.
                        if (_tempVelStorageCounter > 14)
                        {
                            _tempStorageBufferTexture.SetPixels32(_tempStorageArray, 0);
                            _tempStorageBufferTexture.Apply(false);

                            Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                                         , _velocityBuffer.GetBackBuffer()
                                         , _impulsePositionVelMat);

                            //_velocityBuffer.Swap();

                            Graphics.Blit( _velocityBuffer.GetBackBuffer()
                                         , _velocityBuffer.GetFrontBuffer());

                            _blitVelShader = false;
                            _tempVelStorageCounter = 0;
                        }
                        else
                        {
                            _blitVelShader = true;
                        }
                    }
                }
            }
            else if (actorDynamicArray[m] == null)
            {
                //if we find a null entry for an objects transform
                _shouldUpdateActorArray = true;
            }
        }

        if (_blitVelShader)
        {
            //zero out remaining pixels (if its not a full load) to prevent possible bugs with
            //  deleted actors
            for (int m = _tempVelStorageCounter; m < 16; m++)
            {
                _tempStorageArray[m] = _color32Zero;
                _tempStorageArray[m + 16] = _color32Zero;
                _tempStorageArray[m + 32] = _color32Zero;
            }

            _tempStorageBufferTexture.SetPixels32(_tempStorageArray, 0);
            _tempStorageBufferTexture.Apply(false);

            if (_tempVelStorageCounter <= 4)
            {
                Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                             , _velocityBuffer.GetBackBuffer()
                             , _impulsePositionVel4Mat);

                //_velocityBuffer.Swap();

                Graphics.Blit( _velocityBuffer.GetBackBuffer()
                             , _velocityBuffer.GetFrontBuffer());
            }
            else if (_tempVelStorageCounter <= 8)
            {
                Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                             , _velocityBuffer.GetBackBuffer()
                             , _impulsePositionVel8Mat);

                //_velocityBuffer.Swap();

                Graphics.Blit( _velocityBuffer.GetBackBuffer()
                             , _velocityBuffer.GetFrontBuffer());
            }
            else
            {
                Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                             , _velocityBuffer.GetBackBuffer()
                             , _impulsePositionVelMat);

                //_velocityBuffer.Swap();

                Graphics.Blit( _velocityBuffer.GetBackBuffer()
                             , _velocityBuffer.GetFrontBuffer());
            }

            _blitVelShader = false;
            _tempVelStorageCounter = 0;
        }

        if (_shouldUpdateActorArray)
        {
            _inkConnectorScript.SortActorArray();
            _inkConnectorScript.GetActorArrayUpdate(this);
            _shouldUpdateActorArray = false;
        }
    }

    //=============================================================================================

    void UpdateColorActors()
    {
        //set color for each input object
        //length is defined in the fluidConnector script
        for (int n = 0; n < dynamicInputArrayLength; n++)
        {
            if (actorDynamicArray[n] != null && actorDynamicArray[n] is InkColorEmitter)
            {
                InkColorEmitter emitter = actorDynamicArray[n] as InkColorEmitter;
                if (emitter.multiplySizeByScale)
                {
                    _tempColorSize =
                        emitter.colorSize * (emitter.transform.localScale.magnitude * 0.577f);
                }
                else
                {
                    _tempColorSize = emitter.colorSize;
                }

                if ( (emitter.transform.position - transform.position).magnitude
                   < (_boundsMagnitude * 0.525f) + _tempColorSize)
                {
                    if (emitter.useColorMaskTexture && emitter.colorMaskTexture)
                    {
                        _tempPosition = transform.InverseTransformPoint(emitter.transform.position);
                        //locationX, locationY
                        _tempShaderVectorData.x =
                            ((( (_tempPosition.x * transform.localScale.x)
                              / _boundsSize.x) * -1) + 1.0f) * 0.5f;

                        _tempShaderVectorData.y =
                            ((( (_tempPosition.z * transform.localScale.z)
                              / _boundsSize.z) * -1) + 1.0f) * 0.5f;

                        //Color alpha value
                        _tempShaderVectorData.z = emitter.colorValue.a;
                        _impulsePositionTexMat.SetVector("textureData", _tempShaderVectorData);
                        _impulsePositionTexMat.SetTexture("colorTexMask", emitter.colorMaskTexture);

                        _textureRotationAngle =
                            Vector3.Angle(emitter.transform.forward, transform.forward);

                        if (Vector3.Dot(emitter.transform.forward, transform.right) < 0)
                        {
                            _textureRotationAngle *= -1;
                        }
                        _textureRotationQuaternion = Quaternion.Euler(0, 0, _textureRotationAngle);

                        _textureRotationMatrix =
                            Matrix4x4.TRS
                                ( _vector2Zero
                                , _textureRotationQuaternion
                                , new Vector2( 1f / (1.8f * (_tempColorSize / _boundsSize.x))
                                             , 1f / (1.8f * (_tempColorSize / _boundsSize.z))
                                             )
                                );

                        _impulsePositionTexMat.SetMatrix("rotationMatrix", _textureRotationMatrix);

                        Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                                     , _fluidBuffer.GetBackBuffer()
                                     , _impulsePositionTexMat
                                     );

                        //_fluidBuffer.Swap();

                        Graphics.Blit( _fluidBuffer.GetBackBuffer()
                                     , _fluidBuffer.GetFrontBuffer());
                    }
                    else
                    {
                        _tempPosition = transform.InverseTransformPoint(emitter.transform.position);
                        //positionX, positionY

                        _tempCompressedColor1 =
                            EncodeFloatRG(((( (_tempPosition.x * transform.localScale.x)
                                            / _boundsSize.x) * -1) + 1.0f) * 0.5f);

                        _tempCompressedColor2 =
                            EncodeFloatRG(((( (_tempPosition.z * transform.localScale.z)
                                            / _boundsSize.z) * -1) + 1.0f) * 0.5f);

                        _tempStorageArray[_tempColorStorageCounter] =
                            new Color32( _tempCompressedColor1.r
                                       , _tempCompressedColor1.g
                                       , _tempCompressedColor2.r
                                       , _tempCompressedColor2.g
                                       );

                        //color and alpha
                        _tempColor1 = emitter.colorValue;
                        _tempCompressedColor1 =
                            new Color32( (byte)(_tempColor1.r * 255)
                                       , (byte)(_tempColor1.g * 255)
                                       , (byte)(_tempColor1.b * 255)
                                       , (byte)(_tempColor1.a * 255)
                                       );

                        _tempStorageArray[_tempColorStorageCounter + 16] = _tempCompressedColor1;
                        //color size, fallOff
                        _tempCompressedColor1 =
                            EncodeFloatRG((_tempColorSize / _boundsSize.x) * 0.5f);
                        _tempCompressedColor2.r = (byte)(emitter.colorFalloff * 255);

                        _tempStorageArray[_tempColorStorageCounter + 32] =
                            new Color32( _tempCompressedColor1.r
                                       , _tempCompressedColor1.g
                                       , _tempCompressedColor2.r
                                       , 0
                                       );

                        _tempColorStorageCounter++;

                        if (_tempColorStorageCounter > 15)
                        {
                            _tempStorageBufferTexture.SetPixels32(_tempStorageArray, 0);
                            _tempStorageBufferTexture.Apply(false);

                            Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                                         , _fluidBuffer.GetBackBuffer()
                                         , _impulsePositionMat
                                         );

                            //_fluidBuffer.Swap();

                            Graphics.Blit( _fluidBuffer.GetBackBuffer()
                                         , _fluidBuffer.GetFrontBuffer());

                            _blitColorShader = false;
                            _tempColorStorageCounter = 0;
                        }
                        else
                        {
                            _blitColorShader = true;
                        }
                    }
                }

            }
            else if (actorDynamicArray[n] == null)
            {
                _shouldUpdateActorArray = true;
            }
        }

        if (_blitColorShader)
        {
            //zero out remaining pixels (if its not a full load) to prevent possible bugs with
            //  deleted actors
            for (int n = _tempColorStorageCounter; n < 16; n++)
            {
                _tempStorageArray[n] = _color32Zero;
                _tempStorageArray[n + 16] = _color32Zero;
                _tempStorageArray[n + 32] = _color32Zero;
            }

            _tempStorageBufferTexture.SetPixels32(_tempStorageArray, 0);
            _tempStorageBufferTexture.Apply(false);

            if (_tempColorStorageCounter <= 4)
            {
                Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                             , _fluidBuffer.GetBackBuffer()
                             , _impulsePosition4Mat
                             );

                //_fluidBuffer.Swap();

                Graphics.Blit( _fluidBuffer.GetBackBuffer()
                             , _fluidBuffer.GetFrontBuffer());
            }
            else if (_tempColorStorageCounter <= 8)
            {
                Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                             , _fluidBuffer.GetBackBuffer()
                             , _impulsePosition8Mat
                             );

                //_fluidBuffer.Swap();

                Graphics.Blit( _fluidBuffer.GetBackBuffer()
                             , _fluidBuffer.GetFrontBuffer());
            }
            else
            {
                Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                             , _fluidBuffer.GetBackBuffer()
                             , _impulsePositionMat
                             );

                //_fluidBuffer.Swap();

                Graphics.Blit( _fluidBuffer.GetBackBuffer()
                             , _fluidBuffer.GetFrontBuffer());
            }

            _blitColorShader = false;

            _tempColorStorageCounter = 0;
        }

        if (_shouldUpdateActorArray)
        {
            _inkConnectorScript.SortActorArray();
            _inkConnectorScript.GetActorArrayUpdate(this);
            _shouldUpdateActorArray = false;
        }
    }

    //=============================================================================================

    void UpdateFluid()
    {
        _frameDelay++;

        switch (_frameDelay)
        {
            case 1:
                //_transformPosition = transform.position;
                break;
            case 2:
                //_transformLocalScale = transform.localScale;
                break;
            case 3:
                //if (_isTerrain == false)
                {
                    _boundsSize = _mesh.bounds.size;

                    _boundsSize =
                        new Vector3( _boundsSize.x * transform.localScale.x
                                   , _boundsSize.y * transform.localScale.y
                                   , _boundsSize.z * transform.localScale.z
                                   );

                    _boundsMagnitude = _boundsSize.magnitude;
                }

                _frameDelay = 0;
                break;
        }

        //advection color
        if (useColorDissipationTexture)
        {
            Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                         , _fluidBuffer.GetBackBuffer()
                         , _advectionColorTexMat
                         );

            //_fluidBuffer.Swap();

            Graphics.Blit( _fluidBuffer.GetBackBuffer()
                         , _fluidBuffer.GetFrontBuffer());
        }
        else
        {
            Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                         , _fluidBuffer.GetBackBuffer()
                         , _advectionColorMat
                         );

            //_fluidBuffer.Swap();

            Graphics.Blit( _fluidBuffer.GetBackBuffer()
                         , _fluidBuffer.GetFrontBuffer());
        }

        //advection velocity
        if (useVelocityDissipationTexture)
        {
            Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                         , _velocityBuffer.GetBackBuffer()
                         , _advectionVelocityTexMat
                         );

            //_velocityBuffer.Swap();

            Graphics.Blit( _velocityBuffer.GetBackBuffer()
                         , _velocityBuffer.GetFrontBuffer());
        }
        else
        {
            Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                         , _velocityBuffer.GetBackBuffer()
                         , _advectionVelocityMat
                         );

            //_velocityBuffer.Swap();

            Graphics.Blit( _velocityBuffer.GetBackBuffer()
                         , _velocityBuffer.GetFrontBuffer());
        }

        //run divergence with built in clear event
        Graphics.Blit( _divergenceBuffer.GetFrontBuffer()
                     , _divergenceBuffer.GetBackBuffer()
                     , _divergenceMat
                     );

        //_divergenceBuffer.Swap();

        Graphics.Blit( _divergenceBuffer.GetBackBuffer()
                     , _divergenceBuffer.GetFrontBuffer());

        //clear pressure
        Graphics.Blit( _pressureBuffer.GetFrontBuffer()
                     , _pressureBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_pressureBuffer.Swap();

        Graphics.Blit( _pressureBuffer.GetBackBuffer()
                     , _pressureBuffer.GetFrontBuffer());

        //iterate pressure "jacobi relax"
        for (int i = 0; i < pressureIteration; i++)
        {
            Graphics.Blit( _pressureBuffer.GetFrontBuffer()
                         , _pressureBuffer.GetBackBuffer()
                         , _jacobiRelaxMat);

            //_pressureBuffer.Swap();

            Graphics.Blit( _pressureBuffer.GetBackBuffer()
                         , _pressureBuffer.GetFrontBuffer());
        }

        //subtract pressure gradient
        Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                     , _velocityBuffer.GetBackBuffer()
                     , _gradientMat
                     );

        //_velocityBuffer.Swap();

        Graphics.Blit( _velocityBuffer.GetBackBuffer()
                     , _velocityBuffer.GetFrontBuffer());

        if (_shouldUpdateColorWithFluid)
        {
            UpdateColorActors();
        }

        if (_shouldUpdateImpulseWithFluid)
        {
            UpdateImpulseActors();
        }

        if (_shouldUpdateCollisionWithFluid)
        {
            UpdateDynamicCollision();
        }
    }

    //=============================================================================================

    // Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded
    //  properly... so we clamp it.
    Color32 EncodeFloatRG(float v)
    {
        v = Mathf.Clamp(v, 0f, 0.9999f);
        _enc2 = new Vector2(_kEncodeMul2.x * v, _kEncodeMul2.y * v);

        //same as doing frac()... but frac doesnt exist and % has precision loss of about 0.00000005
        _enc2 = new Vector2(_enc2.x % 1, _enc2.y % 1);
        _enc2.x -= _enc2.y * _kEncodeBit;

        return new Color32( (byte)(Mathf.RoundToInt(_enc2.x * 255))
                          , (byte)(_enc2.y * 255), (byte)(0), (byte)(0)
                          );
    }

    //=============================================================================================

    //Decode is only used for testing and could be removed without breaking the simulation.
    float DecodeFloatRG(Color32 colorV)
    {
        return Vector2.Dot(new Vector2(colorV.r, colorV.g), _kDecodeDot2);
    }

    //=============================================================================================

    public void ChangeSimStrength(float strength)
    {
        simStrength = strength;

        _advectionColorMat.SetFloat("simSpeed", simStrength);

        _advectionVelocityMat.SetFloat("simSpeed", simStrength);
    }

    //=============================================================================================

    public void ChangeColorDissipationStrength(float dissipation)
    {
        if (useColorDissipation)
        {
            colorDissipation = dissipation;
            oldColorDissipation = dissipation;

            _advectionColorMat.SetFloat("dissipation", Mathf.Lerp(0.0f, 0.05f, colorDissipation));
        }
        else
        {
            Debug.LogWarning( "Fluid Color dissipation is currently disabled. To see color"
                            + "dissipation, make sure it is enabled."
                            );
        }
    }

    //=============================================================================================

    public void ChangeColorDissipateTo(Color dissipateColor)
    {
        colorDissipateTo = dissipateColor;

        _advectionColorMat.SetVector("colorDissipateTo", colorDissipateTo);
    }

    //=============================================================================================

    public void ChangeColorDissipationBool(bool value)
    {
        useColorDissipation = value;

        if (useColorDissipation)
        {
            colorDissipation = oldColorDissipation;


            _advectionColorMat.SetFloat("dissipation", Mathf.Lerp(0.0f, 0.05f, colorDissipation));
        }
        else
        {
            oldColorDissipation = colorDissipation;

            colorDissipation = 0.0f;

            _advectionColorMat.SetFloat("dissipation", Mathf.Lerp(0.0f, 0.05f, colorDissipation));
        }
    }

    //=============================================================================================

    public void ChangeVelocityDissipationStrength(float dissipation)
    {
        if (useVelocityDissipation)
        {
            velocityDissipation = dissipation;
            oldVelocityDissipation = dissipation;

            _advectionVelocityMat.SetFloat( "dissipation"
                                            , Mathf.Lerp(1.0f, 0.5f, velocityDissipation)
                                            );
        }
        else
        {
            Debug.LogWarning( "Fluid Velocity dissipation is currently disabled. To see velocity"
                            + "dissipation, make sure it is enabled."
                            );
        }
    }

    //=============================================================================================

    public void ChangeVelocityDissipationBool(bool value)
    {
        useVelocityDissipation = value;

        if (useVelocityDissipation)
        {
            velocityDissipation = oldVelocityDissipation;

            _advectionVelocityMat.SetFloat( "dissipation"
                                            , Mathf.Lerp(1.0f, 0.5f, velocityDissipation)
                                            );
        }
        else
        {
            oldVelocityDissipation = velocityDissipation;

            velocityDissipation = 0.0f;

            _advectionVelocityMat.SetFloat( "dissipation"
                                            , Mathf.Lerp(1.0f, 0.5f, velocityDissipation)
                                            );
        }
    }

    //=============================================================================================

    public void SetFluidUpdateFPS(int fps)
    {
        fluidUpdateFPS = fps;

        CancelInvoke("UpdateFluid");

        if (fluidUpdateFPS > 0)
        {
            InvokeRepeating("UpdateFluid", 0, 1f / fluidUpdateFPS);
        }
    }

    //=============================================================================================

    public void SetImpulseUpdateFPS(int fps)
    {
        impulseUpdateFPS = fps;

        if (!_shouldUpdateImpulseWithFluid)
        {
            CancelInvoke("UpdateImpulseActors");

            if (impulseUpdateFPS > 0)
            {
                InvokeRepeating("UpdateImpulseActors", 0.1f, 1f / impulseUpdateFPS);
            }
        }
        else
        {
            Debug.LogWarning( "Impulse Update is currently synced with Fluid update; to use"
                            + "SetImpulseUpdateFPS you need to turn off impulse sync with fluid"
                            + "first.  Try using SetSyncImpulseWithFluid(boolean) first."
                            );
        }
    }

    //=============================================================================================

    public void SetColorUpdateFPS(int fps)
    {
        colorUpdateFPS = fps;

        if (!_shouldUpdateColorWithFluid)
        {
            CancelInvoke("UpdateColorActors");

            if (colorUpdateFPS > 0)
            {
                InvokeRepeating("UpdateColorActors", 0.1f, 1f / colorUpdateFPS);
            }
        }
        else
        {
            Debug.LogWarning( "Color Update is currently synced with Fluid update; to use"
                            + "SetColorUpdateFPS you need to turn off color sync with fluid first."
                            + "Try using SetSyncColorWithFluid(boolean) first."
                            );
        }
    }

    //=============================================================================================

    public void SetSyncCollisionWithFluid(bool syncWithFluid)
    {
        _shouldUpdateCollisionWithFluid = syncWithFluid;
        shouldUpdateCollisionWithFluid = _shouldUpdateCollisionWithFluid;

        if (_shouldUpdateCollisionWithFluid)
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

    //=============================================================================================

    public void SetSyncImpulseWithFluid(bool syncWithFluid)
    {
        _shouldUpdateImpulseWithFluid = syncWithFluid;
        shouldUpdateImpulseWithFluid = _shouldUpdateImpulseWithFluid;


        if (_shouldUpdateImpulseWithFluid)
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

    //=============================================================================================

    public void SetSyncColorWithFluid(bool syncWithFluid)
    {
        _shouldUpdateColorWithFluid = syncWithFluid;
        shouldUpdateColorWithFluid = _shouldUpdateColorWithFluid;

        if (_shouldUpdateColorWithFluid)
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


    //=============================================================================================

    public void SetCollisionUpdateFPS(int fps)
    {
        collisionUpdateFPS = fps;

        if (_shouldUpdateCollisionWithFluid)
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

    //=============================================================================================

    public void ClearAllDynamicBuffers()
    {
        //clear color
        _initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 0.0f));

        Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                     , _fluidBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_fluidBuffer.Swap();

        Graphics.Blit( _fluidBuffer.GetBackBuffer()
                     , _fluidBuffer.GetFrontBuffer());



        //clear velocity
        _initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 0.0f));

        Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                     , _velocityBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_fluidBuffer.Swap();

        Graphics.Blit( _fluidBuffer.GetBackBuffer()
                     , _fluidBuffer.GetFrontBuffer());

        //clear dynamic collision
        _initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 1.0f));

        Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                     , _collisionDynamicBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_fluidBuffer.Swap();

        Graphics.Blit( _fluidBuffer.GetBackBuffer()
                     , _fluidBuffer.GetFrontBuffer());

        //clear divergence
        _initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));

        Graphics.Blit( _divergenceBuffer.GetFrontBuffer()
                     , _divergenceBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_fluidBuffer.Swap();

        Graphics.Blit( _fluidBuffer.GetBackBuffer()
                     , _fluidBuffer.GetFrontBuffer());

        //clear pressure
        _initializeToValueMat.SetVector("initialValue", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));

        Graphics.Blit( _pressureBuffer.GetFrontBuffer()
                     , _pressureBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_fluidBuffer.Swap();

        Graphics.Blit( _fluidBuffer.GetBackBuffer()
                     , _fluidBuffer.GetFrontBuffer());
    }

    //=============================================================================================

    public void ClearColorBuffer(Color defaultColor)
    {
        _initializeToValueMat.SetVector( "initialValue"
                                       , new Vector3( defaultColor.r
                                                    , defaultColor.g
                                                    , defaultColor.b
                                                    )
                                       );

        Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                     , _fluidBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_fluidBuffer.Swap();

        Graphics.Blit( _fluidBuffer.GetBackBuffer()
                     , _fluidBuffer.GetFrontBuffer());
    }

    //=============================================================================================

    public void ClearColorBufferToTexture(Texture2D colorTexture)
    {
        _initializeToTextureMat.SetTexture("initialTexture", colorTexture);

        Graphics.Blit( _fluidBuffer.GetFrontBuffer()
                     , _fluidBuffer.GetBackBuffer()
                     , _initializeToTextureMat
                     );

        //_fluidBuffer.Swap();

        Graphics.Blit( _fluidBuffer.GetBackBuffer()
                     , _fluidBuffer.GetFrontBuffer());
    }

    //=============================================================================================

    public void ClearVelocityBuffer(Vector2 defaultVelocity)
    {
        _initializeVelToValueMat.SetVector("initialValue", defaultVelocity);

        Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                     , _velocityBuffer.GetBackBuffer()
                     , _initializeVelToValueMat
                     );

        //_velocityBuffer.Swap();

        Graphics.Blit( _velocityBuffer.GetBackBuffer()
                     , _velocityBuffer.GetFrontBuffer());
    }

    //=============================================================================================

    public void ClearVelocityBufferToTexture(Texture2D velocityTexture)
    {
        if (velocityTexture != null)
        {
            _initializeVelToTextureMat.SetTexture("initialTexture", velocityTexture);

            Graphics.Blit( _velocityBuffer.GetFrontBuffer()
                         , _velocityBuffer.GetBackBuffer()
                         , _initializeVelToTextureMat
                         );
        }
        else
        {
            Debug.LogError( "ClearVelocityBufferToTexture() was called without supplying a"
                          + "velocityTexture to the function.  The Fluid Velocity buffer has not"
                          + "been cleared to the texture value."
                          );
        }
    }

    //=============================================================================================

    public void ClearDynamicCollisionBuffer()
    {
        _initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 1.0f));

        Graphics.Blit( _collisionDynamicBuffer.GetFrontBuffer()
                     , _collisionDynamicBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_collisionDynamicBuffer.Swap();

        Graphics.Blit( _collisionDynamicBuffer.GetBackBuffer()
                     , _collisionDynamicBuffer.GetFrontBuffer());
    }

    //=============================================================================================

    public void ClearStaticCollisionBuffer()
    {
        _initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 1.0f));

        Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                     , _collisionStaticBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_collisionStaticBuffer.Swap();

        Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                     , _collisionStaticBuffer.GetFrontBuffer());
    }

    //=============================================================================================

    public void RecreateStaticCollisionBuffer(bool applyBoundaryCollision)
    {
        _inkConnectorScript.SortActorArray();
        _inkConnectorScript.GetActorArrayUpdate(this);

        _initializeToValueMat.SetVector("initialValue", new Vector3(0.0f, 0.0f, 1.0f));

        Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                     , _collisionStaticBuffer.GetBackBuffer()
                     , _initializeToValueMat
                     );

        //_collisionStaticBuffer.Swap();

        Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                     , _collisionStaticBuffer.GetFrontBuffer());

        if (applyBoundaryCollision)
        {
            Graphics.Blit( _collisionStaticBuffer.GetFrontBuffer()
                         , _collisionStaticBuffer.GetBackBuffer()
                         , _boundaryOpMat);

            //_collisionStaticBuffer.Swap();

            Graphics.Blit( _collisionStaticBuffer.GetBackBuffer()
                         , _collisionStaticBuffer.GetFrontBuffer());
        }

        UpdateStaticCollision();

    }

    //=============================================================================================
}
