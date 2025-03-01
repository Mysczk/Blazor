using MongoDB.Driver;

namespace BlazorDemo.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase("YourDatabaseName");
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}


/*
 * exeple of use 
 * 
@page "/example"
@inject BlazorDemo.Services.MongoDbService MongoDbService

@code {
    protected override async Task OnInitializedAsync()
    {
        var collection = MongoDbService.GetCollection<YourModel>("YourCollectionName");
        var items = await collection.Find(_ => true).ToListAsync();
        // Use the items as needed
    }
}
*/