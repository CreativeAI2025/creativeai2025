using System.Collections;
using UnityEngine;

public class MenuSkillTreeWindowController : MonoBehaviour, IMenuWindowController
{
    MenuManager _menuManager;
    [SerializeField] private GameObject _uiController;
    private InputSetting _inputSetting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    void Update()
    {
        CheckKeyInput();
    }

    private void CheckKeyInput()
    {
        if (MenuManager.Instance == null)
        {
            return;
        }
        if (MenuManager.Instance.MenuPhase != MenuPhase.SkillTree)
        {
            return;
        }
        if (_inputSetting.GetCancelKeyDown())
        {
            StartCoroutine(HideProcess());
        }
    }

    private IEnumerator HideProcess()
    {
        yield return null;
        MenuManager.Instance.OnSkillTreeCanceled();
        HideWindow();
    }
    public void ShowWindow()
    {
        _uiController.SetActive(true);

        StartCoroutine(SetCloseStateDelay());
    }

    private IEnumerator SetCloseStateDelay()
    {
        yield return null;
    }

    public void HideWindow()
    {
        _uiController.SetActive(false);
    }
}
