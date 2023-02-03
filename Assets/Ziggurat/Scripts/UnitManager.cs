using System;
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
        private SteeringBehaviorData GetSteeringBehaviorData;
        private List<GameObject> _family;
        private Coroutine _figth;

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
            MoveUnity();

            if (Target != null && Target.GetComponent<NPC>() && Target.gameObject.activeSelf)
                AttackZone();
            else
                SearchaTarget();
        }

        private void MoveUnity()
        {
            switch (State)
            {
                case AIStateType.Move_Seek:
                    OnSeek();
                    break;
                case AIStateType.Move_Arrival:
                    OnArrivel();
                    break;
                case AIStateType.Move_Wander:
                    OnWander();
                    break;

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
                    State = AIStateType.Move_Seek;

                }
            }
        }

        private void AttackZone()
        {
            var distance = Vector3.Distance(gameObject.transform.position, Target.transform.position);
            if (distance < DistanceAttack && !_attacking)
            {
                _attacking = true;
                _figth = StartCoroutine(Fight());
                State = AIStateType.Move_Arrival;
            }
            if (distance > DistanceAttack && _attacking)
            {
                _attacking = false;
                State = AIStateType.Move_Seek;

                if (_figth == null) return;
                StopCoroutine(_figth);
                _figth = null;
                //if (distance > DistanceDetection)
                //{
                //    State = AIStateType.Move_Wander;
                //}
            }
        }

        private void TakeDamage()
        {
            if (Target == null || !Target.gameObject.activeSelf)
            {
                _attacking = false;
                State = AIStateType.Move_Wander;

                if (_figth == null) return;
                StopCoroutine(_figth);
                _figth = null;
                return;
            }
            var dis = Vector3.Distance(transform.position, Target.transform.position);


            if (!Target.GetComponent<UnitManager>().GetDamage(FastAttackDamage, name))
            {
                State = AIStateType.Move_Wander;
                Target = null;
                _attacking = false;

                if (_figth == null) return;
                StopCoroutine(_figth);
                _figth = null;
            }


        }

        public bool GetDamage(float score, string killer)
        {
            Health -= score;
            if (Health <= 0)
            {

                Debug.Log("my name: " + gameObject.name + Health + "my killer" + killer);
                _family.Remove(gameObject);
                gameObject.SetActive(false);
                StopAllCoroutines();
                //Destroy(gameObject);
                return false;
            }
            return true;
        }

        IEnumerator Fight()
        {
            var r = 0;
            for (; ; )
            {
                Debug.Log("i'm work: " + r);
                TakeDamage();
                r++;
                yield return new WaitForSeconds(/*60 / FrequencyFastAttackPerMinute*/1);
            }
        }
        

        private void OnSeek()
        {
            if (Target == null) return;

            var data = GetSteeringBehaviorData;
            var desired_velocity = (Target.transform.position - transform.position).normalized * data.MaxVelcity * Speed;
            var steering = GetUpdateIgnoreAxis(desired_velocity, IgnoreAxisType.Y) - GetVelocity(IgnoreAxisType.Y);
            steering = Vector3.ClampMagnitude(steering, data.MaxVelcity) / Mass;

            var velocity = Vector3.ClampMagnitude(GetVelocity() + steering, data.MaxSpeed);
            SetVelocity(velocity);

            var pointTarget = Target.transform.position;
            pointTarget.y = transform.position.y;
            transform.LookAt(pointTarget);
        }
        private void OnArrivel()
        {
            if (Target == null) return;

            var data = GetSteeringBehaviorData;
            var desired_velocity = Target.transform.position - transform.position;
            var sqrLength = desired_velocity.sqrMagnitude;
            

            var steering = GetUpdateIgnoreAxis(desired_velocity, IgnoreAxisType.Y) - GetVelocity(IgnoreAxisType.Y);
            steering = Vector3.ClampMagnitude(steering, data.MaxVelcity) / Mass;

            var velocity = Vector3.ClampMagnitude(GetVelocity() + steering, data.MaxSpeed);
            SetVelocity(velocity);

            var pointTarget = Target.transform.position;
            pointTarget.y = transform.position.y;
            transform.LookAt(pointTarget);
        }
        private void OnWander()
        {
            var data = GetSteeringBehaviorData;
            var center = GetVelocity(IgnoreAxisType.Y).normalized * data.WanderCenterDistance;

            var displacement = Vector3.zero;
            displacement.x = Mathf.Cos(WanderAngel * Mathf.Deg2Rad);
            displacement.z = Mathf.Sin(WanderAngel * Mathf.Deg2Rad);
            displacement = displacement.normalized * data.WanderRadius;

            WanderAngel += UnityEngine.Random.Range(-data.WanderAngelRange, data.WanderAngelRange);

            var desired_velocity = center + displacement;
            var steering = GetUpdateIgnoreAxis(desired_velocity, IgnoreAxisType.Y) - GetVelocity(IgnoreAxisType.Y);
            steering = Vector3.ClampMagnitude(steering, data.MaxVelcity) / Mass;

            var velocity = Vector3.ClampMagnitude(GetVelocity() + steering, data.MaxSpeed);            
            SetVelocity(velocity);

            var pointTarget = desired_velocity + transform.position;
            pointTarget.y = transform.position.y;
            transform.LookAt(pointTarget);
        }



        private Vector3 UpdateIgnoreAxis(Vector3 velocity, IgnoreAxisType ignore)
        {
            if (ignore == IgnoreAxisType.None) return velocity;
            else if (ignore == IgnoreAxisType.Y) velocity.y = 0;
            else if (ignore == IgnoreAxisType.X) velocity.x = 0;
            else if (ignore == IgnoreAxisType.Z) velocity.z = 0;

            return velocity;
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

    }
}