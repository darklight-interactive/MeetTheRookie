

using Darklight.UnityExt.Library;
using UnityEngine;

public class LibraryTestMonoBehaviour : MonoBehaviour
{
    public enum UniverseTestEnum { SUN, MERCUY, VENUS, EARTH, MARS, JUPITER, SATURN, URANUS, NEPTUNE, PLUTO }
    public EnumObjectLibrary<UniverseTestEnum, Sprite> universeLibrary
        = new EnumObjectLibrary<UniverseTestEnum, Sprite>
        {
            ReadOnlyKey = true,
            ReadOnlyValue = true,
            RequiredKeys = new UniverseTestEnum[]
            {
                UniverseTestEnum.SUN,
                UniverseTestEnum.MERCUY,
                UniverseTestEnum.VENUS,
                UniverseTestEnum.EARTH,
                UniverseTestEnum.MARS,
                UniverseTestEnum.JUPITER,
                UniverseTestEnum.SATURN,
                UniverseTestEnum.URANUS,
                UniverseTestEnum.NEPTUNE,
                UniverseTestEnum.PLUTO
            }
        };
}