public class HikaruMeetToAzami : CharatipDisplay
{
    public override void ChangeCharatipVisibility()
    {
        if (charatip.enabled && FlagManager.Instance.HasFlag("NusiBattleFinished"))
        {
            //Debug.Log("マップ選択された");
            charatip.enabled = false;
        }
    }
}