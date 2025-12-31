using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
public class PlacementBoard : MonoBehaviour
{
    [SerializeField] private Image placementBoardImage;
    [SerializeField] private bool turnOn = true;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private PlacementBoardStrip placementBoardStripPrefab;
    [SerializeField] private GameObject placementBoardContent;
    private readonly List<PlacementBoardStrip> strips = new List<PlacementBoardStrip>();

    private void Update()
    {
        if (turnOn)
        {
            placementBoardImage.enabled = true;
            UpdateBoardUI();
            
        }
        else
        {
            placementBoardImage.enabled = false;
        }
    }

    public void InitializePlacementBoard()
    {
        // Avoid re-initializing if already set up
        if (strips.Count > 0) return;

        var source = MarbleManager.Instance != null ? MarbleManager.Instance.availableMarbles : null;
        if (source == null) return;

        // Ensure unique marbles before creating strips
        var uniqueMarbles = source.Where(m => m != null).Distinct().ToList();
        for(int i = 0; i < uniqueMarbles.Count; i++)
        {
            GameObject marble = uniqueMarbles[i];
            PlacementBoardStrip strip = Instantiate(placementBoardStripPrefab, placementBoardContent.transform);
            strip.placementIndex = i;
            strip.SetAssignedMarble(marble);
            strips.Add(strip);
        }
    }



    public List<GameObject> GetSortedMarbles()
    {
        if (targetObject == null || MarbleManager.Instance == null || MarbleManager.Instance.availableMarbles == null)
        {
            Debug.LogWarning("PlacementBoard: Cannot get sorted marbles because targetObject or availableMarbles is null.");
            return new List<GameObject>();
        }

        // Ensure unique by sanitized name (treat same-named marbles as one)
        string Clean(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return name.Replace(" (Clone)", "").Replace("(Clone)", "").Trim();
        }

        return MarbleManager.Instance.availableMarbles
            .Where(m => m != null)
            .OrderBy(marble =>
            {
                Vector3 delta = targetObject.transform.position - marble.transform.position;
                delta.y = 0f;
                return delta.sqrMagnitude;
            })
            .GroupBy(m => Clean(m.name))
            .Select(g => g.First())
            .ToList();
    }
    public void UpdateBoardUI()
    {
        if(strips.Count == 0 || targetObject == null)
        {
            return;
        }

        // Keep rows active; update content per rank/elimination without hiding

        // Rank available marbles by proximity to target
        var availableByRank = GetSortedMarbles();
        var availableSet = new HashSet<GameObject>(availableByRank);
        // Detect eliminated via strips (includes null assigned marbles)
        var eliminatedStrips = new List<PlacementBoardStrip>();
        foreach (var s in strips)
        {
            var am = s.assignedMarble;
            if (am == null || !availableSet.Contains(am))
            {
                eliminatedStrips.Add(s);
            }
        }

        // Name helpers: sanitize and apply eliminated suffix once
        string Sanitize(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            // Remove clone and any prior eliminated suffix variants to avoid duplication
            var cleaned = name.Replace(" (Clone)", "").Replace("(Clone)", "");
            cleaned = cleaned.Replace(" (Eliminated)", "").Replace(" (eliminated)", "");
            return cleaned.Trim();
        }
        string NextDisplayName(GameObject marble, bool eliminatedSuffix)
        {
            var baseName = marble != null ? Sanitize(marble.name) : "";
            var display = string.IsNullOrEmpty(baseName) ? "" : baseName;
            if (eliminatedSuffix)
            {
                var suffix = " (eliminated)"; // lowercase as requested
                if (string.IsNullOrEmpty(display))
                {
                    display = suffix.Trim();
                }
                else if (!display.EndsWith(suffix) && !display.EndsWith(" (Eliminated)") && !display.EndsWith(" (ELIMINATED)"))
                {
                    display += suffix;
                }
            }
            return display;
        }

        // Helper: generate from a base name string (for eliminated strips with null marbles)
        string NextDisplayNameFromBase(string baseName, bool eliminatedSuffix)
        {
            baseName = Sanitize(baseName);
            var display = string.IsNullOrEmpty(baseName) ? "" : baseName;
            if (eliminatedSuffix)
            {
                var suffix = " (eliminated)";
                if (string.IsNullOrEmpty(display))
                {
                    display = suffix.Trim();
                }
                else if (!display.EndsWith(suffix) && !display.EndsWith(" (Eliminated)") && !display.EndsWith(" (ELIMINATED)"))
                {
                    display += suffix;
                }
            }
            return display;
        }

        // Fill rows: ranked marbles first
        int row = 0;
        for (; row < strips.Count && row < availableByRank.Count; row++)
        {
            var marble = availableByRank[row];
            strips[row].SetPlacement(row + 1, marble);
            strips[row].transform.SetSiblingIndex(row);
            // Display sanitized name (no numbering)
            strips[row].placementNameText.text = NextDisplayName(marble, false);
            strips[row].gameObject.SetActive(true);
        }

            // Then eliminated marbles only (avoid duplicates of available marbles)
            // Collapse eliminated by base name to avoid duplicate labels
            var eliminatedUnique = eliminatedStrips
                .GroupBy(es => es.BaseName)
                .Select(g => g.First())
                .ToList();

            int elimIdx = 0;
            for (; row < strips.Count && elimIdx < eliminatedUnique.Count; row++, elimIdx++)
            {
                var es = eliminatedUnique[elimIdx];
                var nameSource = es.assignedMarble; // may be null
                es.SetEliminated(nameSource);
                es.transform.SetSiblingIndex(row);
                // Ensure unique display name using base name even when marble is null
                es.placementNameText.text = NextDisplayNameFromBase(es.BaseName, true);
                es.gameObject.SetActive(true);
            }

        var rt = placementBoardContent.GetComponent<RectTransform>();
        if(rt != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

    }

    // Positions dictionary removed: ranking reads live transform positions
}
