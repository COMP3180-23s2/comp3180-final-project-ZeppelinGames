using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct BenchLevel {
    public string BenchLevelName;
    public GameObject[] switchOn;
}

public class BenchLevels : MonoBehaviour
{
    [SerializeField] private BenchLevel[] benches;
    [SerializeField] private TextMeshProUGUI text;

    private int benchIndex = 0;

    private void Start()
    {
        for (int i = 0; i < benches.Length; i++)
        {
            for (int j = 0; j < benches[i].switchOn.Length; j++)
            {
                benches[i].switchOn[j].SetActive(i == benchIndex);
            }
        }

        text.text = benches[benchIndex].BenchLevelName;
    }

    public void NextBench()
    {
        SetState(false);

        benchIndex++;
        benchIndex = Mathf.Clamp(benchIndex, 0, benches.Length);

        SetState(true);
    }

    public void PrevBench()
    {
        SetState(false);

        benchIndex--;
        benchIndex = Mathf.Clamp(benchIndex, 0, benches.Length);
        
        SetState(true);
    }

    void SetState(bool state)
    {
        GameObject[] objs = benches[benchIndex].switchOn;
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].SetActive(state);
        }

        if (state)
        {
            text.text = benches[benchIndex].BenchLevelName;
        }
    }
}
