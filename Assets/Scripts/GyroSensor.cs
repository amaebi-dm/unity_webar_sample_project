using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

// -----------------------------------------------------------
/// <summary>
/// ジャイロセンサー.
/// </summary>
// -----------------------------------------------------------
[RequireComponent(typeof( Camera ))]
public class GyroSensor : BaseSensor
{
    // -----------------------------------------------------------
    /// <summary>
    /// Androidかどうか.
    /// </summary>
    // -----------------------------------------------------------
    [DllImport( "__Internal" )] static extern int IsAndroid();

    float slerpValue = 0.2f;
    int verticalOffsetAngle = -90;
    int horizontalOffsetAngle = 0;
    bool isAndroid = default;

    void Update() 
    {
        UpdateCameraRotate();
    }

    // -----------------------------------------------------------
    /// <summary>
    /// 起動.
    /// </summary>
    // -----------------------------------------------------------
    public override async UniTask RunAsync()
    {
        contentsManager.DebugLogContents( "GyroSensor Run Start..." );

        // ジャイロセンサーの有効化.
        // iosのみ。この際にパーミッションのダイアログが表示される.
        // 画面を一度ボタン等でタップさせる必要がある.
        Input.gyro.enabled = true;

        await WaitFocusAsync();
        await UniTask.WaitUntil( () => Input.gyro.enabled, cancellationToken : this.gameObject.GetCancellationTokenOnDestroy() );

        // Androidかどうか.
        isAndroid = IsAndroid() == 1;
        Debug.Log( $"IsAndroid : { isAndroid }" );

        // 水平軸をAndroidの時は-90、iosの時は0.
        horizontalOffsetAngle = isAndroid ? -90 : 0;

        contentsManager.DebugLogContents( "GyroSensor Run Finish..." );
    }

    // -----------------------------------------------------------
    /// <summary>
    /// 停止.
    /// </summary>
    // -----------------------------------------------------------
    public override void Stop()
    {
        Input.gyro.enabled = false;
    }

    // -----------------------------------------------------------
    /// <summary>
    /// カメラの向きを取得.
    /// </summary>
    // -----------------------------------------------------------  
    public Vector3 GetCameraEulerAngles()
    {
        return transform.eulerAngles;
    }

    // -----------------------------------------------------------
    /// <summary>
    /// 右手系からUnityの左手系に変更.
    /// </summary>
    /// <param name="q"></param>
    // -----------------------------------------------------------
    Quaternion GyroToUnity( Quaternion q )
    {
        return new Quaternion( q.x, q.y, -q.z, -q.w );
    }

    // -----------------------------------------------------------
    /// <summary>
    /// カメラの向きを更新.
    /// </summary>
    // -----------------------------------------------------------
    void UpdateCameraRotate()
    {
        if( !Input.gyro.enabled ) return;

        // Androidかどうか.
        var _isAndroid = IsAndroid() == 1;
        // ジャイロセンサーから傾きの取得.
        var correctPhoneOrientation = GyroToUnity( Input.gyro.attitude );
        // 縦軸.
        var verricalRotationCorrection = Quaternion.AngleAxis( verticalOffsetAngle, Vector3.left );
        // 横軸.
        var horizontalRotationCorrection = Quaternion.AngleAxis( horizontalOffsetAngle, Vector3.up );
        // 向きを乗算してMainCameraに反映.
        var inGameOrientation = horizontalRotationCorrection * verricalRotationCorrection * correctPhoneOrientation;
        transform.rotation = Quaternion.Slerp( transform.rotation, inGameOrientation, slerpValue );
    }
}
