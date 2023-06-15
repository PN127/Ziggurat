using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public class GateAssistant : MonoBehaviour
    {
        private Panel_Static ps;
        private SteeringBehaviorData GetSteeringBehaviorData;
        private List<UnitManager> _units;

        [SerializeField]
        private GameObject _unit;
        [SerializeField]
        private Transform _spawnPoint;
        [SerializeField]
        private GameObject _panel;        
        [SerializeField]
        private Colour _gateColour;
        public Colour Get_gateColour => _gateColour;
        

        [Space]
        [SerializeField, Header("Передача параметров юнитам")]
        private Text _healthText;
        [SerializeField]
        private Text _speedText;
        [SerializeField]
        private Text _slowAttackDamageText;
        [SerializeField]
        private Text _fastAttackDamageText;
        [SerializeField]
        private Slider _spawnReload;
        [SerializeField]
        private Slider _chanceMiss;
        [SerializeField]
        private Slider _chanceCriticalDamage;
        [SerializeField]
        private Slider _frequencyFastAttackPerMinute;
      

        void Start()
        {
            GetSteeringBehaviorData = ConfigurationManager.Self.GetSteeringBehaviorData;
            ps = new Panel_Static();

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
        
        //Передача информации в панель статистики
        private void unitDead()
        {
            ps.SetCountToDictionary(false, _gateColour);
        }

        //Короутина создания юнитов
        IEnumerator CreateNewUnit()
        {
            int a = 0;
            while (a < 10)
            {
                var data = GetSteeringBehaviorData;
                var unit = Instantiate(_unit, _spawnPoint.position, _spawnPoint.rotation);
                var unitManager = unit.GetComponent<UnitManager>();
                unitManager.Health = float.Parse(_healthText.text);
                unitManager.Speed = float.Parse(_speedText.text);
                unitManager.FastAttackDamage = float.Parse(_fastAttackDamageText.text);
                unitManager.StrongAttackDamage = float.Parse(_slowAttackDamageText.text);
                unitManager.FrequencyFastAttackPerMinute = _frequencyFastAttackPerMinute.value;
                unitManager.Colour = _gateColour;
                unitManager.Target = data.Center.transform;
                unitManager.State = AIStateType.Move_Seek;
                unitManager.ChanceMiss = _chanceMiss.value;
                unitManager.ChanceCriticalDamage = _chanceCriticalDamage.value;
                unitManager.DeadEvent += unitDead;
                unitManager.Id = a;
                _units.Add(unitManager);
                a++;
                ps.SetCountToDictionary(true, _gateColour);
                ps.SetTimeToCreat(_gateColour, _spawnReload.value);
                yield return new WaitForSeconds(_spawnReload.value);
            }
        }
    }
}