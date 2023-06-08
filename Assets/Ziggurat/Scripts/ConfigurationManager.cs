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

        public static List<UnitManager> unitsRed = new List<UnitManager>();
        public static List<UnitManager> unitsGreen = new List<UnitManager>();
        public static List<UnitManager> unitsBlue = new List<UnitManager>();

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
