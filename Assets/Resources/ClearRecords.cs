using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageLists
{
    public List<Stage> Stages;
}
[System.Serializable]
public class Stage
{
    public string id;
    public float clearTime;
}
