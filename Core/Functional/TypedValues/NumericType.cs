﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Functional
{
    public class NumericType<T> : NumericType
    {
        public new static readonly NumericType<T> Unit = new NumericType<T>();

        public NumericType(T value, int power = 1) : base(new TypeStruct(value.ToString(), power))
        {
        }

        protected NumericType() : base(new TypeStruct("Unit", 0))
        {
        }
    }

    public class NumericType
    {
        public static readonly NumericType Unit = new NumericType();

        protected struct TypeStruct
        {
            public readonly String Type;
            public readonly int Power;

            public TypeStruct(string type, int power)
            {
                this.Type = type;
                this.Power = power;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TypeStruct))
                {
                    return false;
                }

                var @struct = (TypeStruct)obj;
                return Type == @struct.Type &&
                       Power == @struct.Power;
            }

            public override string ToString()
            {
                return this.Power != 0 ? (this.Type + (this.Power != 1 ? $"^{this.Power}" : "")) : "Unit";
            }
        }

        protected readonly SortedList<string, TypeStruct> types;

        private NumericType()
        {
            this.types = new SortedList<string, TypeStruct>
            {
                { "Unit", new TypeStruct("Unit", 0) }
            };
        }

        protected NumericType(TypeStruct type)
        {
            this.types = new SortedList<string, TypeStruct>
            {
                { type.Type, type }
            };
        }

        protected NumericType(params TypeStruct[] types)
        {
            this.types = new SortedList<string, TypeStruct>();
            foreach (var type in types)
            {
                this.types.Add(type.Type, type);
            }
        }

        protected NumericType(IEnumerable<TypeStruct> types)
        {
            this.types = new SortedList<string, TypeStruct>();
            foreach (var type in types)
            {
                this.types.Add(type.Type, type);
            }
        }

        public static NumericType CreateNumericType(params NumericType[] others)
        {
            var result = new NumericType();
            foreach (var other in others)
            {
                result = result.Multiply(other);
            }

            return result;
        }

        public override string ToString()
        {
            var result = "";
            if (this.types.Values.Count == 1)
            {
                result = this.types.Values[0].ToString();
            }
            else
            {
                result = "(";
                foreach (var type in this.types.Values)
                {
                    result += $"({type.ToString()})";
                }
                result += ")";
            }
            return result;
        }

        public string ToString(string formatting, params object[] args)
        {
            return string.Format(formatting, args) + this.ToString();
        }

        public static NumericType operator *(NumericType first, NumericType second) => first.Multiply(second);

        public virtual NumericType Multiply(NumericType second)
        {
            if (this == Unit || (this.types.Count == 1 && this.types.ContainsKey("Unit")))
            {
                return new NumericType(second.types.Values);
            }

            var resultTypes = new List<TypeStruct>();

            foreach (var key in this.types.Keys)
            {
                if (second.types.TryGetValue(key, out var otherType))
                {
                    var newPower = this.types[key].Power + otherType.Power;
                    if (newPower != 0)
                    {
                        resultTypes.Add(new TypeStruct(key, newPower));
                    }
                }
                else
                {
                    resultTypes.Add(new TypeStruct(key, this.types[key].Power));
                }
            }

            return new NumericType(resultTypes);
        }

        public static NumericType operator /(NumericType first, NumericType second) => first.Divide(second);

        public virtual NumericType Divide(NumericType second)
        {
            return this.Multiply(new NumericType(second.types.Values.Select((value) => new TypeStruct(value.Type, -1 * value.Power))));
        }

        public static bool operator ==(NumericType first, NumericType second)        
            {
                return !first.Equals(null) && !second.Equals(null) && first.Equals(second);
            }

        public static bool operator !=(NumericType first, NumericType second)
            {
                return !first.Equals(null) && !second.Equals(null) && !(first.Equals(second));
            }

        public bool Equals(NumericType other)
        {
            var isNotNull = !(other is null);
            if (isNotNull)
            {
                var allTypesMatch = this.types.Count == other.types.Count &&
                    this.types.All((pair) => other.types.TryGetValue(pair.Key, out var otherValue) && otherValue.Power == pair.Value.Power);
                var isEitherUnit = this.types.ContainsKey("Unit") || other.types.ContainsKey("Unit");

                return allTypesMatch || isEitherUnit;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
           return !(obj is null) && this.Equals(obj as NumericType);
        }
    }   
}
