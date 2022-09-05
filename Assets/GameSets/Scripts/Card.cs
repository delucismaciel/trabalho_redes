using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card/Card", order = 1)]
public class Card : ScriptableObject
{
    public Sprite texture;
    public int id;
    public Color color;
}
