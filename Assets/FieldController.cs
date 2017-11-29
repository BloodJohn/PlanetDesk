using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Требования к проекту:

//Обратите внимание на производительность, старайтесь минимизировать лаги.Реализуйте с расчетом на то, что проект может работать и на мобильных платформах.
// - какие лаги при 20 объектах?

//Стремитесь, чтобы архитектура и проект были расширяемыми для возможных дальнейших изменений. Подготовьте проект к тому, что в дальнейшем карта будет загружаться с сервера.
// - cellList - вполне сериализуем в json

//Приветствуется краткое описание того, что и как было сделано плюс что можно было бы улучшить в будущем при развитии проекта и на что, возможно, 
//не хватило времени (макс примерно 1-2 страницы).
// - заняло пару часов

//Арт необязателен, но приветствуется.
// - не буду =)




public class FieldController : MonoBehaviour
{
    public const int maxCost = 10000;
    public const int topCount = 20;

    [SerializeField] private PlanetController planetPrefab;
    [SerializeField] private Camera mainCamera;

    private readonly List<PlanetController> viewList = new List<PlanetController>(topCount);
    private readonly List<PlanetController> delList = new List<PlanetController>(topCount);
    private readonly List<PlanetData> cellList = new List<PlanetData>();
    /// <summary>не совсем честно, но зато работает быстрее - сортировать меньше!</summary>
    private readonly Dictionary<int, int> minCostByWidth = new Dictionary<int, int>();

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
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            fieldWidth = Mathf.Max(7, fieldWidth - 2);
        }
        else
        {
            return;
        }

        UpdatePlanetField();
    }

    /// <summary>возвращает все клетки в зоне видимости</summary>
    private IEnumerable<PlanetData> UpdateCellField(int minCost)
    {
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
                    //Ячейка заполняется либо ничем, либо планетой (планеты должны заполнять не менее 30% ячеек)
                    if (Random.value <= 0.3f) newData.cost = Random.Range(0, maxCost);
                    cellList.Add(newData);
                }

                if (newData.cost >= minCost ) yield return newData;
            }
    }

    private void UpdatePlanetField()
    {
        mainCamera.transform.localPosition = new Vector3(0f, 0f, -fieldWidth);

        //сносим все планеты с поля
        while (viewList.Count > 0)
        {
            var delItem = viewList[0];
            viewList.Remove(delItem);
            delItem.gameObject.SetActive(false);
            delList.Add(delItem);

        }

        var minCost = 0;
        minCostByWidth.TryGetValue(fieldWidth, out minCost);
        var cellList = UpdateCellField(minCost);
        //Начиная с N = 10 включается особый режим отображения объектов, при котором отображается только P = 20 планет с самым близким к кораблю рейтингом в видимой области
        if (fieldWidth > 10)
        {
            cellList = cellList
            .OrderByDescending(t => t.cost)
            .Take(topCount);
        }

        var newMinCost = maxCost;

        //создаем планеты
        foreach (var cellData in cellList)
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
            newPlanet.Init(cellData, fieldPos);

            viewList.Add(newPlanet);

            if (cellData.cost < newMinCost) newMinCost = cellData.cost;
        }

        if (!minCostByWidth.ContainsKey(fieldWidth))
            minCostByWidth.Add(fieldWidth, newMinCost);
    }
}
