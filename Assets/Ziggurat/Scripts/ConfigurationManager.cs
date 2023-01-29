using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
	public class ConfigurationManager : MonoBehaviour
	{
		public static ConfigurationManager Self;

		[SerializeField]
		private SteeringBehaviorData _steeringBehaviorData;
        public SteeringBehaviorData GetSteeringBehaviorData => _steeringBehaviorData;

        public static List<GameObject> unitsRed = new List<GameObject>();
        public static List<GameObject> unitsGreen = new List<GameObject>();
        public static List<GameObject> unitsBlue = new List<GameObject>();

        public static GameObject SelectedGate;


        private void Awake()
        {
            if (Self != null)
                Destroy(this);
            else
                Self = this;
        }
    }
}
