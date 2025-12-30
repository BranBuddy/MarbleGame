using System.Collections.Generic;
using UnityEngine;
using Singletons;
using Unity.Mathematics;
using UnityEngine.Pool;
using System.Reflection;

public class StartLineManager : Singleton<StartLineManager>, IDataPeristenceManager
{
    public Dictionary<GameObject, bool> poolOfMarbles; // All marbles that can be used in the game
    public List<GameObject> unlockedMarbles; // Marbles that have been unlocked by the player
    public List<GameObject> availableMarbles; // Marbles that are currently racing
    public Transform[] startLinePositions;

    /// <summary>
    /// Loads save data into runtime pools and rebuilds the unlocked list from the saved entries or from the pool flags.
    /// </summary>
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
    /// Unity lifecycle Awake: ensure pools exist, rebuild unlocked marbles from pool flags, then run base init.
    /// </summary>
    protected override void Awake()
    {
        EnsurePoolsInitialized();

        RebuildUnlockedFromPool();

        base.Awake();
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

        // If nothing ended up marked true, unlock all loaded marbles as a fallback.
        bool anyUnlocked = false;
        foreach (var entry in poolOfMarbles)
        {
            if (entry.Value) { anyUnlocked = true; break; }
        }
        if (!anyUnlocked)
        {
            var keys = new List<GameObject>(poolOfMarbles.Keys);
            foreach (var key in keys)
            {
                poolOfMarbles[key] = true;
            }
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
    /// Instantiates or positions available marbles at the start line and disables unused positions.
    /// </summary>
    internal void PlaceMarblesAtStartLine()
    {
        for (int i = 0; i < availableMarbles.Count && i < startLinePositions.Length; i++)
        {
            var entry = availableMarbles[i];
            if (entry == null) continue;

            // If it's still a prefab asset (not in a valid scene), instantiate and replace it
            if (!entry.scene.IsValid())
            {
                var inst = Instantiate(entry, startLinePositions[i].position, startLinePositions[i].rotation);
                availableMarbles[i] = inst;
            }
            else
            {
                entry.transform.SetPositionAndRotation(startLinePositions[i].position, startLinePositions[i].rotation);
                if (!entry.activeSelf) entry.SetActive(true);
            }
        }

        if(availableMarbles.Count < startLinePositions.Length)
            DisableStartLinePositions();

    }

    /// <summary>
    /// Deactivates start line positions that exceed the available marbles count.
    /// </summary>
    private void DisableStartLinePositions()
    {
        foreach(var pos in startLinePositions)
        {
            if(System.Array.IndexOf(startLinePositions, pos) >= availableMarbles.Count)
            {
                pos.gameObject.SetActive(false);
            }
        } 
    }

}
