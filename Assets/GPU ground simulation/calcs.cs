using UnityEngine;
using System.Collections;

// note: it's better to have data struct size = N times gpu data bus width
// because data is packed tightly in gpu memory, nad we can avoid unnecessary offset calculations in gpu data read operations
// performance is important in our task, so I follow this rule
// usually gpu data bus width is 128 bits or 4 x 4 bytes
// so, if someone has 20 bytes to store in a struct, it's better to add a few dummy bytes abd have 32 bytes struct
// in our case we have 12 values of 4 bytes, which fits the requirement
// 
// actually, more modern videocards than mine have 192 bits bus, but NVIDIA guys said keep it N times 128 so I believe them, they should know it better


// this structdescribes particle's position, velosity, connections and acceleration
public struct soilParticlePos{
	public float x;		// coordinates
	public float y;		//
	public int vx;			// velosity, it's int because interlocked operations (protected kind of write) can't work with float values on hardware level
	public int vy;			//
	public uint c1;		// connections (they will contain uint indexes of the particles the particle is connected with)
	public uint c2;		//
	public uint c3;		//
	public uint c4;		//
	public uint c5;		//
	public uint c6;		//
	public int dvx;			// velosity change, these values store the changes of the particle's velosity
	public int dvy;			// it's used in velocity share operation, where we can overwright velocities while other parallel processes may need it
}

// this struct contains additional particle's parameters, like color or temperature, I haven merged it with soilParticlePos struct, because it's data used rarely
public struct soilParticle{
	public uint color;
	public int dummy0;				// these dummy values are not used, but they stay here to fit the N times 128 bit struct size requirement
	public int dummy1;
	public int dummy2;
	public int impact;
	public int ddvx;
	public int ddvy;
	public float t;
}
public struct explosion{			// struct dexcribing explosion parameters: epicentre coordinates, radius and power of the explosion
	public float x;
	public float y;
	public float r;
	public float f;
}
public struct explosions{			// this struct handles explosions being added by user, it provides a simple interface to add explosions
	public explosion[] expls;
	public explosion[] currentExpl;
	public int ei;
	public void init(){
		expls = new explosion[50];
		currentExpl = new explosion[1];
		ei = 0;
	}
	public void addExpl(float x, float y, float r, float f){
		expls[ei].x = x;
		expls[ei].y = y;
		expls[ei].r = r;
		expls[ei].f = f;
		ei++;
	}
	public void prepare(){
		currentExpl[0] = expls[--ei];
	}
}

static public class calcs  {				// this class handles the shader - initializes, dispatches, exchanges information
	static public ComputeShader _shader;
	// these are kernel indexes, one per each kernel inside compute shader
	static public int kiCalc;
	static public int kiCalcApply;
	static public int kiImpulseShare;
	static public int kiDImpulseApply;
	static public int kiVelShare;
	static public int kiDVelApply;
	static public int kiClean;
	static public int kiVisualize;
	static public int kiDrawGrid;
	static public int kiCleanGrid;
	static public int kiRedrawGrid;
	static public int kiGravMask;
	static public int kiGravMaskSmooth;
	static public int kiDensMask;
	static public int kiDensMaskSmooth;
	static public int kiSetPixels;
	static public int kiOneThreadAction;
	// these arrays will contain initial parameters of the particles
	static public soilParticlePos[] soilParticlesPos;
	static public soilParticle[] soilParticles;
	static public int soilStructSize;
	static public int soilPosStructSize;
	// grid array helps to oprimize interaction between particles, so each particle will not have to check distance to all other particles
	// instead, particle knows its grid cell coordinate, and only checks the distances between itself and contents of the nearby grid cells
	// which turns O(N ^ 2) time to O(N * log(N)) one much faster
	static public int[] grid;
	static public int gridCellSize;
	// gravity and density masks help to stabilize the behavior of the matter made of particles
	static public float[] gravMask;
	static public float[] densMask;
	// these buffers provide data to the shader
	static public ComputeBuffer particlesPosBuffer;
	static public ComputeBuffer particlesBuffer;
	static public ComputeBuffer gridBuffer;
	static public ComputeBuffer pixelsBuffer;
	static public ComputeBuffer explBuffer;
	static public ComputeBuffer gravMaskBuffer;
	static public ComputeBuffer densMaskBuffer;
	// UI elements used for visualization
	static public GameObject mainCanvas;
	static public UnityEngine.UI.Image outputImage;
	// this texture is being filled with pixels by the compute shader, and it's lso assigned to the Image material, so itś being instantly visualized
	static public RenderTexture outputTexture;
	// we are using int type to store particle velocities, and intVelF is a factor to turn float velocity into int value
	// (we need to use the whole range of int type to have both wide range and good precision)
	static public int intVelF;
	// this sotres explosions and passes them to the gpu to affect particles
	static public explosions explContainer;
	static public int numberOfParticles;

	static public void init(){						// initializes everything
		explContainer.init();
		initStuff();
		initGrid();
		initParticles();
		initBuffers();
		initTexture();
		initShader();
		initCanvas();
	}
	static public void initStuff(){					// initialization of grid masks, that help to manage particles
		gravMask = new float[65536];		// both are 256x256
		densMask = new float[65536];		//
	}
	static public void initGrid(){					// inits the grid that helps to accelerate interaction between particles
		gridCellSize = 36;							// this number - 2 = how many particles can be inside one grid
		grid = new int[256 * 256 * gridCellSize];	// so, it's 256x256 grid, and each cell has array of gridCellSize size to store particles' indexes
		int i = 0;
		while (i < grid.Length) {
			grid[i] = 0;
			i++;
		}
	}
	static public void initParticles(){				// sets initial values of particle parameters
		int i;
		intVelF = 1200000;
		uint NOLINK = 10000000;
		numberOfParticles = 2 * 8192;				// here you set the number of particles in simulation
		soilParticlesPos = new soilParticlePos[numberOfParticles];
		soilParticles = new soilParticle[numberOfParticles];
		i = 0;
		while (i < soilParticles.Length) {
			soilParticlesPos[i].x = 256 + 1 * (i % 512) + 0.5f + Random.Range(-0.5f, 0.5f);
			soilParticlesPos[i].y = 10 + 2 * (i / 256) + 0.5f + Random.Range(-0.5f, 0.5f);
			soilParticlesPos[i].vx = (int)(0 * intVelF);	// see, I use intVelF multiplier to turn float format of the speed (pixels/sec) into int format
			soilParticlesPos[i].vy = (int)(0 * intVelF);
			soilParticlesPos[i].c1 = NOLINK;
			soilParticlesPos[i].c2 = NOLINK;
			soilParticlesPos[i].c3 = NOLINK;
			soilParticlesPos[i].c4 = NOLINK;
			soilParticlesPos[i].c5 = NOLINK;
			soilParticlesPos[i].c6 = NOLINK;
			soilParticlesPos[i].dvx = 0;
			soilParticlesPos[i].dvy = 0;
			// here we set initial color of the particles, I made it look like some kind of ground: green top layer and brown bottom
			if (i < numberOfParticles * 0.9f)
				soilParticles[i].color = (uint)196 << 16 | (uint)96 << 8 | (uint)0| (uint)255 << 24;
			else
				soilParticles[i].color = ((uint)Random.Range(10, 20)) << 16 | ((uint)Random.Range(180, 230)) << 8 |((uint)Random.Range(20, 30)) | (uint)255 << 24;


			// alternative coloring, to demonstrate the idea of partciles being open for random coloring
			/*
			if (i % 256 < 85) {
				soilParticles[i].color = (uint)Mathf.Clamp(((i % 256) * 3) * 2, 0, 255) << 8 | (uint)Mathf.Clamp((255 - (i % 256) * 3) * 2, 0, 255) << 16;		// red descends, green grows
			}
			if (i % 256 >= 85 && i % 256 < 2 * 85) {
				soilParticles[i].color = (uint)Mathf.Clamp(((i % 256 - 85) * 3) * 2, 0, 255) | (uint)Mathf.Clamp((255 - (i % 256 - 85) * 3) * 2, 0, 255) << 8;		// green descends, blue grows
			}
			if (i % 256 >= 2 * 85) {
				soilParticles[i].color = (uint)Mathf.Clamp(((i % 256 - 2 * 85) * 3) * 2, 0, 255) << 16 | (uint)Mathf.Clamp((255 - (i % 256 - 2 * 85) * 3) * 2, 0, 255);		// blue descends, red grows
			}
			*/

			// alternative coloring, like everything is snow
			//
			//soilParticles[i].color = 220 << 16 | 220 << 8 | 255;
			//

			soilParticles[i].t = 0;
			i++;
		}
		// the following code is commented out for purpose
		// it was used for little debugs including only a few particles with exact positions to observe their behavior
		/*
		i = 0;
		soilParticlesPos[i].x = 512;
		soilParticlesPos[i].y = 512;
		soilParticlesPos[i].vx = 1 * intVelF;
		soilParticlesPos[i].vy = 0;
		soilParticles[i].color = new Color(0.8f, 0.4f, 0.4f, 1);
		i = 1;
		soilParticlesPos[i].x = 522f;
		soilParticlesPos[i].y = 512;
		soilParticlesPos[i].vx = -1 * intVelF;
		soilParticlesPos[i].vy = 0;
		soilParticles[i].color = new Color(0.4f, 0.4f, 0.8f, 1);

		i = 2;
		soilParticlesPos[i].x = 512;
		soilParticlesPos[i].y = 518;
		soilParticlesPos[i].vx = 0;//-50 * intVelF;
		soilParticlesPos[i].vy = 0;
		soilParticles[i].color = new Color(0.4f, 0.8f, 0.4f, 1);
		i = 3;
		soilParticlesPos[i].x = 518;
		soilParticlesPos[i].y = 518;
		soilParticlesPos[i].vx = 0;//12 * intVelF;
		soilParticlesPos[i].vy = 0;
		soilParticles[i].color = new Color(0.4f, 0.8f, 0.4f, 1);
		*/
		/*
		i = 0;
		while (i < 64) {
			soilParticlesPos[i].x = 512f + 4 * (i / 8);
			soilParticlesPos[i].y = 512 + 4 * (i % 8);
			soilParticlesPos[i].vx = 0;//intVelF * 100;
			soilParticlesPos[i].vy = 0;//-intVelF * 100;
			soilParticles[i].color = new Color(1, 0, 0, 1);
			i++;
		}
		*/
	}
	static public void initBuffers(){			// inits buffers. Structured buffers are a class to send/get data to/from compute shader
		soilPosStructSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(soilParticlePos));
		soilStructSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(soilParticle));
		particlesPosBuffer = new ComputeBuffer(soilParticlesPos.Length, soilPosStructSize);
		particlesPosBuffer.SetData(soilParticlesPos);
		particlesBuffer = new ComputeBuffer(soilParticles.Length, soilStructSize);
		particlesBuffer.SetData(soilParticles);
		gridBuffer = new ComputeBuffer(grid.Length, sizeof(int));
		gridBuffer.SetData(grid);
		pixelsBuffer = new ComputeBuffer(1024 * 1024, 4);
		explBuffer = new ComputeBuffer(1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(explosion)));
		gravMaskBuffer = new ComputeBuffer(gravMask.Length, sizeof(float));
		densMaskBuffer = new ComputeBuffer(densMask.Length, sizeof(float));
	}
	static public void initTexture(){				// inits the texture used for visualization of particles
		outputTexture = new RenderTexture(1024, 1024, 32);
		outputTexture.enableRandomWrite = true;		// this is important to make it work the way we use it: it's being written on gpu side with random access
		outputTexture.Create();						// render texture must be "created" like this
		outputTexture.wrapMode = TextureWrapMode.Clamp;
		outputTexture.filterMode = FilterMode.Point;
	}
	static public void initShader(){			// here we initialize shader by linking the code to ComputeShader class and connecting kernels with buffers
		// so, first we load the shader code to our ComputeShader object
		_shader = Resources.Load<ComputeShader>("particleInteraction");
		// then we get kernel indexes, they are required as shader.Dispatch() parameter
		kiCalc = _shader.FindKernel("calcImpact");
		kiCalcApply = _shader.FindKernel("calcApply");
		kiImpulseShare = _shader.FindKernel("impulseShare");
		kiDImpulseApply = _shader.FindKernel("dImpulseApply");
		kiVelShare = _shader.FindKernel("velShare");
		kiDVelApply = _shader.FindKernel("dVelApply");
		kiClean = _shader.FindKernel("cleanTexture");
		kiVisualize = _shader.FindKernel("visualize");
		kiDrawGrid = _shader.FindKernel("drawGrid");
		kiCleanGrid = _shader.FindKernel("cleanGrid");
		kiRedrawGrid = _shader.FindKernel("redrawGrid");
		kiGravMask = _shader.FindKernel("gravMaskRefresh");
		kiGravMaskSmooth = _shader.FindKernel("gravMaskSmooth");
		kiDensMask = _shader.FindKernel("densMaskRefresh");
		kiDensMaskSmooth = _shader.FindKernel("densMaskSmooth");
		kiSetPixels = _shader.FindKernel("setPixels");
		kiOneThreadAction = _shader.FindKernel("oneThreadAction");
		// now we need to do important thing:
		// kernels need to gain access to the buffers that we have declared in the shader code, and SetBuffer command is the way of making it happen
		// we have many buffers and many kernels, and following code gives access to the buffers, the kernels use
		// it was actually a frequent mistake on y side, to implement everything and forget to give a kernel access to the buffer
		// if you forget, kernel won give you any errors, he'll act like everything is alright, but the data you will try to get from a buffer will be set to default
		// so, if you create a new kernel, don forget to set all buffers it needs to have access to
		_shader.SetBuffer(kiCalc, "grid", gridBuffer);
		_shader.SetBuffer(kiCleanGrid, "grid", gridBuffer);
		_shader.SetBuffer(kiRedrawGrid, "grid", gridBuffer);
		_shader.SetBuffer(kiRedrawGrid, "particlePos", particlesPosBuffer);
		_shader.SetBuffer(kiRedrawGrid, "particles", particlesBuffer);
		_shader.SetBuffer(kiCalc, "particles", particlesBuffer);
		_shader.SetBuffer(kiCalc, "particlePos", particlesPosBuffer);
		_shader.SetBuffer(kiCalcApply, "particles", particlesBuffer);
		_shader.SetBuffer(kiCalcApply, "particlePos", particlesPosBuffer);
		_shader.SetBuffer(kiCalcApply, "gravMask", gravMaskBuffer);
		_shader.SetBuffer(kiCalcApply, "densMask", densMaskBuffer);
		_shader.SetBuffer(kiCalcApply, "expl", explBuffer);
		_shader.SetBuffer(kiCalcApply, "grid", gridBuffer);
		_shader.SetBuffer(kiImpulseShare, "particles", particlesBuffer);
		_shader.SetBuffer(kiImpulseShare, "particlePos", particlesPosBuffer);
		_shader.SetBuffer(kiDVelApply, "particles", particlesBuffer);
		_shader.SetBuffer(kiDVelApply, "particlePos", particlesPosBuffer);
		_shader.SetBuffer(kiVelShare, "particles", particlesBuffer);
		_shader.SetBuffer(kiVelShare, "particlePos", particlesPosBuffer);
		_shader.SetBuffer(kiDImpulseApply, "particles", particlesBuffer);
		_shader.SetBuffer(kiDImpulseApply, "particlePos", particlesPosBuffer);
		_shader.SetBuffer(kiGravMask, "grid", gridBuffer);
		_shader.SetBuffer(kiGravMask, "gravMask", gravMaskBuffer);
		_shader.SetBuffer(kiGravMaskSmooth, "gravMask", gravMaskBuffer);
		_shader.SetBuffer(kiDensMask, "grid", gridBuffer);
		_shader.SetBuffer(kiDensMask, "densMask", densMaskBuffer);
		_shader.SetBuffer(kiDensMaskSmooth, "densMask", densMaskBuffer);
		_shader.SetBuffer(kiSetPixels, "particles", particlesBuffer);
		_shader.SetBuffer(kiSetPixels, "particlePos", particlesPosBuffer);
		_shader.SetBuffer(kiSetPixels, "pixels", pixelsBuffer);
		_shader.SetBuffer(kiSetPixels, "expl", explBuffer);
		_shader.SetBuffer(kiOneThreadAction, "expl", explBuffer);
		_shader.SetBuffer(kiCalcApply, "pixels", pixelsBuffer);
		_shader.SetBuffer(kiClean, "pixels", pixelsBuffer);
		_shader.SetBuffer(kiDrawGrid, "pixels", pixelsBuffer);
		_shader.SetBuffer(kiDrawGrid, "grid", gridBuffer);
		_shader.SetBuffer(kiDrawGrid, "gravMask", gravMaskBuffer);
		_shader.SetBuffer(kiDrawGrid, "densMask", densMaskBuffer);
		// and finally SetTexture is called to let the kernel have access to the RenderTexture we created for visualization
		_shader.SetTexture(kiVisualize, "outputTexture", outputTexture);
		_shader.SetBuffer(kiVisualize, "pixels", pixelsBuffer);
	}
	static public void initCanvas(){			// inits UI, nothing special, just Image on a Canvas, and Image's material uses RenderTexture we created earlier
		mainCanvas = GameObject.Find("mainCanvas");
		mainCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
		mainCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
		mainCanvas.GetComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
		mainCanvas.GetComponent<UnityEngine.UI.CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
		mainCanvas.GetComponent<UnityEngine.UI.CanvasScaler>().matchWidthOrHeight = 1.0f;
		outputImage = GameObject.Find("mainCanvas/image").GetComponent<UnityEngine.UI.Image>();
		outputImage.material.mainTexture = outputTexture;
		outputImage.type = UnityEngine.UI.Image.Type.Simple;
	}
	static public void update(){				// main per frame shader calcs happen here, this is being called from camMaster.Update()
		int i;
		int nGroups = 1 + (numberOfParticles - 1) / 256;	// how many thread groups. There are 256 threads per group, and we need a thread per particle
		// the core thing anyone learning compute shader should understand is the relationship between threads, thread groups and thread indexes
		// I don think I can explain it better than msdn, they have a nice picture, try to understand it, it's not complex at all, just specific
		// this one:
		// https://msdn.microsoft.com/en-us/library/windows/desktop/ff471442(v=vs.85).aspx
		// as soon as you understand it, you will have no problems with using thread index parameter inside the kernel

		// this kernel cleans the pixel array we use to collect color data (which then is used to set texture pixels we see on the screen)
		_shader.Dispatch(kiClean, 1, 1024, 1);

		// this kernel refreshes gravity mask, which is used to determine which particles should be pressed by gravity
		// in the current build I exclude underground particles from gravity affection. It is not necessary, but reduces high pressure in the deep issues
		_shader.Dispatch(kiGravMask, 1, 1, 1);
		// this one smoothes gravity mask a bit
		_shader.Dispatch(kiGravMaskSmooth, 1, 256, 1);
		_shader.Dispatch(kiGravMaskSmooth, 1, 256, 1);

		// this kernel refreshes density mask, that is used to simulate viscosity: the densier surroundings, the more velosity the particle looses
		_shader.Dispatch(kiDensMask, 1, 256, 1);
		// and this one simply smoothes density mask to make density gradient more even
		_shader.Dispatch(kiDensMaskSmooth, 1, 256, 1);



		passExpl();

		// The following doCalcs method is being called 8 times in a row, because it is a solution to have both high precision of calculations and fast work
		// The idea is that steep gradient of intermolecular forces and fast particle speed may cause a particle find itself really close to another particle
		// and that will create a really strong repulse impulse, that whouldn't have happened if the step was smaller
		// So, I made the time step as small as was needed to avoid step size related imprecision and thus instability
		// But smaller step means simulation works slower, so obvious solution is to make more than one step per Update()
		// This obviously increases performance requirement as we need to run more calculations per Upadate()
		// So, if we would have called this method say 100 times per update, fps would drop extremely
		// Therefore, we need to find a balance between simulation speed and good fps
		// On my videocard I have 30 fps for 8 doCalcs() calls. If you have GeForce GTX 1080 you could probably have 50. It would be too fast though
		i = 0;
		while (i < 8) {
			doCalcs(nGroups);
			i++;
		}

		_shader.Dispatch(kiSetPixels, nGroups, 1, 1);

		// this kernel is disabled, it was used to visualize the grid (that holds the amount of particles per little 4x4 grid square)
		// it was useful for debug reasons
		//_shader.Dispatch(kiDrawGrid, 1, 256, 1);

		_shader.Dispatch(kiOneThreadAction, 1, 1, 1);

		_shader.Dispatch(kiVisualize, 1, 1024, 1);



		_shader.Dispatch(kiCleanGrid, 1, 256, 1);
		_shader.Dispatch(kiRedrawGrid, nGroups, 1, 1);
	}
	static public void doCalcs(int nGroups){				// to raise precision, we lowered time step, but for realtime's sake we run this function N times
		_shader.Dispatch(kiCalc, nGroups, 1, 1);			// this is the main kernel, that calculates a time step of the simulation

		_shader.Dispatch(kiVelShare, nGroups, 1, 1);		// these two kernels implement velocity share between connected particles
		_shader.Dispatch(kiDVelApply, nGroups, 1, 1);		//

		_shader.Dispatch(kiImpulseShare, nGroups, 1, 1);	// these ones share momentary impulses, so intermolecular forces are being spread between particle's neighbors
		_shader.Dispatch(kiDImpulseApply, nGroups, 1, 1);	//

		_shader.Dispatch(kiCalcApply, nGroups, 1, 1);
	}
	static public void passExpl(){							// if there's explosion to pass to the shader, we do
		if (explContainer.ei > 0) {
			explContainer.prepare();
			explBuffer.SetData(explContainer.currentExpl);
		}
	}
	static public void destroy(){							// structured buffers have to be released explixitly
		particlesBuffer.Release();
		gridBuffer.Release();
		pixelsBuffer.Release();
		particlesPosBuffer.Release();
		explBuffer.Release();
		gravMaskBuffer.Release();
		densMaskBuffer.Release();
	}
}
