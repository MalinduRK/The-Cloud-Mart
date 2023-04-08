module.exports = app => {
    const item = require("controller.js");
  
    var router = require("express").Router();
  
    // Create a new Lobby
    router.post("/", item.create);
  
    app.use('/api/items', router);
};