using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;
    public int exp = 0;
    public int expToNext = 100;

    public void GainExp(int amount)
    {
        exp += amount;
        if (exp >= expToNext)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        exp = 0;
        expToNext += 50;

        UpgradeUI.Instance.Show();
    }
}
