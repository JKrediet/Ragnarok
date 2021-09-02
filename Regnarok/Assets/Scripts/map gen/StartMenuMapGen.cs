using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartMenuMapGen : MonoBehaviour
{
	public MapGenerator mg;
	public Slider loadingBar;
	private int seed;
	private void Start()
	{
		seed = PlayerPrefs.GetInt("Seed");
		seed = Random.Range(0000, 99999);
		mg.StartGenerating(seed);
		loadingBar.minValue = 0;
		loadingBar.maxValue = 100;
	}
	private void Update()
	{
		loadingBar.value = mg.loadAmount;
	}
}
