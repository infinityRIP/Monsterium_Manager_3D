using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Stats
{
    [Serializable]
    public class Stat
    {
        public float BaseValue;
        
        public virtual float Value {
            get
            {
                if (isDirty || BaseValue != lastBaseValue)
                {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                }
                return _value;
            }}

        protected bool isDirty = true;
        protected float _value;
        protected float lastBaseValue = float.MaxValue;

        protected readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        private readonly Comparison<StatModifier> comparison;
        private readonly PredicateClosoure predicateClosoure;

        private class PredicateClosoure
        {
            public readonly Predicate<StatModifier> Predicate;
            public object SourceToRemove;

            public PredicateClosoure()
            {
                Predicate = modifier => modifier.Source == SourceToRemove;
            }
        }

        public Stat()
        {
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
            comparison = ComparedModifierOrder;
            predicateClosoure = new PredicateClosoure();
        }

        public Stat(float baseValue) : this() 
        {
            BaseValue = baseValue;
        }

        public virtual void AddModifier(StatModifier mod)
        {
            isDirty = true;
            statModifiers.Add(mod);
            statModifiers.Sort(ComparedModifierOrder);
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
           if (statModifiers.Remove(mod))
            {
                isDirty = true ;
                return true;
            }
           return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            predicateClosoure.SourceToRemove = source;
            int numRemovals = statModifiers.RemoveAll(predicateClosoure.Predicate);
            predicateClosoure.SourceToRemove = null;

            if (numRemovals > 0)
            {
                isDirty = true;
                return true;
            }
            return false;
        }
        protected virtual int ComparedModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
            {
                return -1;
            }
            else if (a.Order > b.Order)
            {
                return 1;
            }
            return 0;
        }
        protected virtual float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            statModifiers.Sort(comparison);

            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.Type == StatModifierType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModifierType.PercentAdd)
                {
                    sumPercentAdd += mod.Value;
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModifierType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }

                }
                else if (mod.Type == StatModifierType.PercentMult)
                {
                    finalValue *= 1 + mod.Value;
                }
                
            }  
            return (float)Math.Round(finalValue, 4);
        }
    }
}
