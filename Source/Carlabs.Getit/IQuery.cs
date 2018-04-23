﻿using System.Collections.Generic;
using System.Threading.Tasks;

using GraphQL.Common.Response;

namespace Carlabs.Getit
{
    public interface IQuery
    {
        List<object> SelectList { get; }
        Dictionary<string, object> WhereMap { get; }
        List<GraphQLError> GqlErrors { get; }

        string QueryName { get; }
        string AliasName { get; }
        string QueryComment { get; }
        string RawQuery { get; }

        /// <summary>
        /// Clear the Query and anything related
        /// </summary>
        void Clear();

        /// <summary>
        /// Accepts a string and will use this as the query. Setting
        /// this will overide any other settings and ignore any 
        /// validation checks. If the string is empty it will be
        /// ignored and the existing query builder actions will be 
        /// at play. 
        /// 
        /// WARNING : Calling this will clear all other query elements.
        /// 
        /// </summary>
        /// <param name="rawQuery">The full valid query to be sent to the endpoint</param>
        IQuery Raw(string rawQuery);

        /// <summary>
        /// Sets the query Name
        /// </summary>
        /// <param name="queryName">The Query Name String</param>
        /// <returns>Query</returns>
        IQuery Name(string queryName);

        /// <summary>
        /// Sets the Query Alias name. This is used in graphQL to allow 
        /// multipe queries with the same endpoint (name) to be assembled
        /// into a batch like query. This will prefix the Name in the query.
        /// It will also be used for the Response name processing.
        /// Note that this can be applied to any sub-select as well. GraphQL will
        /// rename the query with the alias name in the response.
        /// </summary>
        /// <param name="alias">The alias name</param>
        /// <returns>Query</returns>
        IQuery Alias(string alias);

        /// <summary>
        /// Add a comment to the Query. This will take a simple string comment
        /// and add it to the Select block in the query. GraphQL formatting will
        /// be automatically added. Multi-line comments can be done with the
        /// '\n' character and it will be automatically converted into a GraphQL
        /// multi-line coment
        /// </summary>
        /// <param name="comment">The comment string</param>
        /// <returns>Query</returns>
        IQuery Comment(string comment);

        /// <summary>
        /// Add this list to the select part of the query. This
        /// accepts any type of list, but must be one of the types
        /// of data we support, primitives, lists, maps
        /// </summary>
        /// <param name="objectList">Generic List of select fields</param>
        /// <returns>Query</returns>
        IQuery Select(IEnumerable<object> objectList);

        /// <summary>
        /// Add a list of simple strings to the selection part of the 
        /// query.
        /// </summary>
        /// <param name="selects">List of strings</param>
        /// <returns>Query</returns>
        IQuery Select(params string[] selects);

        /// <summary>
        /// Adds a sub query to the list
        /// </summary>
        /// <param name="subSelect">A sub-selection, which can be just a query</param>
        /// <returns>Query</returns>
        /// <exception cref="ArgumentException"></exception>
        IQuery Select(IQuery subSelect);

        /// <summary>
        /// Sets up the Parameters part of the GraphQL query. This
        /// accepts a key and a where part that will go into the  
        /// list for later construction into the query. The where part
        /// can be a simple primitive or complex object that will be 
        /// evaluated.
        /// </summary>
        /// <param name="key">The Parameter Name</param>
        /// <param name="where">The value of the parameter, primitive or object</param>
        /// <returns></returns>
        IQuery Where(string key, object where);

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionay that takes &lt;string, object&gt;</param>
        /// <returns>Query</returns>
        /// <throws>Dupekey and others</throws>
        IQuery Where(Dictionary<string, object> dict);

        /// <summary>
        /// Helper to see if any errors were returned with the
        /// last query. No errors does not mean data, just means
        /// no errors found in the GQL client results
        /// </summary>
        /// <returns>Bool true if errors exist, false if not</returns>
        bool HasErrors();

        /// <summary>
        /// Add additional queries to the request. These
        /// will get bundled in as additional queries. This
        /// will affect the query that is executed in the Get()
        /// method and the ToString() output, but will not
        /// change any items specific to this query. Each
        /// query will individually be calling it's ToString()
        /// to get the query to be batched. Use Alias names
        /// where appropriate
        /// </summary>
        /// <param name="query"></param>
        /// <returns>IQuery</returns>
        IQuery Batch(IQuery query);

        /// <summary>
        /// Given a type return the results of a GraphQL query in it. If
        /// the type is a string then will return the JSON string. The resultName
        /// will be automatically set the Name or Alias name if not specified.
        /// For Raw queries you must set the resultName param OR set the Name() in
        /// the query to match.
        /// </summary>
        /// <typeparam name="T">Data Type, typically a list of the record but not always. 
        /// </typeparam>
        /// <param name="resultName">Overide of the Name/Alias of the query</param>
        /// <returns>The type of object stuffed with data from the query</returns>
        /// <exception cref="ArgumentException">Dupe Key</exception>
        Task<T> Get<T>(string resultName = null);

        /// <summary>
        /// Gets the string representation of the GraphQL query. This does some
        /// MINOR checking and will toss if not formatted correctly
        /// </summary>
        /// <returns>The GrapQL Query String, without outer enclosing block</returns>
        /// <throws>ArgumentException</throws>
        string ToString();
    }
}