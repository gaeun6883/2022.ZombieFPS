using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour
{
    public static SceneInfo info;

    public int bestScore;
    public float bestTime;

    public int score;
    public float spendTime;

    private void Start() {
        info = this;
    }
}
