using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : DontDestroySingleton<SoundManager>
{
    public AudioSource audioSourceBGM; // BGMのスピーカー
    public List<AudioClip> audioClipsBGM;    // BGMの音源
    private Dictionary<string, int> audioClipsBGMDict = new Dictionary<string, int>();

    public AudioSource audioSourceSE; // SEのスピーカー
    public List<AudioClip> audioClipsSE; // SEの音源
    private Dictionary<string, int> audioClipsSEDict = new Dictionary<string, int>();

    public override void Awake()
    {
        base.Awake();
        for (int i = 0; i < audioClipsBGM.Count; i++)
        {
            audioClipsBGMDict.Add(audioClipsBGM[i].name, i);
        }

        for (int i = 0; i < audioClipsSE.Count; i++)
        {
            audioClipsSEDict.Add(audioClipsSE[i].name, i);
        }
    }

    /// <summary>
    /// BGMを流す：引数（BGMのID、ボリューム）
    /// </summary>
    /// <param name="bgmIndex"></param>
    /// <param name="volume"></param> 
    public void PlayBGM(int bgmIndex, float volume = 1f)
    {
        AudioClip bgmClip = audioClipsBGM[bgmIndex];

        // 同じBGMを再生している場合は何もしない
        if (audioSourceBGM.clip == bgmClip)
        {
            return;
        }

        if (audioSourceBGM.isPlaying)
        {
            StopBGM();
        }

        // BGMを設定して再生
        audioSourceBGM.clip = bgmClip;
        audioSourceBGM.volume = volume;
        audioSourceBGM.Play();

    }

    /// <summary>
    /// BGMを止める
    /// </summary>
    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    /// <summary>
    /// 効果音を止める
    /// </summary>
    public void StopSE()
    {
        audioSourceSE.Stop();
    }

    /// <summary>
    /// 効果音を流す：引数（効果音のID、ボリューム）
    /// </summary>
    /// <param name="seIndex"></param>
    /// <param name="volume"></param>
    public void PlaySE(int seIndex, float volume = 1f)
    {
        AudioClip seClip = audioClipsSE[seIndex];
        audioSourceSE.PlayOneShot(seClip, volume);
    }

    /// <summary>
    /// 今流しているBGMを引数（BGMのファイル名）の再生に変える
    /// </summary>
    /// <param name="fileName"></param>
    public void ChangeBGM(string fileName)
    {
        if (fileName.Equals("void"))
        {
            StopBGM();
        }
        else if (audioClipsBGMDict.ContainsKey(fileName))
        {
            PlayBGM(audioClipsBGMDict[fileName]);
        }
    }

    /// <summary>
    /// 今流している効果音を引数（効果音のファイル名）の再生に変える
    /// </summary>
    /// <param name="fileName"></param>
    public void ChangeSE(string fileName)
    {
        if (fileName.Equals("void"))
        {
            StopSE();
        }
        else if (audioClipsSEDict.ContainsKey(fileName))
        {
            PlaySE(audioClipsSEDict[fileName]);
        }
    }
}