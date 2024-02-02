function upsertData(collectionName, query, data) {
  db.getCollection(collectionName).updateOne(query, { $set: data }, { upsert: true });
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

const serviceOrdersData = [
  {
    customerName: "Julia Müller",
    email: "julia.mueller@example.com",
    phoneNumber: "0987654321",
    creationDate: new Date("2023-02-20"),
    desiredPickupDate: new Date("2023-02-25"),
    comments: "Schnellstmögliche Bearbeitung erwünscht.",
    status: {
      statusValue: "Offen",
      description: "Die Bestellung ist offen und noch nicht bearbeitet."
    },
    serviceType: {
      _id: "2",
      name: "Grosser Service",
      cost: 59.95 
    },
    priority: {
      _id: "3",
      priorityName: "Express",
      dayCount: -2
    }
  },
  {
    customerName: "Tobias Schmidt",
    email: "tobias.schmidt@example.com",
    phoneNumber: "0123987654",
    creationDate: new Date("2023-03-15"),
    desiredPickupDate: new Date("2023-03-20"),
    comments: "Bitte auf besondere Materialpflege achten.",
    status: {
      statusValue: "Offen",
      description: "Die Bestellung ist offen und noch nicht bearbeitet."
    },
    serviceType: {
      _id: "3",
      name: "Rennski-Service",
      cost: 74.95
    },
    priority: {
      _id: "1",
      priorityName: "Low",
      dayCount: 5
    }
  },
  {
    customerName: "Anna Bergmann",
    email: "anna.bergmann@example.com",
    phoneNumber: "0987612345",
    creationDate: new Date("2023-04-05"),
    desiredPickupDate: new Date("2023-04-10"),
    comments: "Beratung zu weiteren Serviceleistungen gewünscht.",
    status: {
      statusValue: "Offen",
      description: "Die Bestellung ist offen und noch nicht bearbeitet."
    },
    serviceType: {
      _id: "4",
      name: "Bindung montieren und einstellen",
      cost: 24.95
    },
    priority: {
      _id: "2",
      priorityName: "Standard",
      dayCount: 0
    }
  }
];

const employeeData = [
  { username: "Arda", password: "12345678", isLocked: false, failedLoginAttempts: 0 },
  { username: "Lukas", password: "12345678", isLocked: false, failedLoginAttempts: 0 },
  { username: "Goku", password: "12345678", isLocked: false, failedLoginAttempts: 0 },
  { username: "Gojo", password: "12345678", isLocked: false, failedLoginAttempts: 0 }
];

employeeData.forEach(data => {
  upsertData("employees", { username: data.username }, data);
});

serviceTypesData.forEach(data => {
  upsertData("serviceTypes", { _id: data._id }, data);
});

servicePrioritiesData.forEach(data => {
  upsertData("servicePriorities", { _id: data._id }, data);
});

serviceOrdersData.forEach(order => {
  const query = { customerName: order.customerName, creationDate: order.creationDate };
  upsertData("serviceOrders", query, order);
});

db.serviceOrders.aggregate([
  {
    $lookup: {
      from: "serviceTypes",
      localField: "serviceType._id",
      foreignField: "_id",
      as: "serviceTypeInfo"
    }
  },
  {
    $lookup: {
      from: "servicePriorities",
      localField: "priority._id",
      foreignField: "_id",
      as: "priorityInfo"
    }
  },
  {
    $unwind: "$serviceTypeInfo"
  },
  {
    $unwind: "$priorityInfo"
  },
  {
    $set: {
      "serviceType.name": "$serviceTypeInfo.name",
      "serviceType.cost": "$serviceTypeInfo.cost",
      "priority.priorityName": "$priorityInfo.priorityName",
      "priority.dayCount": "$priorityInfo.dayCount"
    }
  },
  {
    $project: {
      serviceTypeInfo: 0,
      priorityInfo: 0
    }
  }
]).pretty();
