// TargetSelectionWindowController.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 💡 ターゲット選択ウィンドウを制御するクラスです。
/// </summary>
public class TargetSelectionWindowController : MonoBehaviour, IBattleWindowController
{
    [SerializeField] private TargetSelectionUIController _uiController;
    BattleManager _battleManager;
    private InputSetting _inputSetting;

    private BattleCommand _actionCommand;
    private int _selectedItemId;
    private int _actorCharacterCursor;

    private int _targetCursorIndex;

    // --- 💡 ターゲット分離のための新しい内部フィールド ---
    private List<int> _activeTargetIds;      // 現在カーソルが対象とする有効なIDリスト（敵のみ or 味方のみ）
    private EffectTarget _actionEffectTarget; // 決定されたアクションのEffectTarget
    private int _uiMarkerOffset;             // UIマーカーの開始位置オフセット (味方=0, 敵=3 など)
    private const int FRIEND_SLOT_COUNT = 3; // UIマーカーにおける味方スロットの数（マーカーリストの境界）
                                             // ターゲット名リストを保持するフィールド
    private List<string> _activeTargetNames;
    // -----------------------------------------------------

    public void SetUpController(BattleManager battleManager)
    {
        _battleManager = battleManager;
        // ...
    }
    void Start()
    {
        _inputSetting = InputSetting.Load();
        HideWindow();
    }

    /// <summary>
    /// ターゲット選択に必要な情報をセットアップします。
    /// </summary>
    public void SetUpTargets(BattleCommand command, int itemId, int actorCursor)
    {
        _actionCommand = command;
        _selectedItemId = itemId;
        _actorCharacterCursor = actorCursor;

        // 2. ターゲットリストとUIオフセットを決定
        _actionEffectTarget = GetActionEffectTarget(command, itemId);
        _activeTargetIds = GeneratePossibleTargetsList(_actionEffectTarget);

        // 3. ターゲット名リストとオフセットを設定
        if (_actionEffectTarget == EffectTarget.EnemySolo || _actionEffectTarget == EffectTarget.EnemyAll)
        {
            _uiMarkerOffset = FRIEND_SLOT_COUNT;
            _activeTargetNames = GetEnemyNamesByIds(_activeTargetIds); // 💡 敵の名前を取得
        }
        else
        {
            _uiMarkerOffset = 0;
            _activeTargetNames = GetCharacterNamesByIds(_activeTargetIds); // 💡 味方の名前を取得
        }

        // UIを初期化し、ターゲットを表示
        // 名前リストもUIコントローラーに渡す
        _uiController.Initialize(targetCount: _activeTargetIds.Count, targetNames: _activeTargetNames);
        _targetCursorIndex = 0;

        // 💡 修正 3: EffectTargetに応じてカーソル表示を分岐
        if (_actionEffectTarget == EffectTarget.EnemyAll || _actionEffectTarget == EffectTarget.FriendAll)
        {
            // 全体対象の場合、全てのターゲットのカーソルを表示
            _uiController.ShowAllActiveCursors(_activeTargetIds.Count);
        }
        else
        {
            // 単体対象の場合、最初のターゲットのみ表示
            _uiController.ShowSelectedCursor(_targetCursorIndex);
        }
    }

    // Update()内でキー入力を処理し、ターゲット選択を行います
    void Update()
    {
        if (BattleManager.Instance.BattlePhase != BattlePhase.SelectTarget) return;

        if (_inputSetting.GetDecideInputDown()) // 決定キー
        {
            OnPressedConfirmButton();
        }
        else if (_inputSetting.GetCancelKeyDown()) // キャンセルキー
        {
            // コマンド選択フェーズに戻る
            BattleManager.Instance.SetBattlePhase(BattlePhase.InputCommand);
            HideWindow();
        }
        else if (_inputSetting.GetBackKeyDown())
        {
            MoveNextTarget();
        }
        else if (_inputSetting.GetForwardKeyDown())
        {
            MovePreviousTarget();
        }
    }

    /// <summary>
    /// ターゲット決定時の処理。アクションの範囲に応じて、単体または全体を決定します。
    /// </summary>
    void OnPressedConfirmButton()
    {
        if (_activeTargetIds == null || _activeTargetIds.Count == 0) return;

        List<int> finalTargetIds;
        bool isTargetFriend;

        // EffectTargetに基づいてターゲットリストと属性を決定
        if (_actionEffectTarget == EffectTarget.EnemyAll || _actionEffectTarget == EffectTarget.FriendAll)
        {
            finalTargetIds = _activeTargetIds;
            isTargetFriend = (_actionEffectTarget == EffectTarget.FriendAll);
        }
        else // 単体選択 (Solo, Own)
        {
            int finalTargetId = _activeTargetIds[_targetCursorIndex];
            finalTargetIds = new List<int> { finalTargetId };
            isTargetFriend = (_actionEffectTarget == EffectTarget.FriendSolo || _actionEffectTarget == EffectTarget.Own);
        }

        // BattleManagerに決定を通知し、アクションを登録させる
        BattleManager.Instance.OnTargetSelected(finalTargetIds, isTargetFriend, _selectedItemId);
        HideWindow();
    }


    // 右キーが押されたら次のターゲットにカーソルを移動する
    private void MoveNextTarget()
    {
        if (_activeTargetIds == null || _activeTargetIds.Count <= 1) return;

        if (_actionEffectTarget == EffectTarget.EnemyAll || _actionEffectTarget == EffectTarget.FriendAll) return;

        int size = _activeTargetIds.Count;
        _targetCursorIndex = (_targetCursorIndex + 1) % size; // 循環処理

        // 💡 UIに渡すインデックスにオフセットを適用
        _uiController.ShowSelectedCursor(_targetCursorIndex);
        Logger.Instance.Log($"次のターゲットに移動。UIインデックス: {_targetCursorIndex}");
    }

    private void MovePreviousTarget()
    {
        if (_activeTargetIds == null || _activeTargetIds.Count <= 1) return;

        if (_actionEffectTarget == EffectTarget.EnemyAll || _actionEffectTarget == EffectTarget.FriendAll) return;

        int size = _activeTargetIds.Count;
        _targetCursorIndex--;

        // 負になったら末尾に戻る (循環)
        if (_targetCursorIndex < 0)
        {
            _targetCursorIndex = size - 1;
        }

        // 💡 UIに渡すインデックスにオフセットを適用
        _uiController.ShowSelectedCursor(_targetCursorIndex);
        Logger.Instance.Log($"前のターゲットに移動。UIインデックス: {_targetCursorIndex}");
    }

    /// <summary>
    /// 💡 新規ヘルパー: 現在のアクションのEffectTargetをデータから取得します。
    /// </summary>
    private EffectTarget GetActionEffectTarget(BattleCommand command, int itemId)
    {
        if (command == BattleCommand.Attack)
        {
            // 攻撃はデフォルトで敵単体 (EnemySolo) と仮定
            return EffectTarget.EnemySolo;
        }

        if (command == BattleCommand.Skill)
        {
            var skillData = SkillDataManager.Instance.GetSkillDataById(itemId);
            if (skillData?.skillEffects != null && skillData.skillEffects.Count > 0)
            {
                // 最初の効果のターゲットを取得
                return skillData.skillEffects.First().EffectTarget;
            }
        }

        if (command == BattleCommand.Item)
        {
            var itemData = ItemDataManager.Instance.GetItemDataById(itemId);
            if (itemData != null)
            {
                return itemData.itemEffect.effectTarget;
            }
        }

        return EffectTarget.EnemySolo;
    }

    /// <summary>
    /// 💡 修正: EffectTargetに基づいてターゲットIDリスト（敵のみ or 味方のみ）を生成します。
    /// </summary>
    private List<int> GeneratePossibleTargetsList(EffectTarget effectTarget)
    {
        // 敵をターゲットとするアクションの場合
        if (effectTarget == EffectTarget.EnemySolo || effectTarget == EffectTarget.EnemyAll)
        {
            // 倒れていない敵キャラクター全員の戦闘中IDをリストにして返す
            return EnemyStatusManager.Instance.GetEnemyStatusList()
                .Where(status => !status.isDefeated && !status.isRunaway)
                .Select(status => status.enemyBattleId)
                .ToList();
        }
        // 味方をターゲットとするアクションの場合
        else if (effectTarget == EffectTarget.FriendSolo || effectTarget == EffectTarget.FriendAll || effectTarget == EffectTarget.Own)
        {
            // 倒れていない味方キャラクター全員のIDをリストにして返す
            return CharacterStatusManager.Instance.partyCharacter
                .Where(id => !CharacterStatusManager.Instance.IsCharacterDefeated(id))
                .ToList();
        }

        return new List<int>();
    }

    public void ShowWindow()
    {
        _uiController.Show();
    }

    public void HideWindow()
    {
        _uiController.Hide();
    }

    private List<string> GetEnemyNamesByIds(List<int> enemyBattleIds)
    {
        List<string> names = new List<string>();
        foreach (int battleId in enemyBattleIds)
        {
            var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(battleId);
            if (enemyStatus != null && enemyStatus.enemyData != null)
            {
                names.Add(enemyStatus.enemyData.enemyName);
            }
            else
            {
                names.Add("不明な敵");
            }
        }
        return names;
    }

    private List<string> GetCharacterNamesByIds(List<int> characterIds)
    {
        List<string> names = new List<string>();
        foreach (int charaId in characterIds)
        {
            var charaData = CharacterDataManager.Instance.GetCharacterData(charaId);
            if (charaData != null)
            {
                // 名前だけでなく、必要に応じてHP/MP情報などもここで取得・整形可能
                names.Add(charaData.characterName);
            }
            else
            {
                names.Add("不明な仲間");
            }
        }
        return names;
    }
}