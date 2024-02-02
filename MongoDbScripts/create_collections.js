const jetStreamDb = db.getSiblingDB("JetStreamAPI");

function createCollectionWithValidation(collectionName, validationSchema) {
  if (!jetStreamDb.getCollectionNames().includes(collectionName)) {
    jetStreamDb.createCollection(collectionName, {
      validator: { $jsonSchema: validationSchema },
      validationAction: "warn"
    });
    print(`Collection ${collectionName} created.`);
  } else {
    print(`Collection ${collectionName} already exists.`);
  }
}

createCollectionWithValidation("serviceOrders", {
  bsonType: "object",
  required: ["customerName", "email", "phoneNumber", "creationDate", "desiredPickupDate", "serviceType", "priority", "status"],
  properties: {
    customerName: { bsonType: "string", minLength: 1, maxLength: 255 },
    email: { bsonType: "string", pattern: "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$" },
    phoneNumber: { bsonType: "string" },
    creationDate: { bsonType: "date" },
    desiredPickupDate: { bsonType: "date" },
    comments: { bsonType: "string" },
    status: {
      bsonType: "object",
      required: ["statusValue", "description"],
      properties: {
        statusValue: { bsonType: "string", enum: ["Offen", "In Bearbeitung", "Abgeschlossen"] },
        description: { bsonType: "string" }
      }
    },
    serviceType: { bsonType: "objectId" },
    priority: { bsonType: "objectId" }
  }
});

createCollectionWithValidation("serviceTypes", {
  bsonType: "object",
  required: ["_id", "name", "cost"],
  properties: {
    _id: { bsonType: "string" },
    name: { bsonType: "string", minLength: 1, maxLength: 100 },
    cost: { bsonType: "decimal", minimum: 0 }
  }
});

createCollectionWithValidation("servicePriorities", {
  bsonType: "object",
  required: ["_id", "priorityName", "dayCount"],
  properties: {
    _id: { bsonType: "string" },
    priorityName: { bsonType: "string", minLength: 1, maxLength: 100 },
    dayCount: { bsonType: "int", minimum: 0 }
  }
});

createCollectionWithValidation("employees", {
  bsonType: "object",
  required: ["username", "password", "isLocked", "failedLoginAttempts"],
  properties: {
    username: { bsonType: "string", minLength: 3 },
    password: { bsonType: "string", minLength: 8 },
    isLocked: { bsonType: "bool" },
    failedLoginAttempts: { bsonType: "int", minimum: 0 }
  }
});