using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public TextMeshProUGUI missionText2;

    public bool timeScaleOnMission = false; // Biến để kiểm soát việc tạm dừng thời gian khi hiển thị nhiệm vụ

    public void ShowMissionDetails(MissionRaw mission)
    {
        if (mission == null)
        {
            CloseMissionDetails();
            return;
        }
        Debug.Log($"Showing mission details for: {mission.tile}, {mission.id}");

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
        DOVirtual.DelayedCall(2f, () =>
        {
            // Nếu panel đã bị đóng trước khi animation xảy ra => thoát
            if (!missionAcceptPanel.activeInHierarchy) return;

            if (questAppearSFX && sfxSource)
                sfxSource.PlayOneShot(questAppearSFX, 0.6f);

            missionAcceptPanel.transform.DOScale(1f, 0.8f)
                .SetEase(Ease.OutSine);

            if (cg != null)
            {
                cg.DOFade(1f, 0.8f)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() =>
                    {
                        // Kiểm tra thêm 1 lần nữa để đảm bảo không ghi Time.timeScale nếu đã bị hủy
                        if (!missionAcceptPanel.activeInHierarchy) return;

                        cg.blocksRaycasts = true;
                        Time.timeScale = 0f;
                        Debug.Log("Set timescale 0");
                    });
            }
        });

    }
    public void CloseMissionDetails()
    {
        Time.timeScale = 1f; // Đặt thời gian về bình thường\
        timeScaleOnMission = false; // Đặt biến để kiểm soát việc tạm dừng thời gian
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
                Time.timeScale = 1f;
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
                return $"Interact with: <color=red>{mission.type}</color>";
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

            // Tách 2 nhóm nhiệm vụ
            var progressingMissions = allMissions.Where(m => !m.IsCompleted).ToList();
            var completedMissions = allMissions.Where(m => m.IsCompleted).ToList();

            // Tạo danh sách hiển thị tối đa 4 nhiệm vụ
            List<Mission> displayMissions = new List<Mission>();

            // Ưu tiên nhiệm vụ chưa hoàn thành
            displayMissions.AddRange(progressingMissions.Take(3));

            // Nếu chưa đủ 4, thêm các nhiệm vụ đã hoàn thành
            if (displayMissions.Count < 3)
            {
                int remaining = 3 - displayMissions.Count;
                displayMissions.AddRange(completedMissions.Take(remaining));
            }

            // Tạo chuỗi hiển thị
            string display = "";
            foreach (var m in displayMissions)
            {
                string status = m.IsCompleted ? "<color=green>Complete</color>" : "<color=red>Progressing</color>";
                if (m.IsCompleted) {
                    display += $"[{status}] {m.Title}\n";
                } else
                {
                    display += $"[{status}] {m.Title}: {m.GetProgressText()}\n";
                }
            }

            missionText.text = display;
            // Cập nhật missionText2 nếu có
            if( missionText2 != null)
                missionText2.text = display;
        }
    }

}

