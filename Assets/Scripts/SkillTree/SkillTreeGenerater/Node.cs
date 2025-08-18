using UnityEngine;

public class Node{
    private int id;//各ノードのID
    private int distX, distY;//ノードの探索距離
    private float x, y;//ノードの実座標
    private int branch;//枝の数

    private string tag;//タグ名（スキルorステータス）

    private int inputCount = 0;   // 入力された回数（どこからか来た回数）
    private int outputCount = 0;  // 出力した回数（どこかへ出した回数）

    public Node(int id, int dist_x, int dist_y, float x, float y) {//コンストラクタ(ノード描写用)
        this.id = id;
        this.distX = dist_x;
        this.distY = dist_y;
        this.x = x;
        this.y = y;
    }

    public Node(int id, int branch) {//コンストラクタ（ライン描写用）
        this.id = id;
        this.branch = branch;//1つのノードに生える枝数
    }

    public Node(int id, string tag) {//コンストラクタ（スキル・ステータス用）
        this.id = id;
        this.tag = tag;
    }

    public int getId() {
        return this.id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getDistX() {
        return this.distX;
    }

    public int getDistY() {
        return this.distY;
    }

    public float getX() {
        return this.x;
    }

    public float getY() {
        return this.y;
    }

    public int getBranch() {
        return this.branch;
    }

    void addInput() {
        inputCount++;
    }
    void addOutput() {
        outputCount++;
    }

    int getInputCount() {
        return inputCount;
    }
    int getOutputCount() {
        return outputCount;
    }

    public string getTag() {
        return this.tag;
    }
}
