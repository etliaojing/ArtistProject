using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InkSwapBuffer
{
    private RenderTexture _front;
    private RenderTexture _back;

    private Camera _camera;
    private GameObject _camObj;

    public InkSwapBuffer(int width, int height, bool useMipMap)
    {
        _camObj = Object.Instantiate( Camera.main.gameObject
                                              , Camera.main.transform.position
                                              , Camera.main.transform.rotation
                                              ) as GameObject;
        _camera = _camObj.GetComponent<Camera>();

        //@TODO: Switch to ARGBFloat?
        if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
        {
            Debug.LogError("System must support RenderTextureFormat.ARGBHalf");
        }

        _front = new RenderTexture( width
                                  , height
                                  , 0
                                  , RenderTextureFormat.ARGBHalf
                                  , RenderTextureReadWrite.Linear
                                  );

        if (!useMipMap)
        {
            _front.useMipMap = false;
            _front.filterMode = FilterMode.Bilinear;
        }

        _front.wrapMode = TextureWrapMode.Clamp;
        _front.Create();
        _camera.targetTexture = _front;
        _camera.Render();
        _camera.targetTexture = null;

        _back = new RenderTexture( width
                                 , height
                                 , 0
                                 , RenderTextureFormat.ARGBHalf
                                 , RenderTextureReadWrite.Linear
                                 );

        if (!useMipMap)
        {
            _back.useMipMap = false;
            _back.filterMode = FilterMode.Bilinear;
        }

        _back.wrapMode = TextureWrapMode.Clamp;
        _back.Create();
        _camera.targetTexture = _back;
        _camera.Render();
        _camera.targetTexture = null;

        //_camera = null;
        //Object.Destroy(_camObj);
        
    }

    public RenderTexture GetFrontBuffer()
    {
        return _front;
    }

    public RenderTexture GetBackBuffer()
    {
        return _back;
    }

    public void Swap()
    {
        RenderTexture temp;
        temp = _front;
        _front = _back;
        _back = temp;
    }


}