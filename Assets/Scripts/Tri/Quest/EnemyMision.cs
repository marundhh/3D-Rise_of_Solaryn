using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    public string enemyType = "Slime";

    public void Die()
    {
        //EventSystem.Dispatch(new EnemyKilledEvent(enemyType));
        Destroy(gameObject);
    }
}
