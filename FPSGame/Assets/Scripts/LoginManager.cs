using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{

    public InputField id;
    public InputField password;
    public Text notify;         // �˻� �ؽ�Ʈ����


    // Start is called before the first frame update
    void Start()
    {
        notify.text = "";
    }

    public void SaveUserData() {
        if (!CheckInput(id.text, password.text)) {
            return;
        }
        // ���̵�� �н����带 ����
        if (!PlayerPrefs.HasKey(id.text)) {
            PlayerPrefs.SetString(id.text, password.text);
            notify.text = "���̵� ������ �Ϸ�Ǿ����ϴ�.";
        }
        else {
            notify.text = "�̹� �����ϴ� ���̵��Դϴ�.";
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
            notify.text = "���̵�� �н����带 �ٽ� �Է��ϼ���.";
        }
    }

    // ���̵�� �н����带 �������� �ߴ��� �˻�.
    bool CheckInput(string id, string pwd) {
        if (id == "" || pwd == "") {
            notify.text = "���̵� ���� �н����带 �Է��� �ּ���.";
            return false;
        }
        
        return true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
