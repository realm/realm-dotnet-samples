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
    
    const payload = fullDoc.payload;
    const creatorId = fullDoc._creatorId;

    var orderId = payload.orderId;
    
    const orderCollection = db.collection(orderCollectionName);
    const order = await orderCollection.findOne({_id: orderId});
    
    var response;

    if(order.content.items.length == 1) {
      response = {
        "status": "Rejected",
        "orderId": orderId,
        "rejectedReason": "There are too few items in this order to be cancelled!"
      }
    } else {
      const update = { 
        "$set": {
          "status" : "Cancelled",
        }
      }
      
      await orderCollection.updateOne({"_id": orderId}, update);
      
      response = {
        "status": "Approved",
        "orderId": orderId,
      }
    }
      
    const requestCollection = db.collection(requestCollectionName);
    const update = { 
      "$set": {
        "status" : "Handled",
        "response": response 
      }
    }
    
    await requestCollection.updateOne({"_id": requestId}, update);
    
  } catch(err) {
    console.log("error while creating response: ", err.message);
  }
};