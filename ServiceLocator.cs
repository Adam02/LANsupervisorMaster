using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LANSupervisor
{
    class ServiceLocator
    {
        static Dictionary<Type, Object> services;
        static ServiceLocator()
        {
            services = new Dictionary<Type, object>();
        }

        public static T GetService<T>() where T : new()
        {
            if (!services.ContainsKey(typeof(T)))
            {
                services.Add(typeof(T), new T());
            }
            return (T)services[typeof(T)];
        }
    }
}
