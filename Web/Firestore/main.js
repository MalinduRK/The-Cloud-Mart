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

// Open dimension fields only if the checkbox is checked
const dimensionCheckbox = document.getElementById('dimension-check');
var dimensionBox = document.getElementById("dimension-box");

dimensionCheckbox.addEventListener('change', function() {
    if (this.checked) {
        dimensionBox.style.display = "block";
    } else {
        dimensionBox.style.display = "none";
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

// Get a reference to the Firestore database
var db = firebase.firestore();

const storage = firebase.storage().ref();

var form = document.getElementById("item-form");

// Get references to the form input fields
var itemCategory = document.getElementById("item-category");
var itemName = document.getElementById("item-name");
var itemPrice = document.getElementById("item-price");
var itemDescription = document.getElementById("item-description");

var itemLength = document.getElementById("item-length");
var itemWidth = document.getElementById("item-width");
var itemHeight = document.getElementById("item-height");

var progressText = document.getElementById("progress-text");

// Handle form submission
form.addEventListener("submit", async function(event) {
    event.preventDefault(); // prevent the form from submitting normally

    console.log("Adding item to firebase...");
    progressText.textContent = "Adding item to firebase...";

    // Get the values from the input fields
    var category = itemCategory.value;
    var name = itemName.value;
    var price = parseFloat(itemPrice.value);
    var description = itemDescription.value;

    // Image
    var imageCheck =imageCheckbox.checked;
    
    // Model values
    var modelCheck = modelCheckbox.checked;
    var length = parseFloat(itemLength.value);;
    var width = parseFloat(itemWidth.value);
    var height = parseFloat(itemHeight.value);

    // Create a new document in the "items" collection with the item data
    db.collection(category).add({
        itemName: name,
        itemDescription: description,
        itemPrice: price,
        sellerName: "sellerbot",
        modelAdded: modelCheck,
        itemLength: length,
        itemWidth: width,
        itemHeight: height
    })
    .then(async function(docRef) {
        console.log("Item added with ID: ", docRef.id);
        // Upload image only if the checkbox is checked
        if (imageCheck) {
            await uploadImage(docRef.id);
        }
        // Get model values only if the checkbox is checked
        if (modelCheck) {
            await uploadModel(docRef.id);
        }
        progressText.textContent = "Completed!";
        alert("Successfully added item!");
        location.reload();
    })
    .catch(function(error) {
        console.error("Error adding item: ", error);
    });
});

async function uploadImage(itemId) {
    console.log('Uploading image');
    progressText.textContent = "Uploading image to firebase...";
    const imageFile = form.elements.image.files[0];

    // Extract the file extension from the file name
    const fileName = imageFile.name;
    const fileExtension = fileName.substr(fileName.lastIndexOf('.') + 1);

    // Upload the image file to Firebase Storage
    const storageRef = firebase.storage().ref().child("images/" + itemId + "_image." + fileExtension);
    await storageRef.put(imageFile);
    //const snapshot = await storageRef.put(imageFile);
    // Get the download URL for the image file
    //imageUrl = await snapshot.ref.getDownloadURL();
}

async function uploadModel(itemId) {
    console.log('Uploading model');
    progressText.textContent = "Uploading model to firebase...";
    modelFile = form.elements.model.files[0];

    // Extract the file extension from the file name
    const fileName = modelFile.name;
    const fileExtension = fileName.substr(fileName.lastIndexOf('.') + 1);

    // Upload the model file to Firebase Storage
    const storageRef_model = firebase.storage().ref().child("models/" + itemId + "_model." + fileExtension);
    await storageRef_model.put(modelFile);
    //const snapshot_model = await storageRef_model.put(modelFile);
    // Get the download URL for the model file
    //modelUrl = await snapshot_model.ref.getDownloadURL();
}