using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

// -----------------------------------------------------------
/// <summary>
/// 地理情報の取得センサー.
/// </summary>
// -----------------------------------------------------------
public class GeoSensor : BaseSensor
{
    // -----------------------------------------------------------
    /// <summary>
    /// 位置情報の精度（メートル単位）.
    /// </summary>
    /// <remarks>
    /// 使用するサービス精度（メートル単位）。これにより、デバイスの最後の一座表の精度が決まります。
    /// 500などの高い値をしていすると、デバイスはGPSチップを使用する必要がなくなり、バッテリー電力を節約できます。
    /// 5-10などの低い値を指定すると、最高の精度が得られますが、GPSチップが必要になるため、バッテリー電力の消費量が増えます。
    /// デフォルト値は10メートルです。
    /// </remarks>
    // -----------------------------------------------------------
    [SerializeField, Header( "位置情報の精度（メートル単位）" )] float desiredAccuracyInMeter = 10f;

    // -----------------------------------------------------------
    /// <summary>
    /// 位置情報の更新単位（メートル単位）.
    /// </summary>
    /// <remarks>
    /// UnityがInput.locationを更新する前にデバイスが横方向に移動する必要がある最小距離（メートル単位）.
    /// 500などの高い値を指定すると、更新が少なくなり、処理に必要なリソースが少なくなります。
    /// デフォルトは10メートルです。
    /// </remarks>
    // -----------------------------------------------------------
    [SerializeField,Header( "位置情報の更新単位（メートル単位）" )] float updateDistanceInMeters = 10f;

    // -----------------------------------------------------------
    /// <summary>
    /// 位置情報は取得済みか.
    /// </summary>
    // -----------------------------------------------------------
    public bool IsRunning => Input.location.status == LocationServiceStatus.Running;

    // -----------------------------------------------------------
    /// <summary>
    /// 起動.
    /// </summary>
    // -----------------------------------------------------------
    public override async UniTask RunAsync()
    {
        contentsManager.DebugLogContents( "GeoSensor Run Start ..." );
        // コンパスを有効化.
        Input.compass.enabled = true;

        // ロケーションの取得を開始.
        Input.location.Start( desiredAccuracyInMeter, updateDistanceInMeters );
        await WaitFocusAsync();
        await UniTask.WaitUntil( () => IsRunning, cancellationToken : this.GetCancellationTokenOnDestroy() );

        contentsManager.DebugLogContents( "GeoSensor Run Finish ..." );
    }

    // -----------------------------------------------------------
    /// <summary>
    /// 停止.
    /// </summary>
    // -----------------------------------------------------------
    public override void Stop()
    {
        Input.compass.enabled = false;
        Input.location.Stop();   
    }

    // -----------------------------------------------------------
    /// <summary>
    /// 現在の地理座標の取得.
    /// </summary>
    // -----------------------------------------------------------
    public double3 GetCurrentLocation()
    {
        if( !IsRunning )
        {
            Debug.LogError( "センサーを起動してください." );
            return double3.zero;
        }

        var locationLastData = Input.location.lastData;

        return new double3( locationLastData.latitude, locationLastData.longitude, locationLastData.altitude );
    }

    // -----------------------------------------------------------
    /// <summary>
    /// 現在の向きの取得（地理的な北極に対する度数で表した進行方向を取得）.
    /// </summary>
    // -----------------------------------------------------------
    public float GetCurrentDirection()
    {
        // デフォルトだと向きが反対に回転するため、逆にする.
        return 360f - Input.compass.trueHeading;
    }
}
