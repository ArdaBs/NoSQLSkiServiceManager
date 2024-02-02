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
      $unwind: "$serviceTypeInfo"
    },
    {
      $group: {
        _id: "$serviceTypeInfo.name",
        DurchschnittsKosten: { $avg: "$serviceTypeInfo.cost" }
      }
    }
  ]);
  