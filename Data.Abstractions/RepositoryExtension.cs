using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Data.Abstractions
{
    public static class RepositoryExtension
    {
        private const char PropertySeprator = ',';
        private const char PropertyValueSeprator = ':';

        #region Paginate

        public static IQueryable<TEntity> DeferredPaginate<TEntity>(this IQueryable<TEntity> entities, int page,
            int pageSize)
        {
            return entities.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IList<TEntity> Paginate<TEntity>(this IQueryable<TEntity> entities, int page, int pageSize)
        {
            return entities.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public static async Task<IList<TEntity>> PaginateAsync<TEntity>(this IQueryable<TEntity> entities, int page,
            int pageSize)
        {
            return await entities.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        #endregion

        #region OrderBy

        // TODO: یکسان سازی این تکنولوژی برای جستجو و مرتب سازی در ریپوزیتوری بیس.
        private static IOrderedQueryable<TEntity> ApplyOrder<TEntity>(IQueryable<TEntity> source, string property,
            string methodName)
        {
            //string[] props = property.Split('.');
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            //foreach (string prop in props)
            //{
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo pi = type.GetProperty(property);
            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
            //}
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                              && method.IsGenericMethodDefinition
                              && method.GetGenericArguments().Length == 2
                              && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), type)
                .Invoke(null, new object[] {source, lambda});
            return (IOrderedQueryable<TEntity>) result;
        }

//        private static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string property)
//        {
//            return ApplyOrder(source, property, "OrderBy");
//        }

        private static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source,
            string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }

        private static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source,
            string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }

        private static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> source,
            string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }

        private static LambdaExpression CreatePropertyLambdaExpression<TEntity>(string property)
        {
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x"); // x
            Expression expr = arg;
            PropertyInfo pi = type.GetProperty(property);
            expr = Expression.Property(expr, pi); // x.Id
            type = pi.PropertyType;
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg); // x => x.Id

            return lambda;
        }

        private static Expression<Func<TEntity, bool>> CreateEqualComparisonLambdaExperession<TEntity>(string property,
            string value)
        {
            Type type = typeof(TEntity);
            var parameter = Expression.Parameter(type, "x"); // x
            Expression expr = parameter;
            PropertyInfo pi = type.GetProperty(property);
            expr = Expression.Property(expr, pi); // x.Id
            type = pi.PropertyType;
            var constant = Expression.Parameter(type, value); // 3
            var body = Expression.Equal(expr, constant); // x.Id >= 3
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter); // x => x.Id >= 3
            return lambda;
        }

        private static IQueryable<TEntity> ApplyFilter<TEntity>(this IQueryable<TEntity> source, string property,
            string value)
        {
            return source.Where(CreateEqualComparisonLambdaExperession<TEntity>(property, value));
        }

        private static IQueryable<TEntity> ApplyAllFilters<TEntity>(this IQueryable<TEntity> source, string properties)
        {
            string[] splitedProperties = properties.Split(PropertySeprator);
            if (!splitedProperties.Any())
                return source;
            for (int i = 0; i < splitedProperties.Length; i++)
            {
                string[] property = splitedProperties[i].Split(PropertyValueSeprator);
                if (property.Count() != 2)
                    return source;
                string propertyName = property[0];
                string filterValue = property[1];
                source = source.ApplyFilter(propertyName, filterValue);
            }

            return source;
        }

        public static IOrderedQueryable<TEntity> ApplyAllOrderBy<TEntity>(this IQueryable<TEntity> source,
            string properties)
        {
            var orderedSource = (IOrderedQueryable<TEntity>) source;
            string[] splitedProperties = properties.Split(PropertySeprator);
            if (!splitedProperties.Any())
                return orderedSource;
            string[] firstProperty = splitedProperties[0].Split(PropertyValueSeprator);
            if (firstProperty.Count() != 2)
                return orderedSource;
            string firstPropertyName = firstProperty[0];
            string firstPropertySortOrder = firstProperty[1];
            if (firstPropertySortOrder == "Asc")
                orderedSource = orderedSource.OrderBy(firstPropertyName);
            else if (firstPropertySortOrder == "Desc")
                orderedSource = orderedSource.OrderByDescending(firstPropertyName);
            else
                return orderedSource;
            for (int i = 1; i < splitedProperties.Length; i++)
            {
                string[] property = splitedProperties[i].Split(PropertyValueSeprator);
                if (property.Count() != 2)
                    return orderedSource;
                string propertyName = property[0];
                string sortOrder = property[1];
                if (sortOrder == "Asc")
                    orderedSource = orderedSource.ThenBy(propertyName);
                else if (sortOrder == "Desc")
                    orderedSource = orderedSource.ThenByDescending(propertyName);
                else
                    return orderedSource;
            }

            return orderedSource;
        }

        public static IOrderedQueryable<TEntity> ApplyOrderBy<TEntity>(this IQueryable<TEntity> query, string sort)
        {
            return query.OrderBy(sort.Replace(':',' '));
        }

        #endregion

        #region Filter

        public static IQueryable<TEntity> ApplyFilter<TEntity>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> filterExpression)
        {
            return source.Where(filterExpression).AsQueryable();
        }

        public static IQueryable<TEntity> ApplyFilter<TEntity>(this IQueryable<TEntity> query, string filterQuery)
        {
            var filters = filterQuery.Split(',');
            foreach (var filter in filters)
            {
                var operand = "=";
                var filterParams = filter.Split(':');
                if (filterParams.Length == 1)
                {
                    filterParams = filter.Split(">=".ToCharArray());
                    operand = ">=";
                }
                if (filterParams.Length == 1)
                {
                    filterParams = filter.Split("<=".ToCharArray());
                    operand = "<=";
                }
                if (filterParams.Length != 2) continue;
                var propertyName = filterParams[0];
                var propertyInfo = typeof(TEntity).GetProperties().SingleOrDefault(p =>
                    p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                if (propertyInfo == null) continue;
                var stringValue = filterParams[1];
//                var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
//                if(!converter.IsValid(value)) continue;
//                var val = converter.ConvertFromString(value);
                if (propertyInfo.PropertyType == typeof(bool))
                {
                    var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                    if(!converter.IsValid(stringValue)) continue;
                    var convertedValue = converter.ConvertFromString(stringValue);
                    var boolValue = Convert.ToBoolean(convertedValue);
                    query = query.Where(boolValue ? propertyName : $"!{propertyName}");
                }
                else
                {
                    query = query.Where(propertyName + " " + operand + " @0", (object) stringValue);
                }
            }

            return query;
        }

        #endregion
    }

    #region AddOrUpdate

    public static class DbSetExtension
    {
        public static void AddOrUpdate<T>(this DbSet<T> dbSet, T data) where T : class
        {
            var context = dbSet.GetContext();
            var ids = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name).ToList();

            var t = typeof(T);
            var keyFields = (
                from prop
                in t.GetProperties() let keyAttr = ids.Contains(prop.Name) where keyAttr select prop).ToList();

            if (keyFields.Count <= 0)
            {
                throw new Exception(
                    $"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
            }

            var entities = dbSet.AsNoTracking().ToList();
            foreach (var keyField in keyFields)
            {
                var keyVal = keyField.GetValue(data);
                entities = entities.Where(p => p.GetType().GetProperty(keyField.Name)?.GetValue(p).Equals(keyVal) == true)
                    .ToList();
            }

            var dbVal = entities.SingleOrDefault();
            if (dbVal != null)
            {
                context.Entry(dbVal).CurrentValues.SetValues(data);
                context.Entry(dbVal).State = EntityState.Modified;
                return;
            }

            dbSet.Add(data);
        }

        public static void AddOrUpdate<T>(this DbSet<T> dbSet, Expression<Func<T, object>> key, T data) where T : class
        {
            var context = dbSet.GetContext();
            var ids = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name);
            var t = typeof(T);
            var keyObject = key.Compile()(data);
            var keyFields = keyObject.GetType().GetProperties().Select(p => t.GetProperty(p.Name)).ToArray();
            if (keyFields == null)
            {
                throw new Exception(
                    $"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
            }

            var keyValues = keyFields.Select(p => p.GetValue(data));
            var entities = dbSet.AsNoTracking().ToList();
            var i = 0;
            foreach (var keyVal in keyValues)
            {
                entities = entities.Where(p => p.GetType().GetProperty(keyFields[i].Name)?.GetValue(p).Equals(keyVal)==true)
                    .ToList();
                i++;
            }

            if (entities.Any())
            {
                var dbVal = entities.SingleOrDefault();
                var keyAttrs = data.GetType().GetProperties().Where(p => ids.Contains(p.Name)).ToList();
                if (keyAttrs.Any() && dbVal !=null)
                {
                    foreach (var keyAttr in keyAttrs)
                    {
                        keyAttr.SetValue(data,
                            dbVal.GetType()
                                .GetProperties()
                                .FirstOrDefault(p => p.Name == keyAttr.Name)
                                ?.GetValue(dbVal));
                    }

                    context.Entry(dbVal).CurrentValues.SetValues(data);
                    context.Entry(dbVal).State = EntityState.Modified;
                    return;
                }
            }

            dbSet.Add(data);
        }
    }

    public static class HackyDbSetGetContextTrick
    {
        public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class
        {
            return (DbContext) dbSet
                .GetType().GetTypeInfo()
                .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(dbSet);
        }
    }

    #endregion
}