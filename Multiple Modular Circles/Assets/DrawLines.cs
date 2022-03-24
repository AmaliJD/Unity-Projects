using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLines : MonoBehaviour
{
    public GameObject canvas;
    private bool showui = true;

    public Dropdown dropdown;
    public Slider slider, nodeSlider, widthSlider, speedSlider, multipleSlider;
    public Text text, nodeText, widthText, widthLabel, speedText, multipleText, animatetext, toggletext, percenttext, shapetext;
    public Toggle snap, percentage, animate, shape;

    public Image dropTemplate, dropView;
    public Image fill, nodeFill, widthFill, speedFill, multipleFill, handle, widthHandle, nodeHandle, speedHandle, multipleHandle, toggle, percentToggle, animateToggle, shapeToggle;
    private bool prevPercent;

    public enum ColorMode
    {
        cycle, length, position, angle, gray
    }
    public ColorMode colormode;

    [Range(1, 3)]
    [SerializeField]
    private float width = 1;

    public int nodes;
    private int prevNodes;

    public float number;
    private float prevNum, speed = 1;

    public int distance;

    [Range (1, 8)]
    public float multiple;
    public Material material;

    [SerializeField]
    private Color startColor, endColor;

    public List<LineRenderer> linerenderers;

    void Awake()
    {
        startColor = Color.cyan;
        endColor = Color.blue;
        linerenderers = new List<LineRenderer>();
        prevNodes = nodes;
        prevNum = number;
        prevPercent = percentage.isOn;

        for (int i = 0; i < nodes; i++)
        {
            GameObject newObj = new GameObject();
            newObj.AddComponent<LineRenderer>();
            newObj.transform.parent = transform;

            LineRenderer lr = newObj.GetComponent<LineRenderer>();
            lr.material = material;
            linerenderers.Add(lr);
        }

        Draw();
    }

    private void Start()
    {
        //GetComponent<AudioSource>().Play();
    }

    public void changeNodes(int n)
    {
        if(n > prevNodes)
        {
            for (int i = prevNodes; i < n; i++)
            {
                GameObject newObj = new GameObject();
                newObj.AddComponent<LineRenderer>();
                newObj.transform.parent = transform;

                LineRenderer lr = newObj.GetComponent<LineRenderer>();
                lr.material = material;
                linerenderers.Add(lr);
            }
        }
        else if(n < prevNodes)
        {
            int i = 0, end = prevNodes - n;
            foreach (Transform t in transform)
            {
                if (i == end)
                    break;

                linerenderers.RemoveAt(0);
                Destroy(t.gameObject);
                i++;
            }
        }

        if(shape.isOn)
        {
            snap.isOn = false;
            slider.wholeNumbers = snap.isOn;
            if (!percentage.isOn)
            {
                /*float diff = n - prevNodes;
                float translate = diff / 2;
                slider.value += translate;
                */
                float per = prevNum / prevNodes;
                per = Mathf.Round(per * multiple) / multiple;
                float anchor = per * prevNodes;

                float diff = anchor - prevNum;
                slider.value = (per * n) - diff;
                number = slider.value;
            }
            else
            {
                /*float num = (slider.value/100) * prevNodes;
                float diff = n - prevNodes;
                float translate = diff / 2;
                num += translate;
                slider.value = (num / nodes) * 100;*/
                float num = (slider.value / 100) * prevNodes;
                float per = slider.value / 100;
                per = Mathf.Round(per * multiple) / multiple;
                float anchor = per * prevNodes;

                float diff = anchor - num;
                slider.value = (((per * n) - diff) / n) * 100;
                number = slider.value;
            }
        }
        
    }

    public void Draw()
    {
        float stepAngle = 360f / nodes;
        for (int i = 0; i < nodes; i++)
        {
            float value = (number * i) % nodes;
            var start = Quaternion.AngleAxis(i * stepAngle, Vector3.forward);
            var end = Quaternion.AngleAxis(value * stepAngle, Vector3.forward);
            var startPosition = start * Vector3.left * distance;
            var endPosition = end * Vector3.left * distance;

            linerenderers[i].SetPosition(0, startPosition);
            linerenderers[i].SetPosition(1, endPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(percentage.isOn != prevPercent)
            snap.isOn = false; slider.wholeNumbers = snap.isOn;

        nodes = (int)nodeSlider.value;
        speed = speedSlider.value;
        multiple = multipleSlider.value;
        width = widthSlider.value;
        colormode = (ColorMode)dropdown.value;

        if (percentage.isOn)
        {
            slider.maxValue = 1000;

            if (percentage.isOn != prevPercent)
                slider.value = ((float)slider.value / (float)nodes) * 100;

            slider.maxValue = 100;

            number = (slider.value / 100) * nodes;
            text.text = (Mathf.Round((float)slider.value * 10) / 10) + "%";
        }
        else
        {
            slider.maxValue = 1000;

            if (percentage.isOn != prevPercent)
                slider.value = ((float)slider.value/100f) * (float)nodes;

            slider.maxValue = nodes;

            number = slider.value;
            text.text = (Mathf.Round(number * 100) / 100) + "";
        }
        //slider.maxValue = nodes;

        slider.wholeNumbers = snap.isOn;
        //number = slider.value;
        //text.text = (Mathf.Round(number * 100) / 100) + "";
        multipleText.text = multiple + "";
        nodeText.text = nodes + "";
        widthText.text = (Mathf.Round(width * 10) / 10) + "";
        speedText.text = (Mathf.Round(speed * 10) / 10) + "";

        if (prevNodes != nodes)
        {
            changeNodes(nodes);
            Draw();
        }

        if (prevNum != number)
            Draw();

        prevNodes = nodes;
        prevNum = number;
        prevPercent = percentage.isOn;

        float index = 0;
        foreach (LineRenderer lr in linerenderers)
        {
            Color c = Color.white;
            Vector3[] positions = new Vector3[lr.positionCount];
            lr.GetPositions(positions);
            switch (colormode)
            {
                case ColorMode.cycle:
                    lr.SetColors(startColor, endColor);
                    break;

                case ColorMode.length:
                    //positions = new Vector3[lr.positionCount];
                    //lr.GetPositions(positions);
                    float length = Vector3.Distance(positions[0], positions[1]);
                    c = Color.HSVToRGB(1 - (length / 1000f), 1, 1);

                    lr.SetColors(c, c);
                    break;

                case ColorMode.position:
                    //c = Color.HSVToRGB((index / linerenderers.Count), 1, 1);
                    float angle1 = Vector3.Angle(positions[0] - Vector3.zero, Vector3.right);
                    Vector3 cross1 = Vector3.Cross(positions[0] - Vector3.zero, Vector3.right);
                    float angle2 = Vector3.Angle(positions[1] - Vector3.zero, Vector3.right);
                    Vector3 cross2 = Vector3.Cross(positions[1] - Vector3.zero, Vector3.right);

                    if (cross1.z < 0)
                    {
                        angle1 = 360 - angle1;
                    }
                    if (cross2.z < 0)
                    {
                        angle2 = 360 - angle2;
                    }

                    c = Color.HSVToRGB(angle1 / 360, 1, 1);
                    Color c1 = Color.HSVToRGB(angle2 / 360, 1, 1);
                    lr.SetColors(c, c1);
                    break;

                case ColorMode.angle:
                    //positions = new Vector3[lr.positionCount];
                    //lr.GetPositions(positions);
                    float angle = Vector3.Angle(positions[1] - positions[0], Vector3.right);
                    Vector3 cross = Vector3.Cross(positions[1] - positions[0], Vector3.right);

                    if (cross.z < 0)
                    {
                        angle = 360 - angle;
                    }

                    c = Color.HSVToRGB(angle / 360, 1, 1);
                    lr.SetColors(c, c);
                    break;

                case ColorMode.gray:
                    c = new Color(1, 1, 1, .7f);
                    Color c2 = new Color(1, 1, 1, .01f);
                    lr.SetColors(c2, c);
                    break;
            }
            
            lr.SetWidth(width, width);

            index++;
        }

        float grayMultiplier = 1;
        if (colormode == ColorMode.gray)
        {
            grayMultiplier = 0;
        }

        Color.RGBToHSV(startColor, out float startH, out float startS, out float startV);
        Color.RGBToHSV(endColor, out float endH, out float endS, out float endV);
        endColor = Color.HSVToRGB(endH - .001f, startS, startV);
        startColor = Color.HSVToRGB(endH - .09f, startS, startV);

        Camera.main.backgroundColor = Color.HSVToRGB(endH, endS * grayMultiplier, .05f);

        fill.color = grayMultiplier == 1 ? endColor : Color.white;
        handle.color = grayMultiplier == 1 ? startColor : new Color(.8f, .8f, .8f, 1);
        text.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        nodeFill.color = grayMultiplier == 1 ? endColor : Color.white;
        nodeHandle.color = grayMultiplier == 1 ? startColor : new Color(.8f, .8f, .8f, 1);
        nodeText.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        speedFill.color = grayMultiplier == 1 ? endColor : Color.white;
        speedHandle.color = grayMultiplier == 1 ? startColor : new Color(.8f, .8f, .8f, 1);
        speedText.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        multipleFill.color = grayMultiplier == 1 ? endColor : Color.white;
        multipleHandle.color = grayMultiplier == 1 ? startColor : new Color(.8f, .8f, .8f, 1);
        multipleText.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        toggletext.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);
        toggle.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        percenttext.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);
        percentToggle.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        animatetext.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);
        animateToggle.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        shapetext.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);
        shapeToggle.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        dropTemplate.color = Color.HSVToRGB(endH, 1 * grayMultiplier, 1);
        dropTemplate.color = new Color(dropTemplate.color.r, dropTemplate.color.g, dropTemplate.color.b, .05f);

        dropView.color = Color.HSVToRGB(startH, .8f * grayMultiplier, 1);
        dropView.color = new Color(dropView.color.r, dropView.color.g, dropView.color.b, .4f);

        widthFill.color = grayMultiplier == 1 ? endColor : Color.white;
        widthHandle.color = grayMultiplier == 1 ? startColor : new Color(.8f, .8f, .8f, 1);
        widthText.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);
        widthLabel.color = Color.HSVToRGB(startH, .8f * grayMultiplier, startV);

        if (animate.isOn)
        {
            snap.isOn = false;
            if (slider.value < slider.maxValue)
            {
                slider.value = Mathf.Clamp(slider.value + Mathf.Pow((speed * Mathf.Pow(.001f, .5f)), 2), 0, slider.maxValue);
            }
            else if(slider.value == slider.maxValue)
            {
                slider.value = 0;
            }
        }

        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        // keyboard shortcuts
        if (Input.GetKeyDown("0"))
        {
            slider.value = 0;
        }
        if (Input.GetKeyDown("1"))
        {
            colormode = ColorMode.cycle;
            dropdown.value = (int)colormode;
        }
        if (Input.GetKeyDown("2"))
        {
            colormode = ColorMode.length;
            dropdown.value = (int)colormode;
        }
        if (Input.GetKeyDown("3"))
        {
            colormode = ColorMode.position;
            dropdown.value = (int)colormode;
        }
        if (Input.GetKeyDown("4"))
        {
            colormode = ColorMode.angle;
            dropdown.value = (int)colormode;
        }
        if (Input.GetKeyDown("5"))
        {
            colormode = ColorMode.gray;
            dropdown.value = (int)colormode;
        }
        if (Input.GetKeyDown("z"))
        {
            animate.isOn = !animate.isOn;
        }
        if (Input.GetKeyDown("x"))
        {
            showui = !showui;
        }

        if(showui)
        {
            Camera.main.gameObject.transform.position = new Vector3(0, -40, -10);
            canvas.SetActive(true);
        }
        else
        {
            Camera.main.gameObject.transform.position = new Vector3(0, 0, -10);
            canvas.SetActive(false);
        }
    }
}
