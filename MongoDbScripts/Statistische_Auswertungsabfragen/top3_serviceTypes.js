db.serviceOrders.aggregate([
    {
      $group: {
        _id: "$serviceType._id",
        Anzahl: { $count: {} }
      }
    },
    {
      $sort: { Anzahl: -1 }
    },
    {
      $limit: 3
    },
    {
      $lookup: {
        from: "serviceTypes",
        localField: "_id",
        foreignField: "_id",
        as: "serviceTypeInfo"
      }
    },
    {
      $unwind: "$serviceTypeInfo"
    },
    {
      $project: {
        ServiceTyp: "$serviceTypeInfo.name",
        Anzahl: 1
      }
    }
  ]);
  