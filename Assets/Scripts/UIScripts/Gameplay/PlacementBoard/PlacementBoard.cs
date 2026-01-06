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

        // Create a strip for each marble instance (including duplicates)
        for(int i = 0; i < source.Count; i++)
        {
            GameObject marble = source[i];
            if (marble == null) continue;
            
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

        return MarbleManager.Instance.availableMarbles
            .Where(m => m != null)
            .OrderBy(marble =>
            {
                Vector3 delta = targetObject.transform.position - marble.transform.position;
                delta.y = 0f;
                return delta.sqrMagnitude;
            })
            .ToList();
    }
    public void UpdateBoardUI()
    {
        if(strips.Count == 0 || targetObject == null)
        {
            return;
        }

        // Rank available marbles by proximity to target
        var availableByRank = GetSortedMarbles();
        var availableSet = new HashSet<GameObject>(availableByRank);
        
        // Detect eliminated: a strip is eliminated if its assigned marble is not in the available set
        var eliminatedStrips = new List<PlacementBoardStrip>();
        var activeStrips = new List<PlacementBoardStrip>();
        
        foreach (var s in strips)
        {
            var am = s.assignedMarble;
            // A marble is eliminated if it's null or not in the available set
            if (am == null || !availableSet.Contains(am))
            {
                eliminatedStrips.Add(s);
            }
            else
            {
                activeStrips.Add(s);
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

        // Fill rows: ranked active marbles first
        int row = 0;
        for (int i = 0; i < availableByRank.Count && row < strips.Count; i++)
        {
            var marble = availableByRank[i];
            // Find the strip assigned to this marble
            var strip = strips.FirstOrDefault(s => s.assignedMarble == marble);
            if (strip == null) continue; // Safety check
            
            strip.SetPlacement(row + 1, marble);
            strip.transform.SetSiblingIndex(row);
            strip.placementNameText.text = NextDisplayName(marble, false);
            strip.gameObject.SetActive(true);
            row++;
        }

        // Then eliminated marbles (show all eliminated, even duplicates)
        for (int i = 0; i < eliminatedStrips.Count && row < strips.Count; i++, row++)
        {
            var es = eliminatedStrips[i];
            var nameSource = es.assignedMarble; // may be null
            es.SetEliminated(nameSource);
            es.transform.SetSiblingIndex(row);
            es.placementNameText.text = NextDisplayNameFromBase(es.BaseName, true);
            es.gameObject.SetActive(true);
        }
        
        // Deactivate any unused strips
        for (int i = row; i < strips.Count; i++)
        {
            strips[i].gameObject.SetActive(false);
        }

        var rt = placementBoardContent.GetComponent<RectTransform>();
        if(rt != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

    }

    // Positions dictionary removed: ranking reads live transform positions

    /// <summary>
    /// Clears all instantiated placement strips and resets the board UI.
    /// Call this when the game is reset so the board is empty until a new game initializes it.
    /// </summary>
    public void ResetBoard()
    {
        // Destroy existing strip game objects
        foreach (var s in strips)
        {
            if (s != null)
            {
                Destroy(s.gameObject);
            }
        }
        strips.Clear();

        // Safety: clear any leftover children under the content container
        if (placementBoardContent != null)
        {
            foreach (Transform child in placementBoardContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Rebuild layout so the UI reflects the cleared state
        var rt = placementBoardContent != null ? placementBoardContent.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
    }
}
