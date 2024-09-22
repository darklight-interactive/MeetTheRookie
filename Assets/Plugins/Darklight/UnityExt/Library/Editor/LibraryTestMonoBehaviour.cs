

using Darklight.UnityExt.Library;
using UnityEngine;

public class LibraryTestMonoBehaviour : MonoBehaviour
{
    public enum UniverseTestEnum { SUN, MERCUY, VENUS, EARTH, MARS, JUPITER, SATURN, URANUS, NEPTUNE, PLUTO }
    public EnumObjectLibrary<UniverseTestEnum, Sprite> universeLibrary
        = new EnumObjectLibrary<UniverseTestEnum, Sprite>(true);
}