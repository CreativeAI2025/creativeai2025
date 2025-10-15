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
    [SerializeField] private int _enemyId;
    private bool _battleFlag = false;
    private InputSetting _inputSetting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    async Task Update()
    {
        if (_battleFlag)
        {
            return;
        }
        if (_inputSetting.GetDecideInputDown())
        {
            await CallEvent();
        }
    }

    private async UniTask CallEvent()
    {
        Debug.Log("バトル開始");
        _battleFlag = true;
        Battle(_enemyId);
        await UniTask.WaitUntil(() => !_battleFlag);
    }

    private void Battle(int enemyId)
    {
        //ここにBattleManagerをIDで開始
    }

    private void Battle()
    {
        //ここにBattleManagerをマップの場所で開始（エンカウント）
    }
}
