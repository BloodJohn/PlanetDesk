using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public const int maxCost = 10000;
    public const int topCount = 20;

    [SerializeField] private PlanetController planetPrefab;

    private readonly List<PlanetController> viewList = new List<PlanetController>(topCount);
    private readonly List<PlanetController> delList = new List<PlanetController>(topCount);
    private readonly List<PlanetData> cellList = new List<PlanetData>();

    private int fieldWidth = 7;
    private Vector2Int fieldPos = Vector2Int.zero;


    private void Start()
    {
        fieldPos = new Vector2Int(0, 0);

        UpdatePlanetField();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            fieldPos.y++;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            fieldPos.y--;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            fieldPos.x++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            fieldPos.x--;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            fieldWidth = Mathf.Min(maxCost, fieldWidth + 2);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            fieldWidth = Mathf.Max(7, fieldWidth - 2);
        }

        else
        {
            return;
        }




        UpdatePlanetField();
    }

    private void UpdatePlanetField()
    {
        //сносим все планеты с поля
        while (viewList.Count > 0)
        {
            var delItem = viewList[0];
            viewList.Remove(delItem);
            delItem.gameObject.SetActive(false);
            delList.Add(delItem);

        }


        for (int x = -fieldWidth / 2; x <= fieldWidth / 2; x++)
            for (int y = -fieldWidth / 2; y < fieldWidth / 2; y++)
            {
                var newPos = new Vector2Int(x + fieldPos.x, y + fieldPos.y);
                var newData = cellList.FirstOrDefault(t => t.pos.x == newPos.x && t.pos.y == newPos.y);
                //если это новая клетка
                if (newData == null)
                {
                    newData = new PlanetData
                    {
                        cost = -1,
                        pos = newPos
                    };
                    if (Random.value <= 0.3f) newData.cost = Random.Range(0, maxCost);
                    cellList.Add(newData);
                }

                //создаем планету
                if (newData.cost >= 0)
                {
                    PlanetController newPlanet;
                    if (delList.Count > 0)
                    {
                        newPlanet = delList[0];
                        delList.Remove(newPlanet);
                    }
                    else
                    {
                        newPlanet = (PlanetController)Instantiate(planetPrefab, transform);

                    }
                    newPlanet.Init(newData, fieldPos);

                    viewList.Add(newPlanet);
                }
            }
    }
}
