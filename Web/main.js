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

//import { getStorage, ref, uploadBytes } from "firebase/storage";

const storage = firebase.storage().ref();

// Get references to the form input fields
var itemName = document.getElementById("item-name");
var itemPrice = document.getElementById("item-price");
var itemDescription = document.getElementById("item-description");
//var itemImageUrl = document.getElementById("item-image-url");
var alertMessage = document.getElementById("alert");

// Handle form submission
var form = document.getElementById("item-form");
form.addEventListener("submit", async function(event) {
    event.preventDefault(); // prevent the form from submitting normally

    // Get the values from the input fields
    var name = itemName.value;
    var price = parseFloat(itemPrice.value);
    var description = itemDescription.value;
    const file = form.elements.image.files[0];
    //var imageUrl = itemImageUrl.value;

    // Upload the file to Firebase Storage
    const storageRef = firebase.storage().ref().child("item-images/" + file.name);
    const snapshot = await storageRef.put(file);

    // Get the download URL for the file
    const imageUrl = await snapshot.ref.getDownloadURL();

    // Create a new document in the "items" collection with the item data
    db.collection("items").add({
        name: name,
        description: description,
        price: price,
        imageUrl: imageUrl
    })
    .then(function(docRef) {
        console.log("Item added with ID: ", docRef.id);
        // optionally redirect to a success page
        
        // Change its background color
        alertMessage.style.display = "block";
    })
    .catch(function(error) {
        console.error("Error adding item: ", error);
    });
});



// firebase storage
/*
import { getStorage, ref, uploadBytes } from "firebase/storage";

const storage = getStorage();

<form id="my-form">
  <input type="text" name="name" placeholder="Item name">
  <input type="text" name="description" placeholder="Item description">
  <input type="number" name="price" placeholder="Item price">
  <input type="file" name="image" accept="image/*">
  <button type="submit">Add Item</button>
</form>

const form = document.getElementById("my-form");

form.addEventListener("submit", async (event) => {
  event.preventDefault();

  const name = form.elements.name.value;
  const description = form.elements.description.value;
  const price = form.elements.price.value;
  const file = form.elements.image.files[0];

  // Upload the file to Firebase Storage
  const storageRef = ref(storage, "item-images/" + file.name);
  const snapshot = await uploadBytes(storageRef, file);

  // Get the download URL for the file
  const imageUrl = await snapshot.ref.getDownloadURL();

  // Save the item data to Firestore
  const itemRef = await db.collection("items").add({
    name: name,
    description: description,
    price: price,
    imageUrl: imageUrl
  });

  console.log("Item added with ID: ", itemRef.id);

  // Reset the form
  form.reset();
});
*/