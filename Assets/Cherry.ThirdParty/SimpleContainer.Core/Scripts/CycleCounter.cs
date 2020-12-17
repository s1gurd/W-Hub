using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleContainer
{
    internal class CycleCounter
    {
        private const int TYPES_LIMIT = 500;
        private const int INDENTATION_SPACES = 1;

        private int _indentation;
        private readonly List<TypePair> _types = new List<TypePair>();

        public void LogType(Type type)
        {
            _types.Add(new TypePair { Indentation = _indentation, Type = type });

            if (_types.Count > TYPES_LIMIT)
            {
                var circleString = GetDependenciesString(_types);
                throw new Exception($"Circular dependency detected:{Environment.NewLine}{circleString}");
            }
        }

        public string Pop()
        {
            var result = GetDependenciesString(_types);
            _types.Clear();
            return result;
        }

        public void Indent()
        {
            _indentation++;
        }

        public void Unindent()
        {
            _indentation--;
        }

        private string GetDependenciesString(List<TypePair> typePairs)
        {
            return string.Join($"{Environment.NewLine}", typePairs.ToArray().Reverse().Select(FormatTypePair));
        }

        private string FormatTypePair(TypePair pair)
        {
            return $"{new string(Enumerable.Range(0, pair.Indentation * INDENTATION_SPACES).Select(_ => ' ').ToArray())}-> {pair.Type.Name}";
        }

        private class TypePair
        {
            public Type Type { get; set; }
            public int Indentation { get; set; }
        }
    }
}