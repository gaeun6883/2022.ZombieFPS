using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    static EndingManager e;
    int resultScore;
    int goal;
    float Record;

    int bestScore;
    float bestRecord;

    public Text endingT;
    public Text recordT;
    public Text BestRecord;

    public void Awake()
    {
        bestScore = 0;
        bestRecord = 987654321f;
        if (e == null) e = this;//教臂沛 按眉 积己
    }

    void Start()
    {
        resultScore = SceneInfo.info.score;
        Record = SceneInfo.info.spendTime;
        bestScore = SceneInfo.info.bestScore;
        bestRecord = SceneInfo.info.bestTime;

        if (resultScore >= 5000)
        {
            endingT.text = "Success Mission!";
            endingT.color = new Color32(255, 185, 0, 255);
        }
        else
        {
            endingT.text = "Game Over!";
        }
        recordT.text = "score : " + resultScore.ToString("000000") + " time : " + (int)Record;

        if (resultScore > bestScore || (resultScore == bestScore && Record < bestRecord))
        {
            SceneInfo.info.bestScore = resultScore;
            SceneInfo.info.bestTime = Record;
        }
        BestRecord.text = "score : " + SceneInfo.info.bestScore.ToString("000000") + " time : " + (int)SceneInfo.info.bestTime;
        EnemyFSM.zombieCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(1);
        } 
    }
}