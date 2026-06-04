#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ErmineGames.Utils
{
    /// <summary>
    /// Reflection utility methods designed exclusively for editor-use.
    /// </summary>
    public static class ReflectionUtils
    {
        private const int BaseSearchDepth = 10;
        private const int BaseRecursionDepth = 5;
        
        private static readonly Dictionary<Type, CachedTypeData> typeCache = new();

        private struct CachedTypeData
        {
            public FieldInfo[] Fields;
            public PropertyInfo[] Properties;
        }

        public static Type FindCallerInStack(Type baseTypeFilter, int skipFrames = 1, int searchDepth = BaseSearchDepth)
        {
            var stackTrace = new StackTrace(fNeedFileInfo: false);
            searchDepth = Math.Min(searchDepth + skipFrames, stackTrace.FrameCount);
            
            for (int depth = skipFrames; depth < searchDepth; depth++)
            {
                var frame = stackTrace.GetFrame(depth);
                var callerType = frame?.GetMethod()?.DeclaringType;

                if (callerType == null)
                {
                    continue;
                }

                if (IsCompilerGenerated(callerType) && callerType.DeclaringType != null)
                {
                    callerType = callerType.DeclaringType;
                }
                
                if (callerType.IsSubclassOf(baseTypeFilter) || callerType == baseTypeFilter)
                {
                    return callerType;
                }
            }
            
            return null;
        }
        
        private static bool IsCompilerGenerated(Type type)
        {
            return type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
        }
        
        public static ReflectedData ReflectType(object value, Type[] superficiallyReflectTypes = null, int maxRecursionDepth = BaseRecursionDepth)
        {
            if (value == null)
            {
                return null;
            }
            
            return ReflectMember(value.GetType(), value, superficiallyReflectTypes, new HashSet<object>(), maxRecursionDepth, 0);
        }
        
        private static ReflectedData ReflectMember(
            MemberInfo memberInfo, 
            object memberValue, 
            Type[] superficiallyReflectTypes, 
            HashSet<object> visitedObjects,
            int maxDepth,
            int currentDepth)
        {
            if (memberInfo == null)
            {
                return null;
            }
            
            var data = new ReflectedData
            {
                MemberName = memberInfo.Name,
                MemberType = memberInfo.MemberType
            };

            if (memberValue == null)
            {
                data.MemberValueType = ReflectedData.ReflectedMemberValueType.Simple;
                data.SimpleValue = "null";
                return data;
            }
            
            var type = memberValue.GetType();

            if (currentDepth > maxDepth)
            {
                data.MemberValueType = ReflectedData.ReflectedMemberValueType.Simple;
                data.SimpleValue = $"[Depth Limit Reached: {type.Name}]";
                return data;
            }

            if (!type.IsValueType)
            {
                if (visitedObjects.Contains(memberValue))
                {
                    data.MemberValueType = ReflectedData.ReflectedMemberValueType.Simple;
                    data.SimpleValue = $"[Cyclic Reference: {type.Name}]";
                    return data;
                }
                
                visitedObjects.Add(memberValue);
            }

            var reflectedType = memberInfo as Type ?? memberInfo.ReflectedType;

            if (type.IsPrimitive || 
                type == typeof(string) || 
                type.IsEnum || 
                (reflectedType != null && type.Assembly != reflectedType.Assembly))
            {
                data.MemberValueType = ReflectedData.ReflectedMemberValueType.Simple;
                data.SimpleValue = memberValue.ToString();
                return data;
            }

            if (!typeCache.TryGetValue(type, out var cachedData))
            {
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                
                if (IsSuperficiallyReflection(type, superficiallyReflectTypes))
                {
                    bindingFlags |= BindingFlags.DeclaredOnly;
                }

                cachedData = new CachedTypeData
                {
                    Fields = type.GetFields(bindingFlags),
                    Properties = type.GetProperties(bindingFlags)
                        .Where(p => p.GetIndexParameters().Length == 0)
                        .ToArray()
                };
                typeCache[type] = cachedData;
            }

            var properties = cachedData.Properties;
            var fields = cachedData.Fields;
            var membersCount = properties.Length + fields.Length;

            if (membersCount == 0)
            {
                data.MemberValueType = ReflectedData.ReflectedMemberValueType.Simple;
                data.SimpleValue = memberValue.ToString();
            }
            else
            {
                data.MemberValueType = ReflectedData.ReflectedMemberValueType.Complex;
                data.ComplexValue = new ReflectedData[membersCount];
                
                int index = 0;
                
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }
                    
                    try
                    {
                        var val = propertyInfo.GetValue(memberValue);
                        data.ComplexValue[index++] = ReflectMember(propertyInfo, val, superficiallyReflectTypes, visitedObjects, maxDepth, currentDepth + 1);
                    }
                    catch (Exception ex)
                    {
                        data.ComplexValue[index++] = new ReflectedData { MemberName = propertyInfo.Name, SimpleValue = $"[Error: {ex.Message}]" };
                    }
                }
                
                foreach (var fieldInfo in fields)
                {
                    try
                    {
                        var val = fieldInfo.GetValue(memberValue);
                        data.ComplexValue[index++] = ReflectMember(fieldInfo, val, superficiallyReflectTypes, visitedObjects, maxDepth, currentDepth + 1);
                    }
                    catch (Exception ex)
                    {
                        data.ComplexValue[index++] = new ReflectedData { MemberName = fieldInfo.Name, SimpleValue = $"[Error: {ex.Message}]" };
                    }
                }
                
                if (index < membersCount)
                {
                    var complexValue = data.ComplexValue;
                    Array.Resize(ref complexValue, index);
                    data.ComplexValue = complexValue;
                }
            }
            
            return data;
        }

        private static bool IsSuperficiallyReflection(Type type, Type[] superficiallyReflectTypes)
        {
            return superficiallyReflectTypes != null && superficiallyReflectTypes.Any(t => type.IsSubclassOf(t) || type == t);
        }

        public static void ClearCache() => typeCache.Clear();

        public class ReflectedData
        {
            public enum ReflectedMemberValueType
            {
                None = 0,
                Simple = 1,
                Complex = 2,
            }
            
            public MemberTypes MemberType { get; set; }
            public ReflectedMemberValueType MemberValueType { get; set; }
            public string MemberName { get; set; }
            public string SimpleValue { get; set; }
            public ReflectedData[] ComplexValue { get; set; }
        }
    }
}
#endif
