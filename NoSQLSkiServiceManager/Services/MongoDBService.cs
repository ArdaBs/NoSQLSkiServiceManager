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

        await CreateCollectionIfNotExistsAsync("serviceTypes", ServiceTypeSchema);
        await InsertServiceTypesAsync();

        await CreateCollectionIfNotExistsAsync("servicePriorities", ServicePrioritySchema);
        await InsertServicePrioritiesAsync();

        await CreateCollectionIfNotExistsAsync("employees", null);
        await InitializeEmployeesAsync();

        await CreateCollectionIfNotExistsAsync("serviceOrders", ServiceOrderSchema);
        await InsertServiceOrderExampleAsync();

        await CreateUsersAsync();
    }

    public async Task CreateServiceOrderIndexesAsync()
    {
        var serviceOrderCollection = _database.GetCollection<ServiceOrder>("serviceOrders");

        var indexKeysDefinition = Builders<ServiceOrder>.IndexKeys
            .Ascending(order => order.ServiceType.Id)
            .Ascending(order => order.Priority.Id);

        await serviceOrderCollection.Indexes.CreateOneAsync(new CreateIndexModel<ServiceOrder>(indexKeysDefinition));
    }

    private async Task CreateCollectionIfNotExistsAsync(string collectionName, BsonDocument schema)
    {
        var collectionList = _database.ListCollectionNames().ToListAsync().Result;
        if (!collectionList.Contains(collectionName))
        {
            await _database.CreateCollectionAsync(collectionName);
            if (schema != null)
            {
                var validator = new BsonDocument { { "$jsonSchema", schema } };
                var validationOptions = new BsonDocument
                {
                    { "collMod", collectionName },
                    { "validator", validator },
                    { "validationLevel", "moderate" },
                    { "validationAction", "warn" }
                };
                await _database.RunCommandAsync<BsonDocument>(validationOptions);
            }
        }
    }


    public async Task InitializeEmployeesAsync()
    {
        var employeeCollection = _database.GetCollection<Employee>("employees");
        var exists = await employeeCollection.Find(_ => true).AnyAsync();
        if (!exists)
        {
            var employees = new List<Employee>
            {
                new Employee { Username = "Arda", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
                new Employee { Username = "Lukas", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
                new Employee { Username = "Goku", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
                new Employee { Username = "Gojo", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 }
            };

            await employeeCollection.InsertManyAsync(employees);
        }
    }


    private static readonly BsonDocument ServiceTypeSchema = new BsonDocument
    {
        { "bsonType", "object" },
        { "required", new BsonArray { "Id", "name", "cost" } },
        { "properties", new BsonDocument
            {
                { "Id", new BsonDocument { { "bsonType", "string" }, { "description", "must be a string and is required" } } },
                { "name", new BsonDocument { { "bsonType", "string" }, { "description", "must be a string and is required" } } },
                { "cost", new BsonDocument { { "bsonType", "decimal" }, { "minimum", 0 }, { "description", "must be a non-negative decimal and is required" } } }
            }
        }
    };

    private static readonly BsonDocument ServicePrioritySchema = new BsonDocument
    {
        { "bsonType", "object" },
        { "required", new BsonArray { "Id", "priorityName", "dayCount" } },
        { "properties", new BsonDocument
            {
                { "Id", new BsonDocument { { "bsonType", "string" }, { "description", "must be a string and is required" } } },
                { "priorityName", new BsonDocument { { "bsonType", "string" }, { "description", "must be a string and is required" } } },
                { "dayCount", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 }, { "description", "must be a non-negative integer and is required" } } }
            }
        }
    };

    private static readonly BsonDocument ServiceOrderSchema = new BsonDocument
    {
        { "bsonType", "object" },
        { "required", new BsonArray { "customerName", "email", "phoneNumber", "creationDate", "desiredPickupDate", "serviceType", "priority", "status" } },
        { "properties", new BsonDocument
            {
                { "customerName", new BsonDocument { { "bsonType", "string" } } },
                { "email", new BsonDocument { { "bsonType", "string" } } },
                { "phoneNumber", new BsonDocument { { "bsonType", "string" } } },
                { "creationDate", new BsonDocument { { "bsonType", "date" } } },
                { "desiredPickupDate", new BsonDocument { { "bsonType", "date" } } },
                { "comments", new BsonDocument { { "bsonType", "string" } } },
                { "status", new BsonDocument
                    {
                        { "bsonType", "object" },
                        { "properties", new BsonDocument
                            {
                                { "statusValue", new BsonDocument { { "bsonType", "string" } } },
                                { "description", new BsonDocument { { "bsonType", "string" } } }
                            }
                        }
                    }
                },
                { "serviceType", new BsonDocument { { "bsonType", "object" } } },
                { "priority", new BsonDocument { { "bsonType", "object" } } }
            }
        }
    };





    private async Task InsertServiceTypesAsync()
    {
        var serviceTypesCollection = _database.GetCollection<ServiceType>("serviceTypes");
        var exists = await serviceTypesCollection.Find(_ => true).AnyAsync();
        if (!exists)
        {

            var serviceTypes = new List<ServiceType>
            {
                new ServiceType { Id = "1", Name = "Kleiner Service", Cost = (decimal)new Decimal128(34.95m) },
                new ServiceType { Id = "2", Name = "Grosser Service", Cost = (decimal)new Decimal128(59.95m) },
                new ServiceType { Id = "3", Name = "Rennski-Service", Cost = (decimal)new Decimal128(74.95m) },
                new ServiceType { Id = "4", Name = "Bindung montieren und einstellen", Cost = (decimal)new Decimal128(24.95m) },
                new ServiceType { Id = "5", Name = "Fell zuschneiden", Cost = (decimal)new Decimal128(14.95m) },
                new ServiceType { Id = "6", Name = "Heisswachsen", Cost = (decimal)new Decimal128(19.95m) }
            };

            await serviceTypesCollection.InsertManyAsync(serviceTypes);
        }
     }



    private async Task InsertServicePrioritiesAsync()
    {
        var servicePrioritiesCollection = _database.GetCollection<ServicePriority>("servicePriorities");
        var exists = await servicePrioritiesCollection.Find(_ => true).AnyAsync();
        if (!exists)
        {
            var servicePriorities = new List<ServicePriority>
            {
                new ServicePriority { Id = "1", PriorityName = "Low", DayCount = 5 },
                new ServicePriority { Id = "2", PriorityName = "Standard", DayCount = 0 },
                new ServicePriority { Id = "3", PriorityName = "Express", DayCount = -2 }
            };
            await servicePrioritiesCollection.InsertManyAsync(servicePriorities);
        }
    }

    private async Task InsertServiceOrderExampleAsync()
    {
        var serviceOrdersCollection = _database.GetCollection<ServiceOrder>("serviceOrders");
        var exists = await serviceOrdersCollection.Find(_ => true).AnyAsync();
        if (!exists)
        {
            var serviceOrderExample = new ServiceOrder
            {
                CustomerName = "Max Mustermann",
                Email = "max.mustermann@example.com",
                PhoneNumber = "0123456789",
                CreationDate = DateTime.Now,
                DesiredPickupDate = DateTime.Now.AddDays(5),
                Comments = "Bitte um sorgfältige Überprüfung der Bindungen.",
                Status = OrderStatus.Offen,
                ServiceType = new ServiceType { Id = "1", Name = "Kleiner Service", Cost = 34.95m },
                Priority = new ServicePriority { Id = "2", PriorityName = "Standard", DayCount = 0 }
            };
            await serviceOrdersCollection.InsertOneAsync(serviceOrderExample);
            await CreateServiceOrderIndexesAsync();
        }
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
