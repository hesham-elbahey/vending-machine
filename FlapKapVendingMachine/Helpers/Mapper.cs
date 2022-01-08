using System;
using System.Reflection;
using FlapKapVendingMachine.Helpers.Interfaces;

namespace FlapKapVendingMachine.Helpers
{
    public class Mapper: IMapper
    {
        public void Map<TSource, TTarget>(TSource source, ref TTarget target)
        {
            PropertyInfo[] sourceProperties = typeof(TSource).GetProperties();
            PropertyInfo[] targetProperties = typeof(TTarget).GetProperties();

            foreach (var targetProperty in targetProperties)
            {
                foreach (var sourceProperty in sourceProperties)
                {
                    if (targetProperty.Name == sourceProperty.Name
                        && sourceProperty.GetValue(source) != null
                        && !sourceProperty.PropertyType.FullName.Contains(nameof(System.Collections)))
                    {
                        targetProperty.SetValue(target, sourceProperty.GetValue(source));
                        continue;
                    }
                }
            }
        }
    }
}
