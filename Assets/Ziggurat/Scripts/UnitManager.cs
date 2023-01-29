using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
    [RequireComponent(typeof(Rigidbody))]

    public class UnitManager : NPC
    {
        private UnitEnvironment _environment;
        private Rigidbody _rb;

        public Transform Target;
        public SteeringBehaviorData GetSteeringBehaviorData { get; private set; }
        public AIStateType State { get; set; }

        public float Mass => _rb.mass;
        public float health;                                     //delete
        public float distanceAttack;                             //delete

        void Start()
        {
            DistanceDetection = 10;
            _environment = GetComponent<UnitEnvironment>();
            _rb = GetComponent<Rigidbody>();
            GetSteeringBehaviorData = ConfigurationManager.Self.GetSteeringBehaviorData;

            health = Health;                                       //delete
        }

        private void FixedUpdate()
        {
            if ( Target != null && Target.GetComponent<NPC>() == null)
            {
                float minDistance = Mathf.Infinity;
                Collider[] _col = Physics.OverlapSphere(transform.position, DistanceDetection, 1 << (int)Masks.Unit);
                foreach (Collider col in _col)
                {
                    if (UnitColour == col.GetComponent<NPC>().UnitColour) return;

                    var distance = Vector3.Distance(gameObject.transform.position, col.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        Target = col.transform;
                    }
                    Debug.Log("my name: " + gameObject.name + "   me enemy: " + Target.gameObject.name);
                }
                
            }
        }


        public Vector3 GetUpdateIgnoreAxis(Vector3 vector, IgnoreAxisType ignore = IgnoreAxisType.None)
        {
            return UpdateIgnoreAxis(vector, ignore);
        }

        public Vector3 GetVelocity(IgnoreAxisType ignore = IgnoreAxisType.None)
        {
            return UpdateIgnoreAxis(_rb.velocity, ignore);
        }

        public void SetVelocity(Vector3 velocity, IgnoreAxisType ignore = IgnoreAxisType.None)
        {
            _rb.velocity = UpdateIgnoreAxis(velocity, ignore);

        }

        private Vector3 UpdateIgnoreAxis(Vector3 velocity, IgnoreAxisType ignore)
        {
            if (ignore == IgnoreAxisType.None) return velocity;
            else if (ignore == IgnoreAxisType.Y) velocity.y = 0;
            else if (ignore == IgnoreAxisType.X) velocity.x = 0;
            else if (ignore == IgnoreAxisType.Z) velocity.z = 0;

            return velocity;
        }

    }
}