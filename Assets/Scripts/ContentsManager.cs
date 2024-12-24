using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

public class ContentsManager : MonoBehaviour
{
    [SerializeField] CameraSensor cameraSensor = null;
    [SerializeField] GeoSensor geoSensor = null;
    [SerializeField] GyroSensor gyroSensor = null;

    [SerializeField] TextMeshProUGUI debugText = null;



    async void Start()
    {
        cameraSensor.SetContentsManager( this );
        geoSensor.SetContentsManager( this );
        gyroSensor.SetContentsManager( this );

        var t1 = cameraSensor.RunAsync();
        var t2 = geoSensor.RunAsync();
        var t3 = gyroSensor.RunAsync();

        await UniTask.WhenAll( t1, t2, t3 ); 

        DebugLogContents( "Finsh All ..." );
    }



    public void DebugLogContents( string log, bool reset = false )
    {
        if( reset == true )
        {
            debugText.text = "";
        }

        if( string.IsNullOrEmpty( debugText.text ) == false )
        {
            debugText.text += "\n";
        }

        debugText.text += log;
        Debug.Log( log );
    }



}
