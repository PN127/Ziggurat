using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
    //родительский класс для юнитов
    public class NPC : MonoBehaviour
    {        
        public float Health { get; set; }
        public float Speed { get; set; }
        public float FastAttackDamage { get; set; }
        public float StrongAttackDamage { get; set; }
        public float FrequencyFastAttackPerMinute { get; set; }
        public float DistanceAttack { get; set; }
        public float DistanceDetection { get; set; }
        public float WanderAngel { get; set; }
        public float ChanceMiss { get; set; }
        public float ChanceCriticalDamage { get; set; }

        //id можно использовать для отслеживания ошибок
        public string Id { get; set; }

        public Colour Colour { get; set; }
        public AIStateType State { get; set; }
               
    }
}
