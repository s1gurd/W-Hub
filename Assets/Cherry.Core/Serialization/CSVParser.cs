using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GameFramework.Example.Serialization
{
    public sealed class CSVParser
    {
        public List<Dictionary<string, string>> Parse(string text)
        {
            var result = new List<Dictionary<string, string>>();

            using (var stringReader = new StringReader(text))
            {
                string line;

                string[] header = null;

                while ((line = stringReader.ReadLine()) != null)
                {
                    var parts = line.Split('\t');

                    if (header == null)
                    {
                        header = parts;
                    }
                    else
                    {
                        var dictionary = new Dictionary<string, string>();

                        for (var i = 0; i < parts.Length; i++)
                        {
                            dictionary.Add(ProcessColumnName(header[i]), parts[i]);
                        }

                        result.Add(dictionary);
                    }
                }
            }

            return result;
        }

        public IList<T> PopObjects<T>(List<Dictionary<string, string>> entries)
            where T : new()
        {
            var result = new List<T>();

            for (var i = entries.Count - 1; i >= 0; i--)
            {
                var entry = entries[i];

                if (TryParseObject<T>(entry, out var parsedObject))
                {
                    result.Add(parsedObject);
                    entries.RemoveAt(i);
                }
            }

            return result;
        }

        private string ProcessColumnName(string columnName)
        {
            return columnName.TrimStart('*').ToLower();
        }

        private string ProcessFieldName(string fieldName)
        {
            return fieldName.ToLower();
        }

        private bool TryParseObject<T>(Dictionary<string, string> entry, out T result)
            where T : new()
        {
            if (entry != null &&
                entry.TryGetValue(ProcessFieldName("CodeClass"), out var codeClass) &&
                string.Equals(typeof(T).Name, codeClass, StringComparison.Ordinal))
            {
                result = new T();
                FillFieldValues(result, entry);
                return true;
            }

            result = default;

            return false;
        }

        private void FillFieldValues<T>(T result, Dictionary<string, string> dictionary)
        {
            var fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                if (dictionary.TryGetValue(ProcessFieldName(field.Name), out var value))
                {
                    var fieldType = field.FieldType;
                    var parsedValue = ParseValue(fieldType, value);

                    field.SetValue(result, parsedValue);
                }
            }
        }

        private object ParseValue(Type targetType, string value)
        {
            object parsedValue;

            if (targetType == typeof(string))
            {
                parsedValue = value;
            }
            else if (targetType == typeof(int))
            {
                if (int.TryParse(value, out var intValue))
                {
                    parsedValue = intValue;
                }
                else
                {
                    throw new FormatException($"Cannot parse value '{value}' as int");
                }
            }
            else if (targetType == typeof(float))
            {
                if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatValue))
                {
                    parsedValue = floatValue;
                }
                else
                {
                    throw new FormatException($"Cannot parse value '{value}' as float");
                }
            }
            else if (targetType.IsEnum)
            {
                try
                {
                    parsedValue = Enum.Parse(targetType, value);
                }
                catch (Exception exception)
                {
                    throw new FormatException($"Cannot parse value '{value}' as enum {targetType.Name}, exception: {exception}");
                }
            }
            else if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType();
                parsedValue = typeof(CSVParser).GetMethod(nameof(ParseArray), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(elementType).Invoke(this, new object [] { value });
            }
            else if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = targetType.GetGenericArguments()[0];
                parsedValue = typeof(CSVParser).GetMethod(nameof(ParseList), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(elementType).Invoke(this, new object[] { value });
            }
            else
            {
                throw new InvalidOperationException($"Invalid field type '{targetType.Name}' to write value '{value}'");
            }

            return parsedValue;
        }

        private object ParseList<T>(string value)
        {
            return ParseEnumerable<T>(value).ToList();
        }

        private object ParseArray<T>(string value)
        {
            return ParseEnumerable<T>(value).ToArray();
        }

        private IEnumerable<T> ParseEnumerable<T>(string value)
        {
            var parts = value.Split(';');

            foreach (var part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    yield return (T)ParseValue(typeof(T), part);
                }
            }
        }
    }
}