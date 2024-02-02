db.serviceOrders.aggregate([
  {
    $lookup: {
      from: "servicePriorities",
      localField: "priority._id",
      foreignField: "_id",
      as: "priorityInfo"
    }
  },
  {
    $unwind: "$priorityInfo"
  },
  {
    $group: {
      _id: "$priorityInfo.priorityName",
      AnzahlOrders: { $count: {} }
    }
  }
]);
