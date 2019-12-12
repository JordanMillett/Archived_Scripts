using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{
    public GameObject Note;

    public void SpawnNotification(string Text)
    {

        GameObject N = Instantiate(Note, this.transform.position, Quaternion.identity);
        N.GetComponent<TextMeshProUGUI>().text = Text;
        N.transform.SetParent(this.transform);

    }
}
