using UnityEngine;
using TMPro;

public class PlacementBoardStrip : MonoBehaviour
{
    [SerializeField] internal TMP_Text placementNumberText;
    [SerializeField] internal TMP_Text placementNameText;
    [SerializeField] internal int placementIndex;
    [SerializeField] internal GameObject assignedMarble;
    [SerializeField] private string baseName;

    // Expose sanitized base name for UI de-duplication when assignedMarble is null
    internal string BaseName => baseName;

    private string CleanName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";
        var cleaned = name.Replace(" (Clone)", "").Replace("(Clone)", "");
        cleaned = cleaned.Replace(" (Eliminated)", "").Replace(" (eliminated)", "");
        return cleaned.Trim();
    }
 
    // Called by PlacementBoard when order updates
    public void SetPlacement(int placementNumber, GameObject marble)
    {
        if (placementNumberText != null)
        {
            placementNumberText.text = placementNumber.ToString() + ".";
        }
        
        if (placementNameText != null && marble != null)
        {
            baseName = CleanName(marble.name);
            placementNameText.text = baseName;
        }
    }

    public void SetAssignedMarble(GameObject marble)
    {
        assignedMarble = marble;
        baseName = marble != null ? CleanName(marble.name) : baseName;
        if (placementNameText != null && !string.IsNullOrEmpty(baseName))
        {
            placementNameText.text = baseName;
        }
    }

    public void SetEliminated(GameObject marble)
    {
        if (placementNumberText != null)
        {
            placementNumberText.text = "—";
        }

        if (placementNameText != null)
        {
            string sourceName = baseName;
            if (string.IsNullOrEmpty(sourceName))
            {
                if (marble != null)
                {
                    sourceName = CleanName(marble.name);
                }
                else if (assignedMarble != null)
                {
                    sourceName = CleanName(assignedMarble.name);
                }
                else
                {
                    sourceName = placementNameText.text;
                }
            }

            var suffix = " (eliminated)";

            // Normalize any existing uppercase suffix back to base name
            if (!string.IsNullOrEmpty(sourceName) && (sourceName.EndsWith(" (Eliminated)") || sourceName.EndsWith(" (ELIMINATED)")))
            {
                sourceName = sourceName.Substring(0, sourceName.LastIndexOf(" ("));
            }

            // Apply lowercase eliminated suffix once
            placementNameText.text = string.IsNullOrEmpty(sourceName) ? "(eliminated)" : sourceName + suffix;
        }
    }
}
