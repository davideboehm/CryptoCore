using System;

namespace Core.Functional
{
    public class Numeric
    {
        public static Numeric Zero = new Numeric();

        private Numeric()
        {
            type = ValueType.zero;
        }

        private enum ValueType
        {
            zero,
            intType,
            doubleType,
            decimalType,
            floatType
        }
        private ValueType type;
        private int intValue;
        private double doubleValue;
        private decimal decimalValue;
        private float floatValue;

        public static bool operator !=(Numeric first, Numeric second)
        {
            return !(first == second);
        }

        public static bool operator ==(Numeric first, Numeric second)
        {
            switch (first.type)
            {
                case ValueType.intType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.intValue == second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.intValue == second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.intValue == second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return (float)first.intValue == second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.doubleType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.doubleValue == second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.doubleValue == second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.doubleValue == second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.doubleValue == second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.decimalType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.decimalValue == second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.decimalValue == (decimal)second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return first.decimalValue == second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.decimalValue == (decimal)second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.floatType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.floatValue == second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.floatValue == second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.floatValue == second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.floatValue == second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public static Numeric operator /(Numeric first, Numeric second)
        {
            switch (first.type)
            {
                case ValueType.intType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.intValue / second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.intValue / second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.intValue / second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return (float)first.intValue / second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.doubleType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.doubleValue / second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.doubleValue / second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.doubleValue / second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.doubleValue / second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.decimalType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.decimalValue / second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.decimalValue / (decimal)second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return first.decimalValue / second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.decimalValue / (decimal)second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.floatType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.floatValue / second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.floatValue / second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.floatValue / second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.floatValue / second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
        
        public static Numeric operator -(Numeric first, Numeric second)
        {
            switch (first.type)
            {
                case ValueType.intType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.intValue - second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.intValue - second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.intValue - second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return (float)first.intValue - second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.doubleType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.doubleValue - second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.doubleValue - second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.doubleValue - second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.doubleValue - second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.decimalType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.decimalValue - second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.decimalValue - (decimal)second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return first.decimalValue - second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.decimalValue - (decimal)second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.floatType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.floatValue - second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.floatValue - second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.floatValue - second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.floatValue - second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public static Numeric operator +(Numeric first, Numeric second)
        {
            switch (first.type)
            {
                case ValueType.intType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.intValue + second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.intValue + second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.intValue + second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return (float)first.intValue + second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.doubleType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.doubleValue + second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.doubleValue + second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.doubleValue + second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.doubleValue + second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.decimalType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.decimalValue + second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.decimalValue + (decimal)second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return first.decimalValue + second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.decimalValue + (decimal)second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.floatType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.floatValue + second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.floatValue + second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.floatValue + second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.floatValue + second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public static Numeric operator *(Numeric first, Numeric second)
        {
            switch (first.type)
            {
                case ValueType.intType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.intValue * second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.intValue * second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.intValue * second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return (float)first.intValue * second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.doubleType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.doubleValue * second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.doubleValue * second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.doubleValue * second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.doubleValue * second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.decimalType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.decimalValue * second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.decimalValue * (decimal)second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return first.decimalValue * second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.decimalValue * (decimal)second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                case ValueType.floatType:
                    {
                        switch (second.type)
                        {
                            case ValueType.intType:
                                {
                                    return first.floatValue * second.intValue;
                                }
                            case ValueType.doubleType:
                                {
                                    return first.floatValue * second.doubleValue;
                                }
                            case ValueType.decimalType:
                                {
                                    return (decimal)first.floatValue * second.decimalValue;
                                }
                            case ValueType.floatType:
                                {
                                    return first.floatValue * second.floatValue;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
        
        public static implicit operator decimal(Numeric value)
        {
            switch (value.type)
            {
                case ValueType.intType:
                    {
                        return value.intValue;
                    }
                case ValueType.doubleType:
                    {
                        return (decimal) value.doubleValue;
                    }
                case ValueType.decimalType:
                    {
                        return value.decimalValue;
                    }
                case ValueType.floatType:
                    {
                        return (decimal) value.floatValue;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public static implicit operator double(Numeric value)
        {
            switch (value.type)
            {
                case ValueType.intType:
                    {
                        return value.intValue;
                    }
                case ValueType.doubleType:
                    {
                        return value.doubleValue;
                    }
                case ValueType.decimalType:
                    {
                        return (double) value.decimalValue;
                    }
                case ValueType.floatType:
                    {
                        return value.floatValue;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public static implicit operator int(Numeric value)
        {
            switch (value.type)
            {
                case ValueType.intType:
                    {
                        return value.intValue;
                    }
                case ValueType.doubleType:
                    {
                        return (int) value.doubleValue;
                    }
                case ValueType.decimalType:
                    {
                        return (int) value.decimalValue;
                    }
                case ValueType.floatType:
                    {
                        return (int) value.floatValue;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public static implicit operator float(Numeric value)
        {
            switch (value.type)
            {
                case ValueType.intType:
                    {
                        return value.intValue;
                    }
                case ValueType.doubleType:
                    {
                        return (float) value.doubleValue;
                    }
                case ValueType.decimalType:
                    {
                        return (float) value.decimalValue;
                    }
                case ValueType.floatType:
                    {
                        return value.floatValue;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public static implicit operator Numeric(decimal value)
        {
            return new Numeric
            {
                type = ValueType.decimalType,
                decimalValue = value
            };
        }

        public static implicit operator Numeric(int value)
        {
            return new Numeric
            {
                type = ValueType.intType,
                intValue = value
            };
        }

        public static implicit operator Numeric(double value)
        {
            return new Numeric
            {
                type = ValueType.doubleType,
                doubleValue = value
            };
        }


        public static implicit operator Numeric(float value)
        {
            return new Numeric
            {
                type = ValueType.floatType,
                floatValue = value
            };
        }
    }
}
