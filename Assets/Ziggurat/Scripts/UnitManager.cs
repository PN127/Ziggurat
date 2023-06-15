using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Ziggurat
{
    [RequireComponent(typeof(Rigidbody))]

    public class UnitManager : NPC
    {
        private UnitEnvironment _environment;
        [SerializeField]
        private Configuration _config;
        private Rigidbody _rb;
        private SteeringBehaviorData GetSteeringBehaviorData;
        private List<UnitManager> _family;
        private Coroutine _figth;
        private AttackType SelectAttack;
        private AnimationType SelectAnimation;


        public Transform Target;
        [SerializeField]
        private Slider _sliderHealth;
        [SerializeField]
        private Canvas MyCanvas;

        public float Mass => _rb.mass;

        private bool _attacking;

        public delegate void NotificationDeadDelegate();
        public event NotificationDeadDelegate DeadEvent;


        void Start()
        {
            _environment = GetComponent<UnitEnvironment>();
            _rb = GetComponent<Rigidbody>();
            GetSteeringBehaviorData = ConfigurationManager.Self.GetSteeringBehaviorData;
            UIManager.ShowHealth += ShowHealthSlider;

            _sliderHealth.maxValue = Health;
            _sliderHealth.value = Health;
            SelectAttack = AttackType.Strong;
            DistanceDetection = GetSteeringBehaviorData.DetectionDistance;
            DistanceAttack = GetSteeringBehaviorData.AttackDistance;


            //Определение принадлежности юнита к цвету
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
            if (gameObject.layer == 0) return;
                
            MoveUnity();

            if (Target != null && Target.GetComponent<NPC>() && Target.gameObject.activeSelf)   //проверка на соответствие противника
                AttackZone();
            else
                SearchaTarget();
            MyCanvas.transform.LookAt(Camera.main.transform);
        }

        //Поведение бота
        private void MoveUnity()
        {
            switch (State)
            {
                case AIStateType.Move_Seek:
                    OnSeek();
                    _environment.Moving(1);
                    break;
                case AIStateType.Move_Arrival:
                    OnArrivel();
                    _environment.Moving(0);
                    break;
                case AIStateType.Move_Wander:
                    OnWander();
                    _environment.Moving(1);
                    break;
                case AIStateType.Fight:
                    OnFight();
                    break;                   

            }

        }

        //Поиск противника
        private void SearchaTarget()
        {
            float minDistance = Mathf.Infinity;
            Collider[] _col = Physics.OverlapSphere(transform.position, DistanceDetection, 1 << (int)Masks.Unit);   //создание коллайдер-сферы с фильтром по маске
            foreach (Collider col in _col)  //перебор всех коллайдеров в этой сфере
            {
                if (col.GetComponent<NPC>() && Colour != col.GetComponent<NPC>().Colour)
                {
                    var distance = Vector3.Distance(gameObject.transform.position, col.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        Target = col.transform;
                    }
                    State = AIStateType.Move_Seek;
                    Target.GetComponent<UnitManager>().DeadEvent += TargetDead;
                    Debug.Log($"my name - {gameObject.name}. Target detected {Target.name}");
                }
            }
        }

        //проверка аттакующей зоны
        private void AttackZone()
        {
            var distance = Vector3.Distance(gameObject.transform.position, Target.transform.position);

            if (distance < DistanceAttack && !_attacking)
            {
                _attacking = true;
                State = AIStateType.Fight;
                _figth = StartCoroutine(Fight());

            }
            else if (distance > DistanceAttack && distance < DistanceDetection && _attacking)    //режим приследования, если противник вне радиуса аттаки
            {
                _attacking = false;
                State = AIStateType.Move_Seek;

                if (_figth == null) return;
                StopCoroutine(_figth);
                _figth = null;
            }
            else if (distance > DistanceDetection && _attacking)
            {
                _attacking = false;
                Target = null;
                State = AIStateType.Move_Wander;
                if (_figth == null) return;
                StopCoroutine(_figth);
                _figth = null;

            }            
        }

        //Нанесение урона
        private void TakeDamage()
        {
            if (Target == null)
            {
                TargetDead();
                return;
            }

            float damageCount = -1;

            //Сопоставление урона типу атаки
            switch (SelectAttack)
            {
                case AttackType.Strong:
                    damageCount = StrongAttackDamage;
                    break;
                case AttackType.Fast:
                    damageCount = FastAttackDamage;
                    break;
            }
            if (damageCount == -1)
            {
                Debug.LogWarning($"Неверно указан тип аттаки или урон выбранной атаки. Проверьте данные");
            }

            if (ChanceChangeAttack(ChanceMiss)) return;

            if (ChanceChangeAttack(ChanceCriticalDamage))
            {
                damageCount += damageCount;
            }


            Target.GetComponent<UnitManager>().GetDamage(damageCount);
        }

        //Получение урона
        public void GetDamage(float damageCount)
        {
            
            Health -= damageCount;
            _sliderHealth.value = Health;
            if (Health <= 0)
            {

                if (Target != null && Target.GetComponent<NPC>())
                    Target.GetComponent<UnitManager>().DeadEvent -= TargetDead;     //Отписка от события смерти цели

                SelectAnimation = AnimationType.Die;
                _family.Remove(this);
                StopAllCoroutines();

                _environment.StartAnimation(_config.GetDictionary[SelectAnimation]);
                gameObject.layer = 0;

                DeadEvent?.Invoke();               
            }

        }       

        //поведение бота после смерти цели
        private void TargetDead()
        {
            State = AIStateType.Move_Wander;
            Target = null;
            _attacking = false;

            if (_figth != null) StopCoroutine(_figth);
            _figth = null;
        }

        //Короутина атаки
        IEnumerator Fight()
        {
            var r = 0;
            for (; ; )
            {
                if (!_attacking) yield return null;
             
                if (r > 4)
                {
                    Debug.LogError($"{gameObject.name}{Id} i'm work: {r} my target: {Target.name}");
                }

                Select_FastOrSlowAttack();
                _environment.StartAnimation(_config.GetDictionary[SelectAnimation]);
                TakeDamage();
                r++;
                yield return new WaitForSeconds(5);//to do
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

            if(sqrLength < data.ArrivalDistance * data.ArrivalDistance)
            {
                desired_velocity = desired_velocity / data.ArrivalDistance;
            }

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
        private void OnFight()
        {
            var velocity = Vector3.zero;
            SetVelocity(velocity);

            if (Target == null) return;
            var pointTarget = Target.transform.position;
            pointTarget.y = transform.position.y;
            transform.LookAt(pointTarget);
        }

        //реализация частоты разных типов атаки
        private void Select_FastOrSlowAttack()
        {
            var r = UnityEngine.Random.value;
            if (FrequencyFastAttackPerMinute < r)
            {
                SelectAttack = AttackType.Strong;
                SelectAnimation = AnimationType.StrongAttack;
            }
            else
            {
                SelectAttack = AttackType.Fast;
                SelectAnimation = AnimationType.FastAttack;
            }
        }

        private bool ChanceChangeAttack(float modification)
        {
            var r = UnityEngine.Random.value;
            if (r < modification)
                return true;
            else
                return false;
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
        private void ShowHealthSlider(bool on)
        {
            _sliderHealth.gameObject.SetActive(on);
        }
    }
}