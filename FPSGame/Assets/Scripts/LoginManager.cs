using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{

    public InputField id;
    public InputField password;
    public Text notify;         // 검사 텍스트변수


    // Start is called before the first frame update
    void Start()
    {
        notify.text = "";
    }

    public void SaveUserData() {
        if (!CheckInput(id.text, password.text)) {
            return;
        }
        // 아이디와 패스워드를 저장
        if (!PlayerPrefs.HasKey(id.text)) {
            PlayerPrefs.SetString(id.text, password.text);
            notify.text = "아이디 생성이 완료되었습니다.";
        }
        else {
            notify.text = "이미 존재하는 아이디입니다.";
        }
    }

    public void CheckUserData() {
        if (!CheckInput(id.text, password.text)) {
            return;
        }

        string pass = PlayerPrefs.GetString(id.text);
        if (password.text == pass) {
            SceneManager.LoadScene(1);
        }
        else {
            notify.text = "아이디와 패스워드를 다시 입력하세요.";
        }
    }

    // 아이디와 패스워드를 공란으로 했는지 검사.
    bool CheckInput(string id, string pwd) {
        if (id == "" || pwd == "") {
            notify.text = "아이디 도는 패스워드를 입력해 주세요.";
            return false;
        }
        
        return true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
