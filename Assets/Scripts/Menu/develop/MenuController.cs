using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuRoot;
    public GameObject[] detailPanels;   // メニュートップから遷移できるパネル（キャラ詳細、スキル、アイテム、スキルボード）を登録して
    private bool isMenuOpen = false;
    private InputSetting _inputSetting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    void Update()
    {
        // メニューが開いていないとき、表示する。
        // メニューが開いているとき、非表示にする。
        if (!isMenuOpen)
        {
            if (_inputSetting.GetMenuKeyDown())
            {
                isMenuOpen = !isMenuOpen;
                menuRoot.SetActive(isMenuOpen);
                Debug.Log("Menu Open");
            }
        }
        else
        {
            // メニューを閉じるとき、１つでも開いているパネルがあれば（＝別の画面を表示中）、拒否する
            if (_inputSetting.GetMenuKeyDown() || _inputSetting.GetCancelKeyDown())
            {
                if (IsAnyDetailPanelOpen())
                {
                    Debug.Log("Can't Close Menu because of Detail Panels' Active");
                    return;
                }
                isMenuOpen = !isMenuOpen;
                menuRoot.SetActive(isMenuOpen);
                Debug.Log("Menu Close");
            }
        }
    }

    // 全てのパネルを表示/非表示にする
    void SetAllDetailPanels(bool boolian)
    {
        foreach (var panel in detailPanels)
        {
            panel.SetActive(boolian);
        }
    }

    // 起動しているパネルが存在しているか
    bool IsAnyDetailPanelOpen()
    {
        foreach (var panel in detailPanels)
        {
            if (panel.activeSelf)
                return true;
        }
        return false;
    }
}
