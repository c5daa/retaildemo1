// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq.Expressions;

namespace Contoso.DataAccess.CosmosDB.Mongo.ModelBase
{
    public interface ISpecification<TEntity>
    {
        /// <summary>
        /// Gets or sets the func delegate query to execute against the repository for searching records.
        /// </summary>
        Expression<Func<TEntity, bool>> Predicate { get; }
    }
}
