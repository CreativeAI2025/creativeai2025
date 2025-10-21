using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
public class EnemyEncountManager : MonoBehaviour
{
    private class EncounterDataBase
    {
        private List<EncounterData> _encounterDataList;
        private Dictionary<string, EncounterData> _encounterDataDict;

        public async void Initialize()
        {
            AsyncOperationHandle<IList<EncounterData>> handle = Addressables.LoadAssetsAsync<EncounterData>(AddressablesLabels.Encount, null);
            await handle.Task;
            _encounterDataList = new List<EncounterData>(handle.Result);
            handle.Release();
            _encounterDataDict = _encounterDataList.ToDictionary(data => data.mapName, data => data);
            Debug.Log($"敵とのエンカウント情報の読み込み完了：{_encounterDataList.Count}");
        }

        public EncounterData getEncounterDataByMapName(string name)
        {
            _encounterDataDict.TryGetValue(name, out EncounterData encounterData);
            return encounterData;
        }
    }

    // エンカウントに関するデータベース
    private EncounterDataBase _encounterDataBase;
    // 上のデータベースから現在いるエンカウント情報を格納する
    private EncounterData encounterData;
    // 今いるシーン名
    private string _mapNameNow;
    // 歩数を記録する
    private int walkTimes;
    // シーン移動or戦闘終了後、5歩以内はエンカウントしない。
    private const int NoEncountWalkTimes = 5;
    // シーン移動or戦闘終了後、20歩以上はエンカウント確率をn倍にする（nは次の変数）
    private const int DoubleEncountWalkTimes = 20;
    // エンカウント確率の倍率
    private const int DoubleEncountRate = 2;
    // 同時にエンカウントする敵の最大値
    private const int EncountMax = 5;
    // 同時にエンカウントする敵の最小値
    private const int EncountMin = 1;
    private bool IsEncounted;
    // エンカウント情報があるかどうかの判別
    private InputSetting _inputSetting;
    async void Awake()
    {
        _encounterDataBase = new();
        _encounterDataBase.Initialize();
    }

    public void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    public void Update()
    {
        if (_inputSetting.GetRightKeyDown())
        {
            IncreaseEncountProbability();
        }
        else if (_inputSetting.GetLeftKeyDown())
        {
            Initialize();
        }
    }

    /// <summary>
    /// シーンが切り替わった際に、出現確率をリセットする。
    /// </summary>
    public void Initialize()
    {
        // ここでデータベースの読み込みを待つのもあり
        _mapNameNow = SceneManager.GetActiveScene().name;   // 現在いるシーン名の取得
        encounterData = _encounterDataBase.getEncounterDataByMapName(_mapNameNow);  // シーン名からエンカウント情報を取得
        if (!encounterData)
        {
            Debug.Log("現在いるシーンに、エンカウント情報は記載されていません。");
            IsEncounted = false;
            return;
        }
        IsEncounted = true;
        ResetEncount();

    }

    /// <summary>
    /// 戦闘終了後にエンカウント確率をリセットする
    /// </summary>
    public void ResetEncount()
    {
        walkTimes = 0;
    }

    /// <summary>
    /// バトルが開始するエンカウント確率を上げる
    /// マップの歩行処理を行うところで呼びたい
    /// </summary>
    public void IncreaseEncountProbability()
    {
        if (!IsEncounted)
        {
            return;
        }
        walkTimes++;    // この関数が呼ばれた（＝プレイヤーが歩いた）ので、歩数を＋１する
        Debug.Log($"[EnemyEncountManager]現在の歩数：{walkTimes}");

        // エンカウントしない歩数のため、return
        if (walkTimes <= NoEncountWalkTimes)
        {
            return;
        }

        int probability = (int)encounterData.rate;   // 現在のシーンの確率
        if (walkTimes < DoubleEncountWalkTimes)
        {
            // 特定の歩数を越えていないので、確率はそのままでエンカウントチェック
            Encount(probability);
        }
        else
        {
            // 特定の歩数を越えたので、確率を上げてエンカウントチェック
            Encount(probability * DoubleEncountRate);
        }


    }

    /// <summary>
    /// エンカウントする過程を処理する
    /// </summary>
    /// <param name="probability"></param> <summary>
    /// 現在のエンカウント確率
    /// </summary>
    /// <param name="probability"></param>
    private void Encount(int probability)
    {
        int rate = Random.Range(0, 100);
        if (rate <= probability)
        {
            // エンカウントでの戦闘を呼び起こす
            Debug.Log("エンカウントした！");
            List<int> enemyIds = GiveEnemyId(encounterData);
            BattleManager.Instance.StartBattle(enemyIds);
        }
        else
        {
            Debug.Log("エンカウントしなかった！");
        }
    }

    /// <summary>
    /// リスト化された敵IDを返す
    /// </summary>
    /// <param name="ed"></param>
    /// <returns></returns>
    private List<int> GiveEnemyId(EncounterData ed)
    {
        List<int> enemyId = new List<int>();    // このリストに敵のIDを入れる
        List<EnemyRate> er = ed.enemyRates; // エンカウンターデータから、個別の敵のIDと確率を持ってくる

        int rndMin = EncountMin * EncountMin;   // 累乗を計算する演算子はC#にない。using Systemを使えば解決できるが、今度はRandomがSystemかUnityEngineかが見分けがつかなくなってエラーをはくのが面倒だった（これも解決方法あり）
        int rndMax = EncountMax * EncountMax;
        int rnd = Random.Range(rndMin, rndMax);
        int times;
        for (times = EncountMin; times < EncountMax; times++)
        {
            int border = (EncountMax - times) * (EncountMax - times);
            if (rnd > border)
            {
                break;
            }
        }
        Debug.Log($"エンカウント抽選結果　乱数＿{rnd}、敵の数＿{times}");

        for (int i = 0; i < times; i++)
        {
            int id = GetEnemyIdRandom(er);
            enemyId.Add(id);
        }

        return enemyId;
    }


    /// <summary>
    /// 個別の敵のリストを引数に渡して、その中からランダムに敵のIDを返す
    /// </summary>
    /// <param name="er"></param>
    /// <returns></returns>
    private int GetEnemyIdRandom(List<EnemyRate> er)
    {
        int sum = 0;    // 個別の敵の確率を足す
        foreach (var val in er)
        {
            sum += val.rate;
        }
        int rnd = Random.Range(0, sum); // 個別の確率を足し、0~合計値までの間で乱数を作る
        int amount = er[0].rate;
        int times;
        for (times = 0; amount <= rnd; times++)
        {
            amount += er[times].rate;
        }
        int enemyId = er[times].enemyId;
        Debug.Log($"敵ID：{enemyId}");
        return enemyId;
    }

    /// <summary>
    /// 現在いる場所で、敵が発生するように設定されているかの判別
    /// 誤字にも注意
    /// </summary>
    /// <returns></returns>
    private bool IsLocated(string mapName)
    {
        return _encounterDataBase.getEncounterDataByMapName(mapName);
    }
}
