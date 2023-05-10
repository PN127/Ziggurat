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
        private AttackType SelectAttack;

        public float Mass => _rb.mass;

        private bool _attacking;

        public delegate void NotificationDeadDelegate();
        public event NotificationDeadDelegate DeadEvent;

        void Start()
        {
            SelectAttack = AttackType.Slow;
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

            if (Target != null && Target.GetComponent<NPC>() && Target.gameObject.activeSelf)   //проверка на соответствие противника
                AttackZone();
            else
                SearchaTarget();
        }

        //Поведение бота
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
                    //case AIStateType.Wait:                   
                    //    break;

            }

        }

        //Поиск противника
        private void SearchaTarget()
        {
            float minDistance = Mathf.Infinity;
            Collider[] _col = Physics.OverlapSphere(transform.position, DistanceDetection, 1 << (int)Masks.Unit);   //создание коллайдер-сферы с фильтром по маске
            foreach (Collider col in _col)  //перебор всех коллайдеров в этой сфере
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
                    Target.GetComponent<UnitManager>().DeadEvent += TargetDead;
                }
            }
        }

        private void Wait()
        {
            //transform.position += Vector3.up * Time.deltaTime;
        }

        //Метод обрабатывает поведение бота после смерти цели
        private void TargetDead()
        {
            State = AIStateType.Move_Wander;
            Target = null;
            _attacking = false;

            if (_figth == null) return;
            StopCoroutine(_figth);
            _figth = null;
        }

        //проверка аттакующей зоны
        private void AttackZone()
        {
            var distance = Vector3.Distance(gameObject.transform.position, Target.transform.position);
            if (distance < DistanceAttack && !_attacking)   //режим аттаки, если противник в радиусе атаки
            {
                _attacking = true;
                _figth = StartCoroutine(Fight());
                State = AIStateType.Move_Arrival;
            }
            else if (distance > DistanceAttack && _attacking)    //режим приследования, если противник вне радиуса аттаки
            {
                _attacking = false;
                State = AIStateType.Move_Seek;

                if (_figth == null) return;
                StopCoroutine(_figth);
                _figth = null;
            }

            //else if(distance > DistanceDetection)     //Задел на будущее. Если противник выходит из радиуса обнаружения бот переходит в блуждающее состояние
            //{
            //    State = AIStateType.Move_Wander;
            //}
        }

        //Нанесение урона
        private void TakeDamage()
        {
            if (Target == null)
            {
                TargetDead();
                return;
            }

            Select_FastOrSlowAttack();
            Target.GetComponent<UnitManager>().GetDamage(SelectAttack, name);            
        }

        //Получение урона
        public bool GetDamage(AttackType attack, string killer)
        {
            float point;

            //Сопоставление урона типу атаки
            if (attack == AttackType.Slow)
                point = SlowAttackDamage;
            else if (attack == AttackType.Fast)
                point = FastAttackDamage;
            else
            {
                Debug.LogError("Неверно указан тип атаки");
                return false;
            }

            Debug.Log($"AFTER: Colour = {Colour}, Health = {Health}, Attack type = {Convert.ToString(attack)}, point = {point}");

            Health -= point;
            if (Health <= 0)
            {
                if (Target != null)
                    Target.GetComponent<UnitManager>().DeadEvent -= TargetDead;     //Отписка от события смерти противника

                _family.Remove(gameObject);
                gameObject.SetActive(false);
                StopAllCoroutines();

                DeadEvent?.Invoke();

                return false;
            }
            Debug.Log($"BEFORE: Colour = {Colour}, Health = {Health}, Attack type = {Convert.ToString(attack)}, point = {point}");

            return true;
        }

        IEnumerator Fight()
        {
            var r = 0;
            for (; ; )
            {
                if (!_attacking) yield return null;
                if (r > 4) Debug.LogError("i'm work: " + r + " my target: " + Target.name);
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

        private void Select_FastOrSlowAttack()
        {
            var r = UnityEngine.Random.value;
            if (r > FrequencyFastAttackPerMinute)
                SelectAttack = AttackType.Slow;
            else
                SelectAttack = AttackType.Fast;

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