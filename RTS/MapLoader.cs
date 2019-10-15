using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MapLoader : MonoBehaviour
{
    [System.Serializable]
    public struct Map
    {
	    public string Name;
        public Texture Icon;
        public int xHeight;
        public int yHeight;
        public int PlayerAmount;
        public string SceneName;
    }

    public GameObject MapPrefab;
    public Map[] Maps;

    List<GameObject> Tabs = new List<GameObject>();

    int Selected = 0;

    void Start()
    {
        //create all maps using prefabs and filling out the required information using an array
        for(int i = 0; i < Maps.Length; i++)
            MakeTab(i);

        RefreshTabs();
    }

    void MakeTab(int Index)
    {

        GameObject NewMap = Instantiate(MapPrefab, this.transform.position, Quaternion.identity);
        NewMap.transform.SetParent(this.transform);
        NewMap.name = "tab_" + Maps[Index].SceneName;

        Tabs.Add(NewMap);

        NewMap.transform.GetChild(0).GetComponent<RawImage>().texture = Maps[Index].Icon; 
        NewMap.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Maps[Index].xHeight + "x" + Maps[Index].yHeight;
        NewMap.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Maps[Index].PlayerAmount + " Players";
        NewMap.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = Maps[Index].Name;

    }

    public void ChangeTab(bool Left)
    {

        if(Left)
            if(Selected == 0)
                Selected = Maps.Length - 1;
            else
                Selected--;

        if(!Left)
            if(Selected == Maps.Length - 1)
                Selected = 0;
            else
                Selected++;

        RefreshTabs();

    }
    
    void RefreshTabs()
    {

        for(int i = 0; i < Maps.Length; i++)
            if(i != Selected)
                Tabs[i].SetActive(false);
            else
                Tabs[Selected].SetActive(true);

    }

    public void Play()
    {

        SceneManager.LoadScene(Maps[Selected].SceneName, LoadSceneMode.Single);

        //loadscene(maps[selected].scenename);

    }

}
