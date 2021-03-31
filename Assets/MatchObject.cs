using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum matchType
{
    empty = -1,
    type1,
    type2,
    type3,
    type4,
    type5,
    bomb,
    block,
    COUNT,
}

public class MatchObject : MonoBehaviour
{
    public Sprite matchIcon;
    public matchType tileMatchType;
}
