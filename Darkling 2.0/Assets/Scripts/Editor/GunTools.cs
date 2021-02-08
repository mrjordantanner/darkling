using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

[ExecuteInEditMode]
[CustomEditor(typeof(Gun))]
public class GunTools : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Gun gun = (Gun)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Hip"))
        {
            gun.transform.localPosition = gun.pose.hipPosition;
            gun.transform.localEulerAngles = gun.pose.hipRotation;
        }
        if (GUILayout.Button("Set As New"))
        {
            gun.pose.hipPosition = gun.transform.localPosition;
            gun.pose.hipRotation = gun.transform.localEulerAngles;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Aim"))
        {
            gun.transform.localPosition = gun.pose.aimPosition;
            gun.transform.localEulerAngles = gun.pose.aimRotation;
        }
        if (GUILayout.Button("Set As New"))
        {
            gun.pose.aimPosition = gun.transform.localPosition;
            gun.pose.aimRotation = gun.transform.localEulerAngles;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Run"))
        {
            gun.transform.localPosition = gun.pose.runPosition;
            gun.transform.localEulerAngles = gun.pose.runRotation;
        }
        if (GUILayout.Button("Set As New"))
        {
            gun.pose.runPosition = gun.transform.localPosition;
            gun.pose.runRotation = gun.transform.localEulerAngles;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Jump"))
        {
            gun.transform.localPosition = gun.pose.jumpPosition;
            gun.transform.localEulerAngles = gun.pose.jumpRotation;
        }
        if (GUILayout.Button("Set As New"))
        {
            gun.pose.jumpPosition = gun.transform.localPosition;
            gun.pose.jumpRotation = gun.transform.localEulerAngles;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reload"))
        {
            gun.transform.localPosition = gun.pose.reloadPosition;
            gun.transform.localEulerAngles = gun.pose.reloadRotation;
        }
        if (GUILayout.Button("Set As New"))
        {
            gun.pose.reloadPosition = gun.transform.localPosition;
            gun.pose.reloadRotation = gun.transform.localEulerAngles;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Recoil"))
        {
            gun.transform.localPosition = gun.pose.recoilPosition;
            gun.transform.localEulerAngles = gun.pose.recoilRotation;
        }
        if (GUILayout.Button("Set As New"))
        {
            gun.pose.recoilPosition = gun.transform.localPosition;
            gun.pose.recoilRotation = gun.transform.localEulerAngles;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Aim Recoil"))
        {
            gun.transform.localPosition = gun.pose.aimRecoilPosition;
            gun.transform.localEulerAngles = gun.pose.aimRecoilRotation;
        }
        if (GUILayout.Button("Set As New"))
        {
            gun.pose.aimRecoilPosition = gun.transform.localPosition;
            gun.pose.aimRecoilRotation = gun.transform.localEulerAngles;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Landing"))
        {
            gun.transform.localPosition = gun.pose.landingPosition;
            gun.transform.localEulerAngles = gun.pose.landingRotation;
        }
        if (GUILayout.Button("Set As New"))
        {
            gun.pose.landingPosition = gun.transform.localPosition;
            gun.pose.landingRotation = gun.transform.localEulerAngles;
        }
        GUILayout.EndHorizontal();
        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Calc Fire Rate"))
        //{
        //    gun.CalculateShotDelay();
        //}
        //GUILayout.EndHorizontal();

    }
}
