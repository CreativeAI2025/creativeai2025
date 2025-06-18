using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    public RectTransform cursor;    // カーソル画像
    public Button[] buttons;    // キャラ・スキル・アイテム・スキルボードの順にセット
    private int currentIndex = 0;
    public GameObject[] panels;
    private InputSetting _inputSetting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
        cursor.position = buttons[0].transform.position;
        cursor.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf) return;

        // 左右移動と選択
        if (_inputSetting.GetLeftKeyDown())
        {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            UpdateCursorPosition();
        }
        else if (_inputSetting.GetRightKeyDown())
        {
            currentIndex = (currentIndex + 1) % buttons.Length;
            UpdateCursorPosition();
        }
        if (_inputSetting.GetDecideInputDown())
        {
            ActivatePanel(currentIndex);
        }
    }

    void UpdateCursorPosition()
    {
        cursor.position = buttons[currentIndex].transform.position;
    }

    void ActivatePanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
    }
}
