using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Sample.ConsoleApplication.Infrastructure
{
    /// <summary>
    /// Unity IOC 对象创建器
    /// </summary>
    public class Container
    {
        private static IUnityContainer UnityContainer { get; set; }

        private static void Init()
        {
            if (UnityContainer != null) return;

            var container = new UnityContainer();
            var section = (UnityConfigurationSection)System.Configuration.ConfigurationManager.GetSection(UnityConfigurationSection.SectionName);
            section.Configure(container);
            UnityContainer = container;
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>()
        {
            Init();
            return UnityContainer.Resolve<T>();
        }
    }
}
