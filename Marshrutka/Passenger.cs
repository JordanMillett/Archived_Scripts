using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    //DATATYPES
    public enum Status
	{
		Waiting,
		Approved,
		Denied
	};

    //PUBLIC COMPONENTS
    public Transform DocumentLocation;
    public GameObject DocumentPrefab;
    public AudioSourceController ASC;
    public Conversation C;
    public Person P;
    public bool Male;
    public Texture2D Face;

    //PUBLIC VARS
    public Document.FakedPart Faked;
    public bool Busy = false;
    public Status CurrentStatus = Status.Waiting;
    public int Weight;
    public int HeightFoot;
    public int HeightInch;

    //PUBLIC LISTS
    public List<Texture2D> Clothes;
    public List<AudioClip> Noises;

    //COMPONENTS
    TextBox TB;
    Manager M;
    Animator An;

    //VARS
    bool isSpeaking = false;

    //LISTS
    
    void Start()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();
        C = M.Conversations[Random.Range(0, M.Conversations.Count)];
        P = M.People[Random.Range(0, M.People.Count)];
        Male = Random.value > 0.5f;
        Face = Male ? M.MaleFaces[Random.Range(0, M.MaleFaces.Count)] : M.FemaleFaces[Random.Range(0, M.FemaleFaces.Count)];

        Weight = Random.Range(GameSettings.WeightRange.x, GameSettings.WeightRange.y);
        HeightFoot = Random.Range(GameSettings.HeightFootRange.x, GameSettings.HeightFootRange.y);
        HeightInch = Random.Range(GameSettings.HeightInchRange.x, GameSettings.HeightInchRange.y);

        GameObject Model = Instantiate(P.Model, Vector3.zero, Quaternion.identity);
        Model.transform.SetParent(this.transform.GetChild(0));
        Model.transform.localPosition = Vector3.zero;
        Model.transform.localEulerAngles = Vector3.zero;

        float ScaleWeight = ((float)Weight - GameSettings.WeightRange.x)/(GameSettings.WeightRange.y - GameSettings.WeightRange.x);
        ScaleWeight = Mathf.Lerp(GameSettings.WeightLerp.x, GameSettings.WeightLerp.y, ScaleWeight);

        int Inches = (HeightFoot * 12) + HeightInch;
        int MinHeight = (GameSettings.HeightFootRange.x * 12);
        int MaxHeight = (GameSettings.HeightFootRange.y * 12) + (GameSettings.HeightInchRange.y);
        float ScaleHeight = ((float)Inches - (float)MinHeight)/((float)MaxHeight - (float)MinHeight);
        ScaleHeight = Mathf.Lerp(GameSettings.HeightLerp.x, GameSettings.HeightLerp.y, ScaleHeight);
        
        Model.transform.localScale = new Vector3(ScaleWeight, ScaleHeight, ScaleWeight);

        An = Model.GetComponent<Animator>();
        An.SetBool("Walking", false);

        Model.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_BaseMap", Clothes[Random.Range(0, Clothes.Count)]);  //SHIRT
        Model.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().materials[3].SetTexture("_BaseMap", Clothes[Random.Range(0, Clothes.Count)]);  //PANTS

        //Model.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().materials[1].SetColor("_BaseColor", GetSkinColor(Face));  //FACE
        Model.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().materials[2].SetTexture("_BaseMap", Face);  //FACE
        

        InvokeRepeating("MakeNoise", Random.Range(10f, 70f), Random.Range(10f, 70f));
    }    

    void MakeNoise()
    {
        if(ASC && !isSpeaking)
        {
            ASC.Sound = Noises[Random.Range(0, Noises.Count)];
            ASC.SetVolume(0.25f);

            ASC.Play();
        }
    }

    public void Load(BusStop BS, PlayerController Bus, int SeatID)
    {
        Debug.Log("Seat ID :" + SeatID);
        StartCoroutine(GetOnBus(BS, Bus, SeatID));
    }

    public void UnLoad(BusStop BS, PlayerController Bus)
    {
        StartCoroutine(GetOffBus(BS, Bus));
    }

    IEnumerator GetOnBus(BusStop BS, PlayerController Bus, int SeatID)
    {
        TB = GameObject.FindWithTag("TextBox").GetComponent<TextBox>();

        yield return new WaitForSeconds(1f);    //WAIT FOR A SECOND
        
        Busy = true;
        StartCoroutine(GoTo(Bus.Door.position, Bus.Door.rotation)); //GO TO BUS DOOR
        while(Busy)
            yield return null;

        this.transform.SetParent(Bus.transform);

        yield return new WaitForSeconds(0.25f);

        Busy = true;
        StartCoroutine(GoTo(Bus.Stand.position, Bus.Stand.rotation));   //GO UP TO DRIVER
        while(Busy)
            yield return null;

        while(TB.isActive)      //WAIT TILL DRIVER IS FREE TO SPEAK
        {
            yield return null;
        }

        isSpeaking = true;      //SPEAK TO DRIVER
        TB.CurrentVoice = P.V;
        TB.CurrentSource = ASC;
        ASC.SetVolume(1f);

        int Index = 0;
        while(Index < C.IntroLines.Count)
        {
            TB.DisplayText(C.IntroLines[Index]);        
            while(TB.isPrinting)
            {
                yield return null;
            }
            Index++;
        }

        GameObject Documents = Instantiate(DocumentPrefab, DocumentLocation.position, DocumentLocation.rotation);   //SPAWN DOCUMENT
        Documents.transform.SetParent(DocumentLocation);
        Documents.GetComponent<Document>().P = this;
        Bus.TalkingTo = this;

        while(CurrentStatus == Status.Waiting) //WAIT FOR APPROVAL OR DENIAL
        {
            yield return null;
        }

        TB.Toggle();
        isSpeaking = false;
        Documents.GetComponent<DashObject>().Delete();

        if(CurrentStatus == Status.Approved)        //IF APPROVED
        {
            isSpeaking = true;      //SPEAK TO DRIVER AFTER DENIAL
            TB.CurrentVoice = P.V;
            TB.CurrentSource = ASC;
            ASC.SetVolume(1f);
            if(Faked == Document.FakedPart.None)
            {
                GameSettings.SC.SetMoney(GameSettings.SC.GetMoney() + GameSettings.PassengerIncome);
                
            }
            else
            {
                M.MUI.Fines++;
                Bus.B.ShowWarning(true);
            }

            Index = 0;
            while(Index < C.AcceptedLines.Count)
            {
                TB.DisplayText(C.AcceptedLines[Index]);        
                while(TB.isPrinting)
                {
                    yield return null;
                }
                Index++;
            }

            while(TB.isPrinting) //WAIT TO DISABLE TEXT BOX
            {
                yield return null;
            }

            BS.PaidCount++;     //ALERT BUS STOP TO SEND NEXT PERSON
            
            Busy = true;
            StartCoroutine(GoTo(Bus.Seats[SeatID].position, Bus.Seats[SeatID].rotation));   //FIND A SEAT AND GO TO IT
            while(Busy)
                yield return null;

            TB.Toggle();
            isSpeaking = false;

            yield return new WaitForSeconds(0.5f);
            
            Bus.CurrentPassengers.Add(this);
            BS.LoadedAmount++;
        }else
        {
            isSpeaking = true;      //SPEAK TO DRIVER AFTER DENIAL
            TB.CurrentVoice = P.V;
            TB.CurrentSource = ASC;
            ASC.SetVolume(1f);
            if(Faked == Document.FakedPart.None)
            {
                M.MUI.Fines++;
                Bus.B.ShowWarning(false);
            }
            
            Index = 0;
            while(Index < C.DenialLines.Count)
            {
                TB.DisplayText(C.DenialLines[Index]);        
                while(TB.isPrinting)
                {
                    yield return null;
                }
                Index++;
            }

            while(TB.isPrinting) //WAIT TO DISABLE TEXT BOX
            {
                yield return null;
            }

            Busy = true;
            StartCoroutine(GoTo(Bus.Door.position, Bus.Door.rotation));        //GO TO DOOR
            while(Busy)
                yield return null;

            yield return new WaitForSeconds(0.25f);

            TB.Toggle();
            isSpeaking = false;

            Busy = true;
            StartCoroutine(GoTo(BS.GetOff.position, BS.GetOff.rotation));      //GO TO BUS STOP EXIT
            while(Busy)
                yield return null;

            this.transform.SetParent(null);

            BS.PaidCount++;     //ALERT BUS STOP TO SEND NEXT PERSON

            BS.LoadedAmount++;//tell bus person had been dealt with

            Busy = true;
            StartCoroutine(GoTo(BS.DeleteLocation.position, BS.DeleteLocation.rotation));   //GO BEHIND BUS STOP
            while(Busy)
                yield return null;

            yield return new WaitForSeconds(1f);
            
            Destroy(this.gameObject);           //DESPAWN BEHIND BUS STOP
        }
    }

    IEnumerator GetOffBus(BusStop BS, PlayerController Bus)
    {
        yield return new WaitForSeconds(0.25f);
        
        Busy = true;
        StartCoroutine(GoTo(Bus.Stand.position, Bus.Stand.rotation));
        while(Busy)
            yield return null;

        yield return new WaitForSeconds(0.25f);

        Busy = true;
        StartCoroutine(GoTo(Bus.Door.position, Bus.Door.rotation));
        while(Busy)
            yield return null;

        yield return new WaitForSeconds(0.25f);

        Busy = true;
        StartCoroutine(GoTo(BS.GetOff.position, BS.GetOff.rotation));
        while(Busy)
            yield return null;

        yield return new WaitForSeconds(0.25f);
        
        Bus.CurrentPassengers.Remove(this);
        Bus.UnLoadedAmount++;
        this.transform.SetParent(null);

        Busy = true;
        StartCoroutine(GoTo(BS.DeleteLocation.position, BS.DeleteLocation.rotation));
        while(Busy)
            yield return null;

        yield return new WaitForSeconds(1f);

        Destroy(this.gameObject);
    }

    IEnumerator GoTo(Vector3 Location, Quaternion Angles)
    {
        Busy = true;
        An.SetBool("Walking", true);
        Vector3 StartPos = this.transform.position;
        Quaternion StartRot = this.transform.rotation;
        float Distance = Vector3.Distance(StartPos, Location);

        float Speed = 0.025f;

        float Traveled = 0f;
        while (Traveled < Distance)
        {
            this.transform.position = Vector3.Lerp(StartPos, Location, Traveled/Distance);
            this.transform.rotation = Quaternion.Lerp(StartRot, Angles, Traveled/Distance);

            Traveled += Speed;
            yield return new WaitForSeconds(0.01f);
        }

        An.SetBool("Walking", false);
        Busy = false;
    }

    public void GetResponse(Status Response)
    {
        if(CurrentStatus == Status.Waiting)
        {
            CurrentStatus = Response;
        }
    }

    public Color GetSkinColor(Texture2D tex)
    {
        Color[] colors = tex.GetPixels();
		int pixelCount = colors.Length;
		float r = 0f;
        float g = 0f;
        float b = 0f;
        float Lightness = 0f;

		foreach (Color color in colors) 
        {
			r += color.r;
			g += color.g;
			b += color.b;
            Lightness += color.r + color.g + color.b;
		}

		r /= pixelCount;
	    g /= pixelCount;
		b /= pixelCount;
        Lightness /= pixelCount * 3f;

		Color averageColor = new Color (r, g, b, 1f);
		//return Color.Lerp(averageColor, new Color(255f/255f, 225f/255f, 189f/255f, 1f), 0.75f);

        return Color.Lerp(averageColor, new Color(255f/255f, 235f/255f, 224f/255f, 1f), Mathf.Lerp(0.75f, 1f, Lightness));
	}
}
