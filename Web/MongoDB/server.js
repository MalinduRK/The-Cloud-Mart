const express = require('express');
const app = express();
const http = require('http');
const server = http.createServer(app);
const path = require('path');

// parse requests of content-type - application/json
app.use(express.json());

// parse requests of content-type - application/x-www-form-urlencoded
app.use(express.urlencoded({ extended: true }));

// Connecting static file path to express
app.use(express.static(path.join(__dirname, "resources")));

// Routes
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, '/resources/index.html'));
});

require('routes')(app);

db.mongoose
    .connect("mongodb+srv://MalinduRK:e0r2SWjof7Yh8e98@rpg-cluster.nb4qi3z.mongodb.net/The-Cloud-Mart?retryWrites=true&w=majority", {
        useNewUrlParser: true,
        useUnifiedTopology: true
    })
    .then(() => {
        console.log("Successfully connect to MongoDB.");
        initial();
    })
    .catch(err => {
        console.error("Connection error", err);
        process.exit();
    });           

// set port, listen for requests
const PORT = 3000;
// This isn't app.listen but server.listen
server.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}.`);
});