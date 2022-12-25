using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameLabel;    // ���� ���� UI ������Ʈ ����
    Text gameText;  //���� ���� UI ������Ʈ ������Ʈ ����
    public Text scoreText;

    PlayerMove player;

    public int enemyLimit = 10;

    public static GameManager gm;  // �̱��� ��ü ����
    public int goal;

    public int score;

    bool Success;
    public float surviveTime;

    public void Awake() {
        if (gm == null) {
            gm = this;  // �̱��� ��ü ����
        }
    }

    // ���� ���� ������ ����
    public enum GameState { 
        Ready, 
        Run, 
        GameOver
    }

    public GameState gState;

    private void Start() {
        gState = GameState.Ready;
        gameText = gameLabel.GetComponent<Text>();
        gameText.text = "Ready...";
        gameText.color = new Color32(255, 185, 0, 255);
        StartCoroutine(ReadyToStart());
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
    }

    IEnumerator ReadyToStart() {
        yield return new WaitForSeconds(2f);
        gameText.text = "Go!";
        yield return new WaitForSeconds(0.5f);
        gameLabel.SetActive(false);
        gState = GameState.Run;
    }


    // Update is called once per frame
    void Update()
    {
        if (!player)
            return;
        if (player.hp <= 0) {
            //�÷��̾��� �ִϸ��̼��� �����.
            player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0f);
            gState = GameState.GameOver;    // ���¸� ���ӿ��� ���·� ����
            SceneInfo.info.spendTime = surviveTime;
            SceneInfo.info.score = score;
            DontDestroyOnLoad(SceneInfo.info);
            SceneManager.LoadScene("Ending");
            return;
        }

        surviveTime += Time.deltaTime;
    }

    public void AddScore(int added) {
        score += added;
        scoreText.text = "SCORE " + score.ToString("000000");
    }
}
