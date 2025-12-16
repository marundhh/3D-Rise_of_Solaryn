using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public WeaponData selectedWeaponData;
    public GameObject model;
    private Transform floatingPosition;
    private Transform handPosition;

    private GameObject spawnedWeapon;

    public static WeaponManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InitData();
        FindWeaponPositions();
        SpawnWeapon();
        selectedWeaponData.Init();
    }
    private void InitData()
    {
        selectedWeaponData = ClassDataPlayerChoose.instance.weaponData;
    }

    private void FindWeaponPositions()
    {
        if (model == null)
        {
            Debug.LogError("Model is null!");
            return;
        }

        ModelRoot modelRoot = model.GetComponentInChildren<ModelRoot>(true);
        if (modelRoot == null)
        {
            Debug.LogError("Can not find script ModelRoot in model!");
            return;
        }

        floatingPosition = modelRoot.FloatingPosition.transform;
        handPosition = modelRoot.HandPosition.transform;

        if (!selectedWeaponData.isHand && floatingPosition != null)
        {
            FloatingFollower follower = floatingPosition.gameObject.GetComponent<FloatingFollower>();
            if (follower == null)
            {
                follower = floatingPosition.gameObject.AddComponent<FloatingFollower>();
                follower.target = model.transform;
            }
        }
    }

    private void SpawnWeapon()
    {
        if (selectedWeaponData == null)
        {
            Debug.LogWarning("Weapon prefab is null");
            return;
        }

        Transform parent = selectedWeaponData.isHand ? handPosition : floatingPosition;

        if (parent == null)
        {
            Debug.LogError("Spawn possition error");
            return;
        }

        spawnedWeapon = Instantiate(selectedWeaponData.weaponModel);
        spawnedWeapon.transform.SetParent(parent, worldPositionStays: false);
    }

    public void ChangeWeapon(WeaponData newWeaponData)
    {
        if (newWeaponData == null)
        {
            Debug.LogWarning("New weapon data is null!");
            return;
        }

        if (spawnedWeapon != null)
        {
            Destroy(spawnedWeapon);
            spawnedWeapon = null;
        }

        selectedWeaponData = newWeaponData;

        FindWeaponPositions();

        SpawnWeapon();

        selectedWeaponData.Init();

        Debug.Log($"Changed weapon to: {selectedWeaponData.name}");
    }

}
