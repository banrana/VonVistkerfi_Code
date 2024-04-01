using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using SickscoreGames;

namespace SickscoreGames.HUDNavigationSystem
{
	[CreateAssetMenu (fileName="New Scene Configuration", menuName=HNS.PublisherName+"/"+HNS.Name+"/New Scene Configuration")]
	public class HNSSceneConfiguration : ScriptableObject
	{
		#region Variables
		// OVERRIDES
		public bool overrideRadarSettings = false;
		public bool overrideCompassBarSettings = false;
		public bool overrideIndicatorSettings = false;

		// RADAR
		public bool useRadar = true;
		public RadarModes radarMode = RadarModes.RotateRadar;
		public float radarZoom = 1f;
		public float radarRadius = 50f;
		public float radarMaxRadius = 75f;
		public bool useRadarScaling = true;
		public float radarScaleDistance = 15f;
		public float radarMinScale = .8f;
		public bool useRadarFading = true;
		public float radarFadeDistance = 10f;
		public float radarMinFade = 0f;
		public bool useRadarHeightSystem = true;
		public float radarDistanceAbove = 10f;
		public float radarDistanceBelow = 10f;

		// COMPASS BAR
		public bool useCompassBar = true;
		public float compassBarRadius = 150f;

		// INDICATOR
		public bool useIndicators = true;
		public float indicatorRadius = 25f;
		public float indicatorHideDistance = 3f;
		public bool useOffscreenIndicators = true;
		public float indicatorOffscreenBorder = .075f;
		public bool useIndicatorScaling = true;
		public float indicatorScaleRadius = 15f;
		public float indicatorMinScale = .8f;
		public bool useIndicatorFading = true;
		public float indicatorFadeRadius = 15f;
		public float indicatorMinFade = 0f;

		
		#endregion
	}
}

