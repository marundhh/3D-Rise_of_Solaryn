using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


    public interface ICharacterClass
    {
        ClassType CharType { get; }
    }
    public enum ClassType
    {
        Knight,
        Wizard, //Sửa lại chính tả cho đúng
        Archer
    }

