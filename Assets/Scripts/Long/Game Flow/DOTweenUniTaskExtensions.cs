using DG.Tweening;
using Cysharp.Threading.Tasks;

public static class DOTweenUniTaskExtensions
{
    public static UniTask AwaitForCompletion(this Tween tween)
    {
        var tcs = new UniTaskCompletionSource();

        if (tween == null || !tween.active || tween.IsComplete())
        {
            tcs.TrySetResult();
            return tcs.Task;
        }

        tween.OnComplete(() => tcs.TrySetResult());
        tween.OnKill(() => tcs.TrySetResult());

        return tcs.Task;
    }
}