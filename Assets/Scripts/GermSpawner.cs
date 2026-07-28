using System.Collections.Generic;
using UnityEngine;

// 外した時に増える「予備の菌」をまとめて管理する。
// あらかじめシーンに用意して非アクティブにしておいたGermを、外すたびにランダムで1つ有効化する。
public class GermSpawner : MonoBehaviour
{
    [SerializeField] private List<Germ> reserveGerms = new List<Germ>();
    [SerializeField] private ClearManager clearManager;
    [SerializeField] private PlusOnePopup plusOnePopup;

    public void SpawnOne()
    {
        reserveGerms.RemoveAll(g => g == null);

        if (reserveGerms.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, reserveGerms.Count);
        Germ germ = reserveGerms[index];
        reserveGerms.RemoveAt(index);

        germ.Activate();

        if (clearManager != null)
        {
            clearManager.AddGerm();
        }

        if (plusOnePopup != null)
        {
            plusOnePopup.Show();
        }
    }
}
