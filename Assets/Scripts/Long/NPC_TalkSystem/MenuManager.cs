using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class MenuManager : MonoBehaviour
{
   public static MenuManager Instance;

   public List <string> menuName = new List<string>(); 
   public List<GameObject> menuPanel = new List<GameObject>();

  
   public Dictionary<string, GameObject> menuDictionary = new Dictionary<string, GameObject>();

   private void Awake()
   {
         if (Instance == null)
         {
              Instance = this;
              DontDestroyOnLoad(gameObject);
         }
         else
         {
              Destroy(gameObject);
         }

       if (menuPanel != null)
       {
         for (int i = 0; i < menuPanel.Count; i++)
         {
             if (menuPanel[i] != null && !menuDictionary.ContainsKey(menuName[i]))
             {
                 menuDictionary.Add(menuName[i], menuPanel[i]);
                 menuPanel[i].SetActive(false); // Ensure all menus are initially inactive
             }
         }
       }
   }
   public void OpenMenu(string name)
   {
       if (menuDictionary.ContainsKey(name))
       {
           menuDictionary[name].SetActive(true);
       }
   }

   public void CloseMenu(string name)
   {
       if (menuDictionary.ContainsKey(name))
       {
           menuDictionary[name].SetActive(false);
       }
    }
}
