using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
	[Serializable]
	public struct SteeringBehaviorData
	{
		[Tooltip("Велечина стремления к цели"), SerializeField, Range(1f, 100f)]
		public float MaxVelcity;
		[Tooltip("Максимальная скорость"), SerializeField, Range(1f, 10f)]
		public float MaxSpeed;
		[Tooltip("Расстояние начала прибытия"), SerializeField, Range(0.1f, 100f)]
		public float ArrivalDistance;

		[Tooltip("Расстояние до центра окружности блуждания"), SerializeField, Range(1f, 100f)]
		public float WanderCenterDistance;
		[Tooltip("Радиус окружности блуждания"), SerializeField, Range(1f, 100f)]
		public float WanderRadius;
		[Tooltip("Угл смещения при блуждании"), SerializeField, Range(1f, 100f)]
		public float WanderAngelRange;

		[Tooltip("Дистанция Обнаружения"), SerializeField, Range(0.5f, 100f)]
		public float DetectionDistance;
		[Tooltip("Дистанция атаки"), SerializeField, Range(0.5f, 100f)]
		public float AttackDistance;

		public GameObject Center;
	}

	public enum AttackType : byte
    {
		Strong = 0,
		Fast = 1,
    }

	public enum AIStateType : byte
	{
		None = 0,
		Wait = 1,
		Fight = 229,
		Move_Seek = 230,
		Move_Flee = 231,
		Move_Arrival = 232,
		Move_Wander = 233,
		Move_Pursuing = 234,
		Move_Evading = 235,
	}

	public enum AnimationType : byte
	{
		Move = 0,
		FastAttack = 1,
		StrongAttack = 2,
		Die = 3
	}

	[System.Flags]
	public enum IgnoreAxisType : byte
	{
		None = 0,
		X = 1,
		Y = 2,
		Z = 4
	}

	public enum Colour : byte
	{
		Red = 0,
		Green = 1,
		Blue = 2
	}

	public enum Masks : byte
    {
		Gate = 8,
		Unit = 9
    }



	[System.Serializable]
	public class AnimationKeyDictionary : SerializableDictionaryBase<AnimationType, string> { }
}
