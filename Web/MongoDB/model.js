const mongoose = require("mongoose");

const Item = mongoose.model(
    "Item",
    new mongoose.Schema({
        itemName: String,
        itemPrice: String,
        itemDescription: String
    })
);

module.exports = Item;