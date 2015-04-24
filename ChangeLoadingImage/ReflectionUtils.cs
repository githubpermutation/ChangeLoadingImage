using System;
using System.Reflection;

namespace ChangeLoadingImage
{
    public static class ReflectionUtils
    {
        //courtesy of nlight
        public static void WritePrivate<T> (UnityEngine.Object o, string fieldName, object value)
        {
            FieldInfo[] fields = typeof(T).GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (FieldInfo f in fields) {
                if (f.Name == fieldName) {
                    f.SetValue (o, value);
                    break;
                }
            }
        }

        public static T getReflectedValue<T, T2> (string fieldname, T2 obj)
        {
            FieldInfo field = typeof(T2).GetField (fieldname, BindingFlags.Instance | BindingFlags.NonPublic);

            T retval = default(T);
            object value = field.GetValue (obj);
            if (value != null) {
                retval = (T)value;
            }
            
            return retval;
        } 
    }
}