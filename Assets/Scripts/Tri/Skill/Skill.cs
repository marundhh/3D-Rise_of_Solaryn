using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill System/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float cooldown;
    public string description;
    // Thêm hiệu ứng hoặc hành động thực thi kỹ năng nếu muốn
}
