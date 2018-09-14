using System;
using System.Collections.Generic;
using System.Linq;
using CSharpSampleApp.Services.Search;

namespace CSharpSampleApp.Examples.Search
{
    /// <summary>
    /// Merges search results from multiple endpoints
    /// </summary>
    internal class SearchResultsMergingEnumerable
    {
        public static IEnumerable<IEntityHit> Create(IEnumerable<IEnumerable<IEntityHit>> searchResultPipelines)
        {
            var activePipelines = InitializePipelines(searchResultPipelines);

            var nextHit = GetNextHitAndRemoveDepletedPipelines(activePipelines);
            if(nextHit == null) yield break;

            do
            {
                yield return nextHit;

                nextHit = GetNextHitAndRemoveDepletedPipelines(activePipelines);
            } while (nextHit != null && activePipelines.Count > 0);
        }

        private static List<IEnumerator<IEntityHit>> InitializePipelines(IEnumerable<IEnumerable<IEntityHit>> searchResultPipelines)
        {
            Console.WriteLine("Merge: Initializing all pipelines");

            var pipelines = searchResultPipelines.Select(p => p.GetEnumerator()).ToList();

            // Make a clone for iteration, to avoid exception on editing the original list
            var pipelinesSnapshot = pipelines.ToList();
            foreach (var pipeline in pipelinesSnapshot)
            {
                var pipelineHasItems = pipeline.MoveNext();

                if (!pipelineHasItems) pipelines.Remove(pipeline);
            }

            return pipelines;
        }

        private static IEntityHit GetNextHitAndRemoveDepletedPipelines(ICollection<IEnumerator<IEntityHit>> activePipelines)
        {
            Console.WriteLine("Merge: Fetching next item");

            var currentItems = activePipelines.Select(p => p.Current);

            var nextHit = GetItemWithMaxRank(currentItems);
            if (nextHit == null)
            {
                activePipelines.Clear();
                return null;
            }

            var nextHitProducer = activePipelines.First(p => ReferenceEquals(p.Current, nextHit));

            var nextHitProducerHasNextItem = nextHitProducer.MoveNext();

            if (!nextHitProducerHasNextItem) activePipelines.Remove(nextHitProducer);

            return nextHit;
        }

        private static IEntityHit GetItemWithMaxRank(IEnumerable<IEntityHit> currentItems)
        {
            IEntityHit maxRankHit = null;
            foreach (var hit in currentItems)
            {
                if (maxRankHit == null || hit.Rank > maxRankHit.Rank) maxRankHit = hit;
            }

            return maxRankHit;
        }
    }
}
