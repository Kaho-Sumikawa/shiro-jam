using System.Collections.Generic;
using UnityEngine;

// バイ菌の位置データを一元管理する。
// モニター城の子として見た目（球）を出す一方、実物の城には見た目を出さず、
// 命中判定は「実物の城でのヒット位置」をローカル座標に変換してこのデータと距離比較するだけで行う。
// モニター城と実物の城は同じプレハブ（同スケール・同ピボット）である前提。
public class GermField : MonoBehaviour
{
    [SerializeField] private Transform monitorCastle;
    [SerializeField] private Transform realCastle;
    [SerializeField] private GameObject germPrefab;
    [SerializeField] private List<Vector3> germLocalPositions = new List<Vector3>();
    [SerializeField] private float hitRadius = 0.1f;

    private readonly List<GameObject> monitorInstances = new List<GameObject>();

    private void Start()
    {
        foreach (Vector3 localPosition in germLocalPositions)
        {
            GameObject instance = Instantiate(germPrefab);
            instance.transform.SetParent(monitorCastle, false);
            instance.transform.localPosition = localPosition;
            monitorInstances.Add(instance);
        }
    }

    public bool TryHit(Vector3 worldPoint)
    {
        Vector3 localPoint = realCastle.InverseTransformPoint(worldPoint);

        for (int i = 0; i < germLocalPositions.Count; i++)
        {
            if (Vector3.Distance(localPoint, germLocalPositions[i]) <= hitRadius)
            {
                Destroy(monitorInstances[i]);
                germLocalPositions.RemoveAt(i);
                monitorInstances.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
