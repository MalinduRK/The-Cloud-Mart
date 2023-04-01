// Open model fields only if the checkbox is checked
const modelCheckbox = document.getElementById('model-check');
var modelBox = document.getElementById("model-box");

modelCheckbox.addEventListener('change', function() {
    if (this.checked) {
        modelBox.style.display = "block";
    } else {
        modelBox.style.display = "none";
    }
});

// Open image field only if the checkbox is checked
const imageCheckbox = document.getElementById('image-check');
var addImageButton = document.getElementById("add-image-button");

imageCheckbox.addEventListener('change', function() {
    if (this.checked) {
        addImageButton.style.display = "block";
    } else {
        addImageButton.style.display = "none";
    }
});

// Initialize Firebase
var firebaseConfig = {
    apiKey: "AIzaSyBIYvM5ZsdY5C-xH8qIIRDruHs3aPGMW54",
    authDomain: "the-cloud-mart.firebaseapp.com",
    databaseURL: "https://the-cloud-mart-default-rtdb.asia-southeast1.firebasedatabase.app",
    projectId: "the-cloud-mart",
    storageBucket: "the-cloud-mart.appspot.com",
    messagingSenderId: "558540507839",
    appId: "1:558540507839:web:bf367f1785365b7178540a",
    measurementId: "G-F2EEES7N52"
};

firebase.initializeApp(firebaseConfig);

const storage = firebase.storage().ref();

var form = document.getElementById("item-form");

// Get references to the form input fields
var itemName = document.getElementById("item-name");
var itemPrice = document.getElementById("item-price");
var itemDescription = document.getElementById("item-description");

var itemLength = document.getElementById("item-length");
var itemWidth = document.getElementById("item-width");
var itemHeight = document.getElementById("item-height");

// Handle form submission
form.addEventListener("submit", async function(event) {
    event.preventDefault(); // prevent the form from submitting normally

    // Get the values from the input fields
    var name = itemName.value;
    var price = parseFloat(itemPrice.value);
    var description = itemDescription.value;
    const imageFile = form.elements.image.files[0];
    // Model values
    var modelCheck = modelCheckbox.checked;
    var length = null;
    var width = null;
    var height = null;
    var modelFile = null;
    var modelUrl = null;
    
    var imageUrl = null;
    // Upload image only if the checkbox is checked
    if (imageCheckbox.checked) {
        console.log('Uploading image');
        // Upload the image file to Firebase Storage
        const storageRef = firebase.storage().ref().child("item-images/" + imageFile.name);
        const snapshot = await storageRef.put(imageFile);
        // Get the download URL for the image file
        imageUrl = await snapshot.ref.getDownloadURL();
    }

    // Get model values only if the checkbox is checked
    if (modelCheck) {
        console.log('Uploading model');
        length = parseFloat(itemLength.value);
        width = parseFloat(itemWidth.value);
        height = parseFloat(itemHeight.value);
        modelFile = form.elements.model.files[0];

        // Upload the model file to Firebase Storage
        const storageRef_model = firebase.storage().ref().child("item-models/" + modelFile.name);
        const snapshot_model = await storageRef_model.put(modelFile);
        // Get the download URL for the model file
        modelUrl = await snapshot_model.ref.getDownloadURL();
    }

    // Create a new document in the "items" collection with the item data
    /*
    $.ajax({
        type: "POST",
        url: "http://localhost:3000/items",
        data: JSON.stringify({ "itemName": itemName, "itemPrice": itemPrice, "itemDescription" : itemDescription }),
        contentType: "application/json",
        success: function (result) {
            console.log(result);
        },
        error: function (result, status) {
            console.log(result);
        }
    });*/
});