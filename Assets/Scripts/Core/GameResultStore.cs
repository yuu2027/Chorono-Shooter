
public static class GameResultStore
{
    public static int Score { get; private set; }
    public static int KillCount { get; private set; }
    public static float PlayTime { get; private set; }
    public static int HitCount { get; private set; }
    public static bool HasResult { get; private set; }

    public static void Save(int score, int killCount, float playTime, int hitCount)
    {
        Score = score;
        KillCount = killCount;
        PlayTime = playTime;
        HitCount = hitCount;
        HasResult = true;
    }
}
