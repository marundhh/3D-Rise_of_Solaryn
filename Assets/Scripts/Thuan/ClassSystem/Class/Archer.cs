using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour, ICharacterClass
{
    public ClassType CharType
    {
        get
        {
            return ClassType.Archer;
        }
    }
}
