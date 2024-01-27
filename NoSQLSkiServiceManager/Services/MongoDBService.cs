using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSkiServiceManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MongoDBService
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;

    public MongoDBService(string connectionString, string databaseName)
    {
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
    }

    public async Task EnsureDatabaseSetupAsync()
    {
        var collections = await _database.ListCollectionNames().ToListAsync();

        if (!collections.Contains("serviceTypes"))
        {
            await _database.CreateCollectionAsync("serviceTypes");
            await InitializeServiceTypesAsync();
        }

        if (!collections.Contains("servicePriorities"))
        {
            await _database.CreateCollectionAsync("servicePriorities");
            await InitializeServicePrioritiesAsync();
        }

        if (!collections.Contains("employees"))
        {
            await InitializeEmployeesAsync();
        }

        await CreateUsersAsync();
    }


    public async Task InitializeEmployeesAsync()
    {
        var employeeCollection = _database.GetCollection<Employee>("employees");
        var employees = new List<Employee>
        {
            new Employee { Username = "Arda", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Username = "Lukas", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Username = "Goku", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Username = "Gojo", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 }
        };

        await employeeCollection.InsertManyAsync(employees);
    }

    private async Task InitializeServiceTypesAsync()
    {
        var serviceTypesCollection = _database.GetCollection<ServiceType>("serviceTypes");
        var serviceTypes = new List<ServiceType>
        {
            new ServiceType { Id = "1", Name = "Kleiner Service", Cost = 34.95m },
            new ServiceType { Id = "2", Name = "Grosser Service", Cost = 59.95m },
            new ServiceType { Id = "3", Name = "Rennski-Service", Cost = 74.95m },
            new ServiceType { Id = "4", Name = "Bindung montieren und einstellen", Cost = 24.95m },
            new ServiceType { Id = "5", Name = "Fell zuschneiden", Cost = 14.95m },
            new ServiceType { Id = "6", Name = "Heisswachsen", Cost = 19.95m }
        };

        await serviceTypesCollection.InsertManyAsync(serviceTypes);
    }

    private async Task InitializeServicePrioritiesAsync()
    {
        var servicePrioritiesCollection = _database.GetCollection<ServicePriority>("servicePriorities");
        var servicePriorities = new List<ServicePriority>
        {
            new ServicePriority { Id = "1", PriorityName = "Low", DayCount = 5 },
            new ServicePriority { Id = "2", PriorityName = "Standard", DayCount = 0 },
            new ServicePriority { Id = "3", PriorityName = "Express", DayCount = -2 }
        };

        await servicePrioritiesCollection.InsertManyAsync(servicePriorities);
    }

    public async Task CreateUsersAsync()
    {
        var adminDatabase = _client.GetDatabase("admin");

        var users = await adminDatabase.RunCommandAsync<BsonDocument>(new BsonDocument("usersInfo", 1));
        var usersArray = users["users"].AsBsonArray;

        var jetStreamApiMasterExists = usersArray.Any(u => u["user"].AsString == "JetStreamApiMaster" && u["db"].AsString == "admin");
        if (!jetStreamApiMasterExists)
        {
            var createUserCommand = new BsonDocument {
            { "createUser", "JetStreamApiMaster" },
            { "pwd", "SavePassword1234" },
            { "roles", new BsonArray {
                new BsonDocument { { "role", "dbOwner" }, { "db", "JetStreamAPI" } }
            }}
        };
            await adminDatabase.RunCommandAsync<BsonDocument>(createUserCommand);
        }

        var backendUserExists = usersArray.Any(u => u["user"].AsString == "BackendUser" && u["db"].AsString == "admin");
        if (!backendUserExists)
        {
            var createUserCommand = new BsonDocument {
            { "createUser", "BackendUser" },
            { "pwd", "BackendUserPassword123" },
            { "roles", new BsonArray {
                new BsonDocument { { "role", "readWrite" }, { "db", "JetStreamAPI" } }
            }}
        };
            await adminDatabase.RunCommandAsync<BsonDocument>(createUserCommand);
        }
    }
}
