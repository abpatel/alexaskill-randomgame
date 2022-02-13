using Alexa.NET.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill
{
    static class  Extensions
    {
        public static T GetValue<T>(this Session session, string key)
        {
            T t = (T)Convert.ChangeType(session.Attributes[key], typeof(T));
            return t;
        }

        public static bool TryGetValue<T>(this Session session, string key, out T t)
        {
            t = default(T);
            object result;
            if(session.Attributes.TryGetValue(key, out result))
            {
                t = (T)Convert.ChangeType(result, typeof(T));
                return true;
            }
            return false;
        }

        public static void SetValue<T>(this Session session, string key, T value)
        {
            session.Attributes[key] = value;
        }
    }
}
