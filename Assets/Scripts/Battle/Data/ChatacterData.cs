using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/SimpleRpg/CharacterData")]
public class CharacterData : ScriptableObject
{
  /// <summary>
  /// キャラクターのIDです。
  /// </summary>
  public int characterId;

  /// <summary>
  /// キャラクターの名前です。
  /// </summary>
  public string characterName;

  public string characterNameEn;

  /// <summary>
  /// キャラクターの見た目（等身大）
  /// </summary>
  public Sprite sprite;
}