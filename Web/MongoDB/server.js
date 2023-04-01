const express = require('express');
const router = express.Router();
const { MongoClient } = require('mongodb');
const mongoose = require('mongoose');
const bodyParser = require('body-parser');
const app = express();
const path = require('path');

// Use body-parser middleware to parse form data
router.use(express.urlencoded({ extended: true }));

// Connecting static file path to express
app.use(express.static(path.join(__dirname, "resources")));

// Routes
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, '/resources/index.html'));
});

// Define a route for handling form submissions
router.post('/items', async (req, res) => {
    // Get the data from the form
    const name = req.body['item-name'];
    const price = req.body['item-price'];
    const description = req.body['item-description'];

    // Connect to the MongoDB cluster
    const uri = 'mongodb+srv://MalinduRK:e0r2SWjof7Yh8e98@rpg-cluster.nb4qi3z.mongodb.net/The-Cloud-Mart?retryWrites=true&w=majority';
    const client = new MongoClient(uri, { useNewUrlParser: true, useUnifiedTopology: true });

    try {
        await client.connect();
        const database = client.db('The-Cloud-Mart');
        const collection = database.collection('Items');

        // Create a new item document with the form data
        const newItem = { name: name, price: price, description: description };

        // Insert the new item document into the database
        await collection.insertOne(newItem);
        res.status(201).json({ message: 'Item added successfully' });
    } catch (error) {
        console.error(error);
        res.status(500).json({ message: 'Error adding item' });
    } finally {
        await client.close();
    }
});

app.listen(3000, () => {
    console.log('Server listening on port 3000');
});

module.exports = router;