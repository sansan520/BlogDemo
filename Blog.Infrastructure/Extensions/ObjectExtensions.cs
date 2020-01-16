using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Blog.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 单个对象资源塑型
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static ExpandoObject ToDynamic<TSource>(this TSource source,string fields=null)
        {
            if (source==null) {
                throw new ArgumentNullException(nameof(source));
            }
            var dataShapedObject = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(fields)) {
                //BindingFlags System.Reflection;
                PropertyInfo[] propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase| BindingFlags.Public|BindingFlags.Instance);
                foreach (PropertyInfo item in propertyInfos) {
                    var propertyValue = item.GetValue(source);
                    ((IDictionary<string, object>)dataShapedObject).Add(item.Name,propertyValue);
                }
                return dataShapedObject;
            }
            var fieldsAfterSplit = fields.Split(',').ToList();
            foreach (string field in fieldsAfterSplit) {
                var propertyName = field.Trim();
                var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    throw new Exception($"Can't found property ¡®{typeof(TSource)}¡¯ on ¡®{propertyName}¡¯");
                }
                var propertyValue = propertyInfo.GetValue(source);
                ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }
            return dataShapedObject;
        }
    }
}
