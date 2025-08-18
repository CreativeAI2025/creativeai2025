using System.Collections.Generic;
using UnityEngine;

public class DataSetting : MonoBehaviour{

    //int cols = 11;//列
    int rows;//行
    int cellSize = 75;//行間距離
    int nodeSum = -1;//ノードの数のカウント
    float width = 960;//スキルツリーの幅

    Dictionary<int, int> nodelimitPerRow = new Dictionary<int,int>();//階層によるノード数の制限
    Dictionary<int, float[]> linelimitPerRow = new Dictionary<int, float[]>();//遷移による枝数の制限
    Dictionary<string, float[]> skill_or_statusPerRow = new Dictionary<string, float[]>();//スキル・ステータスの変移確率(スキル、ステータス、初期状態)
    Dictionary<int, string[]> skillData = new Dictionary<int, string[]>();// スキル名とスキルの説明のデータ
    Dictionary<string, string[]> statusData = new Dictionary<string, string[]>();// ステータス名とステータスの説明のデータ

    List<Node> nodeData = new List<Node>();//ノードデータの保存
    List<Node> lineData = new List<Node>();//ラインデータの保存
    List<int[]> connections = new List<int[]>();// IDの遷移を記録

    void set(){
        reset();
        DataSet();
        NodeDataSet();
    }

    void reset() {
        rows = 0;
        nodeSum = 0;
        nodeData.Clear();
        lineData.Clear();
    }

    void DataSet() {
        NodeLimitData();
        lineLimitData();
        SkillOrStatusData();
        SkillData();
        StatusData();
    }

    void NodeLimitData() {//各階層でノードの個数の制限
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

    void lineLimitData() {//階層のノード数に対して枝数の確率(最高枝数６)
        // 階層のノード数が2個のとき → 枝1: 50%, 枝2: 30%, 枝3: 20%
        linelimitPerRow.Add(0, new float[]{0, 0, 0, 0, 0, 0, 0});
        linelimitPerRow.Add(1, new float[]{0, 0, 0, 0, 1, 0, 0});
        linelimitPerRow.Add(2, new float[]{0, 0.05f, 0.05f, 0, 0, 0, 0.9f});
        linelimitPerRow.Add(3, new float[]{0, 0, 0, 0, 1, 0, 0});
        linelimitPerRow.Add(4, new float[]{0, 0, 1, 0, 0, 0, 0});
        linelimitPerRow.Add(5, new float[]{0, 0, 0, 0.6f, 0, 0, 0.4f});
        linelimitPerRow.Add(6, new float[]{0, 0, 0, 0, 0, 0, 0});
        linelimitPerRow.Add(7, new float[]{0, 0, 1, 0, 0, 0, 0});
    }

    void SkillOrStatusData() {//入力に対して次がスキルまたステータスの確率(スキル、ステータス、初期状態)
        skill_or_statusPerRow.Add("スキル", new float[]{0.518f, 0.482f});
        skill_or_statusPerRow.Add("ステータス", new float[]{0.435f, 0.565f});
        skill_or_statusPerRow.Add("初期状態", new float[]{0.857f, 0.143f});//初期状態
    }

    void SkillData() {//ユーザが触るのはここだけ
        skillData.Add(0, new string[]{"エターナルブリザード", "相手に150ダメージの特殊攻撃"});
        skillData.Add(1, new string[]{"めちゃつよパンチ", "相手に200ダメージの物理攻撃"});
        skillData.Add(2, new string[]{"ヒールライト", "味方1人のHPを50回復する魔法"});
        skillData.Add(3, new string[]{"サンダーストライク", "相手に180ダメージの特殊攻撃"});
        skillData.Add(4, new string[]{"ファイアボール", "相手に120ダメージの特殊攻撃"});
        skillData.Add(5, new string[]{"アイスシールド", "味方全体の防御力を2ターン上昇させる"});
        skillData.Add(6, new string[]{"ポイズンニードル", "相手を3ターンの間毒状態にする"});
        skillData.Add(7, new string[]{"ブレイブスラッシュ", "相手に160ダメージの物理攻撃"});
        skillData.Add(8, new string[]{"マジックバリア", "味方全体の魔法防御力を3ターン上昇させる"});
        skillData.Add(9, new string[]{"リザレクション", "味方1人をHP30%で復活させる"});
        skillData.Add(10, new string[]{"シャドウステップ", "自分の回避率を2ターン上昇させる"});
        skillData.Add(11, new string[]{"ギガインパクト", "相手に300ダメージの物理攻撃"});
        skillData.Add(12, new string[]{"ウィンドカッター", "相手に130ダメージの特殊攻撃"});
    }

    void StatusData() {
        statusData.Add("ステータス", new string[]{"攻撃力アップ", "攻撃力が5%上昇"});
    }

    void NodeDataSet() {
        int id = 0;
        for (int x = 0; x < nodelimitPerRow.Count; x++) {
                rows++;
            }
        
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < nodelimitPerRow[y]; x++) {
                nodeSum++;
            }
        }

        if (nodeSum > id) {
            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < nodelimitPerRow[y]; x++) {
                    float drawPosX = x * cellSize - nodelimitPerRow[y] * cellSize / 2 + width / 2 + cellSize / 2;
                    float drawPosY = y * cellSize + cellSize /2;
                    nodeData.Add(new Node(id, x, y, drawPosX, drawPosY));
                    id++;
                }
            }
        }
    }

    //今の枝数を受け取り、確率に基づいて次の枝数を決める関数
    int getBranchCountFromDistribution(int nodeSum) {
        float[] probs = linelimitPerRow[nodeSum];// その階層での枝数の確率分布を入れる
        float r = Random.Range(0,1);//0~0.9999..までの乱数
        float sum = 0;//確率の和

        for (int i = 0; i < probs.Length; i++) {
            sum += probs[i];
            if (r < sum) return i;//枝の本数を返す
        }

        return probs.Length - 1;//枝の本数を返す
    }

    //入力の個数を数えて出力数を決める
    int input_for_out(int branch, int nowid) {
        int input = 0;

        foreach (int[] pair in connections) {
            int from = pair[0];
            int to = pair[1];

            if (nowid == to || nowid == from) {
            input++;
            }
        }

        return branch - input;
    }

    Dictionary<int, int> branchNum = new Dictionary<int,int>();//接続済みペアの格納

    void branchNumCheck() {
        for (int i = 0; i < nodeSum; i++) {
            int branch = 0;
            foreach (int[] pair in connections) {
                int from = pair[0];
                int to = pair[1];
                if (to == i || from == i) branch++;
            }
            branchNum.Add(i, branch);
        }
    }

    HashSet<string> usedConnections = new HashSet<string>(); // 使用済みパターンの格納

    void firstconnectRange(int nowStart, int nowEnd, int beforeStart, int beforeEnd) {
        for (int i = nowStart; i <= nowEnd; i++) {


            int branchCount = lineData[i].getBranch();

            HashSet<int> used = new HashSet<int>(); // すでにつかったIDの重複防止セット

            int j = 0;

            do {
            j = (int)Random.Range(beforeStart, beforeEnd + 1); // nowEnd ~ nextEnd のランダム(一つ下の階層)
            } while (used.Contains(j));
            used.Add(j);
            connections.Add(new int[]{j, i});
            string key = j + "-" + i; // 接続パターンを文字列化
            usedConnections.Add(key); // 使用済みとして登録
        }
    }

    void secondconnectRange(int nowStart, int nowEnd, int beforeStart, int beforeEnd) {
        for (int i = nowStart; i <= nowEnd; i++) {

            int branchCount = lineData[i].getBranch();
            int outputCount = branchCount - branchNum[i];

            HashSet<int> used = new HashSet<int>(); // ノード i に対する接続先重複防止

            int tries = 0;

            while (outputCount > 0) {
                int j = (int) Random.Range(beforeStart, beforeEnd + 1);

                string key = j + "-" + i; // 接続パターンを文字列化

                if (!used.Contains(j) && i != j && branchNum[j] < lineData[j].getBranch() && !usedConnections.Contains(key)) {

                    connections.Add(new int[]{j, i});
                    used.Add(j);
                    usedConnections.Add(key); // 使用済みとして登録

                    branchNum.Add(j, branchNum[j] + 1);
                    branchNum.Add(i, branchNum[i] + 1);

                    outputCount--;

                    // println("Connect: " + j + " -> " + i + " (usedConnections Added: " + key + ")");
                }

                tries++;
                if (tries > 1000) {
                    // println("Too many tries at node " + i);
                    break;
                }
            }
        }
    }

    // その階層のノード数でノードに生える枝数を決める
    void initializeNodes() {
        lineData.Clear();
        int id = 0;
        for (int i = 0; i < rows; i++) {
            for (int n = 0; n < nodelimitPerRow[i]; n++) {
            if (i <= 0) lineData.Add(new Node(id, 2));//初期状態
            if (0 < i) lineData.Add(new Node(id, getBranchCountFromDistribution(nodelimitPerRow[i])));
            //println(id, nodelimitPerRow[i]);
            id++;
            }
            //println("ID:" + lineData[i].getId() + "枝：" + lineData[i].getBranch());
        }
    }

    // 接続を作成
    void generateRandomConnections() {
        connections.Clear();  // 前回の接続をリセット
        usedConnections.Clear();
        lineData.Clear();
        int nodesum = nodeSum - 1;
        //int sum = 0;
        int bsum = 0;

        initializeNodes();// ノードに生える枝数を決める

        for (int y = rows - 1; y > 0; y--) {
            //if (y > 0) println(nodesum - nodelimitPerRow[y] + 1, nodesum, nodesum - nodelimitPerRow[y] - nodelimitPerRow[y-1] + 1, nodesum - nodelimitPerRow[y]);
            if (y > 0) firstconnectRange(nodesum - nodelimitPerRow[y] + 1, nodesum, nodesum - nodelimitPerRow[y] - nodelimitPerRow[y-1] + 1, nodesum - nodelimitPerRow[y]);
            bsum = nodesum;
            nodesum -= nodelimitPerRow[y];
        }

        branchNumCheck();

        nodesum = nodeSum - 1;
        bsum = 0;

        for (int y = rows - 1; y > 0; y--) {
            //if (y > 0) println(nodesum - nodelimitPerRow[y] + 1, nodesum, nodesum - nodelimitPerRow[y] - nodelimitPerRow[y-1] + 1, nodesum - nodelimitPerRow[y]);
            if (y > 0) secondconnectRange(nodesum - nodelimitPerRow[y] + 1, nodesum, nodesum - nodelimitPerRow[y] - nodelimitPerRow[y-1] + 1, nodesum - nodelimitPerRow[y]);
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
}