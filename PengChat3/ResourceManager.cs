using System;
using System.Reflection;
using System.Resources;

namespace PengChat3
{
    internal static class ResourceManager
    {
        private static Assembly m_ResData = null;
        private static System.Resources.ResourceManager m_ResMgr = null;

        public static void LoadResource(string fullPathAsm, string className)
        {
            m_ResData = Assembly.LoadFrom(fullPathAsm);
            m_ResMgr = new System.Resources.ResourceManager(className, m_ResData);
        }

        public static string GetStringByKey(string key)
        {
            object o = m_ResMgr.GetObject(key);
            return (o != null) ? o.ToString() : "";
        }

        public static object GetObjectByKey(string key)
        {
            return m_ResMgr.GetObject(key);
        }
    }
}