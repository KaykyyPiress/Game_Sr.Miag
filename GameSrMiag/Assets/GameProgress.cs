using System.Collections.Generic;

public static class GameProgress
{
    public static bool initialized = false;

    public static int maxLives = 4;
    public static int currentLives = 4;

    public static int totalPasteisSupremos = 6;
    public static int collectedPasteisSupremos = 0;

    private static HashSet<string> collectedPastelIds = new HashSet<string>();
    private static HashSet<string> deadEnemyIds = new HashSet<string>();
    private static HashSet<string> deadBossIds = new HashSet<string>();

    public static void ResetProgress(int lives, int totalPasteis)
    {
        initialized = true;

        maxLives = lives;
        currentLives = lives;

        totalPasteisSupremos = totalPasteis;
        collectedPasteisSupremos = 0;

        collectedPastelIds.Clear();
        deadEnemyIds.Clear();
        deadBossIds.Clear();
    }

    // =========================
    // PASTÉIS
    // =========================
    public static bool IsPastelCollected(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return collectedPastelIds.Contains(id);
    }

    public static void MarkPastelCollected(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (!collectedPastelIds.Contains(id))
        {
            collectedPastelIds.Add(id);
            collectedPasteisSupremos++;
        }
    }

    // =========================
    // INIMIGOS
    // =========================
    public static bool IsEnemyDead(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return deadEnemyIds.Contains(id);
    }

    public static void MarkEnemyDead(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (!deadEnemyIds.Contains(id))
        {
            deadEnemyIds.Add(id);
        }
    }

    // =========================
    // BOSSES
    // =========================
    public static bool IsBossDead(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return deadBossIds.Contains(id);
    }

    public static void MarkBossDead(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (!deadBossIds.Contains(id))
        {
            deadBossIds.Add(id);
        }
    }
}