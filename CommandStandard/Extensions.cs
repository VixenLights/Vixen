using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandStandard {
    internal static class Extensions {
        public static bool HasAttribute(this Type t, Type attrType) {
            return t.GetCustomAttributes(attrType, false).Length > 0;
        }

        public static IEnumerable<Type> FindAttributedTypes(this Type targetType, Type attributeType) {
            return
                from t in targetType.GetNestedTypes()
                where t.HasAttribute(attributeType)
                select t;
        }

        public static bool HasFieldOfType<T>(this Type targetType, Type attributeType, T valueToMatch) {
            foreach(System.Reflection.FieldInfo fi in targetType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) {
                if(fi.GetCustomAttributes(attributeType, false).Length > 0) {
                    if(fi.GetValue(null).Equals(valueToMatch)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
