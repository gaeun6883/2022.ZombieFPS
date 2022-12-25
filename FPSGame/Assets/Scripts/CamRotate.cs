using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float rotSpeed = 200f;
    public float zoomRotSpeed = 50f;

    float mx = 0;   // 회전값을 누적시킬 변수들
    float my = 0;

    bool zooming;

    public void Zoom() {
        Camera.main.fieldOfView = 15f;
        zooming = true;
    }

    public void ZoomOut() {
        if (zooming) {
            Camera.main.fieldOfView = 60f;
            zooming = false;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float angleSpeed = 0;

        if (zooming)
            angleSpeed = zoomRotSpeed;
        else
            angleSpeed = rotSpeed;

        if (GameManager.gm.gState == GameManager.GameState.GameOver)
            return;

        // 마우스 입력을 받는다.
        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");

        // 회전 값 변수에 마우스 입력 값만큼 미리 누적시킴
        mx += mouse_X * angleSpeed * Time.deltaTime;
        my += mouse_Y * angleSpeed * Time.deltaTime;

        my = Mathf.Clamp(my, -90f, 90f);
        transform.eulerAngles = new Vector3(-my, mx, 0);

        /*
        // 마우스 입력값으로 회전 방향을 결정함
        Vector3 dir = new Vector3(-mouse_Y, mouse_X, 0);

        // 회전 방향으로 물체를 회전시킴
        Vector3 rot = transform.eulerAngles + dir * rotSpeed * Time.deltaTime;

        // x축 회전(상하)값을 -90 ~ 90도 사이로 제한
        rot.x = Mathf.Clamp(rot.x, -90f, 90f);

        transform.eulerAngles = rot;
        */

    }
    public void Rebond(float power) {
        StopAllCoroutines();
        StartCoroutine(RebondAction(power));
        mx += Random.Range(-.2f, .2f); ;
    }

    IEnumerator RebondAction(float power) {
        float spendTime = 0;
        while (spendTime <= .3f) {
            //my += (-10 * spendTime + 4) * Time.deltaTime * 4.0f;
            my += (-30 * spendTime + 4) * Time.deltaTime * 4.0f;
            transform.eulerAngles = new Vector3(-my, mx, 0);
            yield return null;
            spendTime += Time.deltaTime;
        }
    }
}
