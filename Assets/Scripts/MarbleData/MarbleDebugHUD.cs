using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MarbleMovement))]
[DisallowMultipleComponent]
public class MarbleDebugHUD : MonoBehaviour
{
    [Tooltip("Offset above marble for label positioning")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.5f, 0f);
    [Tooltip("Text color for the debug label")]
    [SerializeField] private Color textColor = Color.white;
    [Tooltip("Font size for the debug label")]
    [SerializeField] private int fontSize = 14;
    [Tooltip("Optional font to use for world label; if null, uses OS font fallback")]
    [SerializeField] private Font hudFont;
    [Tooltip("Toggle to show/hide the IMGUI overlay")]
    [SerializeField] private bool showIMGUI = false;
    [Tooltip("Render a world-space label above the marble")]
    [SerializeField] private bool showWorldLabel = false;
    [Tooltip("If true, the world label faces the camera (billboard). If false, it keeps the marble's rotation.")]
    [SerializeField] private bool faceCamera = false;

    private MarbleMovement movement;
    private GUIStyle style;
    private Canvas worldCanvas;
    private Text worldLabel;

    private void Awake()
    {
        movement = GetComponent<MarbleMovement>();
        style = new GUIStyle
        {
            fontSize = fontSize,
            alignment = TextAnchor.MiddleCenter
        };

        // Clean up any stale canvases from previous runs
        foreach (Transform child in transform)
        {
            if (child.name == "DebugHUD_Canvas")
            {
                Destroy(child.gameObject);
            }
        }

        if (showWorldLabel)
        {
            // Reuse existing canvas if already present to avoid duplicates
            Transform existing = transform.Find("DebugHUD_Canvas");
            if (existing != null)
            {
                worldCanvas = existing.GetComponent<Canvas>();
                worldLabel = existing.GetComponentInChildren<Text>();
            }

            if (worldCanvas == null)
            {
                GameObject canvasGO = new GameObject("DebugHUD_Canvas");
                canvasGO.transform.SetParent(transform, false);
                worldCanvas = canvasGO.AddComponent<Canvas>();
                worldCanvas.renderMode = RenderMode.WorldSpace;
                worldCanvas.gameObject.layer = gameObject.layer;

                // Only render on the main camera to avoid duplication
                var graphicRaycaster = canvasGO.AddComponent<GraphicRaycaster>();
                
                var scaler = canvasGO.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10f;
                worldCanvas.transform.localScale = Vector3.one * 0.01f;
            }

            if (worldLabel == null)
            {
                GameObject textGO = new GameObject("Label");
                textGO.transform.SetParent(worldCanvas.transform, false);
                worldLabel = textGO.AddComponent<Text>();
                // Pick font: use assigned font if provided, else try OS fonts
                Font fontToUse = hudFont;
                if (fontToUse == null)
                {
                    string[] candidates = new string[] { "Segoe UI", "Arial", "Tahoma", "Verdana" };
                    fontToUse = Font.CreateDynamicFontFromOSFont(candidates, fontSize);
                }
                worldLabel.font = fontToUse;
                worldLabel.color = textColor;
                worldLabel.alignment = TextAnchor.MiddleCenter;
                worldLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
                worldLabel.verticalOverflow = VerticalWrapMode.Overflow;
                worldLabel.fontSize = fontSize;

                RectTransform rect = worldLabel.rectTransform;
                rect.sizeDelta = new Vector2(160f, 60f);
                rect.localPosition = Vector3.zero;
            }
        }
    }

    private void Update()
    {
        if (showWorldLabel && worldCanvas != null && worldLabel != null && movement != null)
        {
            Camera cam = Camera.main ?? FindObjectOfType<Camera>();
            Vector3 pos = transform.position + worldOffset;
            worldCanvas.transform.position = pos;
            if (faceCamera && cam != null)
            {
                // Face camera (billboard)
                worldCanvas.transform.rotation = Quaternion.LookRotation(worldCanvas.transform.position - cam.transform.position, Vector3.up);
            }
            worldLabel.text = BuildInfoString();
        }
    }

    private void OnGUI()
    {
        if (!showIMGUI || movement == null)
            return;

        Camera cam = Camera.main ?? FindObjectOfType<Camera>();
        if (cam == null)
            return;

        style.normal.textColor = textColor;

        Vector3 worldPos = transform.position + worldOffset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
        if (screenPos.z < 0f)
            return; // behind camera

        string info = BuildInfoString();

        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        Vector2 size = style.CalcSize(new GUIContent(info));
        Rect rect = new Rect(x - size.x / 2f, y - size.y / 2f, size.x, size.y);
        GUI.Label(rect, info, style);
    }

    private string BuildInfoString()
    {
        return $"{gameObject.name}\n" +
               $"effSpeed: {movement.EffSpeed:F2} m/s\n" +
               $"velocity: {movement.CurrentSpeed:F2} m/s\n" +
               $"effAccel: {movement.EffAcceleration:F2}\n" +
               $"cap: {(movement.EnforceSpeedCap ? "on" : "off")} massThrust: {(movement.ThrustUsesMass ? "on" : "off")}";
    }
}