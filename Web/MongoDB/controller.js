const db = require("model.js");

// Create and Save a new 
exports.create = (req, res) => {
    // Validate request
    if (!req.body['item-name']) {
        res.status(400).send({ message: "Item name can not be empty!" });
        return;
    }

    // Create an Item
    const item = new Item({
        itemName: req.body['item-name'],
        itemPrice: req.body['item-price'],
        itemDescription: req.body['item-description']
    });

    // Save Item in the database
    item
        .save(item)
        .then(data => {
            res.send(data);
        })
        .catch(err => {
            res.status(500).send({
                message:
                err.message || "Some error occurred while creating the Item."
            });
        });
};