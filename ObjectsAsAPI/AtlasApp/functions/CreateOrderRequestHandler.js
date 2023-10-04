const orderCollectionName = "Order";
const requestCollectionName = "CreateOrderRequest";

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
    
    const content = fullDoc.content;
    const creatorId = fullDoc._creatorId;

    var orderRef;
    var status;
    var rejectedReason;

    if(content.items == undefined || content.items.length < 1) {
      status = "Rejected";
      rejectedReason = "There are no items in this order!";
    } else {
      const orderCollection = db.collection(orderCollectionName);
      status = "Approved";
      
      const order = {
        "_id" : new BSON.ObjectId(),
        "_creatorId" : creatorId,
        "status": "Approved",
        "content": content,
      };
      
      const orderId = (await orderCollection.insertOne(order)).insertedId;
      
      orderRef = {
        $ref: orderCollectionName,
        $id: orderId,
        $db: databaseName,
      };
    }
      
    const requestCollection = db.collection(requestCollectionName);
    const update = { 
      "$set": {
        "status" : status,
        "order": orderRef,
        "rejectedReason": rejectedReason
      }
    }
    
    await requestCollection.updateOne({"_id": requestId}, update);
    
  } catch(err) {
    console.log("error while creating response: ", err.message);
  }
};
