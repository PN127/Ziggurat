using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public class GateAssistant : MonoBehaviour
    {

        [SerializeField]
        private GameObject _unit;
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private Colour _gateColour;
        public Colour Get_gateColour => _gateColour;

        private SteeringBehaviorData GetSteeringBehaviorData;

        [SerializeField]
        private Text _healthText;
        [SerializeField]
        private Text _speedText;
        [SerializeField]
        private Text _slowAttackDamageText;
        [SerializeField]
        private Text _fastAttackDamageText;
        [SerializeField]
        private Slider _spawnReloadValue;
        [SerializeField]
        private Slider _frequencyFastAttackPerMinuteValue;

        private List<GameObject> _units;

        private float _healthUnit;
        private float _speedUnit;        
        private float _slowAttackDamageUnit;
        private float _fastAttackDamageUnit;
        private float _spawnReload;
        private float _frequencyFastAttackPerMinuteUnit;

        private void Awake()
        {
            RayCast.ShowEvent += (colour) => SaveParams();
        }

        void Start()
        {
            GetSteeringBehaviorData = ConfigurationManager.Self.GetSteeringBehaviorData;

            SaveParams();

            switch (_gateColour)
            {
                case Colour.Red:
                    _units = ConfigurationManager.unitsRed;
                    break;
                case Colour.Green:
                    _units = ConfigurationManager.unitsGreen;
                    break;
                case Colour.Blue:
                    _units = ConfigurationManager.unitsBlue;
                    break;
            }
            StartCoroutine(CreateNewUnit());
        }

        void Update()
        {
            
        }

       

        private void SaveParams()
        {
            _healthUnit = float.Parse(_healthText.text);
            _speedUnit = float.Parse(_speedText.text);
            _slowAttackDamageUnit = float.Parse(_slowAttackDamageText.text);
            _fastAttackDamageUnit = float.Parse(_fastAttackDamageText.text);
            _spawnReload = _spawnReloadValue.value;
            _frequencyFastAttackPerMinuteUnit = _frequencyFastAttackPerMinuteValue.value;
        }

        IEnumerator CreateNewUnit()
        {
            int a = 0;
            while (a < 15)
            {
                var data = GetSteeringBehaviorData;
                var unit = Instantiate(_unit);
                //unit.transform.position = transform.position + new Vector3(0, 7, 0);
                var unitManager = unit.GetComponent<UnitManager>();
                unitManager.Health = _healthUnit;
                unitManager.Speed = _speedUnit;
                unitManager.FastAttackDamage = _fastAttackDamageUnit;
                unitManager.SlowAttackDamage = _slowAttackDamageUnit;
                unitManager.FrequencyFastAttackPerMinute = _frequencyFastAttackPerMinuteUnit;
                unitManager.Colour = _gateColour;
                unitManager.Target = data.Center.transform;
                unitManager.State = AIStateType.Move_Seek;
                _units.Add(unit);
                a++;
                yield return new WaitForSeconds(_spawnReload);
            }
        }
    }
}