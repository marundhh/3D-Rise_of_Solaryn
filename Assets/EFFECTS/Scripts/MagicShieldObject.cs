using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShieldObject : MonoBehaviour
{

    public void MagicShieldCast()
    {
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.zero;
        Sequence shieldSeq = DOTween.Sequence();


        shieldSeq.Append(gameObject.transform.DOScale(2.2f, 0.8f)
            .SetEase(Ease.OutBack));


        shieldSeq.Append(gameObject.transform.DOScale(2.0f, 0.4f)
            .SetEase(Ease.InQuad));
    }
    public void MagicShieldEnd()
    {
        Sequence endSeq = DOTween.Sequence();

        endSeq.Append(gameObject.transform.DOScale(2.2f, 0.4f)
            .SetEase(Ease.OutQuad));


        endSeq.Append(gameObject.transform.DOScale(0f, 0.8f)
            .SetEase(Ease.InBack))
            .OnComplete(() => gameObject.SetActive(false));
    }
}
