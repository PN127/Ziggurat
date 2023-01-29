using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
    public class NPC : MonoBehaviour
    {
        public float Health { get; set; }
        public float Speed { get; set; }
        public float FastAttackDamage { get; set; }
        public float SlowAttackDamage { get; set; }
        public float FrequencyFastAttackPerMinute { get; set; }
        public float DistanceAttack { get; set; }
        public float DistanceDetection { get; set; }
        

        public Colour UnitColour { get; set; }
    }
}
