using System;
using System.Linq.Expressions;

namespace DataManagement.DapperLib.Core
{
    /// <summary>
    /// Fluent query builder contract for SELECT/INSERT/UPDATE/DELETE with join, filter, group, having, and order support.
    /// </summary>
    public interface IQueryBuilder<T, TResult> where T : class
    {
        /// <summary>
        /// Build SELECT columns from entity expression.
        /// </summary>
        IQueryBuilder<T, TResult> Select(Expression<Func<T, TResult>> selector);
        /// <summary>
        /// Build SELECT columns from entity and joined entity expressions.
        /// </summary>
        IQueryBuilder<T, TResult> Select<TJoin, TSelect>(Expression<Func<T, TJoin, TSelect>> selector) where TJoin : class;
        /// <summary>
        /// Set raw SELECT clause.
        /// </summary>
        IQueryBuilder<T, TResult> SelectRaw(string selectClause);

        /// <summary>
        /// Add generic JOIN with ON expression.
        /// </summary>
        IQueryBuilder<T, TResult> Join<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias, PgJoinType joinType = PgJoinType.Inner) where TJoin : class;
        /// <summary>
        /// Add INNER JOIN.
        /// </summary>
        IQueryBuilder<T, TResult> InnerJoin<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias) where TJoin : class;
        /// <summary>
        /// Add LEFT JOIN.
        /// </summary>
        IQueryBuilder<T, TResult> LeftJoin<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias) where TJoin : class;
        /// <summary>
        /// Add RIGHT JOIN.
        /// </summary>
        IQueryBuilder<T, TResult> RightJoin<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias) where TJoin : class;
        /// <summary>
        /// Add FULL JOIN.
        /// </summary>
        IQueryBuilder<T, TResult> FullJoin<TJoin>(Expression<Func<T, TJoin, bool>> on, string alias) where TJoin : class;
        /// <summary>
        /// Add CROSS JOIN.
        /// </summary>
        IQueryBuilder<T, TResult> CrossJoin<TJoin>(string alias) where TJoin : class;
        /// <summary>
        /// Add NATURAL JOIN.
        /// </summary>
        IQueryBuilder<T, TResult> NaturalJoin<TJoin>(string alias, PgJoinType joinType = PgJoinType.Inner) where TJoin : class;
        /// <summary>
        /// Add JOIN USING (...).
        /// </summary>
        IQueryBuilder<T, TResult> JoinUsing<TJoin>(string alias, PgJoinType joinType, params string[] columns) where TJoin : class;
        /// <summary>
        /// Add JOIN LATERAL.
        /// </summary>
        IQueryBuilder<T, TResult> JoinLateral(string alias, string subQuerySql, string onClause, bool left = false);

        /// <summary>
        /// Add WHERE for base entity.
        /// </summary>
        IQueryBuilder<T, TResult> Where(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Add WHERE using base and joined entity.
        /// </summary>
        IQueryBuilder<T, TResult> Where<TJoin>(Expression<Func<T, TJoin, bool>> predicate) where TJoin : class;
        /// <summary>
        /// Set raw WHERE clause.
        /// </summary>
        IQueryBuilder<T, TResult> WhereRaw(string whereClause);

        /// <summary>
        /// Add GROUP BY for base entity.
        /// </summary>
        IQueryBuilder<T, TResult> GroupBy(Expression<Func<T, object>> keySelector);
        /// <summary>
        /// Add GROUP BY using base and joined entity.
        /// </summary>
        IQueryBuilder<T, TResult> GroupBy<TJoin>(Expression<Func<T, TJoin, object>> keySelector) where TJoin : class;
        /// <summary>
        /// Set raw GROUP BY clause.
        /// </summary>
        IQueryBuilder<T, TResult> GroupByRaw(string groupByClause);

        /// <summary>
        /// Add HAVING for base entity.
        /// </summary>
        IQueryBuilder<T, TResult> Having(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Add HAVING using base and joined entity.
        /// </summary>
        IQueryBuilder<T, TResult> Having<TJoin>(Expression<Func<T, TJoin, bool>> predicate) where TJoin : class;
        /// <summary>
        /// Set raw HAVING clause.
        /// </summary>
        IQueryBuilder<T, TResult> HavingRaw(string havingClause);

        /// <summary>
        /// Add ORDER BY for base entity.
        /// </summary>
        IQueryBuilder<T, TResult> OrderBy(Expression<Func<T, object>> keySelector, bool descending = false);
        /// <summary>
        /// Add ORDER BY using base and joined entity.
        /// </summary>
        IQueryBuilder<T, TResult> OrderBy<TJoin>(Expression<Func<T, TJoin, object>> keySelector, bool descending = false) where TJoin : class;
        /// <summary>
        /// Add ORDER BY DESC for base entity.
        /// </summary>
        IQueryBuilder<T, TResult> OrderByDescending(Expression<Func<T, object>> keySelector);
        /// <summary>
        /// Add ORDER BY DESC using base and joined entity.
        /// </summary>
        IQueryBuilder<T, TResult> OrderByDescending<TJoin>(Expression<Func<T, TJoin, object>> keySelector) where TJoin : class;
        /// <summary>
        /// Add secondary ORDER BY for base entity.
        /// </summary>
        IQueryBuilder<T, TResult> ThenBy(Expression<Func<T, object>> keySelector, bool descending = false);
        /// <summary>
        /// Add secondary ORDER BY using base and joined entity.
        /// </summary>
        IQueryBuilder<T, TResult> ThenBy<TJoin>(Expression<Func<T, TJoin, object>> keySelector, bool descending = false) where TJoin : class;
        /// <summary>
        /// Add secondary ORDER BY DESC for base entity.
        /// </summary>
        IQueryBuilder<T, TResult> ThenByDescending(Expression<Func<T, object>> keySelector);
        /// <summary>
        /// Add secondary ORDER BY DESC using base and joined entity.
        /// </summary>
        IQueryBuilder<T, TResult> ThenByDescending<TJoin>(Expression<Func<T, TJoin, object>> keySelector) where TJoin : class;
        /// <summary>
        /// Set raw ORDER BY clause.
        /// </summary>
        IQueryBuilder<T, TResult> OrderByRaw(string orderByClause);
        /// <summary>
        /// Set LIMIT value for SELECT.
        /// </summary>
        IQueryBuilder<T, TResult> Limit(int count);
        /// <summary>
        /// Set OFFSET value for SELECT.
        /// </summary>
        IQueryBuilder<T, TResult> Offset(int count);
        /// <summary>
        /// LINQ-style alias for LIMIT.
        /// </summary>
        IQueryBuilder<T, TResult> Take(int count);
        /// <summary>
        /// LINQ-style alias for OFFSET.
        /// </summary>
        IQueryBuilder<T, TResult> Skip(int count);

        /// <summary>
        /// Build INSERT from object initializer.
        /// </summary>
        IQueryBuilder<T, TResult> Insert(Expression<Func<T>> entityInitializer);
        /// <summary>
        /// Build UPDATE SET from object initializer.
        /// </summary>
        IQueryBuilder<T, TResult> Update(Expression<Func<T>> entityInitializer);
        /// <summary>
        /// Switch to DELETE mode.
        /// </summary>
        IQueryBuilder<T, TResult> Delete();

        /// <summary>
        /// Build the final SQL statement.
        /// </summary>
        string BuildQuery();
        /// <summary>
        /// Reset internal query state.
        /// </summary>
        void Reset();
    }
}
