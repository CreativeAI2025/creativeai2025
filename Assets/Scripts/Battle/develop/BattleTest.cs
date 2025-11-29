using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;

public class BattleTest : MonoBehaviour
{
    [SerializeField] string filename;
    [SerializeField] GameInitializer _initializer;
    private bool _battleFlag = false;
    private InputSetting _inputSetting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
        BattleManager.Instance.OnBattleEnd += () => _battleFlag = false;
        _initializer.InitializeGame();
    }

    // Update is called once per frame
    async Task Update()
    {
        if (_battleFlag)
        {
            return;
        }
        if (Input.GetKey(KeyCode.B))
        {
            await CallEvent();    // 戦闘が開始したら、戦闘が終了するまで待つ
        }
    }

    private async UniTask CallEvent()
    {
        Debug.Log("バトル開始");
        _battleFlag = true;
        Battle(filename);
        await UniTask.WaitUntil(() => !_battleFlag);
        Debug.Log("戦闘イベント終了");
    }

    private void Battle(string filename)
    {
        BattleManager.Instance.InitializeFromJson(filename);
    }
}
