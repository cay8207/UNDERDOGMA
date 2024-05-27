using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnButton : MonoBehaviour
{
    void Start()
    {
        // 버튼 클릭 이벤트에 메서드 연결
        gameObject.GetComponent<Button>().onClick.AddListener(LoadMainMenu);
    }

    // 메인 화면으로 돌아가는 메서드
    void LoadMainMenu()
    {
        // 원하는 메인 화면 씬의 이름으로 변경
        SceneManager.LoadScene("GameStart");
    }
}
