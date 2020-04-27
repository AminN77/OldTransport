using Cross.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Cross
{
    public class Utility : IUtility
    {
        public string[] GetEntityProperties(object entity, params string[] exceptProperties)
        {
            // Implement this method without Where for more performance after use models interface 
            // in RepositoryBase intead of classes.
            var propertyInfos = entity.GetType().GetProperties()/*.Where(p => !p.GetGetMethod().IsVirtual)*/.ToArray();
            var properties = new List<string>();
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyName = propertyInfo.Name;
                if (!exceptProperties.Contains(propertyName) && propertyInfo.GetMethod.IsFinal)
                    properties.Add(propertyName);
            }
            return properties.ToArray();
        }
    }
}
