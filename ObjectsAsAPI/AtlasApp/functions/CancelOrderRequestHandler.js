const orderCollectionName = "Order";
const requestCollectionName = "CancelOrderRequest";

const serviceName = "mongodb-atlas";
const databaseName = "ObjectsDatabase";

exports = async function(changeEvent) {
  try {
    var fullDoc = changeEvent.fullDocument;

    if (fullDoc.status !== "Pending") {
      return;
    }
    
    const requestId = changeEvent.documentKey._id;
    const db = context.services.get(serviceName).db(databaseName);
    
    const creatorId = fullDoc._creatorId;

    var orderId = fullDoc.orderId;
    
    const orderCollection = db.collection(orderCollectionName);
    const order = await orderCollection.findOne({_id: orderId});
    
    var status;
    var rejectedReason;

    if(order.content.items.length <= 1) {
      status = "Rejected";
      rejectedReason = "There are too few items in this order to be cancelled!";
    } else {
      const update = { 
        "$set": {
          "status" : "Cancelled",
        }
      }
      
      await orderCollection.updateOne({"_id": orderId}, update);
      
      status = "Approved";
    }
      
    const requestCollection = db.collection(requestCollectionName);
    const update = { 
      "$set": {
        "status" : status,
        "rejectedReason" : rejectedReason,
      }
    }
    
    await requestCollection.updateOne({"_id": requestId}, update);
    
  } catch(err) {
    console.log("error while creating response: ", err.message);
  }
};