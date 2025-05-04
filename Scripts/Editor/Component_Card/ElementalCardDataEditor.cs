using Core.Utils;
using Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ElementalCardDataSO))]
    public class ElementalCardDataEditor : UnityEditor.Editor
    {
        // Serialized properties
        private SerializedProperty cardId;
        private SerializedProperty cardKeyName;
        private SerializedProperty cardName;
        private SerializedProperty description;
        private SerializedProperty cardType;
        private SerializedProperty rarity;
        private SerializedProperty cost;
        private SerializedProperty artwork;
    
        // Stats
        private SerializedProperty attack;
        private SerializedProperty defense;
        private SerializedProperty health;
        private SerializedProperty speed;
    
        // Element properties
        private SerializedProperty elementType;
        private SerializedProperty napAmIndex;
        
        // Foldout states
        private bool showCardInfo = true;
        private bool showElementInfo = true;
        private bool showStats = true;
    
        private void OnEnable()
        {
            // Find all serialized properties
            cardId = serializedObject.FindProperty("cardId");
            cardKeyName = serializedObject.FindProperty("cardKeyName");
            cardName = serializedObject.FindProperty("cardName");
            description = serializedObject.FindProperty("description");
            cardType = serializedObject.FindProperty("cardType");
            rarity = serializedObject.FindProperty("rarity");
            cost = serializedObject.FindProperty("cost");
            artwork = serializedObject.FindProperty("artwork");
        
            attack = serializedObject.FindProperty("attack");
            defense = serializedObject.FindProperty("defense");
            health = serializedObject.FindProperty("health");
            speed = serializedObject.FindProperty("speed");
        
            // Find element properties
            elementType = serializedObject.FindProperty("elementType");
            napAmIndex = serializedObject.FindProperty("napAmIndex");
        }
    
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            // Card Info Section
            DrawCardInfoSection();
        
            // Element Info Section
            DrawElementInfoSection();
        
            // Stats Section
            DrawStatsSection();
        
            serializedObject.ApplyModifiedProperties();
        }
    
        private void DrawCardInfoSection()
        {
            EditorGUILayout.Space(10);
            showCardInfo = EditorGUILayout.Foldout(showCardInfo, "Card Information", true, EditorStyles.foldoutHeader);
        
            if (showCardInfo)
            {
                EditorGUI.indentLevel++;
            
                EditorGUILayout.PropertyField(cardId);
                EditorGUILayout.PropertyField(cardKeyName);
                EditorGUILayout.PropertyField(cardName);
            
                EditorGUILayout.PropertyField(description, GUILayout.Height(60));
            
                EditorGUILayout.PropertyField(cardType);
                EditorGUILayout.PropertyField(rarity);
                EditorGUILayout.PropertyField(cost);
                EditorGUILayout.PropertyField(artwork);
            
                EditorGUI.indentLevel--;
            }
        }
    
        private void DrawElementInfoSection()
        {
            EditorGUILayout.Space(10);
            showElementInfo = EditorGUILayout.Foldout(showElementInfo, "Element & Nạp Âm", true, EditorStyles.foldoutHeader);
        
            if (showElementInfo)
            {
                EditorGUI.indentLevel++;
            
                // Element Type dropdown
                EditorGUILayout.PropertyField(elementType, new GUIContent("Element Type"));
            
                // Nạp Âm index simple slider
                if (elementType.enumValueIndex != (int)ElementType.None)
                {
                    EditorGUILayout.IntSlider(napAmIndex, 1, 6, new GUIContent("Nạp Âm Index"));
                }
            
                EditorGUI.indentLevel--;
            }
        }
    
        private void DrawStatsSection()
        {
            EditorGUILayout.Space(10);
            showStats = EditorGUILayout.Foldout(showStats, "Card Stats", true, EditorStyles.foldoutHeader);
        
            if (showStats)
            {
                EditorGUI.indentLevel++;
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Attack", GUILayout.Width(60));
                EditorGUILayout.PropertyField(attack, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Defense", GUILayout.Width(60));
                EditorGUILayout.PropertyField(defense, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Health", GUILayout.Width(60));
                EditorGUILayout.PropertyField(health, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Speed", GUILayout.Width(60));
                EditorGUILayout.PropertyField(speed, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            
                EditorGUI.indentLevel--;
            }
        }
    }
}