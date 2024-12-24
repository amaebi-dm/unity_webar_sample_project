using Cysharp.Threading.Tasks;
using UnityEngine;

// -----------------------------------------------------------
/// <summary>
/// センサー類の基底クラス.
/// </summary>
// -----------------------------------------------------------
public abstract class BaseSensor : MonoBehaviour
{
    //! フォーカスがアプリにあるか？
    bool hasFocus = default;

    //! テスト用コンテンツマネージャー.
    protected ContentsManager contentsManager = null;

    // -----------------------------------------------------------
    /// <summary>
    /// センサー起動.
    /// </summary>
    // -----------------------------------------------------------
    public abstract UniTask RunAsync();

    // -----------------------------------------------------------
    /// <summary>
    /// センサー停止.
    /// </summary>
    // -----------------------------------------------------------
    public abstract void Stop();

    // -----------------------------------------------------------
    /// <summary>
    /// フォーカスがアプリに当たるのを待機.
    /// </summary>
    // -----------------------------------------------------------
    protected async UniTask WaitFocusAsync()
    {
        await UniTask.Delay( 100, cancellationToken : this.gameObject.GetCancellationTokenOnDestroy() );
        await UniTask.WaitUntil( () => hasFocus, cancellationToken : this.gameObject.GetCancellationTokenOnDestroy() );
    }

    // -----------------------------------------------------------
    /// <summary>
    /// OnApplicationFocus.
    /// </summary>
    /// <param name="focusStatus"> アプリケーションにフォーカスがある時True. </param>
    // -----------------------------------------------------------
    void OnApplicationFocus( bool focusStatus ) 
    {
        hasFocus = focusStatus;
    }


    public void SetContentsManager( ContentsManager cm )
    {
        contentsManager = cm;
    }
}
