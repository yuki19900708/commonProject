public static class ConstValueMgr
{
    private static int Merge_Monster_Extra_Ap_ID = 103;
    private static int Skip_Sleep_Extra_Ap_ID = 104;
    private static int Chapter_MaxAP_ID = 406;


    private static int mergeExtraAp;
    private static int skipSleepExtraAp;
    private static int chapterMaxAp;


    /// <summary>
    /// 合成初级龙附加的额外体力
    /// </summary>
    public static int MergeExtraAp
    {
        get { return mergeExtraAp; }
    }
    /// <summary>
    /// 钻石跳过休息时间额外附加体力
    /// </summary>
    public static int SkipSleepExtraAp
    {
        get { return skipSleepExtraAp; }
    }

    public static int ChapterMaxAp
    {
        get { return chapterMaxAp; }
    }

    public static void BindEvent()
    {
        //ResUpdateMgr.Event_DownloadFinish += Response_InitializeConstValue;
    }

    private static void Response_InitializeConstValue()
    {
        mergeExtraAp = TableDataMgr.GetSingleConstValue(Merge_Monster_Extra_Ap_ID).value;
        skipSleepExtraAp = TableDataMgr.GetSingleConstValue(Skip_Sleep_Extra_Ap_ID).value;
        chapterMaxAp = TableDataMgr.GetSingleConstValue(Chapter_MaxAP_ID).value;
    }
}
