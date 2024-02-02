function checkAndCreateIndex(collection, indexSpecification, indexOptions) {
  const indexes = collection.getIndexes();
  const indexName = indexOptions.name;

  const indexExists = indexes.some(index => index.name === indexName);

  if (!indexExists) {
    collection.createIndex(indexSpecification, indexOptions);
    print(`Index ${indexName} created.`);
  } else {
    print(`Index ${indexName} already exists.`);
  }
}

const jetStreamDb = db.getSiblingDB("JetStreamAPI");

checkAndCreateIndex(jetStreamDb.serviceOrders, { "serviceType._id": 1, "priority._id": 1 }, { name: "serviceType_priority_index" });
checkAndCreateIndex(jetStreamDb.employees, { "username": 1 }, { unique: true, name: "username_unique_index" });
