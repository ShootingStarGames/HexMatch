
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourSingleton<UIManager> {

    [SerializeField]
    GameObject panel;
    [SerializeField]
    Text topText;
    [SerializeField]
    Text swapText;
    [SerializeField]
    Text scoreText;

    public void Back()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void ReStart()
    {
        Toggle();
        HexagonManager.Instance.GameStart();
    }

    public void UpdateTop(int i, int j, int k)
    {
        topText.text = "남은 양 : " + i;
        swapText.text = j.ToString();
        scoreText.text = "점수 : " + k;
    }
}
