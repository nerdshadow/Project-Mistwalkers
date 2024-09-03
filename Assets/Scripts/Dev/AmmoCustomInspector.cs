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
        EditorGUILayout.Space(10);
        //ammoStatsBuffer.ammoType = (AmmoStats.AmmoType)EditorGUILayout.EnumPopup("CurrentAmmoType", ammoStatsBuffer.ammoType);
        switch (ammoStatsBuffer.ammoType)
        {
            case AmmoType.Bullet:
                {
                    ammoStatsBuffer.bulletTrailVfx_Prefab = (GameObject)EditorGUILayout.ObjectField("Bullet TrailVFX",ammoStatsBuffer.bulletTrailVfx_Prefab, typeof(GameObject), true);
                    ammoStatsBuffer.trailLifetime = EditorGUILayout.FloatField("Trail Life Time", ammoStatsBuffer.trailLifetime);
                    break;
                }
            case AmmoType.CannonShell:
                {
                    ammoStatsBuffer.shellPrefab = (GameObject)EditorGUILayout.ObjectField("Shell Prefab", ammoStatsBuffer.shellPrefab, typeof(GameObject), true);
                    ammoStatsBuffer.shellSpeed = EditorGUILayout.FloatField("Shell speed", ammoStatsBuffer.shellSpeed);
                    ammoStatsBuffer.shellLifeTime= EditorGUILayout.FloatField("Shell lifetime", ammoStatsBuffer.shellLifeTime);
                    break;
                }
            case AmmoType.LaserBatteries:
                {
                    ammoStatsBuffer.laserTrailVfx_Prefab = (GameObject)EditorGUILayout.ObjectField("Laser TrailVFX", ammoStatsBuffer.laserTrailVfx_Prefab, typeof(GameObject), true);
                    ammoStatsBuffer.trailLifetime = EditorGUILayout.FloatField("Trail Life Time", ammoStatsBuffer.trailLifetime);
                    break;
                }
        }
    }
}
