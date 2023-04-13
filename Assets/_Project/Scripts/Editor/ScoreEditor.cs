using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Score), true)]
public class ScoreEditor : Editor
{
    static float reduce = 5;
    static float maxValue = 50;
    static float minValue = 50;
    static float gain = 5;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Score score = target as Score;

        EditorGUILayout.LabelField("QA Testing");
        reduce = EditorGUILayout.FloatField("Reduce " + score.ScoreDisplayName, reduce);

        if (GUILayout.Button("Lose " + reduce + " " + score.ScoreDisplayName))
        {
            score.ReduceScore(reduce);
        }

        GUILayout.Space(10);

        gain = EditorGUILayout.FloatField("Gain " + score.ScoreDisplayName, gain);

        if (GUILayout.Button("Gain " + gain + " " + score.ScoreDisplayName))
        {
            score.AddScore(gain);
        }

        GUILayout.Space(10);

        maxValue = EditorGUILayout.FloatField("New max " + score.ScoreDisplayName, maxValue);

        if (GUILayout.Button("Change max " + score.ScoreDisplayName + " to " + maxValue))
        {
            score.ChangeMaxValue(maxValue, true);
        }

        minValue = EditorGUILayout.FloatField("New min " + score.ScoreDisplayName, maxValue);

        if (GUILayout.Button("Change min" + score.ScoreDisplayName + " to " + minValue))
        {
            score.ChangeMinValue(minValue, true);
        }
    }
}