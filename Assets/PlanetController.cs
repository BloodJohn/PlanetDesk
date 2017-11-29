using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public PlanetData data;

    public void Init(PlanetData newData, Vector2Int shift)
    {
        gameObject.SetActive(true);
        data = newData;

        transform.localPosition = new Vector3(data.pos.x - shift.x, data.pos.y - shift.y, 0);
        name = string.Format("planet {0},{1}", data.pos.x, data.pos.y);
    }
}

/// <summary>Каждой планете случайно присваивается “рейтинг” от 0 до 10 000, который отображается числом над планетой.</summary>
public class PlanetData
{
    public Vector2Int pos;
    public int cost;
}