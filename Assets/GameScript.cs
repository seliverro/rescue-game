using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameScript : MonoBehaviour
{
    public GameSettings GameSettings;
    public GameObject Wolf;
    public GameObject Diamond;
    public GameObject Cow;

    private CowScript CowScript;

    private float LeftConstraint;
    private float RightConstraint;
    private float BottomConstraint;
    private float TopConstraint;

    private int DiamondsGathered;

    private Text TextDiamonds;
    private Text TextSeconds;
    private Text TextFinishStatus;
    private Text Text1;
    private Text Text2;
    private Text Text3;

    public GameObject TextFinishStatusObject;
    public GameObject FinishPanel;
    public GameObject Text1Object;
    public GameObject Text2Object;
    public GameObject Text3Object;
    
    private float TotalSeconds;
    private bool Paused;
    private List<WolfScript> WolfScripts = new List<WolfScript>();

    private bool Inited;

    private SortedList<float, DateTime> HiScoresList;

    // Start is called before the first frame update
    void Start()
    {
        TotalSeconds = -GameSettings.StartDelaySeconds;
            
        TextFinishStatus = TextFinishStatusObject.GetComponent<Text>();
        CowScript = Cow.GetComponent<CowScript>();

        LeftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f)).x;
        RightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f)).x;

        BottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f)).y;
        TopConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height)).y;

        InitWolfs();

        TextDiamonds = GameObject.Find("TextDiamonds").GetComponent<Text>();
        TextDiamonds.text = 0 + " diamonds";

        TextSeconds = GameObject.Find("TextSeconds").GetComponent<Text>();
        TextSeconds.text = 0 + " seconds";

        Text1 = Text1Object.GetComponent<Text>();
        Text2 = Text2Object.GetComponent<Text>();
        Text3 = Text3Object.GetComponent<Text>();
        
        LoadHiScores();
    }

    private void InitWolfs()
    {
        var offsetHorz = (RightConstraint - LeftConstraint) * 0.05f;
        var offsetVert = (TopConstraint - BottomConstraint) * 0.05f;

        for (var i = 0; i < GameSettings.WolfCount; i++)
        {
            var clone = Instantiate(
                Wolf,
                new Vector2(
                    Random.Range(LeftConstraint + offsetHorz, RightConstraint - offsetHorz),
                    Random.Range(BottomConstraint + offsetVert, TopConstraint - offsetVert)),
                Quaternion.identity);

            var wolfScript = clone.GetComponent<WolfScript>();
            wolfScript.CowTouched += WolfScriptOnCowTouched;
            wolfScript.GetPausedStatus = () => Paused;

            WolfScripts.Add(wolfScript);

            clone.SetActive(true);
        }
    }

    void RunWolfs()
    {
        foreach (var wolfScript in WolfScripts)
        {
            wolfScript.Direction = new Vector2(Random.Range(-2f, +2f), Random.Range(-2f, +2f));
        }
    }

    private void DiamondScriptOnCowTouched(object sender, EventArgs e)
    {
        DiamondsGathered++;

        TextDiamonds.text = DiamondsGathered + " diamonds";

        if (DiamondsGathered == GameSettings.DiamondCount)
        {
            WinGame();
        }
    }

    private void WolfScriptOnCowTouched(object sender, EventArgs e)
    {
        if (Inited)
            FailGame();
    }

    void InitDiamonds()
    {
        var offsetHorz = (RightConstraint - LeftConstraint) * 0.025f;
        var offsetVert = (TopConstraint - BottomConstraint) * 0.025f;

        for (var i = 0; i < GameSettings.DiamondCount; i++)
        {
            var clone = Instantiate(
                Diamond,
                new Vector2(
                    Random.Range(LeftConstraint + offsetHorz, RightConstraint - offsetHorz),
                    Random.Range(BottomConstraint + offsetVert, TopConstraint - offsetVert)),
                Quaternion.identity);

            var diamondScript = clone.GetComponent<DiamondScript>();
            diamondScript.CowTouched += DiamondScriptOnCowTouched;
            
            clone.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            PlayerPrefs.DeleteKey("hiscores");
        }
        
        if (!Paused)
        {
            TotalSeconds += Time.deltaTime;
            TextSeconds.text = TotalSeconds.ToString("F") + " seconds";

            if (TotalSeconds >= 0 && !Inited)
            {
                InitDiamonds();
                RunWolfs();
                Inited = true;
            }
        }
    }
    
    private void LoadHiScores()
    {
        var hiscoresStr = PlayerPrefs.GetString("hiscores", null);
        HiScoresList = JsonConvert.DeserializeObject<SortedList<float, DateTime>>(hiscoresStr) ?? new SortedList<float, DateTime>();
    }

    private void SaveHiScores()
    {
        var hiScoresStr = JsonConvert.SerializeObject(HiScoresList);
        PlayerPrefs.SetString("hiscores", hiScoresStr);
    }
    
    private void FailGame()
    {
        SetPaused(true);
        FinishPanel.SetActive(true);
        TextFinishStatus.text = "You failed...";
        SetHiScoresUI(new DateTime());
    }

    private void WinGame()
    {
        SetPaused(true);
        FinishPanel.SetActive(true);
        TextFinishStatus.text = "You win!";

        var now = DateTime.Now;
        HiScoresList.Add(TotalSeconds, now);
        SaveHiScores();
        SetHiScoresUI(now);
    }

    private void SetHiScoresUI(DateTime myDt)
    {
        if (HiScoresList.Count > 0) SetText(Text1, 0, myDt); else Text1.text = "1. --------------------";
        if (HiScoresList.Count > 1) SetText(Text2, 1, myDt); else Text2.text = "2. --------------------";
        if (HiScoresList.Count > 2) SetText(Text3, 2, myDt); else Text3.text = "3. --------------------";
    }

    private void SetText(Text obj, int i, DateTime myDt)
    {
        obj.text = (i+1).ToString() + ". " + HiScoresList.Keys[i].ToString("F") + " seconds at " + HiScoresList[HiScoresList.Keys[i]];
        obj.color = HiScoresList[HiScoresList.Keys[i]] == myDt ? new Color32( 50, 200, 50, 255) : new Color32( 50, 50, 50, 255);
    }

    private void SetPaused(bool paused)
    {
        Paused = paused;
        CowScript.Paused = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}