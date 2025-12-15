using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MissionDisplay : MonoBehaviour
{
    [SerializeField] private AudioClip questAppearSFX;
    [SerializeField] private AudioSource sfxSource;


    public GameObject missionAcceptPanel; // Panel hiển thị nhiệm vụ
    public TextMeshProUGUI missionTile; // Tiêu đề nhiệm vụ
    public TextMeshProUGUI missionDescription; // Mô tả nhiệm vụ
    public TextMeshProUGUI missionReward; // Phần thưởng nhiệm vụ
    public TextMeshProUGUI missionTarget; // Mục tiêu nhiệm vụ


    public TextMeshProUGUI missionText;



    public void ShowMissionDetails(MissionRaw mission)
    {
        if (mission == null)
        {
            CloseMissionDetails();
            return;
        }

      

        missionAcceptPanel.SetActive(true);
        var cg = missionAcceptPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
        }

        // Bắt đầu với scale nhỏ hoàn toàn
        missionAcceptPanel.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        // Gán nội dung
        missionTile.text = mission.tile;
        missionDescription.text = mission.description;
        missionReward.text = $"Reward: \n   <color=yellow>{mission.rewardGold} Gold</color> \n   <color=green>{mission.rewardExp}</color> EXP ";
        missionTarget.text = GetType(mission);

        // Bắt đầu delay
        DOVirtual.DelayedCall(1f, () =>
        {
            if (questAppearSFX && sfxSource)
                sfxSource.PlayOneShot(questAppearSFX, 0.6f);

            missionAcceptPanel.transform.DOScale(1f, 0.8f)
           .SetEase(Ease.OutSine);

            if (cg != null)
            {
                cg.DOFade(1f, 0.8f)
                  .SetEase(Ease.OutSine)
                  .OnComplete(() => { cg.blocksRaycasts = true;  });
                  }
        });
    }
    public void CloseMissionDetails()
    {
        Time.timeScale = 1f; // Đặt thời gian về bình thường
        Debug.Log("time scale set to 1");
        var cg = missionAcceptPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
            cg.DOFade(0f, 0.4f).SetEase(Ease.InSine);
        }

        missionAcceptPanel.transform.DOScale(0.8f, 0.4f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                missionAcceptPanel.SetActive(false);
            });
    }


    public string GetType(MissionRaw mission)
    {
        switch(mission.missionType)
        {
            case MissionType.KillEnemy:
                return $"Kill: {mission.count}  <color=red>{mission.type}s</color>" ;
            case MissionType.TalkToNPC:
                return $"Talking with: <color=red>{mission.type}</color>";
            case MissionType.CollectionItem:
                return $"Collect: <color=red>{mission.type}</color>";
            case MissionType.FindWay:
                return $"Reach to: <color=red>{mission.type}</color>";
            default:
                return "Unknown";
        }
    }
    private void Start()
    {
        StartCoroutine(MissionChecker());
    }

    private IEnumerator MissionChecker()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            var allMissions = MissionManager.Instance.GetAllMissions();
            int count = allMissions.Count;
            string display = "";

            int startIndex = Mathf.Max(0, count - 4); 

            for (int i = startIndex; i < count; i++)
            {
                var m = allMissions[i];
                string status = m.IsCompleted ? "<color=green>Complete</color>" : "<color=red>Progressing</color>";
                display += $"[{status}] {m.Title}: {m.GetProgressText()} \n";
            }

            missionText.text = display;
        }
    }
}

