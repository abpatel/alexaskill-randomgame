using Alexa.NET.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace NumberGameSkill
{
    static class  Extensions
    {
        public static T GetValue<T>(this Session session, string key)
        {
            T t = (T)Convert.ChangeType(session.Attributes[key], typeof(T));
            return t;
        }

        public static void SetValue<T>(this Session session, string key, T value)
        {
            session.Attributes[key] = value;
        }
    }
}
