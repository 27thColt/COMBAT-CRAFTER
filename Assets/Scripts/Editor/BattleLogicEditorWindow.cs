using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static BattleLogic;

public class BattleLogicEditorWindow : EditorWindow {
    
    private EnemyType _checkVulEnemyType;
    private ItemType _checkVulItemType;
    private string _cvOutput = null;


    private int _dcAtk = 0;
    private List<int> _dcOutputNE = new List<int> {0, 0};
    private List<int> _dcOutputSE = new List<int> {0, 0};
    private System.Random _rnd = new System.Random();

    [MenuItem("Window/Battle Logic")]
    static void Open() {
        BattleLogicEditorWindow _window = (BattleLogicEditorWindow)EditorWindow.GetWindow(typeof(BattleLogicEditorWindow));
        _window.Show();
    }

    private void OnGUI() {
        
        #region Check Vulnerabilities
        EditorGUILayout.LabelField("Check Vulnerabilities", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Attacking Item", GUILayout.MaxWidth(100));
        _checkVulItemType = (ItemType)EditorGUILayout.ObjectField(_checkVulItemType, typeof(ItemType), false, GUILayout.MaxWidth(150));

        EditorGUILayout.LabelField("Defending Enemy", GUILayout.MaxWidth(100));
        _checkVulEnemyType = (EnemyType)EditorGUILayout.ObjectField(_checkVulEnemyType, typeof(EnemyType), false, GUILayout.MaxWidth(150));

        EditorGUILayout.EndVertical();

        if (_checkVulEnemyType != null && _checkVulItemType != null) {
            if (CheckVulnerabilities(_checkVulEnemyType, _checkVulItemType)) {
                _cvOutput = "Super Effective!";
            } else {
                _cvOutput = "Not Very Effective...";
            }

            EditorGUILayout.LabelField(_cvOutput);
        }

        EditorGUILayout.EndHorizontal();
        
        
        #endregion

        #region Damage Calculator

        EditorGUILayout.LabelField("Damage Calculator", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        _dcAtk = EditorGUILayout.IntField("Attack", _dcAtk);

        _dcOutputNE = CalculateMinMaxDamage(_dcAtk, neModifier);
        _dcOutputSE = CalculateMinMaxDamage(_dcAtk, seModifier);
        

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("NE Output: [" +  _dcOutputNE[0] + ", " + CalculateAttackDamage(_dcAtk, neModifier) + ", " + _dcOutputNE[1] + "]");
        EditorGUILayout.LabelField("SE Output: [" +  _dcOutputSE[0] + ", " + CalculateAttackDamage(_dcAtk, seModifier) + ", " + _dcOutputSE[1] + "]");

        EditorGUILayout.EndHorizontal();

        #endregion
        
    }
}
