using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(AmmoStats))]
public class AmmoCustomInspector : Editor
{
    [HideInInspector]
    AmmoStats ammoStatsBuffer;
    void OnEnable()
    {
        ammoStatsBuffer = (AmmoStats)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //ammoStatsBuffer.ammoType = (AmmoStats.AmmoType)EditorGUILayout.EnumPopup("CurrentAmmoType", ammoStatsBuffer.ammoType);
        switch (ammoStatsBuffer.ammoType)
        {
            case AmmoStats.AmmoType.Bullet:
                {
                    ammoStatsBuffer.bString = EditorGUILayout.TextField("Name", ammoStatsBuffer.bString);
                    //mTest.objName = EditorGUILayout.TextField("Name", mTest.objName);
                    //mTest.addon = (GameObject)EditorGUILayout.ObjectField("Addon", mTest.addon, typeof(GameObject), true);
                    break;
                }
            case AmmoStats.AmmoType.CannonShell:
                {
                    ammoStatsBuffer.sString = EditorGUILayout.TextField("Name", ammoStatsBuffer.sString);
                    //mTest.objName = EditorGUILayout.TextField("Name", mTest.objName);
                    //mTest.addon = (GameObject)EditorGUILayout.ObjectField("Addon", mTest.addon, typeof(GameObject), true);
                    break;
                }
            case AmmoStats.AmmoType.LaserBatteries:
                {
                    ammoStatsBuffer.lString = EditorGUILayout.TextField("Name", ammoStatsBuffer.lString);
                    //mTest.objName = EditorGUILayout.TextField("Name", mTest.objName);
                    //mTest.zType = (ZBehaviour)EditorGUILayout.EnumPopup("ZType", mTest.zType);
                    break;
                }
        }
    }
}
