/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	08.09.2021
 * Version	: 	4.1.2
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graphes : MonoBehaviour
{
    [SerializeField] private Sprite dotSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;
    private List<IGraphVisualObject> graphVisualObjectList;
    private List<RectTransform> yLabelList;

    // Cached values
    public List<int> valueList;
    private IGraphVisual graphVisual;
    private int maxVisibleValueAmount;
    private Func<int, string> getAxisLabelX;
    private Func<float, string> getAxisLabelY;
    private float xSize;
    private bool startYScaleAtZero;

    private void Awake()
    {
        graphContainer = transform.Find("Panel/GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();

        startYScaleAtZero = true;
        gameObjectList = new List<GameObject>();
        graphVisualObjectList = new List<IGraphVisualObject>();
        yLabelList = new List<RectTransform>();

        valueList = new List<int>();

        IGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1, 1, 1, .5f));
        IGraphVisual barGraphVisual = new BarChartVisual(graphContainer, Color.white, .8f);

        transform.Find("LineGraphButton").GetComponent<Button>().onClick.AddListener(btnLinef);
        transform.Find("BarChartButton").GetComponent<Button>().onClick.AddListener(btnBarf);
    }

    public void updateGraph(List<int> data)
    {
        valueList = data;
        IGraphVisual barGraphVisual = new BarChartVisual(graphContainer, Color.white, .8f);
        showGraph(valueList, barGraphVisual, -1, (int _i) => "H" + (_i + 1), (float _f) => "" + Mathf.RoundToInt(_f));
    }

    private void btnLinef()
    {
        IGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1, 1, 1, .5f));
        setGraphVisual(lineGraphVisual);
    }
    private void btnBarf()
    {
        IGraphVisual barGraphVisual = new BarChartVisual(graphContainer, Color.white, .8f);
        setGraphVisual(barGraphVisual);
    }
    private void setGraphVisual(IGraphVisual graphVisual)
    {
        showGraph(this.valueList, graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, this.getAxisLabelY);
    }
    private void showGraph(List<int> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        this.valueList = valueList;
        this.graphVisual = graphVisual;
        this.getAxisLabelX = getAxisLabelX;
        this.getAxisLabelY = getAxisLabelY;

        // Test for label defaults
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }

        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        if (maxVisibleValueAmount <= 0)
        {
            // Show all if no amount specified
            maxVisibleValueAmount = valueList.Count;
        }

        if (maxVisibleValueAmount > valueList.Count)
        {
            // Validate the amount to show the maximum
            maxVisibleValueAmount = valueList.Count;
        }

        this.maxVisibleValueAmount = maxVisibleValueAmount;

        // Clean up previous graph
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
        foreach (IGraphVisualObject graphVisualObject in graphVisualObjectList)
        {
            graphVisualObject.cleanUp();
        }
        graphVisualObjectList.Clear();
        yLabelList.Clear();

        // Grab the width and height from the container
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMinimum;
        float yMaximum;

        calculateYScale(out yMinimum, out yMaximum);

        // Set the distance between each point on the graph
        xSize = graphWidth / (maxVisibleValueAmount + 1);

        // Cycle through all visible data points
        int xIndex = 0;
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            // Add data point visual
            graphVisualObjectList.Add(graphVisual.createGraphVisualObject(new Vector2(xPosition, yPosition), xSize));

            // Label X
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -10f);
            labelX.GetComponent<Text>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            // Dash X
            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -7f);
            gameObjectList.Add(dashX.gameObject);

            xIndex++;
        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            // Label Y
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            yLabelList.Add(labelY);
            gameObjectList.Add(labelY.gameObject);

            // Dash Y
            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }
    }

    private void calculateYScale(out float yMinimum, out float yMaximum)
    {
        yMinimum = yMaximum = 0;

        if (valueList.Count != 0)
        {
            // Identify y Min and Max values
            yMaximum = valueList[0];
            yMinimum = valueList[0];

            for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
            {
                int value = valueList[i];
                if (value > yMaximum)
                {
                    yMaximum = value;
                }
                if (value < yMinimum)
                {
                    yMinimum = value;
                }
            }
            float yDifference = yMaximum - yMinimum;
            if (yDifference <= 0)
            {
                yDifference = 5f;
            }
            yMaximum = yMaximum + (yDifference * 0.2f);
            yMinimum = yMinimum - (yDifference * 0.2f);

            if (startYScaleAtZero)
            {
                yMinimum = 0f; //Start the graph at zero
            }
        }
    }

    /**
     * Interface definition for showing visual for a data point.
     */
    private interface IGraphVisual
    {
        IGraphVisualObject createGraphVisualObject(Vector2 graphPosition, float graphPositionWidth);
    }

    /**
     * Represents a single Visual Object in the graph.
     */
    private interface IGraphVisualObject
    {
        void setGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth);
        void cleanUp();
    }

    /**
     * Displays data points as a Bar Chart.
     */
    private class BarChartVisual : IGraphVisual
    {
        private RectTransform graphContainer;
        private Color barColor;
        private float barWidthMultiplier;

        public BarChartVisual(RectTransform graphContainer, Color barColor, float barWidthMultiplier)
        {
            this.graphContainer = graphContainer;
            this.barColor = barColor;
            this.barWidthMultiplier = barWidthMultiplier;
        }

        public IGraphVisualObject createGraphVisualObject(Vector2 graphPosition, float graphPositionWidth)
        {
            GameObject barGameObject = createBar(graphPosition, graphPositionWidth); // *.9f --> gab between bars

            BarChartVisualObject barChartVisualObject = new BarChartVisualObject(barGameObject, barWidthMultiplier);
            barChartVisualObject.setGraphVisualObjectInfo(graphPosition, graphPositionWidth);

            return barChartVisualObject;
        }

        private GameObject createBar(Vector2 graphPosition, float barWidth)
        {
            GameObject gameObject = new GameObject("bar", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = barColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
            rectTransform.sizeDelta = new Vector2(barWidth * barWidthMultiplier, graphPosition.y);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(.5f, 0f);
            return gameObject;
        }

        public class BarChartVisualObject : IGraphVisualObject
        {
            private GameObject barGameObject;
            private float barWidthMultiplier;

            public BarChartVisualObject(GameObject barGameObject, float barWidthMultiplier)
            {
                this.barGameObject = barGameObject;
                this.barWidthMultiplier = barWidthMultiplier;
            }
            public void setGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth)
            {
                RectTransform rectTransform = barGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
                rectTransform.sizeDelta = new Vector2(graphPositionWidth * barWidthMultiplier, graphPosition.y);
            }

            public void cleanUp()
            {
                Destroy(barGameObject);
            }
        }
    }

    /*
     * Displays data points as a Line Graph
     * */
    private class LineGraphVisual : IGraphVisual
    {
        private RectTransform graphContainer;
        private Sprite dotSprite;
        private LineGraphVisualObject lastLineGraphVisualObject;
        private Color dotColor, dotConnectionColor;

        public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor)
        {
            this.graphContainer = graphContainer;
            this.dotSprite = dotSprite;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;
            lastLineGraphVisualObject = null;
        }
        public IGraphVisualObject createGraphVisualObject(Vector2 graphPosition, float graphPositionWidth)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            GameObject dotGameObject = createDot(graphPosition);

            gameObjectList.Add(dotGameObject);
            GameObject dotConnectionGameObject = null;
            if (lastLineGraphVisualObject != null)
            {
                dotConnectionGameObject = createDotConnection(lastLineGraphVisualObject.getGraphPosition(), dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionGameObject);
            }

            LineGraphVisualObject lineGraphVisualObject = new LineGraphVisualObject(dotGameObject, dotConnectionGameObject, lastLineGraphVisualObject);
            lineGraphVisualObject.setGraphVisualObjectInfo(graphPosition, graphPositionWidth);

            lastLineGraphVisualObject = lineGraphVisualObject;

            return lineGraphVisualObject;
        }
        private GameObject createDot(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("dot", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().sprite = dotSprite;
            gameObject.GetComponent<Image>().color = dotColor;
            gameObject.GetComponent<Image>().useSpriteMesh = true;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(6, 6);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            return gameObject;
        }
        private GameObject createDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3f);
            rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
            rectTransform.localEulerAngles = new Vector3(0, 0, (Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI));
            return gameObject;
        }
        public class LineGraphVisualObject : IGraphVisualObject
        {
            public event EventHandler OnChangedGraphVisualObjectInfo;
            private GameObject dotGameObject;
            private GameObject dotConnectionGameObject;
            private LineGraphVisualObject lastVisualObject;

            public LineGraphVisualObject(GameObject dotGameObject, GameObject dotConnectionGameObject, LineGraphVisualObject lastVisualObject)
            {
                this.dotGameObject = dotGameObject;
                this.dotConnectionGameObject = dotConnectionGameObject;
                this.lastVisualObject = lastVisualObject;

                if (lastVisualObject != null)
                {
                    lastVisualObject.OnChangedGraphVisualObjectInfo += LasVisualObject_OnChangedGraphVisualObjectInfo;
                }
            }

            private void LasVisualObject_OnChangedGraphVisualObjectInfo(object sender, EventArgs e)
            {
                updateDotConnection();
            }

            public void setGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth)
            {
                RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = graphPosition;

                updateDotConnection();

                if (OnChangedGraphVisualObjectInfo != null) OnChangedGraphVisualObjectInfo(this, EventArgs.Empty);
            }

            public void cleanUp()
            {
                Destroy(dotGameObject);
                Destroy(dotConnectionGameObject);
            }

            public Vector2 getGraphPosition()
            {
                RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                return rectTransform.anchoredPosition;
            }

            private void updateDotConnection()
            {
                if (dotConnectionGameObject != null)
                {
                    RectTransform dotConnectionRectTransform = dotConnectionGameObject.GetComponent<RectTransform>();
                    Vector2 dir = (lastVisualObject.getGraphPosition() - getGraphPosition()).normalized;
                    float distance = Vector2.Distance(getGraphPosition(), lastVisualObject.getGraphPosition());
                    dotConnectionRectTransform.sizeDelta = new Vector2(distance, 3f);
                    dotConnectionRectTransform.anchoredPosition = getGraphPosition() + dir * distance * .5f;
                    dotConnectionRectTransform.localEulerAngles = new Vector3(0, 0, (Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI));
                }
            }

        }
    }

}
