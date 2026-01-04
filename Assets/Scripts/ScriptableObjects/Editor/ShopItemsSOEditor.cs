using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShopItemsSO))]
public class ShopItemsSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShopItemsSO shopItem = (ShopItemsSO)target;

        // Draw the default inspector for all fields except saleDiscountPercentage
        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;
        
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;
            
            // Skip the script field and saleDiscountPercentage (we'll handle it conditionally)
            if (prop.name == "m_Script" || prop.name == "saleDiscountPercentage")
                continue;
                
            EditorGUILayout.PropertyField(prop, true);
        }

        // Only show saleDiscountPercentage if isOnSale is true
        if (shopItem.isOnSale)
        {
            SerializedProperty discountProp = serializedObject.FindProperty("saleDiscountPercentage");
            EditorGUILayout.PropertyField(discountProp, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
