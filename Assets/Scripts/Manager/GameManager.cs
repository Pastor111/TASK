using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class HighScore
{
    public int Score;
}

public class GameManager : MonoBehaviour
{
    [Header("===Enemy Manager===")]
    public Vector2 MinMaxSpawnTime;
    public float SpawnRadius;
    public GameObject[] Enemies;
    [Space]
    [Space]
    [Header("===UI===")]
    public GameObject NotificationUI;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;
    public GameObject GameOverScreen;
    public TextMeshProUGUI GameOverScoreText;
    public TextMeshProUGUI GameOverHighScoreText;

    private int Score;
    private HighScore Record;

    public static GameManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        StartCoroutine(GetNewEnemyPos());
        Time.timeScale = 1.0f;
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        if(Score >= Record.Score)
        {
            Record.Score = Score;
        }

        ScoreText.text = $"Score : {Score}";
        HighScoreText.text = $"HighScore : {Record.Score}";
        
    }

    void Save()
    {
        string txt = JsonUtility.ToJson(Record, true);
        File.WriteAllText(Application.dataPath + "/Save.data", txt);
    }
    
    void Load()
    {
        if(File.Exists(Application.dataPath + "/Save.data"))
        {
            string txt = File.ReadAllText(Application.dataPath + "/Save.data");
            Record = JsonUtility.FromJson<HighScore>(txt);
        }
        else
        {
            Record = new HighScore(){Score = 0};
        }
    }

    public void DoGameOver()
    {
        Save();
        GameOverScreen.SetActive(true);
        GameOverScoreText.text = $"Score : {Score}";
        GameOverHighScoreText.text = $"HighScore : {Score}";
        Time.timeScale = 0.0f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowNotification(float time, string message, Color color)
    {
        StartCoroutine(Notif(time, message, color)); 
    }

    public void AddScore(int addScore = 10)
    {
        Score += addScore;
    }

    IEnumerator Notif(float t, string msg, Color color)
    {
        NotificationUI.SetActive(true);
        NotificationUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = msg;
        NotificationUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color;
        yield return new WaitForSeconds(t);
        NotificationUI.SetActive(false);

    }

    #region  Enemy Manager Stuff

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(Random.Range(MinMaxSpawnTime.x, MinMaxSpawnTime.y));
        StartCoroutine(GetNewEnemyPos());
    }

    IEnumerator GetNewEnemyPos()
    {
        bool foundPos = false;
        while(foundPos == false)
        {
            var pos = GetPointInRadius(Vector3.zero, SpawnRadius);

            if(Physics2D.OverlapCircleAll(pos, .1f).Length > 0)
            {
                //cant spawn here
                yield return null;
            }
            else
            {
                Instantiate(Enemies[0], pos, Quaternion.identity);
                foundPos = true;
                yield return null;
            }
        }

        StartCoroutine(SpawnTimer());
    }
    #endregion

    Vector3 GetPointInRadius(Vector3 centerOfRadius, float radius)
    {
        //Vector3 centerOfRadius = target.position;
        //float radius = Innacuracy;
        Vector3 t = centerOfRadius + (Vector3)(radius * UnityEngine.Random.insideUnitCircle);

        t.z = 0.0f;

        return t;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.DrawWireSphere(Vector3.zero, SpawnRadius);    
    }

}
