using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Blog.Infrastructure.HelperServices
{
    public class TypeService : ITypeService
    {
        /// <summary>
        /// 判断类中是否有指定的属性字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        public bool TypeHasProperties<T>(string fields)
        {
            if (!string.IsNullOrEmpty(fields)) {
                var fieldsAfterSplit = fields.Split(',');
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    if (string.IsNullOrEmpty(propertyName))
                        continue;

                    var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase);
                    if (null == propertyInfo) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
