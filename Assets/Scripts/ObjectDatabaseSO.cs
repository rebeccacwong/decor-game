using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public enum ItemCategory
{
    DiningChairs, DiningTables, Beds, Sofas
}

[CreateAssetMenu]
public class ObjectDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;
}

[Serializable]
public class ObjectData
{
    [field:SerializeField]
    public string Name { get; private set; }

    [field:SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public int SwatchID { get; private set; } = 0;

    [field: SerializeField]
    public Color SwatchColor1 { get; private set; }

    [field: SerializeField]
    public Color SwatchColor2 { get; private set; }

    [field:SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field:SerializeField]
    public GameObject Prefab { get; private set; }

    [field:SerializeField]
    public Sprite Image { get; private set; }

    [field:SerializeField]
    public ItemCategory Category { get; private set; }
}