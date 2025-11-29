using UnityEngine;
using System;

public class AnimationWindowController : MonoBehaviour
{

    [SerializeField] private GameObject[] _cursorObjects;
    private int _cursor;
    private InputSetting _inputSetting;
    public event Action OnTimelineEnd;
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    void Update()
    {
        if (_inputSetting.GetForwardKeyDown())
        {
            MoveUpCursor();
        }
        else if (_inputSetting.GetBackKeyDown())
        {
            MoveDownCursor();
        }
        else if (_inputSetting.GetDecideInputDown())
        {
            if (_cursor == 0)
            {
                StopTimeLine();
            }
            HideWindow();
        }
    }

    private void MoveUpCursor()
    {
        _cursor++;
        if (_cursor >= _cursorObjects.Length)
        {
            _cursor = 0;
        }
        SetCursor(_cursor);
    }

    private void MoveDownCursor()
    {
        _cursor--;
        if (_cursor < 0)
        {
            _cursor = _cursorObjects.Length - 1;
        }
        SetCursor(_cursor);
    }

    private void StopTimeLine()
    {
        OnTimelineEnd?.Invoke();
    }

    private void SetCursor(int index)
    {
        HideAllCursor();
        _cursorObjects[index].SetActive(true);
    }

    private void HideAllCursor()
    {
        foreach (var cursor in _cursorObjects)
        {
            cursor.SetActive(false);
        }
    }

    public void ShowWindow()
    {
        gameObject.SetActive(true);
        _cursor = 1;
        SetCursor(_cursor);
    }

    public void HideWindow()
    {
        gameObject.SetActive(false);
    }
}
