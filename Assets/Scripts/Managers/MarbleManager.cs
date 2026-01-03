using UnityEngine;
using Singletons;
using System.Collections.Generic;
using System.Reflection;
public class MarbleManager : Singleton<MarbleManager>, IDataPeristenceManager
{
    public Dictionary<GameObject, bool> poolOfMarbles; // All marbles that can be used in the game
    public List<GameObject> unlockedMarbles; // Marbles that have been unlocked by the player
    public List<GameObject> availableMarbles; // Marbles that are currently racing

    protected override void Awake()
    {
        EnsurePoolsInitialized();
        RebuildUnlockedFromPool();

        base.Awake();
    }

    public void LoadData(GameData data)
    {
        EnsurePoolsInitialized();
        // If saved unlocked list exists, use it; otherwise seed defaults.
        if (data != null && data.poolOfMarbles != null && data.poolOfMarbles.Count > 0)
        {
            unlockedMarbles.Clear();
            // Mark unlocked entries in the pool from saved list
            foreach (var marble in data.poolOfMarbles)
            {
                if (marble != null)
                {
                    poolOfMarbles[marble] = true;
                    unlockedMarbles.Add(marble);
                }
            }
        }
        else
        {
            // Fallback: use any marbles already marked true in the pool
            RebuildUnlockedFromPool();
        }
    }
    /// <summary>
    /// Persists the currently unlocked marbles list into the save data.
    /// </summary>
    public void SaveData(GameData data)
    {
        data.poolOfMarbles = this.unlockedMarbles;
    }

     /// <summary>
    /// Rebuilds unlockedMarbles from poolOfMarbles entries marked true.
    /// </summary>
    private void RebuildUnlockedFromPool()
    {
        unlockedMarbles.Clear();
        foreach (var marble in poolOfMarbles)
        {
            if (marble.Value && marble.Key != null)
            {
                unlockedMarbles.Add(marble.Key);
            }
        }
    }

     /// <summary>
    /// Ensures collections exist and seeds the pool from Resources, setting each entry's flag based on its MarbleSO.isUnlocked.
    /// If none are flagged unlocked, all loaded marbles are unlocked as a fallback.
    /// </summary>
    public void EnsurePoolsInitialized()
    {
        poolOfMarbles ??= new Dictionary<GameObject, bool>();
        unlockedMarbles ??= new List<GameObject>();
        availableMarbles ??= new List<GameObject>();

        if (poolOfMarbles.Count == 0)
        {
            // Load all prefabs from Resources/PrefabsToLoad/Marbles and mark them unlocked by default.
            var prefabs = Resources.LoadAll<GameObject>("PrefabsToLoad/Marbles");
            foreach (var prefab in prefabs)
            {
                if (prefab != null && !poolOfMarbles.ContainsKey(prefab))
                {
                    poolOfMarbles.Add(prefab, GetUnlockFlagFromPrefab(prefab));
                }
            }
        }

        SyncUnlocksFromInventory();

        // If nothing ended up marked true and inventory has no selections, unlock all loaded marbles as a fallback.
        bool anyUnlocked = false;
        foreach (var entry in poolOfMarbles)
        {
            if (entry.Value) { anyUnlocked = true; break; }
        }
        if (!anyUnlocked && !HasInventorySelections())
        {
            var keys = new List<GameObject>(poolOfMarbles.Keys);
            foreach (var key in keys)
            {
                poolOfMarbles[key] = true;
            }
        }
    }

    /// <summary>
    /// Forwards start-line placement to the scene StartLineManager (if present).
    /// </summary>
    public void PlaceMarblesAtStartLine()
    {
        var startLineManager = FindAnyObjectByType<StartLineManager>();
        if (startLineManager != null)
        {
            startLineManager.PlaceMarblesAtStartLine();
        }
        else
        {
            Debug.LogWarning("MarbleManager.PlaceMarblesAtStartLine: No StartLineManager found in the scene.");
        }
    }

    /// <summary>
    /// Inspects a prefab for a marbleDataSO field and returns its isUnlocked flag; defaults to true if none found.
    /// </summary>
    private bool GetUnlockFlagFromPrefab(GameObject prefab)
    {
        if (prefab == null) return true;

        var behaviours = prefab.GetComponents<MonoBehaviour>();
        foreach (var mb in behaviours)
        {
            if (mb == null) continue;
            var field = mb.GetType().GetField("marbleDataSO", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null && typeof(MarbleSO).IsAssignableFrom(field.FieldType))
            {
                var so = field.GetValue(mb) as MarbleSO;
                if (so != null)
                {
                    return so.isUnlocked;
                }
            }
        }

        return true; // default to unlocked if no data found
    }

    /// <summary>
    /// Marks marbles as unlocked when their prefab name appears in the player's owned item list.
    /// </summary>
    private void SyncUnlocksFromInventory()
    {
        var inventory = PlayerInventoryManager.Instance;
        if (inventory == null || inventory.ownedItems == null) return;

        foreach (var prefab in new List<GameObject>(poolOfMarbles.Keys))
        {
            if (prefab == null) continue;
            if (inventory.ownedItems.Contains(prefab.name))
            {
                poolOfMarbles[prefab] = true;
            }
        }
    }

    private bool HasInventorySelections()
    {
        var inventory = PlayerInventoryManager.Instance;
        return inventory != null && inventory.ownedItems != null && inventory.ownedItems.Count > 0;
    }
}
