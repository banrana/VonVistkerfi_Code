using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SickscoreGames;

namespace SickscoreGames.HUDNavigationSystem
{
	[AddComponentMenu (HNS.Name + "/HUD Navigation Canvas"), DisallowMultipleComponent]
	public class HUDNavigationCanvas : MonoBehaviour
	{
		private static HUDNavigationCanvas _Instance;
		public static HUDNavigationCanvas Instance {
			get {
				if (_Instance == null) {
					_Instance = FindObjectOfType<HUDNavigationCanvas> ();
				}
				return _Instance;
			}
		}


		#region Variables
		public _RadarReferences Radar;
		public _CompassBarReferences CompassBar;
		public _IndicatorReferences Indicator;

		public float CompassBarCurrentDegrees { get; private set; }

		public bool isEnabled // use the EnableCanvas(bool) method to change this value.
		{
			get { return _isEnabled; }
			private set { _isEnabled = value; }
		}
		[SerializeField]private bool _isEnabled = true;

		private HUDNavigationSystem _HUDNavigationSystem;
		#endregion


		#region Main Methods
		void Awake ()
		{
			if (_Instance != null) {
				Destroy (this.gameObject);
				return;
			}

			_Instance = this;
		}


		void Start ()
		{
			// assign references
			if (_HUDNavigationSystem == null)
				_HUDNavigationSystem = HUDNavigationSystem.Instance;

			// dont destroy on load
			if (_HUDNavigationSystem != null && _HUDNavigationSystem.KeepAliveOnLoad)
				DontDestroyOnLoad (this.gameObject);
		}


		/// <summary>
		/// Enable / Disable the canvas at runtime.
		/// </summary>
		/// <param name="value">value</param>
		public void EnableCanvas (bool value)
		{
			if (value == isEnabled)
				return;

			// enable/disable canvas
			isEnabled = value;
			this.gameObject.SetActive (value);
		}
		#endregion


		#region Radar Methods
		public void InitRadar ()
		{
			// check references
			if (Radar.Panel == null || Radar.Radar == null || Radar.ElementContainer == null) {
				ReferencesMissing ("Radar");
				return;
			}

			// show radar
			ShowRadar (true);
		}


		public void ShowRadar (bool value)
		{
			if (Radar.Panel != null)
				Radar.Panel.gameObject.SetActive (value);
		}


		public void UpdateRadar (Transform rotationReference, RadarModes radarType)
		{
			// assign map / player indicator rotation
			if (radarType == RadarModes.RotateRadar) {
				// set radar rotation
				Radar.Radar.transform.rotation = Quaternion.Euler (Radar.Panel.transform.eulerAngles.x, Radar.Panel.transform.eulerAngles.y, rotationReference.eulerAngles.y);
				if (Radar.PlayerIndicator != null)
					Radar.PlayerIndicator.transform.rotation = Radar.Panel.transform.rotation;
			} else {
				// set player indicator rotation
				Radar.Radar.transform.rotation = Radar.Panel.transform.rotation;
				if (Radar.PlayerIndicator != null)
					Radar.PlayerIndicator.transform.rotation = Quaternion.Euler (Radar.Panel.transform.eulerAngles.x, Radar.Panel.transform.eulerAngles.y, -rotationReference.eulerAngles.y);
			}
		}
		#endregion


		#region Compass Bar Methods
		public void InitCompassBar ()
		{
			// check references
			if (CompassBar.Panel == null || CompassBar.Compass == null || CompassBar.ElementContainer == null) {
				ReferencesMissing ("Compass Bar");
				return;
			}

			// show compass bar
			ShowCompassBar (true);
		}


		public void ShowCompassBar (bool value)
		{
			if (CompassBar.Panel != null)
				CompassBar.Panel.gameObject.SetActive (value);
		}


		public void UpdateCompassBar (Transform rotationReference)
		{
			// set compass bar texture coordinates
			CompassBar.Compass.uvRect = new Rect ((rotationReference.eulerAngles.y / 360f) - .5f, 0f, 1f, 1f);

			// calculate 0-360 degrees value
			Vector3 perpDirection = Vector3.Cross (Vector3.forward, rotationReference.forward);
			float angle = Vector3.Angle (new Vector3 (rotationReference.forward.x, 0f, rotationReference.forward.z), Vector3.forward);
			CompassBarCurrentDegrees = (perpDirection.y >= 0f) ? angle : 360f - angle;
		}
		#endregion


		#region Indicator Methods
		public void InitIndicators ()
		{
			// check references
			if (Indicator.Panel == null || Indicator.ElementContainer == null) {
				ReferencesMissing ("Indicator");
				return;
			}

			// show indicators
			ShowIndicators (true);
		}


		public void ShowIndicators (bool value)
		{
			if (Indicator.Panel != null)
				Indicator.Panel.gameObject.SetActive (value);
		}
		#endregion


		


		#region Utility Methods
		void ReferencesMissing (string feature)
		{
			Debug.LogErrorFormat ("{0} references are missing! Please assign them on the HUDNavigationCanvas component.", feature);
			this.enabled = false;
		}
		#endregion


		#region Subclasses
		[System.Serializable]
		public class _RadarReferences
		{
			public RectTransform Panel;
			public RectTransform Radar;
			public RectTransform PlayerIndicator;
			public RectTransform ElementContainer;
		}


		[System.Serializable]
		public class _CompassBarReferences
		{
			public RectTransform Panel;
			public RawImage Compass;
			public RectTransform ElementContainer;
		}


		[System.Serializable]
		public class _IndicatorReferences
		{
			public RectTransform Panel;
			public RectTransform ElementContainer;
		}
		#endregion
	}
}
