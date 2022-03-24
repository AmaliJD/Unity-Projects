using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
	public List<Cell> neighbors;
	private Color color;
	private CellGrid cellgrid;
	public float red, green, blue, outRed, outGreen, outBlue;
	public float influence, selffluence, hurt;
	public int spark;

	// colormode -> main color		type -> maturetype
	public int colormode, type;
	public bool fullcolor, mature, dead;

	public float deadtime, lifespan;
	private float deadtimer, lifespantimer;

	private SpriteRenderer sprite;

    public void Setup()
    {
		neighbors = new List<Cell>();
		sprite = GetComponent<SpriteRenderer>();

		type = Random.Range(0, 4);
		type = 2;
		influence = Random.Range(0f, .2f);
		selffluence = Random.Range(0f, .3f);
		hurt = Random.Range(.002f, .02f);

		deadtime = Random.Range(0f, 3f);
		lifespan = Random.Range(0f, 30f);
	}

	public void addNeighbor(Cell c) { neighbors.Add(c); }
	public void setGrid(CellGrid cg) { cellgrid = cg; }

	public void addRed(float r)
    {
		red = Mathf.Clamp(red + r, 0, 1);
		if (red == 1) mature = true;
	}

	public void addGreen(float g)
	{
		green = Mathf.Clamp(green + g, 0, 1);
		if (green == 1) mature = true;
	}

	public void addBlue(float b)
	{
		blue = Mathf.Clamp(blue + b, 0, 1);
		if (blue == 1) mature = true;
	}

	public void addAll(float a)
	{
		red = Mathf.Clamp(red + a, 0, 1);
		green = Mathf.Clamp(green + a, 0, 1);
		blue = Mathf.Clamp(blue + a, 0, 1);
		if (blue == 1 || green == 1 || red == 1) mature = true;
	}

	// get colors from neighbors
	public void grab()
	{
		if (dead) return;

		foreach(Cell c in neighbors)
        {
			if(colormode == c.colormode)
            {
				lifespan = (lifespan + c.lifespan) / 2;
				hurt = (hurt + c.hurt) / 2;
				influence = (influence + c.influence) / 2;
				selffluence = (selffluence + c.selffluence) / 2;
			}

			if (!mature)
			{
				switch (c.colormode)
				{
					case 0: addRed(-hurt); addGreen(-hurt); addBlue(-hurt); break;
					case 1: addRed(c.outRed * influence); addGreen(-c.outRed * influence); addBlue(-c.outRed * influence); break;
					case 2: addGreen(c.outGreen * influence); addRed(-c.outGreen * influence); addBlue(-c.outGreen * influence); break;
					case 3: addBlue(c.outBlue * influence); addGreen(-c.outBlue * influence); addRed(-c.outBlue * influence); break;
					case 4: addBlue(c.outBlue * influence); addGreen(c.outGreen * influence); addRed(c.outRed * influence); break;
				}
			}
			else if(lifespantimer < lifespan)
            {
				switch (type)
                {
					case 0:
						addRed(-hurt); addGreen(-hurt); addBlue(-hurt);
						break;

					case 1:
						if(c.colormode == colormode)
                        {
							addRed(-hurt * 2); addGreen(-hurt * 2); addBlue(-hurt * 2);
						}
                        else
                        {
							lifespan *= Random.Range(1f, 1.02f);

							if (spark == 1)
							{
								switch (c.colormode)
								{
									case 0: addRed(-hurt); addGreen(-hurt); addBlue(-hurt); break;
									case 1: addRed(c.outRed * influence * 2); break;
									case 2: addGreen(c.outGreen * influence * 2); break;
									case 3: addBlue(c.outBlue * influence * 2); break;
									case 4: addBlue(c.outBlue * (influence / 10)); addGreen(c.outGreen * (influence / 10)); addRed(c.outRed * (influence / 10)); break;
								}
							}

							if (c.colormode != 0)
							{
								switch (colormode)
								{
									case 1: addRed(selffluence); break;
									case 2: addGreen(selffluence); break;
									case 3: addBlue(selffluence); break;
									case 4: addBlue(selffluence); addGreen(selffluence); addRed(selffluence); deadtime *= 1.01f; lifespan *= Random.Range(.97f, 1f); break;
									default: addRed(-hurt); addGreen(-hurt); addBlue(-hurt); break;
								}
							}
						}
						break;

					case 2:
						if (c.colormode == colormode)
						{
							lifespantimer -= (Time.deltaTime/4);
							switch (c.colormode)
							{
								case 1: addRed(c.outRed * influence / 5); break;
								case 2: addGreen(c.outGreen * influence / 5); break;
								case 3: addBlue(c.outBlue * influence / 5); break;
								case 4: addBlue(c.outBlue * influence); addGreen(c.outGreen * influence); addRed(c.outRed * influence); break;
								default: addRed(-hurt); addGreen(-hurt); addBlue(-hurt); break;
							}
						}
						else
                        {
							addRed(-hurt * 5); addGreen(-hurt * 5); addBlue(-hurt * 5);
						}
						break;

					case 3:
						if (c.colormode != colormode)
						{
							switch (c.colormode)
							{
								case 0: addRed(-hurt); addGreen(-hurt); addBlue(-hurt); break;
								case 1: addRed(c.outRed * influence); break;
								case 2: addGreen(c.outGreen * influence); break;
								case 3: addBlue(c.outBlue * influence); break;
								case 4: addBlue(c.outBlue * influence); addGreen(c.outGreen * influence); addRed(c.outRed * influence); lifespan *= .95f;  break;
							}

							if (c.colormode != 0)
							{
								switch (colormode)
								{
									case 1: addRed(2 * selffluence); break;
									case 2: addGreen(2 * selffluence); break;
									case 3: addBlue(2 * selffluence); break;
									case 4: addBlue(selffluence); addGreen(selffluence); addRed(selffluence); lifespan *= 1.01f; break;
									default: addRed(-hurt); addGreen(-hurt); addBlue(-hurt); break;
								}
							}
						}
						else
						{
							if(spark == 1)
                            {
								addBlue(Random.Range(0, .003f)); addGreen(Random.Range(0, .003f)); addRed(Random.Range(0, .003f)); lifespan *= .8f;
							}
							else
                            {
								addBlue(Random.Range(-.005f, 0)); addGreen(Random.Range(-.005f, 0)); addRed(Random.Range(-.005f, 0)); lifespan *= .9f;
							}
							
						}
						break;

					case 4:
						lifespan = 10;
						c.addRed(-.6f); c.addGreen(-.6f); c.addBlue(-.6f);

						int R = Random.Range(0, 5);
						if (R == 0)
                        {
							c.type = 4;
							c.deadtimer = -30;
						}
						break;
				}
			}
			else
            {
				addRed(-hurt * 3); addGreen(-hurt * 3); addBlue(-hurt * 3);
				c.addRed(-c.hurt * 3); c.addGreen(-c.hurt * 3); c.addBlue(-c.hurt * 3);
			}
		}
	}

	// determine the overall color of the node
	public void setColor()
	{
		float h, s, v;
		Color.RGBToHSV(color, out h, out s, out v);
		//if (red > 0 && colormode == 4) { colormode = 4; }
		if (red > green && red > blue) { colormode = 1; color = new Color(red, 0, 0); }
		else if (green > red && green > blue) { colormode = 2; color = new Color(0, green, 0); }
		else if (blue > green && blue > red) { colormode = 3; color = new Color(0, 0, blue); }
		else if (v > .5f && s < .95f) { colormode = 4; /*Debug.Log("WHITE");*/ color = new Color(red, green, blue); }
		else if(red == 0 && green == 0 && blue == 0 && colormode != 0) { colormode = 0; color = new Color(0, 0, 0); mature = false; dead = true; }
		else if(colormode == 1) { color = new Color(red, 0, 0); }
		else if (colormode == 2) { color = new Color(0, green, 0); }
		else if (colormode == 3) { color = new Color(0, 0, blue); }
		else if (colormode == 4) { color = new Color(red, green, blue); }

		if (type == 4)
			color = Color.black;

		if (fullcolor)
		{
			color = new Color(red, green, blue);

			if (type == 4)
				color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		}

		sprite.color = color;
	}

	public void updateOutput()
    {
		outBlue = blue;
		outGreen = green;
		outRed = red;
    }

    public void UpdateFunction()
    {
		fullcolor = cellgrid.fullcolor;

		if(deadtimer >= deadtime)
        {
			dead = false;
			mature = false;
			deadtimer = 0;

			type = Random.Range(0, 4);
			influence = Random.Range(0f, .2f);
			selffluence = Random.Range(0f, .3f);
			hurt = Random.Range(.002f, .02f);

			deadtime = Random.Range(0f, 4f);
			lifespan = Random.Range(0f, 30f);
		}

		if(dead)
        {
			deadtimer += Time.deltaTime;
			lifespantimer = 0;
        }
		else
        {
			lifespantimer += Time.deltaTime;
			deadtimer = 0;
		}			
	}
}
