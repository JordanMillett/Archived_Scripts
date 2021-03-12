using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CustomizationMenu : MonoBehaviour
{
    MenuManager MM;
    
    Material[] Mats = new Material[4];

    SkinnedMeshRenderer Model;
    GameObject Cam;
    //SkinnedMeshRenderer RagdollModel;

    //Material[] RagdollMats = new Material[4];

    public Player PC;

    int SelectedIndex = -1;

    public GameObject Picker;

    public List<RawImage> Labels;

    public RawImage CustomFaceIcon;
    public Texture2D CustomFace;
    public List<byte> CustomFaceData = new List<byte>();
    public TexturePaint TP;

    public GameObject Main;

    Color[] MeshColors = new Color[3]
    {
        new Color(89f/255f,177f/255f,94f/255f,1),
        new Color(247f/255f,145f/255f,82f/255f,1),
        new Color(37f/255f,113f/255f,140f/255f,1)
    };

    int faceIndex = 0;

    public void Init()
    {
        PC = GameObject.FindWithTag("Player").GetComponent<Player>();
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
        Cam = PC.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).gameObject;
        Model =         PC.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();
        //RagdollModel =  PC.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();

        Model.gameObject.layer = 8;
        Mats = Model.materials;
        //RagdollMats = RagdollModel.materials;

        TP.CM = this;

        Main.SetActive(true);
        Picker.SetActive(false);
        TP.gameObject.SetActive(false);

        InitializeSettings();
        InitializeCustomFace();
        Apply();
        SaveSettings();
        SaveCustomFace();
    }

    void Update()
    {
        Apply();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(SelectedIndex != -1)
            {
                TogglePicker(-1);
            }else if(TP.gameObject.activeSelf)
            {
                TP.gameObject.SetActive(false);
                Main.SetActive(true);
            }
            else
            {
                MM.SetScreen("Pause");
            }
        }
    }

    void Apply()
    {
        Mats[0].SetColor("_albedo", MeshColors[0]);         //SKIN
        Mats[3].SetColor("_albedo", MeshColors[0]);
        //RagdollMats[0].SetColor("_albedo", MeshColors[0]);
        //RagdollMats[3].SetColor("_albedo", MeshColors[0]);
        Labels[0].color = MeshColors[0];
        
        Mats[2].SetColor("_albedo", MeshColors[1]);         //PANTS
        //RagdollMats[2].SetColor("_albedo", MeshColors[1]);
        Labels[2].color = MeshColors[2];

        Mats[1].SetColor("_albedo", MeshColors[2]);         //SHIRT
        //RagdollMats[1].SetColor("_albedo", MeshColors[2]);
        Labels[1].color = MeshColors[1];

        if(faceIndex != 4)
        {
            Mats[3].SetTexture("_face", GameServer.GS.GetFace(faceIndex)); //FACE
            //RagdollMats[3].SetTexture("_face", GameServer.GS.GetFace(faceIndex));
        }else
        {
            Mats[3].SetTexture("_face", CustomFace); //FACE
            //RagdollMats[3].SetTexture("_face", CustomFace);
        }
    }

    void OnEnable()
    {
        Cam = GameObject.FindWithTag("Player").transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).gameObject;
        Cam.SetActive(true);
    }

    void OnDisable()
    {
        if(Cam != null)
            Cam.SetActive(false);
        Picker.SetActive(false);
        TP.gameObject.SetActive(false);
        Main.SetActive(true);
        SaveSettings();
    }

    public void SetFaceIndex(int Index)
    {
        faceIndex = Index;

        if(faceIndex == 4)
        {
            TP.Face = CustomFace;
            Main.SetActive(false);
            TP.gameObject.SetActive(true);
        }
    }

    public void CloseFaceScreen()
    {
        Main.SetActive(true);
        TP.gameObject.SetActive(false);
    }

    public void SetColor(Color C)    //Index of mesh color is determined by which color picker menu is open
    {
        MeshColors[SelectedIndex] = C;
        TogglePicker(-1);
    }

    public void TogglePicker(int index)
    {
        if(index == -1)
        {
            SelectedIndex = index;
            Picker.SetActive(false);
        }else
        {
            SelectedIndex = index;
            Picker.SetActive(true);
        }
    }

    public void InitToGhost()
    {
        //OnlineGhost.SendColors(faceIndex, MeshColors[0], MeshColors[1], MeshColors[2]);
        PC.ChangeColors(faceIndex, MeshColors[0], MeshColors[2], MeshColors[1], CustomFaceData);
    }

    void InitializeSettings()
    {
        //Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "customize.txt";
 
        if(!File.Exists(path)) 
        {
            SettingsToFile();
        }

        StreamReader SR = new StreamReader(path);
        string settingText = SR.ReadToEnd();
        SR.Close();

        string[] settingsLines = settingText.Split("\n"[0]);

        //Debug.Log(MeshColors[0]);

        faceIndex = int.Parse(settingsLines[0]);                                //FACE
        ColorUtility.TryParseHtmlString("#" + settingsLines[1], out MeshColors[0]);   //SKIN
        ColorUtility.TryParseHtmlString("#" + settingsLines[2], out MeshColors[1]);   //PANTS
        ColorUtility.TryParseHtmlString("#" + settingsLines[3], out MeshColors[2]);   //SHIRT

        //Debug.Log(MeshColors[0]);

    }

    public void SaveSettings()  //clear and write to file
    {
        string path = Application.persistentDataPath + "customize.txt";
        File.WriteAllText(path, "");
        SettingsToFile();

        if(PC != null)
            InitToGhost();
    }

    void SettingsToFile()
    {
        string path = Application.persistentDataPath + "customize.txt";

        StreamWriter SW = new StreamWriter(path);

        string settingsLines = "";

        settingsLines += (int) faceIndex + "\n";
        settingsLines += ColorUtility.ToHtmlStringRGB(MeshColors[0]) + "\n";
        settingsLines += ColorUtility.ToHtmlStringRGB(MeshColors[1]) + "\n";
        settingsLines += ColorUtility.ToHtmlStringRGB(MeshColors[2]) + "\n";

        SW.Write(settingsLines);
        SW.Close();
    }

    void InitializeCustomFace()
    {
        //Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "customface.png";
 
        if(!File.Exists(path)) 
        {
            CustomFaceToFile();
        }

        //StreamReader SR = new StreamReader(path);
        //string settingText = SR.ReadToEnd();
        //SR.Close();

        //string[] settingsLines = settingText.Split("\n"[0]);

        byte[] CustomFaceDataArray = File.ReadAllBytes(path);

        CustomFaceData.Clear();
        for(int i = 0; i < CustomFaceDataArray.Length; i++)
        {
            CustomFaceData.Add(CustomFaceDataArray[i]);
        }

        CustomFace = new Texture2D(32, 32, TextureFormat.Alpha8, false);
        CustomFace.LoadImage(CustomFaceDataArray);
        CustomFace.filterMode = FilterMode.Point;
        CustomFace.Apply();

    }

    public void SaveCustomFace()  //clear and write to file
    {
        CustomFaceIcon.texture = CustomFace;
        string path = Application.persistentDataPath + "customface.png";
        File.WriteAllText(path, "");
        CustomFaceToFile();
    }

    void CustomFaceToFile()
    {
        string path = Application.persistentDataPath + "customface.png";

        byte[] CustomFaceDataArray = CustomFace.EncodeToPNG();
    
        CustomFaceData.Clear();
        for(int i = 0; i < CustomFaceDataArray.Length; i++)
        {
            CustomFaceData.Add(CustomFaceDataArray[i]);
        }

        File.WriteAllBytes(path, CustomFaceDataArray);
    }

    /*
    Sprite itemBGSprite = Resources.Load<Sprite>( "_Defaults/Item Images/_Background" );
    Texture2D itemBGTex = itemBGSprite.texture;
    byte[] itemBGBytes = itemBGTex.EncodeToPNG();
    File.WriteAllBytes( formattedCampaignPath + "/Item Images/Background.png" , itemBGBytes );
    */
}
