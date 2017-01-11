using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace PortablePhotoGallery.Azure.Extensions
{
    public static class TableQueryExtensions
    {
        public static TableQuery<T> WhereInPartition<T>(this TableQuery<T> tableQuery, string partitionKey)
        {
            if (tableQuery == null)
                throw new ArgumentNullException(nameof(tableQuery));

            if (string.IsNullOrEmpty(partitionKey))
                throw new ArgumentNullException(nameof(partitionKey));

            return tableQuery.Where(TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey),
                                                                       QueryComparisons.Equal,
                                                                       partitionKey));
        }
    }
}