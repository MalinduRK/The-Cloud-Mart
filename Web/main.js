// Initialize Firebase
var firebaseConfig = {
    // your Firebase config here
    apiKey: "AIzaSyBIYvM5ZsdY5C-xH8qIIRDruHs3aPGMW54",
    authDomain: "the-cloud-mart.firebaseapp.com",
    databaseURL: "https://the-cloud-mart-default-rtdb.asia-southeast1.firebasedatabase.app",
    //databaseURL: "https://the-cloud-mart-default-rtdb.firebaseio.com ",
    projectId: "the-cloud-mart",
    storageBucket: "the-cloud-mart.appspot.com",
    messagingSenderId: "558540507839",
    appId: "1:558540507839:web:bf367f1785365b7178540a",
    measurementId: "G-F2EEES7N52"
};

firebase.initializeApp(firebaseConfig);

// Get a reference to the Firestore database
var db = firebase.firestore();

// Get references to the form input fields
var itemName = document.getElementById("item-name");
var itemPrice = document.getElementById("item-price");
var itemDescription = document.getElementById("item-description");
//var itemImageUrl = document.getElementById("item-image-url");

// Handle form submission
var form = document.getElementById("item-form");
form.addEventListener("submit", function(event) {
    event.preventDefault(); // prevent the form from submitting normally

    // Get the values from the input fields
    var name = itemName.value;
    var price = parseFloat(itemPrice.value);
    var description = itemDescription.value;
    //var imageUrl = itemImageUrl.value;

    // Create a new document in the "items" collection with the item data
    db.collection("items").add({
        name: name,
        description: description,
        price: price,
        //imageUrl: imageUrl
    })
    .then(function(docRef) {
        console.log("Item added with ID: ", docRef.id);
        // optionally redirect to a success page
    })
    .catch(function(error) {
        console.error("Error adding item: ", error);
    });
});