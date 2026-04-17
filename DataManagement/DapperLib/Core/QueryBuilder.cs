using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

/*
HOW TO USE QueryBuilder<T, TResult>

1) SELECT + JOIN + WHERE + ORDER BY
var sql = new QueryBuilder<Person, object>()
    .InnerJoin<Department>((p, d) => p.DepartmentId == d.Id, "d")
    .SelectRaw("t.Id, t.Name, d.DepartmentName")
    .Where(p => p.Age >= 18)
    .OrderBy(p => p.Name)
    .BuildQuery();

2) GROUP BY + HAVING
var sql = new QueryBuilder<Person, object>()
    .InnerJoin<Department>((p, d) => p.DepartmentId == d.Id, "d")
    .SelectRaw("d.DepartmentName, COUNT(t.Id) AS PersonCount")
    .GroupBy<Department>((p, d) => d.DepartmentName)
    .HavingRaw("COUNT(t.Id) >= 2")
    .OrderByRaw("PersonCount DESC")
    .BuildQuery();

3) INSERT
var sql = new QueryBuilder<Person, object>()
    .Insert(() => new Person { Name = "Tien", Age = 25, Email = "tien@gmail.com" })
    .BuildQuery();

4) UPDATE
var sql = new QueryBuilder<Person, object>()
    .Update(() => new Person { Email = "new@gmail.com", Age = 26 })
    .Where(p => p.Id == 10)
    .BuildQuery();

5) DELETE
var sql = new QueryBuilder<Person, object>()
    .Delete()
    .Where(p => p.Id == 10)
    .BuildQuery();
*/

namespace DataManagement.DapperLib.Core
{
    public enum PgJoinType
    {
        Inner,
        Left,
        Right,
        Full
    }

    internal enum QueryMode
    {
        Select,
        Insert,
        Update,
        Delete
    }

    public class QueryBuilder<T, TResult> : IQueryBuilder<T, TResult> where T : class
    {
        private const string BaseAlias = "t";
        private readonly Dictionary<Type, string> _aliases = new Dictionary<Type, string> { [typeof(T)] = BaseAlias };
        private readonly List<string> _joins = new List<string>();

        private QueryMode _mode = QueryMode.Select;
        private string _selectClause = BaseAlias + ".*";
        private string _whereClause = string.Empty;
        private string _groupByClause = string.Empty;
        private string _havingClause = string.Empty;
        private readonly List<string> _orderByClauses = new List<string>();
        private int? _limit;
        private int? _offset;
        private readonly List<string> _insertColumns = new List<string>();
        private readonly List<string> _insertValues = new List<string>();
        private readonly List<string> _setClauses = new List<string>();

        /// <summary>
        /// Map enum join type to PostgreSQL join keyword.
        /// </summary>
        private static string GetJoinKeyword(PgJoinType joinType)
        {
            switch (joinType)
            {
                case PgJoinType.Inner:
                    return "INNER JOIN";
                case PgJoinType.Left:
                    return "LEFT JOIN";
                case PgJoinType.Right:
                    return "RIGHT JOIN";
                case PgJoinType.Full:
                    return "FULL JOIN";
                default:
                    throw new NotSupportedException($"Join type {joinType} is not supported.");
            }
        }

        /// <summary>
        /// Switch query mode and reset current builder state.
        /// </summary>
        private void EnsureMode(QueryMode mode)
        {
            if (_mode == mode)
            {
                return;
            }

            _mode = mode;
            _whereClause = string.Empty;
            _groupByClause = string.Empty;
            _havingClause = string.Empty;
            _joins.Clear();
            _orderByClauses.Clear();
            _limit = null;
            _offset = null;
            _selectClause = BaseAlias + ".*";
            _insertColumns.Clear();
            _insertValues.Clear();
            _setClauses.Clear();
            _aliases.Clear();
            _aliases[typeof(T)] = BaseAlias;
        }

        /// <summary>
        /// Guard SELECT-only features (JOIN/GROUP BY/HAVING/ORDER BY).
        /// </summary>
        private void EnsureSelectMode(string featureName)
        {
            if (_mode != QueryMode.Select)
            {
                throw new NotSupportedException($"{featureName} is only supported for SELECT mode.");
            }
        }

        /// <summary>
        /// Resolve alias for a registered entity type.
        /// </summary>
        private string GetAliasForType(Type type)
        {
            if (_aliases.TryGetValue(type, out var alias))
            {
                return alias;
            }

            throw new NotSupportedException($"Type {type.Name} is not registered in the query. Add a JOIN first.");
        }

        /// <summary>
        /// Convert CLR values to PostgreSQL literals.
        /// </summary>
        private string FormatLiteral(object? value)
        {
            if (value == null)
            {
                return "NULL";
            }

            if (value is string text)
            {
                return $"'{text.Replace("'", "''")}'";
            }

            if (value is bool flag)
            {
                return flag ? "TRUE" : "FALSE";
            }

            if (value is DateTime dateTime)
            {
                return $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            }

            if (value is DateTimeOffset dateTimeOffset)
            {
                return $"'{dateTimeOffset:yyyy-MM-dd HH:mm:ss zzz}'";
            }

            if (value is char c)
            {
                return $"'{c.ToString().Replace("'", "''")}'";
            }

            return value.ToString() ?? "NULL";
        }

        /// <summary>
        /// Evaluate expression to a runtime value.
        /// </summary>
        private object? EvaluateExpressionValue(Expression expression)
        {
            var boxed = Expression.Convert(expression, typeof(object));
            return Expression.Lambda<Func<object?>>(boxed).Compile().Invoke();
        }

        /// <summary>
        /// Resolve runtime value for non-parameter expressions (captured vars/member chains).
        /// </summary>
        private bool TryResolveExpressionValue(Expression expression, out object? value)
        {
            if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                return TryResolveExpressionValue(unary.Operand, out value);
            }

            if (expression is ConstantExpression constant)
            {
                value = constant.Value;
                return true;
            }

            if (expression is MemberExpression memberExpression)
            {
                if (memberExpression.Expression is ParameterExpression)
                {
                    value = null;
                    return false;
                }

                object? instance = null;
                if (memberExpression.Expression != null && !TryResolveExpressionValue(memberExpression.Expression, out instance))
                {
                    value = null;
                    return false;
                }

                if (memberExpression.Member is System.Reflection.FieldInfo field)
                {
                    value = field.GetValue(instance);
                    return true;
                }

                if (memberExpression.Member is System.Reflection.PropertyInfo property)
                {
                    value = property.GetValue(instance);
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Convert expression tree to SQL predicate snippet.
        /// </summary>
        private string BuildExpression(Expression expression)
        {
            if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                return BuildExpression(unary.Operand);
            }

            if (expression is BinaryExpression binary)
            {
                var left = BuildExpression(binary.Left);
                var right = BuildExpression(binary.Right);
                string op;
                switch (binary.NodeType)
                {
                    case ExpressionType.Equal:
                        op = left == "NULL" || right == "NULL" ? "IS" : "=";
                        break;
                    case ExpressionType.NotEqual:
                        op = left == "NULL" || right == "NULL" ? "IS NOT" : "<>";
                        break;
                    case ExpressionType.GreaterThan:
                        op = ">";
                        break;
                    case ExpressionType.LessThan:
                        op = "<";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        op = ">=";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        op = "<=";
                        break;
                    case ExpressionType.AndAlso:
                        op = "AND";
                        break;
                    case ExpressionType.OrElse:
                        op = "OR";
                        break;
                    default:
                        throw new NotSupportedException($"Operator {binary.NodeType} is not supported.");
                }

                return $"({left} {op} {right})";
            }

            if (expression is MemberExpression memberExpFromParameter && memberExpFromParameter.Expression is ParameterExpression parameterExpression)
            {
                var alias = GetAliasForType(parameterExpression.Type);
                return $"{alias}.{memberExpFromParameter.Member.Name}";
            }

            if (expression is MemberExpression memberExp)
            {
                if (TryResolveExpressionValue(memberExp, out var captured))
                {
                    return FormatLiteral(captured);
                }

                throw new NotSupportedException($"Member expression {memberExp} is not supported.");
            }

            if (expression is ConstantExpression constant)
            {
                return FormatLiteral(constant.Value);
            }

            if (expression is MethodCallExpression containsCall && containsCall.Method.Name == "Contains" && containsCall.Object != null)
            {
                var column = BuildExpression(containsCall.Object);
                var value = EvaluateExpressionValue(containsCall.Arguments[0])?.ToString() ?? string.Empty;
                return $"{column} LIKE '%{value.Replace("'", "''")}%'";
            }

            if (expression is MethodCallExpression startsWithCall && startsWithCall.Method.Name == "StartsWith" && startsWithCall.Object != null)
            {
                var column = BuildExpression(startsWithCall.Object);
                var value = EvaluateExpressionValue(startsWithCall.Arguments[0])?.ToString() ?? string.Empty;
                return $"{column} LIKE '{value.Replace("'", "''")}%'";
            }

            if (expression is MethodCallExpression endsWithCall && endsWithCall.Method.Name == "EndsWith" && endsWithCall.Object != null)
            {
                var column = BuildExpression(endsWithCall.Object);
                var value = EvaluateExpressionValue(endsWithCall.Arguments[0])?.ToString() ?? string.Empty;
                return $"{column} LIKE '%{value.Replace("'", "''")}'";
            }

            throw new NotSupportedException($"Expression type {expression.NodeType} is not supported.");
        }

        /// <summary>
        /// Convert selector expression to SQL column list.
        /// </summary>
        private IEnumerable<string> BuildColumnExpressions(Expression body)
        {
            if (body is NewExpression newExpression)
            {
                return newExpression.Arguments.SelectMany(BuildColumnExpressions);
            }

            if (body is MemberExpression memberExp && memberExp.Expression is ParameterExpression parameter)
            {
                return new[] { $"{GetAliasForType(parameter.Type)}.{memberExp.Member.Name}" };
            }

            if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                return BuildColumnExpressions(unary.Operand);
            }

            if (body is ParameterExpression parameterExpression)
            {
                var alias = GetAliasForType(parameterExpression.Type);
                return parameterExpression.Type.GetProperties().Select(x => $"{alias}.{x.Name}");
            }

            throw new NotSupportedException("Only simple member selections are supported.");
        }

        /// <summary>
        /// Build final SELECT column string.
        /// </summary>
        private string BuildSelectColumns(Expression body)
        {
            return string.Join(", ", BuildColumnExpressions(body));
        }

        /// <summary>
        /// Parse object initializer syntax: () => new T { ... }.
        /// </summary>
        private static MemberInitExpression GetMemberInit(Expression body)
        {
            if (body is MemberInitExpression init)
            {
                return init;
            }

            if (body is UnaryExpression unary && unary.Operand is MemberInitExpression innerInit)
            {
                return innerInit;
            }

            throw new NotSupportedException("Use object initializer syntax: () => new T { ... }.");
        }

        /// <summary>
        /// Extract assigned members from object initializer.
        /// </summary>
        private static IEnumerable<MemberAssignment> GetAssignments(MemberInitExpression init)
        {
            return init.Bindings.OfType<MemberAssignment>();
        }

        public IQueryBuilder<T, TResult> Select(Expression<Func<T, TResult>> selector)
        {
            EnsureMode(QueryMode.Select);
            _selectClause = BuildSelectColumns(selector.Body);
            return this;
        }

        public IQueryBuilder<T, TResult> Select<TJoin, TSelect>(Expression<Func<T, TJoin, TSelect>> selector) where TJoin : class
        {
            EnsureMode(QueryMode.Select);
            _selectClause = BuildSelectColumns(selector.Body);
            return this;
        }

        public IQueryBuilder<T, TResult> SelectRaw(string selectClause)
        {
            EnsureMode(QueryMode.Select);
            _selectClause = selectClause;
            return this;
        }

        public IQueryBuilder<T, TResult> Insert(Expression<Func<T>> entityInitializer)
        {
            EnsureMode(QueryMode.Insert);

            var init = GetMemberInit(entityInitializer.Body);
            foreach (var assignment in GetAssignments(init))
            {
                _insertColumns.Add(assignment.Member.Name);
                _insertValues.Add(FormatLiteral(EvaluateExpressionValue(assignment.Expression)));
            }

            if (_insertColumns.Count == 0)
            {
                throw new NotSupportedException("INSERT requires at least one assigned property.");
            }

            return this;
        }

        public IQueryBuilder<T, TResult> Update(Expression<Func<T>> entityInitializer)
        {
            EnsureMode(QueryMode.Update);

            var init = GetMemberInit(entityInitializer.Body);
            foreach (var assignment in GetAssignments(init))
            {
                var value = FormatLiteral(EvaluateExpressionValue(assignment.Expression));
                _setClauses.Add($"{assignment.Member.Name} = {value}");
            }

            if (_setClauses.Count == 0)
            {
                throw new NotSupportedException("UPDATE requires at least one assigned property.");
            }

            return this;
        }

        public IQueryBuilder<T, TResult> Delete()
        {
            EnsureMode(QueryMode.Delete);
            return this;
        }

        public IQueryBuilder<T, TResult> Join<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias, PgJoinType joinType = PgJoinType.Inner) where TJoin : class
        {
            EnsureSelectMode("JOIN");

            _aliases[typeof(TJoin)] = alias;
            var joinKeyword = GetJoinKeyword(joinType);
            var onClause = BuildExpression(on.Body);
            _joins.Add($"{joinKeyword} {typeof(TJoin).Name} {alias} ON {onClause}");
            return this;
        }

        public IQueryBuilder<T, TResult> InnerJoin<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias) where TJoin : class
        {
            return Join(on, alias, PgJoinType.Inner);
        }

        public IQueryBuilder<T, TResult> LeftJoin<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias) where TJoin : class
        {
            return Join(on, alias, PgJoinType.Left);
        }

        public IQueryBuilder<T, TResult> RightJoin<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias) where TJoin : class
        {
            return Join(on, alias, PgJoinType.Right);
        }

        public IQueryBuilder<T, TResult> FullJoin<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias) where TJoin : class
        {
            return Join(on, alias, PgJoinType.Full);
        }

        public IQueryBuilder<T, TResult> CrossJoin<TJoin>(string alias) where TJoin : class
        {
            EnsureSelectMode("CROSS JOIN");

            _aliases[typeof(TJoin)] = alias;
            _joins.Add($"CROSS JOIN {typeof(TJoin).Name} {alias}");
            return this;
        }

        public IQueryBuilder<T, TResult> NaturalJoin<TJoin>(string alias, PgJoinType joinType = PgJoinType.Inner) where TJoin : class
        {
            EnsureSelectMode("NATURAL JOIN");

            _aliases[typeof(TJoin)] = alias;
            string keyword;
            switch (joinType)
            {
                case PgJoinType.Inner:
                    keyword = "NATURAL JOIN";
                    break;
                case PgJoinType.Left:
                    keyword = "NATURAL LEFT JOIN";
                    break;
                case PgJoinType.Right:
                    keyword = "NATURAL RIGHT JOIN";
                    break;
                case PgJoinType.Full:
                    keyword = "NATURAL FULL JOIN";
                    break;
                default:
                    throw new NotSupportedException($"Join type {joinType} is not supported for NATURAL JOIN.");
            }

            _joins.Add($"{keyword} {typeof(TJoin).Name} {alias}");
            return this;
        }

        public IQueryBuilder<T, TResult> JoinUsing<TJoin>(string alias, PgJoinType joinType, params string[] columns) where TJoin : class
        {
            EnsureSelectMode("JOIN USING");

            if (columns == null || columns.Length == 0)
            {
                throw new ArgumentException("At least one column is required for USING().", nameof(columns));
            }

            _aliases[typeof(TJoin)] = alias;
            var joinKeyword = GetJoinKeyword(joinType);
            var usingClause = string.Join(", ", columns);
            _joins.Add($"{joinKeyword} {typeof(TJoin).Name} {alias} USING ({usingClause})");
            return this;
        }

        public IQueryBuilder<T, TResult> JoinLateral(string alias, string subQuerySql, string onClause, bool left = false)
        {
            EnsureSelectMode("LATERAL JOIN");

            var joinKeyword = left ? "LEFT JOIN LATERAL" : "JOIN LATERAL";
            _joins.Add($"{joinKeyword} ({subQuerySql}) {alias} ON {onClause}");
            return this;
        }

        public IQueryBuilder<T, TResult> Where(Expression<Func<T, bool>> predicate)
        {
            _whereClause = BuildExpression(predicate.Body);
            return this;
        }

        public IQueryBuilder<T, TResult> Where<TJoin>(Expression<Func<T, TJoin, bool>> predicate) where TJoin : class
        {
            _whereClause = BuildExpression(predicate.Body);
            return this;
        }

        public IQueryBuilder<T, TResult> WhereRaw(string whereClause)
        {
            _whereClause = whereClause;
            return this;
        }

        public IQueryBuilder<T, TResult> GroupBy(Expression<Func<T, object>> keySelector)
        {
            EnsureSelectMode("GROUP BY");
            _groupByClause = string.Join(", ", BuildColumnExpressions(keySelector.Body));
            return this;
        }

        public IQueryBuilder<T, TResult> GroupBy<TJoin>(Expression<Func<T, TJoin, object>> keySelector) where TJoin : class
        {
            EnsureSelectMode("GROUP BY");
            _groupByClause = string.Join(", ", BuildColumnExpressions(keySelector.Body));
            return this;
        }

        public IQueryBuilder<T, TResult> GroupByRaw(string groupByClause)
        {
            EnsureSelectMode("GROUP BY");
            _groupByClause = groupByClause;
            return this;
        }

        public IQueryBuilder<T, TResult> Having(Expression<Func<T, bool>> predicate)
        {
            EnsureSelectMode("HAVING");
            _havingClause = BuildExpression(predicate.Body);
            return this;
        }

        public IQueryBuilder<T, TResult> Having<TJoin>(Expression<Func<T, TJoin, bool>> predicate) where TJoin : class
        {
            EnsureSelectMode("HAVING");
            _havingClause = BuildExpression(predicate.Body);
            return this;
        }

        public IQueryBuilder<T, TResult> HavingRaw(string havingClause)
        {
            EnsureSelectMode("HAVING");
            _havingClause = havingClause;
            return this;
        }

        public IQueryBuilder<T, TResult> OrderBy(Expression<Func<T, object>> keySelector, bool descending = false)
        {
            EnsureSelectMode("ORDER BY");
            _orderByClauses.Clear();
            var direction = descending ? "DESC" : "ASC";
            _orderByClauses.AddRange(BuildColumnExpressions(keySelector.Body).Select(x => $"{x} {direction}"));
            return this;
        }

        public IQueryBuilder<T, TResult> OrderBy<TJoin>(Expression<Func<T, TJoin, object>> keySelector, bool descending = false) where TJoin : class
        {
            EnsureSelectMode("ORDER BY");
            _orderByClauses.Clear();
            var direction = descending ? "DESC" : "ASC";
            _orderByClauses.AddRange(BuildColumnExpressions(keySelector.Body).Select(x => $"{x} {direction}"));
            return this;
        }

        public IQueryBuilder<T, TResult> OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            return OrderBy(keySelector, true);
        }

        public IQueryBuilder<T, TResult> OrderByDescending<TJoin>(Expression<Func<T, TJoin, object>> keySelector) where TJoin : class
        {
            return OrderBy(keySelector, true);
        }

        public IQueryBuilder<T, TResult> ThenBy(Expression<Func<T, object>> keySelector, bool descending = false)
        {
            EnsureSelectMode("ORDER BY");
            var direction = descending ? "DESC" : "ASC";
            _orderByClauses.AddRange(BuildColumnExpressions(keySelector.Body).Select(x => $"{x} {direction}"));
            return this;
        }

        public IQueryBuilder<T, TResult> ThenBy<TJoin>(Expression<Func<T, TJoin, object>> keySelector, bool descending = false) where TJoin : class
        {
            EnsureSelectMode("ORDER BY");
            var direction = descending ? "DESC" : "ASC";
            _orderByClauses.AddRange(BuildColumnExpressions(keySelector.Body).Select(x => $"{x} {direction}"));
            return this;
        }

        public IQueryBuilder<T, TResult> ThenByDescending(Expression<Func<T, object>> keySelector)
        {
            return ThenBy(keySelector, true);
        }

        public IQueryBuilder<T, TResult> ThenByDescending<TJoin>(Expression<Func<T, TJoin, object>> keySelector) where TJoin : class
        {
            return ThenBy(keySelector, true);
        }

        public IQueryBuilder<T, TResult> OrderByRaw(string orderByClause)
        {
            EnsureSelectMode("ORDER BY");
            _orderByClauses.Clear();
            _orderByClauses.Add(orderByClause);
            return this;
        }

        public IQueryBuilder<T, TResult> Limit(int count)
        {
            EnsureSelectMode("LIMIT");

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "LIMIT must be greater than or equal to 0.");
            }

            _limit = count;
            return this;
        }

        public IQueryBuilder<T, TResult> Offset(int count)
        {
            EnsureSelectMode("OFFSET");

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "OFFSET must be greater than or equal to 0.");
            }

            _offset = count;
            return this;
        }

        public IQueryBuilder<T, TResult> Take(int count)
        {
            return Limit(count);
        }

        public IQueryBuilder<T, TResult> Skip(int count)
        {
            return Offset(count);
        }

        public string BuildQuery()
        {
            var query = new StringBuilder();

            switch (_mode)
            {
                case QueryMode.Insert:
                    query.AppendLine($"INSERT INTO {typeof(T).Name} ({string.Join(", ", _insertColumns)})");
                    query.Append($"VALUES ({string.Join(", ", _insertValues)})");
                    break;

                case QueryMode.Update:
                    query.AppendLine($"UPDATE {typeof(T).Name} {BaseAlias}");
                    query.AppendLine($"SET {string.Join(", ", _setClauses)}");
                    if (!string.IsNullOrWhiteSpace(_whereClause))
                    {
                        query.Append($"WHERE {_whereClause}");
                    }
                    break;

                case QueryMode.Delete:
                    query.AppendLine($"DELETE FROM {typeof(T).Name} {BaseAlias}");
                    if (!string.IsNullOrWhiteSpace(_whereClause))
                    {
                        query.Append($"WHERE {_whereClause}");
                    }
                    break;

                default:
                    query.AppendLine($"SELECT {_selectClause}");
                    query.AppendLine($"FROM {typeof(T).Name} {BaseAlias}");

                    foreach (var join in _joins)
                    {
                        query.AppendLine(join);
                    }

                    if (!string.IsNullOrWhiteSpace(_whereClause))
                    {
                        query.AppendLine($"WHERE {_whereClause}");
                    }

                    if (!string.IsNullOrWhiteSpace(_groupByClause))
                    {
                        query.AppendLine($"GROUP BY {_groupByClause}");
                    }

                    if (!string.IsNullOrWhiteSpace(_havingClause))
                    {
                        query.AppendLine($"HAVING {_havingClause}");
                    }

                    if (_orderByClauses.Count > 0)
                    {
                        query.AppendLine($"ORDER BY {string.Join(", ", _orderByClauses)}");
                    }

                    if (_limit.HasValue)
                    {
                        query.AppendLine($"LIMIT {_limit.Value}");
                    }

                    if (_offset.HasValue)
                    {
                        query.Append($"OFFSET {_offset.Value}");
                    }
                    break;
            }

            return query.ToString().TrimEnd();
        }

        public void Reset()
        {
            _mode = QueryMode.Select;
            _selectClause = BaseAlias + ".*";
            _whereClause = string.Empty;
            _groupByClause = string.Empty;
            _havingClause = string.Empty;
            _joins.Clear();
            _orderByClauses.Clear();
            _limit = null;
            _offset = null;
            _insertColumns.Clear();
            _insertValues.Clear();
            _setClauses.Clear();
            _aliases.Clear();
            _aliases[typeof(T)] = BaseAlias;
        }
    }
}
