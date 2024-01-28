const existingDbs = db.adminCommand({ listDatabases: 1 }).databases.map(db => db.name);
if (!existingDbs.includes("JetStreamAPI")) {
  const jetStreamDb = db.getSiblingDB("JetStreamAPI");

  function createCollectionWithValidation(db, collectionName, validationSchema) {
    db.createCollection(collectionName, {
      validator: {
        $jsonSchema: validationSchema
      },
      validationAction: "warn"
    });
  }

  function upsertData(collectionName, query, update) {
    db[collectionName].update(query, update, { upsert: true });
  }

  createCollectionWithValidation(jetStreamDb, "serviceOrders", {
    bsonType: "object",
    required: ["customerName", "email", "phoneNumber", "creationDate", "desiredPickupDate", "serviceType", "priority"],
    properties: {
      customerName: { bsonType: "string", description: "must be a string and is required" },
      email: { bsonType: "string", pattern: "^.+@.+$", description: "must be a string in email format and is required" },
      phoneNumber: { bsonType: "string", description: "must be a string and is required" },
      creationDate: { bsonType: "date", description: "must be a date and is required" },
      desiredPickupDate: { bsonType: "date", description: "must be a date and is required" },
      comments: { bsonType: "string", description: "must be a string" },
      status: { enum: ["Offen", "In Bearbeitung", "Abgeschlossen"], description: "must be a valid status" },
      serviceType: { bsonType: "objectId", description: "must be an objectId and is required" },
      priority: { bsonType: "objectId", description: "must be an objectId and is required" }
    }
  });

  createCollectionWithValidation(jetStreamDb, "serviceTypes", {
    bsonType: "object",
    required: ["_id", "name", "cost"],
    properties: {
      _id: { bsonType: "int", description: "must be an integer and is required" },
      name: { bsonType: "string", description: "must be a string and is required" },
      cost: { bsonType: "decimal", minimum: 0, description: "must be a non-negative decimal and is required" }
    }
  });

  createCollectionWithValidation(jetStreamDb, "servicePriorities", {
    bsonType: "object",
    required: ["_id", "priorityName", "dayCount"],
    properties: {
      _id: { bsonType: "int", description: "must be an integer and is required" },
      priorityName: { bsonType: "string", description: "must be a string and is required" },
      dayCount: { bsonType: "int", description: "must be an integer and is required" }
    }
  });

  createCollectionWithValidation(jetStreamDb, "employees", {
    bsonType: "object",
    required: ["username", "password", "isLocked", "failedLoginAttempts"],
    properties: {
      username: { bsonType: "string", description: "must be a string and is required" },
      password: { bsonType: "string", description: "must be a string and is required" },
      isLocked: { bsonType: "bool", description: "must be a boolean and is required" },
      failedLoginAttempts: { bsonType: "int", minimum: 0, description: "must be a non-negative integer and is required" }
    }
  });

  print("Collections in JetStreamAPI Datenbank erfolgreich erstellt.");
} else {
  print("Datenbank JetStreamAPI existiert bereits.");
}

const serviceTypesData = [
  { _id: "1", name: "Kleiner Service", cost: NumberDecimal("34.95") },
  { _id: "2", name: "Grosser Service", cost: NumberDecimal("59.95") },
  { _id: "3", name: "Rennski-Service", cost: NumberDecimal("74.95") },
  { _id: "4", name: "Bindung montieren und einstellen", cost: NumberDecimal("24.95") },
  { _id: "5", name: "Fell zuschneiden", cost: NumberDecimal("14.95") },
  { _id: "6", name: "Heisswachsen", cost: NumberDecimal("19.95") }
];

const servicePrioritiesData = [
  { _id: "1", priorityName: "Low", dayCount: 5 },
  { _id: "2", priorityName: "Standard", dayCount: 0 },
  { _id: "3", priorityName: "Express", dayCount: -2 }
];

serviceTypesData.forEach(data => {
  upsertData("serviceTypes", { _id: data._id }, { $set: data });
});

servicePrioritiesData.forEach(data => {
  upsertData("servicePriorities", { _id: data._id }, { $set: data });
});

const serviceOrderExample = {
  customerName: "Max Mustermann",
  email: "max.mustermann@example.com",
  phoneNumber: "0123456789",
  creationDate: new Date(),
  pickupDate: new Date(),
  desiredPickupDate: new Date(new Date().setDate(new Date().getDate() + 5)),
  comments: "Bitte um sorgfältige Überprüfung der Bindungen.",
  status: "Offen",
  serviceType: { _id: "1", name: "Kleiner Service", cost: NumberDecimal("34.95") },
  priority: { _id: "2", priorityName: "Standard", dayCount: 0 }
};

serviceTypesData.forEach(data => {
  upsertData("serviceTypes", { _id: data._id }, { $set: data });
});

servicePrioritiesData.forEach(data => {
  upsertData("servicePriorities", { _id: data._id }, { $set: data });
});

upsertData("serviceOrders", { customerName: serviceOrderExample.customerName }, { $set: serviceOrderExample });

