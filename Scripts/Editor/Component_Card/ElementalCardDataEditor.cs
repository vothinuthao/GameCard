// File: Scripts/Editor/ElementalCardDataEditor.cs

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
    
        // Element properties - renamed to avoid conflict
        private SerializedProperty cardElementType; // Changed from elementType
        private SerializedProperty metalNapAm;
        private SerializedProperty woodNapAm;
        private SerializedProperty waterNapAm;
        private SerializedProperty fireNapAm;
        private SerializedProperty earthNapAm;
    
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
        
            // Find the correct property name for elementType
            cardElementType = serializedObject.FindProperty("elementType");
            metalNapAm = serializedObject.FindProperty("metalNapAm");
            woodNapAm = serializedObject.FindProperty("woodNapAm");
            waterNapAm = serializedObject.FindProperty("waterNapAm");
            fireNapAm = serializedObject.FindProperty("fireNapAm");
            earthNapAm = serializedObject.FindProperty("earthNapAm");
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
                EditorGUILayout.PropertyField(cardElementType, new GUIContent("Element Type"));
            
                // Show appropriate Nạp Âm based on selected element
                EditorGUILayout.Space(5);
                GUIStyle boldStyle = new GUIStyle(EditorStyles.boldLabel);
                GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
                boxStyle.padding = new RectOffset(10, 10, 10, 10);
            
                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.LabelField("Nạp Âm Configuration", boldStyle);
            
                ElementType currentElement = (ElementType)cardElementType.enumValueIndex;
                switch (currentElement)
                {
                    case ElementType.Metal:
                        EditorGUILayout.PropertyField(metalNapAm, new GUIContent("Kim Nạp Âm"));
                        EditorGUILayout.HelpBox(GetNapAmDescription(ElementType.Metal, metalNapAm.enumValueIndex), MessageType.Info);
                        break;
                
                    case ElementType.Wood:
                        EditorGUILayout.PropertyField(woodNapAm, new GUIContent("Mộc Nạp Âm"));
                        EditorGUILayout.HelpBox(GetNapAmDescription(ElementType.Wood, woodNapAm.enumValueIndex), MessageType.Info);
                        break;
                
                    case ElementType.Water:
                        EditorGUILayout.PropertyField(waterNapAm, new GUIContent("Thủy Nạp Âm"));
                        EditorGUILayout.HelpBox(GetNapAmDescription(ElementType.Water, waterNapAm.enumValueIndex), MessageType.Info);
                        break;
                
                    case ElementType.Fire:
                        EditorGUILayout.PropertyField(fireNapAm, new GUIContent("Hỏa Nạp Âm"));
                        EditorGUILayout.HelpBox(GetNapAmDescription(ElementType.Fire, fireNapAm.enumValueIndex), MessageType.Info);
                        break;
                
                    case ElementType.Earth:
                        EditorGUILayout.PropertyField(earthNapAm, new GUIContent("Thổ Nạp Âm"));
                        EditorGUILayout.HelpBox(GetNapAmDescription(ElementType.Earth, earthNapAm.enumValueIndex), MessageType.Info);
                        break;
                
                    case ElementType.None:
                    default:
                        EditorGUILayout.HelpBox("Select an element type to configure Nạp Âm", MessageType.Warning);
                        break;
                }
            
                EditorGUILayout.EndVertical();
            
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
    
        private string GetNapAmDescription(ElementType element, int napAmIndex)
        {
            // Description implementation remains the same
            switch (element)
            {
                case ElementType.Metal:
                    switch ((MetalNapAm)napAmIndex)
                    {
                        case MetalNapAm.SwordQi: 
                            return "Kiếm Khí: Tăng sức mạnh đâm xuyên, gây thêm sát thương";
                        case MetalNapAm.Hardness: 
                            return "Cương Nghị: Tăng phòng thủ và khả năng chống chịu";
                        case MetalNapAm.Purity: 
                            return "Thanh Tịnh: Loại bỏ hiệu ứng tiêu cực, tăng độ tinh khiết";
                        case MetalNapAm.Reflection: 
                            return "Phản Chiếu: Phản lại một phần sát thương về phía đối thủ";
                        case MetalNapAm.Spirit: 
                            return "Linh Khí: Tăng khả năng phản xạ và kháng hiệu ứng";
                        case MetalNapAm.Calmness: 
                            return "Trầm Tĩnh: Giảm mức độ hiệu ứng tiêu cực, tăng phòng thủ";
                        default: 
                            return "Không xác định";
                    }
                
                case ElementType.Wood:
                    switch ((WoodNapAm)napAmIndex)
                    {
                        case WoodNapAm.Growth: 
                            return "Sinh Trưởng: Hồi phục máu và tăng trưởng sức mạnh";
                        case WoodNapAm.Flexibility: 
                            return "Linh Hoạt: Tăng khả năng né tránh và linh hoạt";
                        case WoodNapAm.Symbiosis: 
                            return "Cộng Sinh: Cung cấp hiệu ứng hỗ trợ cho đồng minh";
                        case WoodNapAm.Regeneration: 
                            return "Tái Sinh: Hồi phục mạnh mẽ theo thời gian";
                        case WoodNapAm.Toxin: 
                            return "Độc Tố: Gây sát thương độc theo thời gian";
                        case WoodNapAm.Shelter: 
                            return "Che Chắn: Tạo lá chắn bảo vệ trước sát thương";
                        default: 
                            return "Không xác định";
                    }
                
                case ElementType.Water:
                    switch ((WaterNapAm)napAmIndex)
                    {
                        case WaterNapAm.Adaptation: 
                            return "Thích Nghi: Thay đổi để thích ứng với điều kiện";
                        case WaterNapAm.Ice: 
                            return "Băng Giá: Làm chậm đối thủ và gây sát thương băng";
                        case WaterNapAm.Flow: 
                            return "Dòng Chảy: Tăng tính cơ động và tốc độ";
                        case WaterNapAm.Mist: 
                            return "Sương Mù: Giảm độ chính xác của đối thủ, tăng né tránh";
                        case WaterNapAm.Reflection: 
                            return "Phản Ánh: Phản chiếu và bẻ cong các hiệu ứng";
                        case WaterNapAm.Purification: 
                            return "Thanh Tẩy: Xóa bỏ hiệu ứng tiêu cực và hồi phục";
                        default: 
                            return "Không xác định";
                    }
                
                case ElementType.Fire:
                    switch ((FireNapAm)napAmIndex)
                    {
                        case FireNapAm.Burning: 
                            return "Thiêu Đốt: Gây sát thương cháy liên tục";
                        case FireNapAm.Explosion: 
                            return "Bùng Nổ: Gây sát thương cao ngay lập tức";
                        case FireNapAm.Passion: 
                            return "Nhiệt Huyết: Tăng sức mạnh tấn công và tốc độ";
                        case FireNapAm.Light: 
                            return "Ánh Sáng: Phơi bày điểm yếu và loại bỏ ẩn thân";
                        case FireNapAm.Forging: 
                            return "Rèn Luyện: Cường hóa sức mạnh theo thời gian";
                        case FireNapAm.Incineration: 
                            return "Thiêu Rụi: Gây sát thương hủy diệt cực cao";
                        default: 
                            return "Không xác định";
                    }
                
                case ElementType.Earth:
                    switch ((EarthNapAm)napAmIndex)
                    {
                        case EarthNapAm.Solidity: 
                            return "Kiên Cố: Tăng cường phòng thủ và độ bền";
                        case EarthNapAm.Gravity: 
                            return "Trọng Lực: Kéo chậm và hạn chế di chuyển của đối thủ";
                        case EarthNapAm.Fertility: 
                            return "Màu Mỡ: Tăng hiệu quả của các hiệu ứng hồi phục";
                        case EarthNapAm.Volcano: 
                            return "Núi Lửa: Gây sát thương diện rộng mạnh mẽ";
                        case EarthNapAm.Crystal: 
                            return "Tinh Thể: Tăng cường và khuếch đại sức mạnh";
                        case EarthNapAm.Terra: 
                            return "Đại Địa: Cung cấp khả năng kiên cố và kiểm soát";
                        default: 
                            return "Không xác định";
                    }
                
                default:
                    return "Chọn loại nguyên tố để xem thông tin Nạp Âm";
            }
        }
    }
}