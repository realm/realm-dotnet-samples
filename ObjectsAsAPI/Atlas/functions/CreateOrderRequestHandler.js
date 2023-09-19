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
    
    const payload = fullDoc.payload;
    const creatorId = fullDoc._creatorId;

    var response;

    if(payload.content.items == undefined) {
      response = {
        "status": "Rejected",
        "rejectedReason": "There are no items in this order!"
      }
    } else {
      const orderCollection = db.collection(orderCollectionName);
      
      const order = {
        "_id" : new BSON.ObjectId(),
        "_creatorId" : creatorId,
        "status": "Approved",
        "content": payload.content,
      };
      
      const orderId = (await orderCollection.insertOne(order)).insertedId;
      
      const orderRef = {
        $ref: orderCollectionName,
        $id: orderId,
        $db: databaseName,
      };
      
      response = {
        "order": orderRef,
        "status": "Approved",
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
