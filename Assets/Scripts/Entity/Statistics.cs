using System;
using Mekaiju.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Mekaiju.Entity
{
    using HealthStatisticValue = Mekaiju.Utils.EnumArray<Mekaiju.MechaPart, float>;

    [Serializable]
    public abstract class IStatistic
    {
        public abstract T Apply<T>(ModifierCollection p_collection);
        public abstract T Get<T>();
    }

    public abstract class Statistic<Tp> : IStatistic
    {
        [SerializeField]
        protected Tp _value;

        public override T Get<T>()
        {
            if (_value is T t_casted)
            {
                return t_casted;
            }
            else
            {
                Debug.LogWarning($"(Get)Trying to cast statistics with wrong type (eg. {typeof(T).Name})!");
                return default(T);
            }
        }
    }

    [Serializable]
    public class FloatStatistic : Statistic<float>
    {
        public override T Apply<T>(ModifierCollection p_collection)
        {
            var t_compute = p_collection.ComputeValue(_value);
            if (t_compute is T t_casted)
            {
                return t_casted;
            }
            else
            {
                Debug.LogWarning($"(Apply)Trying to cast statistics with wrong type (eg. {typeof(T).Name})!");
                return default(T);
            }
        }
    }

    [Serializable]
    public class HealthStatistic : Statistic<HealthStatisticValue>
    {
        public override T Apply<T>(ModifierCollection p_collection)
        {
            var t_compute = _value.Select((t_key, t_value) => p_collection.ComputeValue(t_value));
            if (t_compute is T t_casted)
            {
                return t_casted;
            }
            else
            {
                Debug.LogWarning($"(Apply)Trying to cast statistics with wrong type (eg. {typeof(T).Name})!");
                return default(T);
            }
        }
    }

    public enum StatisticKind
    {
        Speed, Defense, Damage, Health
    }
}
