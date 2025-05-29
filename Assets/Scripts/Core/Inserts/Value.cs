using System;

using UnityEngine;

namespace Core.Module
{
    /// <summary>
    /// Generic Value Data Type
    /// </summary>
    [System.Serializable]
    public struct Value
    {
        private readonly object value;
        public ValueType type;

        public bool IsNull
        {
            get
            {
                return value == null;
            }
        }
        public Value(object value, ValueType type)
        {
            this.value = value;
            this.type = type;
        }

        public float GetNumber()
        {
            if (type is not (ValueType.Number))
            {
                if(type == ValueType.Fraction)
                {
                    var frac = GetFraction();
                    return frac.numerator / (float)frac.denominator;
                }
                return 0.0f;
            }
            return Convert.ToSingle(value);
        }

        public Fraction GetFraction()
        {
            if(type != ValueType.Fraction)
            {
                if(type is ValueType.Number)
                {
                    return new Fraction(GetNumber());
                }
                else
                {
                    return new Fraction(1, 1);
                }
            }

            return (Fraction)value;
        }

        public string GetString()
        {
            return value == null ? "" : value.ToString();
        }

        public bool GetBoolean()
        {
            return (bool)value;
        }

        public override string ToString()
        {
            switch(type)
            {
                case ValueType.Number:
                    {
                        float val = GetNumber();
                        if(IsWholeNumber(val))
                        {
                            return ((int)val).ToString();
                        }

                        return val.ToString();
                    }
            }
            return value.ToString();
        }

        /// <summary>
        /// Convert string to specified value
        /// </summary>
        /// <param name="param">string parameter</param>
        /// <param name="type">value type</param>
        /// <returns>Value struct with parameter and type specified</returns>
        public static Value Parse(string param, ValueType type)
        {
            object obj = null;
            switch (type)
            {
                case ValueType.Number:
                    {
                        _ = float.TryParse(param, out float val);
                        obj = val;
                        break;
                    }
                case ValueType.Word or ValueType.Letter:
                    {
                        // if only one letter found, change to Letter Value Type
                        if (param.Length <= 1)
                        {
                            obj = param.Length == 0 ? ' ' : param[0];
                            type = ValueType.Letter;
                        }
                        else
                        {
                            obj = param;
                            type = ValueType.Word;
                        }
                        break;
                    }
                case ValueType.Boolean:
                    {
                        obj = param is "True" or "true" or "t" or "1";
                        break;
                    }
            }

            return new Value(obj, type);
        }

        public static implicit operator Value(int number)
        {
            return new Value(number, ValueType.Number);
        }

        public static implicit operator Value(float number)
        {
            return new Value(number, ValueType.Number);
        }

        public static implicit operator Value(string word)
        {
            return new Value(word, ValueType.Word);
        }

        public static implicit operator int(Value number)
        {
            return (int)number.GetNumber();
        }

        public static implicit operator float(Value number)
        {
            return number.GetNumber();
        }

        public static implicit operator string(Value number)
        {
            return number.GetString();
        }

        public static bool IsWholeNumber(float value)
        {
            return Mathf.Abs(value - Mathf.Floor(value)) < Mathf.Epsilon;
        }
    }

    public enum ValueType
    {
        Number, Letter, Word, Option, Boolean, Fraction
    }
}