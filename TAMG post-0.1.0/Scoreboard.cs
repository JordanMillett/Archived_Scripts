using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public GameObject UserScoreParent;
    public GameObject UserScorePrefab;

    List<GameObject> Scores = new List<GameObject>();

    int StartingY = 210;
    int YOffset = 40;

    void OnEnable()
    {
        if (Scores.Count > 0)
            for (int i = 0; i < Scores.Count; i++)
                Destroy(Scores[i].gameObject);

        float yLoc = StartingY;

        for (int i = 0; i < Client.Instance.ServerInstance.PlayerStats.Count; i++)
        {
            GameObject NewScore = Instantiate(UserScorePrefab, Vector3.zero, Quaternion.identity);
            NewScore.transform.SetParent(UserScoreParent.transform);
            NewScore.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, yLoc, 0f);
            NewScore.GetComponent<RectTransform>().transform.localScale = Vector3.one;

            yLoc -= YOffset;

            Scores.Add(NewScore);

            NewScore.GetComponent<UserScore>().Set(Client.Instance.ServerInstance.PlayerStats[i]);
        }


    }
}
