using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour, ICharacterClass
{
    public ClassType CharType
    {
        get
        {
            return ClassType.Knight;
        }
    }
}
