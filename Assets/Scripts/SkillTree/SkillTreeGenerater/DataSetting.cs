using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DataSetting : MonoBehaviour
{

    //int cols = 11;//列
    int rows;//行
    [SerializeField] int cellSizeX = 75;//行間距離
    [SerializeField] int cellSizeY = 75;//行間距離
    [SerializeField] float positionX = 0;
    [SerializeField] float positionY = 0;
    int nodeSum = -1;//ノードの数のカウント
    int skillCount = 0;
    int statusCount = 0;

    Dictionary<int, int> nodelimitPerRow = new Dictionary<int, int>();//階層によるノード数の制限
    Dictionary<int, float[]> linelimitPerRow = new Dictionary<int, float[]>();//遷移による枝数の制限
    Dictionary<string, float[]> skill_or_statusPerRow = new Dictionary<string, float[]>();//スキル・ステータスの変移確率(スキル、ステータス、初期状態)
    Dictionary<int, string[]> skillData = new Dictionary<int, string[]>();// スキル名とスキルの説明のデータ
    Dictionary<string, string[]> statusData = new Dictionary<string, string[]>();// ステータス名とステータスの説明のデータ
    Dictionary<int, string> tagData = new Dictionary<int, string>();// IDとスキル・ステータスの格納

    public List<Node> nodeData = new List<Node>();//ノードデータの保存
    public List<Node> lineData = new List<Node>();//ラインデータの保存
    public List<int[]> connections = new List<int[]>();//IDの遷移を記録
    public List<Skill> nodeSkillData = new List<Skill>();//スキルをもつノードの情報の保存

    public Dictionary<int, string> getTagData()
    {
        return this.tagData;
    }

    public void set()
    {
        reset();
        NodeDataSet();
        generateRandomConnections();
    }

    public void reset()
    {
        rows = 0;
        nodeSum = 0;
        nodeData.Clear();
        lineData.Clear();
        connections.Clear();
        tagData.Clear();
        nodeSkillData.Clear();
        branchNum.Clear();
    }

    /// <summary>
    /// データをセットする
    /// </summary>
    public void DataSet()
    {
        NodeLimitData();
        lineLimitData();
        SkillOrStatusData();
        SkillData();
        StatusData();
    }

    public void NodeLimitData()
    {//各階層でノードの個数の制限
        nodelimitPerRow.Add(0, 1);
        nodelimitPerRow.Add(1, 2);
        nodelimitPerRow.Add(2, 4);
        nodelimitPerRow.Add(3, 2);
        nodelimitPerRow.Add(4, 7);
        nodelimitPerRow.Add(5, 5);
        nodelimitPerRow.Add(6, 7);
        nodelimitPerRow.Add(7, 3);
        nodelimitPerRow.Add(8, 1);
        nodelimitPerRow.Add(9, 2);
        nodelimitPerRow.Add(10, 1);
    }

    public void lineLimitData()
    {//階層のノード数に対して枝数の確率(最高枝数６)
        // 階層のノード数が2個のとき → 枝1: 50%, 枝2: 30%, 枝3: 20%
        linelimitPerRow.Add(0, new float[] { 0, 0, 0, 0, 0, 0, 0 });
        linelimitPerRow.Add(1, new float[] { 0, 0, 0, 0, 1, 0, 0 });
        linelimitPerRow.Add(2, new float[] { 0, 0.05f, 0.05f, 0, 0, 0, 0.9f });
        linelimitPerRow.Add(3, new float[] { 0, 0, 0, 0, 1, 0, 0 });
        linelimitPerRow.Add(4, new float[] { 0, 0, 1, 0, 0, 0, 0 });
        linelimitPerRow.Add(5, new float[] { 0, 0, 0, 0.6f, 0, 0, 0.4f });
        linelimitPerRow.Add(6, new float[] { 0, 0, 0, 0, 0, 0, 0 });
        linelimitPerRow.Add(7, new float[] { 0, 0, 1, 0, 0, 0, 0 });
    }

    public void SkillOrStatusData()
    {//入力に対して次がスキルまたステータスの確率(スキル、ステータス、初期状態)
        skill_or_statusPerRow.Add("スキル", new float[] { 0.518f, 0.482f });
        skill_or_statusPerRow.Add("ステータス", new float[] { 0.435f, 0.565f });
        skill_or_statusPerRow.Add("初期状態", new float[] { 0.857f, 0.143f });//初期状態
    }

    public void SkillData()
    {//ユーザが触るのはここだけ
        skillData.Add(0, new string[] { "エターナルブリザード", "相手に150ダメージの特殊攻撃" });
        skillData.Add(1, new string[] { "めちゃつよパンチ", "相手に200ダメージの物理攻撃" });
        skillData.Add(2, new string[] { "ヒールライト", "味方1人のHPを50回復する魔法" });
        skillData.Add(3, new string[] { "サンダーストライク", "相手に180ダメージの特殊攻撃" });
        skillData.Add(4, new string[] { "ファイアボール", "相手に120ダメージの特殊攻撃" });
        skillData.Add(5, new string[] { "アイスシールド", "味方全体の防御力を2ターン上昇させる" });
        skillData.Add(6, new string[] { "ポイズンニードル", "相手を3ターンの間毒状態にする" });
        skillData.Add(7, new string[] { "ブレイブスラッシュ", "相手に160ダメージの物理攻撃" });
        skillData.Add(8, new string[] { "マジックバリア", "味方全体の魔法防御力を3ターン上昇させる" });
        skillData.Add(9, new string[] { "リザレクション", "味方1人をHP30%で復活させる" });
        skillData.Add(10, new string[] { "シャドウステップ", "自分の回避率を2ターン上昇させる" });
        skillData.Add(11, new string[] { "ギガインパクト", "相手に300ダメージの物理攻撃" });
        skillData.Add(12, new string[] { "ウィンドカッター", "相手に130ダメージの特殊攻撃" });
    }

    public void StatusData()
    {
        statusData.Add("ステータス", new string[] { "攻撃力アップ", "攻撃力が5%上昇" });
    }

    /// <summary>
    /// ノードの位置を決める
    /// </summary>
    public void NodeDataSet()
    {
        int id = 0;
        for (int x = 0; x < nodelimitPerRow.Count; x++)
        {
            rows++;
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < nodelimitPerRow[y]; x++)
            {
                nodeSum++;
            }
        }

        if (nodeSum > id)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < nodelimitPerRow[y]; x++)
                {
                    float drawPosX = x * cellSizeY - nodelimitPerRow[y] * cellSizeY / 2 - positionY;//x座標
                    float drawPosY = y * cellSizeX - ((cellSizeX * ((float)rows)) / 2.0f) + cellSizeX / 2.0f + positionX;//y座標
                    nodeData.Add(new Node(id, x, y, drawPosX, drawPosY));
                    id++;
                }
            }
        }
    }

    /// <summary>
    /// スキルデータのセッティング
    /// </summary>
    public void SkillDataSet()
    {
        for (int i = 0; i < skillData.Count; i++)
        {
            serchSkillDescription(skillData[i]);
        }
    }


    /// <summary>
    /// 今の枝数を受け取り、確率に基づいて次の枝数を決める関数
    /// </summary>
    /// <param name="nodeSum"></param>
    /// <returns></returns>
    public int getBranchCountFromDistribution(int nodeSum)
    {
        float[] probs = linelimitPerRow[nodeSum];// その階層での枝数の確率分布を入れる
        float r = Random.Range(0, 1);//0~0.9999..までの乱数
        float sum = 0;//確率の和

        for (int i = 0; i < probs.Length; i++)
        {
            sum += probs[i];
            if (r < sum) return i;//枝の本数を返す
        }

        return probs.Length - 1;//枝の本数を返す
    }

    /// <summary>
    /// 入力の個数を数えて出力数を決める
    /// </summary>
    /// <param name="branch"></param>
    /// <param name="nowid"></param>
    /// <returns></returns>
    public int input_for_out(int branch, int nowid)
    {
        int input = 0;

        foreach (int[] pair in connections)
        {
            int from = pair[0];
            int to = pair[1];

            if (nowid == to || nowid == from)
            {
                input++;
            }
        }

        return branch - input;
    }

    Dictionary<int, int> branchNum = new Dictionary<int, int>();//接続済みペアの格納

    //  接続済みペアの格納
    public void branchNumCheck()
    {
        for (int i = 0; i < nodeSum; i++)
        {
            int branch = 0;
            foreach (int[] pair in connections)
            {
                int from = pair[0];
                int to = pair[1];
                if (to == i || from == i) branch++;
            }
            branchNum.Add(i, branch);
        }
    }

    HashSet<string> usedConnections = new HashSet<string>(); // 使用済みパターンの格納

    /// <summary>
    /// 一本だけ下のノードからラインを引く
    /// </summary>
    /// <param name="nowStart"></param>
    /// <param name="nowEnd"></param>
    /// <param name="beforeStart"></param>
    /// <param name="beforeEnd"></param>
    public void firstconnectRange(int nowStart, int nowEnd, int beforeStart, int beforeEnd)
    {
        for (int i = nowStart; i <= nowEnd; i++)
        {


            int branchCount = lineData[i].getBranch();

            HashSet<int> used = new HashSet<int>(); // すでにつかったIDの重複防止セット

            int j = 0;

            do
            {
                j = (int)Random.Range(beforeStart, beforeEnd + 1); // nowEnd ~ nextEnd のランダム(一つ下の階層)
            } while (used.Contains(j));
            used.Add(j);
            connections.Add(new int[] { j, i });
            string key = j + "-" + i; // 接続パターンを文字列化
            usedConnections.Add(key); // 使用済みとして登録
        }
    }

    /// <summary>
    /// エッジ数の再現に沿って足りない線を引く
    /// </summary>
    /// <param name="nowStart"></param>
    /// <param name="nowEnd"></param>
    /// <param name="beforeStart"></param>
    /// <param name="beforeEnd"></param>
    public void secondconnectRange(int nowStart, int nowEnd, int beforeStart, int beforeEnd)
    {
        for (int i = nowStart; i <= nowEnd; i++)
        {

            int branchCount = lineData[i].getBranch();
            int outputCount = branchCount - branchNum[i];

            HashSet<int> used = new HashSet<int>(); // ノード i に対する接続先重複防止

            int tries = 0;

            while (outputCount > 0)
            {
                int j = (int)Random.Range(beforeStart, beforeEnd + 1);

                string key = j + "-" + i; // 接続パターンを文字列化

                if (!used.Contains(j) && i != j && branchNum[j] < lineData[j].getBranch() && !usedConnections.Contains(key))
                {

                    connections.Add(new int[] { j, i });
                    used.Add(j);
                    usedConnections.Add(key); // 使用済みとして登録

                    branchNum[j] = branchNum[j] + 1;
                    branchNum[i] = branchNum[i] + 1;

                    outputCount--;

                    // println("Connect: " + j + " -> " + i + " (usedConnections Added: " + key + ")");
                }

                tries++;
                if (tries > 1000)
                {
                    // println("Too many tries at node " + i);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// その階層のノード数でノードに生える枝数を決める
    /// </summary>
    public void initializeNodes()
    {
        lineData.Clear();
        int id = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int n = 0; n < nodelimitPerRow[i]; n++)
            {
                if (i <= 0) lineData.Add(new Node(id, 2));//初期状態
                if (0 < i) lineData.Add(new Node(id, getBranchCountFromDistribution(nodelimitPerRow[i])));
                //println(id, nodelimitPerRow[i]);
                id++;
            }
            //println("ID:" + lineData[i].getId() + "枝：" + lineData[i].getBranch());
        }
    }

    /// <summary>
    /// 接続を作成
    /// </summary>    
    public void generateRandomConnections()
    {
        connections.Clear();  // 前回の接続をリセット
        usedConnections.Clear();
        lineData.Clear();
        int nodesum = nodeSum - 1;
        //int sum = 0;
        int bsum = 0;

        initializeNodes();// ノードに生える枝数を決める

        for (int y = rows - 1; y > 0; y--)
        {
            //if (y > 0) println(nodesum - nodelimitPerRow[y] + 1, nodesum, nodesum - nodelimitPerRow[y] - nodelimitPerRow[y-1] + 1, nodesum - nodelimitPerRow[y]);
            if (y > 0) firstconnectRange(nodesum - nodelimitPerRow[y] + 1, nodesum, nodesum - nodelimitPerRow[y] - nodelimitPerRow[y - 1] + 1, nodesum - nodelimitPerRow[y]);
            bsum = nodesum;
            nodesum -= nodelimitPerRow[y];
        }

        branchNumCheck();

        nodesum = nodeSum - 1;
        bsum = 0;

        for (int y = rows - 1; y > 0; y--)
        {
            //if (y > 0) println(nodesum - nodelimitPerRow[y] + 1, nodesum, nodesum - nodelimitPerRow[y] - nodelimitPerRow[y-1] + 1, nodesum - nodelimitPerRow[y]);
            if (y > 0) secondconnectRange(nodesum - nodelimitPerRow[y] + 1, nodesum, nodesum - nodelimitPerRow[y] - nodelimitPerRow[y - 1] + 1, nodesum - nodelimitPerRow[y]);
            bsum = nodesum;
            nodesum -= nodelimitPerRow[y];
        }

        //for (int y = 0; y < rows; y++) {
        //  //if (y < rows - 1) println(sum, sum + nodelimitPerRow[y] - 1,bsum, sum + + nodelimitPerRow[y] + nodelimitPerRow.get(y + 1) - 1);
        //  //if (y < rows - 1) connectRange(sum, sum + nodelimitPerRow[y] - 1, bsum, sum + nodelimitPerRow[y] + nodelimitPerRow.get(y + 1) - 1);
        //  bsum = sum;
        //  sum += nodelimitPerRow[y];
        //}
    }

    /// <summary>
    /// リストへのスキルデータの格納
    /// </summary>
    /// <param name="skilldata"></param>  
    void serchSkillDescription(string[] skilldata)
    {
        string name = skilldata[0];//スキル名
        string explain = skilldata[1];//説明文

        string subject = null;//対象
        string action = null;//行動(攻撃、回復など)
        int power = -1;//効果量
        string type = null;//種類（物理攻撃、特殊攻撃など）
        string status = null;//対象ステータス
        string extra = null;//追加効果
        int duration = -1;//持続ターン

        // 対象の抽出
        if (explain.Contains("相手に") || explain.Contains("相手を"))
        {
            subject = "相手";
        }
        else if (explain.Contains("味方1人の") || explain.Contains("味方1人を"))
        {
            subject = "味方1人";
        }
        else if (explain.Contains("味方全体の"))
        {
            subject = "味方全体";
        }
        else if (explain.Contains("自分の"))
        {
            subject = "自分";
        }
        else
        {
            subject = "不明";
        }

        // 攻撃
        var result = Regex.Match(explain, @"(\d+)[^0-9]*(物理|特殊)攻撃");
        if (result.Success)
        {
            power = int.Parse(result.Groups[1].Value);
            action = "攻撃";
            type = result.Groups[2].Value + "攻撃";
        }

        // 回復
        result = Regex.Match(explain, @"(\d+)%?回復");
        if (result.Success)
        {
            power = int.Parse(result.Groups[1].Value);
            action = "回復";
            type = explain.Contains("%") ? "%回復" : "回復";
        }

        // 復活
        result = Regex.Match(explain, @"(\d+)%?で復活");
        if (result.Success)
        {
            power = int.Parse(result.Groups[1].Value);
            action = "復活";
            type = explain.Contains("%") ? "%で復活" : "復活";
        }

        // バフ・デバフ
        result = Regex.Match(explain, @"(\d+)ターン");
        if (result.Success)
        {
            duration = int.Parse(result.Groups[1].Value);
            if (explain.Contains("上昇") || explain.Contains("アップ"))
            {
                action = "強化";
                type = "バフ";
            }
            else if (explain.Contains("低下") || explain.Contains("ダウン"))
            {
                action = "弱体";
                type = "デバフ";
            }

            if (explain.Contains("回避率")) status = "回避率";
            if (explain.Contains("魔法防御率")) status = "魔法防御";
            if (explain.Contains("防御力")) status = "防御力";
        }

        // 追加効果
        if (explain.Contains("毒")) extra = "毒";
        if (explain.Contains("麻痺")) extra = "麻痺";
        if (explain.Contains("睡眠")) extra = "睡眠";

        // データ格納
        nodeSkillData.Add(new Skill(name, subject, action, power, type, status, extra, duration));
    }

    public void TagSet() // スキル・ステータスの振り分け
    {
        tagData[0] = "初期状態";
        HashSet<int> usedid = new HashSet<int>();

        // --- ソート (C#ではList.Sortを使う) ---
        connections.Sort((a, b) => a[0].CompareTo(b[0]));

        foreach (int[] pair in connections)
        {
            int from = pair[0];
            int to = pair[1];

            if (from == 0 && !usedid.Contains(to))
            {
                tagData[to] = tagName("初期状態");
            }

            if (!usedid.Contains(to))
            {
                if (tagData.ContainsKey(from))
                {
                    tagData[to] = tagName(tagData[from]);
                }
            }

            usedid.Add(to);
        }

        SScount();
    }

    /// <summary>
    /// 前の状態を受け取り、確率に基づいて次の状態を決める関数
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    string tagName(string tag)
    {
        int tagNum = 0; // タグ番号
        float[] probs;

        if (!skill_or_statusPerRow.TryGetValue(tag, out probs))
        {
            probs = new float[] { 0.0f };
        }

        float r = UnityEngine.Random.value; // 0.0 ~ 1.0 未満
        float sum = 0f;

        for (int i = 0; i < probs.Length; i++)
        {
            sum += probs[i];
            if (r < sum)
            {
                tagNum = i;
                break;
            }
        }

        if (r >= sum) tagNum = probs.Length - 1;

        if (tagNum == 0)
        {
            return "スキル";
        }
        else if (tagNum == 1)
        {
            return "ステータス";
        }
        return null;
    }

    void SScount()
    {
        skillCount = 0;
        statusCount = 0;

        foreach (var kv in tagData)
        {
            if (kv.Value == "スキル")
            {
                skillCount++;
            }
            else if (kv.Value == "ステータス")
            {
                statusCount++;
            }
        }

        // Debug.Log($"Skill={skillCount}, Status={statusCount}");//スキル・ステータスの数
    }
}