const serviceName = "mongodb-atlas";
const databaseName = "restaurantsDb";
const restaurantsCollectionName = "restaurants";
const requestsCollectionName = "requestsInfo";

// Add base path for Data API
const baseDataApiPath = "";

exports = async function(changeEvent){
  
  if(baseDataApiPath.length === 0) {
    throw Error("Remember to add your own base path for Data API!")
  }
  
  const cuisine = changeEvent.fullDocument.cuisine;
  const requestTimestamp = changeEvent.fullDocument.timestamp;

  const db = context.services.get(serviceName).db(databaseName);
  const restaurantsCollection = db.collection(restaurantsCollectionName);
  const requestsCollection = db.collection(requestsCollectionName);
  
  // Check if we already requested the same data earlier
  const info = await requestsCollection.findOne({"cuisine": cuisine});
  
  if (info != null) {
    const minutesDifference = (requestTimestamp - info.lastRequestTimestamp) / (1000*60);
    if(minutesDifference < 10) {
      // We requested the data less than 10 minutes ago, nothing to do
      return;
    }
  }
  
  // Retrieve restaurants from API
  const parsedResult = await fetchRestaurantsFromAPI(cuisine);
  
  // Add retrieved data to Atlas
  const bulkWriteOperation = parsedResult.map( doc => ({
    updateOne: {
      filter: { _id: new BSON.ObjectId(doc._id) },
      update: { $set: {
        _id: new BSON.ObjectId(doc._id),
        name: doc.name,
        borough: doc.borough,
        cuisine: cuisine,
      }},
      upsert: true //Insert if not already present
    },
  }));
  
  await restaurantsCollection.bulkWrite(bulkWriteOperation, {ordered: false});
  
  // Update request info collection with last request timestamp
  const newInfo = {
    "cuisine": cuisine,
    "lastRequestTimestamp": new Date()
  }
  
  await requestsCollection.updateOne(
    {"cuisine": cuisine}, //Filter
    { "$set":  newInfo},
    { "upsert": true } //Insert if not already present
    )
};

async function fetchRestaurantsFromAPI(cuisine) {
  var body = {
    "dataSource": "mongodb-atlas",
     "database": "sample_restaurants",
     "collection": "restaurants",
     "limit": 20,
     "filter": {
      "cuisine": cuisine
    },
    "projection": {
      "name": 1,
      "borough": 1
    }
  }
  
  var result = await context.http.post({
    url: baseDataApiPath + "/action/find",
    body: JSON.stringify(body),
    headers: { 
      "Content-Type": ["application/json"],
      "Accept": ["application/json"]}
  });
  
  return JSON.parse(result.body.text()).documents;
}