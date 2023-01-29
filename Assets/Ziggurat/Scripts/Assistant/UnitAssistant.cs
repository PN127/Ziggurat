using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
    public class UnitAssistant : MonoBehaviour
    {
        private LinkedList<GameObject> _units;


        void Start()
        {
        }

        void Update()
        {
            foreach (var unit in ConfigurationManager.unitsRed)
                OnSeek(unit.GetComponent<UnitManager>());
            foreach (var unit in ConfigurationManager.unitsBlue)
                OnSeek(unit.GetComponent<UnitManager>());
            foreach (var unit in ConfigurationManager.unitsGreen)
                OnSeek(unit.GetComponent<UnitManager>());
        }        

        public void OnSeek(UnitManager _unit)
        {
            if (_unit.Target == null) 
                _unit.Target = gameObject.transform;

            var data = _unit.GetSteeringBehaviorData;
            var desired_velocity = (_unit.Target.transform.position - _unit.transform.position).normalized * data.MaxVelcity * _unit.Speed;
            var steering = _unit.GetUpdateIgnoreAxis(desired_velocity, IgnoreAxisType.Y) - _unit.GetVelocity(IgnoreAxisType.Y);
            steering = Vector3.ClampMagnitude(steering, data.MaxVelcity) / _unit.Mass;

            var velocity = Vector3.ClampMagnitude(_unit.GetVelocity() + steering, data.MaxSpeed);
            if (velocity.x <= 0.01f && velocity.y == 0.01f)
                velocity = Vector3.zero;
            _unit.SetVelocity(velocity);

            var pointTarget = _unit.Target.transform.position;
            pointTarget.y = _unit.transform.position.y;
            _unit.transform.LookAt(pointTarget);
        }
    }
}
