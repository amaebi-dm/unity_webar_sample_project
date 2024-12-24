using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof( RawImage ))]
public class CameraSensor : BaseSensor
{
    //! カメラプレビュー.
    RawImage cameraPreview = null;
    //! Webカメラ.
    WebCamTexture webCamTexture = null;
    //! カメラデバイス.
    WebCamDevice[] devices = null;

    //! カメラの許可を取得しているか.
    bool isAuthorization => Application.HasUserAuthorization( UserAuthorization.WebCam );

    // -----------------------------------------------------------
    /// <summary>
    /// 起動.
    /// </summary>
    // -----------------------------------------------------------
    public override async UniTask RunAsync()
    {
        contentsManager.DebugLogContents( "WenCamera Run Start...." );

        if( isAuthorization == true )
        {
            SetWebCamera();
            return;
        }

        await Application.RequestUserAuthorization( UserAuthorization.WebCam );
        await  WaitFocusAsync();

        if( isAuthorization == true )
        {
            SetWebCamera();
        }

        contentsManager.DebugLogContents( "WenCamera Run Finish...." );
    }

    // -----------------------------------------------------------
    /// <summary>
    /// 停止.
    /// </summary>
    // -----------------------------------------------------------
    public override void Stop()
    {
        webCamTexture = null;
        if( cameraPreview != null ) cameraPreview.color = Color.black;
    }

    // -----------------------------------------------------------
    /// <summary>
    /// WebCameraの設定.
    /// </summary>
    // -----------------------------------------------------------
    void SetWebCamera()
    {
        Debug.Log( "WebCamera Found..." );

        devices = WebCamTexture.devices;

        for( int cameraIndex = 0; cameraIndex < devices.Length; ++cameraIndex )
        {
            Debug.Log( $"Camera Index `{ cameraIndex }`" );
            Debug.Log( $"Name : { devices[cameraIndex].name }" );
            Debug.Log( $"IsFrontFacing : { devices[cameraIndex].isFrontFacing }" );
            Debug.Log( $"Kind : { devices[cameraIndex].kind }" );
        }

        if( devices.Length > 0 )
        {
            if( cameraPreview == null )
            {
                cameraPreview = GetComponent<RawImage>();
            }

            // 0 : Front / 1 : Back.
            var deviceName = devices[1].name;
            webCamTexture = new WebCamTexture( deviceName );
            cameraPreview.color = Color.white;
            cameraPreview.texture = webCamTexture;
            webCamTexture.Play();
        }
    }
}
