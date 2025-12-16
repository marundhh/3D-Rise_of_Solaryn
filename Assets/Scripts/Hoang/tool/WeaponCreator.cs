using UnityEngine;
using UnityEditor;
using System.IO;

public class WeaponCreator : EditorWindow
{
    private GameObject weaponModel;
    private string weaponName = "NewWeapon";
    private int weaponID = 0;
    private float weaponDamage = 10f;
    private int weaponPrice = 100;
    private Sprite weaponIcon;
    private RuntimeAnimatorController animatorController;

    private string prefabPath = "Assets/Prefabs/Phuc/Weapon";
    private string dataPath = "Assets/Scripts/Phuc/WeaponData";
    private string iconPath = "Assets/Materials/Hoang/Weapon"; // thêm thư mục để chứa icon

    [MenuItem("Tools/Weapon Creator")]
    public static void ShowWindow()
    {
        GetWindow<WeaponCreator>("Weapon Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Weapon Creator", EditorStyles.boldLabel);

        weaponModel = (GameObject)EditorGUILayout.ObjectField("Weapon Model", weaponModel, typeof(GameObject), false);
        weaponName = EditorGUILayout.TextField("Weapon Name", weaponName);
        weaponID = EditorGUILayout.IntField("Weapon ID", weaponID);
        weaponDamage = EditorGUILayout.FloatField("Damage", weaponDamage);
        weaponPrice = EditorGUILayout.IntField("Price", weaponPrice);
        weaponIcon = (Sprite)EditorGUILayout.ObjectField("Icon", weaponIcon, typeof(Sprite), false);
        animatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(RuntimeAnimatorController), false);

        prefabPath = EditorGUILayout.TextField("Prefab Save Path", prefabPath);
        dataPath = EditorGUILayout.TextField("Data Save Path", dataPath);
        iconPath = EditorGUILayout.TextField("itcon Save Path", iconPath);

        if (GUILayout.Button("Create Weapon") && weaponModel != null)
        {
            CreateWeapon();
        }
    }

    private void CreateWeapon()
    {
        if (weaponModel == null)
        {
            Debug.LogError(" Weapon Model chưa được chọn!");
            return;
        }

        // tạo thư mục nếu chưa có
        if (!Directory.Exists(prefabPath)) Directory.CreateDirectory(prefabPath);
        if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
        if (!Directory.Exists(iconPath)) Directory.CreateDirectory(iconPath);

        // tạo instance tạm từ weaponModel
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(weaponModel);

        if (instance == null)
        {
            Debug.LogError(" PrefabUtility.InstantiatePrefab() trả về NULL. Có thể weaponModel không phải prefab hoặc asset hợp lệ.");
            return;
        }

        instance.name = weaponName;

        // collider lớn
        BoxCollider box = instance.GetComponent<BoxCollider>();
        if (box == null) box = instance.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(2f, 3f, 2f);
        box.center = Vector3.zero;

        // thêm ItemPickup
        var pickup = instance.GetComponent<ItemPickup>();
        if (pickup == null) pickup = instance.AddComponent<ItemPickup>();

        // tạo WeaponData
        WeaponData weaponData = ScriptableObject.CreateInstance<WeaponData>();
        weaponData.id = weaponID;
        weaponData.weaponName = weaponName;
        weaponData.damage = weaponDamage;
        weaponData.price = weaponPrice;
        weaponData.isHand = true;
        weaponData.weaponModel = weaponModel;
        weaponData.controller = animatorController;

        // nếu chưa chọn icon → tự chụp bằng AssetPreview
        if (weaponIcon == null)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(weaponModel);
            if (preview != null)
            {
                string relativeIconPath = $"{iconPath}/{weaponName}.png";
                File.WriteAllBytes(relativeIconPath, preview.EncodeToPNG());
                AssetDatabase.Refresh();

                TextureImporter importer = AssetImporter.GetAtPath(relativeIconPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.SaveAndReimport();
                }

                weaponIcon = AssetDatabase.LoadAssetAtPath<Sprite>(relativeIconPath);
            }
        }

        weaponData.icon = weaponIcon;

   
        string assetPath = $"{dataPath}/{weaponName}.asset";
        AssetDatabase.CreateAsset(weaponData, assetPath);

        // gán data vào pickup
        pickup.weaponData = weaponData;

        // lưu prefab
        string prefabFile = $"{prefabPath}/{weaponName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(instance, prefabFile);

        // xóa instance trong scene
        DestroyImmediate(instance);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

       
    }

}
