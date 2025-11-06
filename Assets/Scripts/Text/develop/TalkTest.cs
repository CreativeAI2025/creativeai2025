using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;

public class TalkTest : MonoBehaviour
{
    private string fileName = "TextDataSample";
    private bool _conversationFlag = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ConversationTextManager.Instance.OnConversationEnd += () => _conversationFlag = false;
    }

    // Update is called once per frame
    async Task Update()
    {
        if (_conversationFlag)
        {
            return;
        }
        await CallEvent();
    }

    private async UniTask CallEvent()
    {
        Debug.Log("会話開始");
        _conversationFlag = true;
        Conversation(fileName);
        await UniTask.WaitUntil(() => !_conversationFlag);
    }

    private void Conversation(string fileName)
    {
        ConversationTextManager.Instance.InitializeFromJson(fileName);
    }
    
}
