using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace LinkShortener.Extensions
{
    public static class CosmosExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>
        (
            this FeedIterator<T> iterator, 
            [EnumeratorCancellation]CancellationToken token = default
        )
        {
            while (iterator.HasMoreResults)
            {
                if (token.IsCancellationRequested)
                {
                    yield break;
                }

                foreach (var item in await iterator.ReadNextAsync(token))
                {
                    yield return item;
                    
                    if (token.IsCancellationRequested)
                    {
                        yield break;
                    }
                }
            }
        }

        public static IAsyncEnumerable<T> ToCosmosAsyncEnumerable<T>
        (
            this IQueryable<T> queryable,
            [EnumeratorCancellation] CancellationToken token = default
        )
        {
            
            return queryable.ToFeedIterator().ToAsyncEnumerable(token);
        }


        public static  IOrderedQueryable<T> AsQueryable<T>(this Container container)
        {
            return container.GetItemLinqQueryable<T>();
        }

    }
}