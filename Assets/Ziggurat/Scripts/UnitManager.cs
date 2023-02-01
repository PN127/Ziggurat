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
        private List<GameObject> _family;

        public float Mass => _rb.mass;

        private bool _attacking;

        void Start()
        {
            DistanceDetection = 10;
            DistanceAttack = 2;
            _environment = GetComponent<UnitEnvironment>();
            _rb = GetComponent<Rigidbody>();
            GetSteeringBehaviorData = ConfigurationManager.Self.GetSteeringBehaviorData;

            switch (Colour)
            {
                case Colour.Red:
                    _family = ConfigurationManager.unitsRed;
                    break;
                case Colour.Green:
                    _family = ConfigurationManager.unitsGreen;
                    break;
                case Colour.Blue:
                    _family = ConfigurationManager.unitsBlue;
                    break;
            }

        }

        private void FixedUpdate()
        {
            if (Target != null)
            {
                if (Target.GetComponent<NPC>())
                    AttackZone();
                else
                    SearchaTarget();
            }
        }

        private void SearchaTarget()
        {
            float minDistance = Mathf.Infinity;
            Collider[] _col = Physics.OverlapSphere(transform.position, DistanceDetection, 1 << (int)Masks.Unit);
            foreach (Collider col in _col)
            {
                if (Colour != col.GetComponent<NPC>().Colour)
                {
                    var distance = Vector3.Distance(gameObject.transform.position, col.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        Target = col.transform;
                    }
                    //Debug.Log("my name: " + gameObject.name + "   name enemy: " + Target.gameObject.name);
                }
            }
        }

        private void AttackZone()
        {
            var distance = Vector3.Distance(gameObject.transform.position, Target.transform.position);
            if (distance < DistanceAttack && !_attacking)
            {
                _attacking = true;
                StartCoroutine(Fight());
            }
            if (distance > DistanceAttack)
            {
                _attacking = false;
                StopCoroutine(Fight());
            }
        }

        private void TakeDamage()
        {
            Target.GetComponent<UnitManager>().GetDamage(FastAttackDamage);
        }

        public void GetDamage(float score)
        {
            Health -= score;
            Debug.Log("my name: " + gameObject.name + Health);
            if (Health == 0)
            {
                Debug.Log("  1 my family: " + _family.Count);
                _family.Remove(gameObject);
                Debug.Log("  2 my family: " + _family.Count);
                gameObject.SetActive(false);
            }
            Debug.Log("my family: " + _family.Count);

        }

        IEnumerator Fight()
        {
            int a = 0;
            //while (a < 10)
            {
                TakeDamage();

                yield return new WaitForSeconds(/*60 / FrequencyFastAttackPerMinute*/1);
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