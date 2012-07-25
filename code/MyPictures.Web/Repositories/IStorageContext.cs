namespace MyPictures.Web.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure;

    public interface IStorageContext
    {
        CloudStorageAccount StorageAccount { get; }

        IQueryable<T> Query<T>(string tableName);

        void AddEntity<T>(T obj, string tableName);

        void AddEntities<T>(IEnumerable<T> objs, string tableName);
        
        void AddOrUpdateEntity<T>(T obj, string tableName) where T : ITableServiceEntity;

        void DeleteEntity<T>(T obj, string tableName);

        void DeleteEntities<T>(IEnumerable<T> objs, string tableName);

        string SaveBlob(string objId, byte[] content, string contentType);

        void DeleteBlob(string objId);
    }
}
