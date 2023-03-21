const firebaseConfig = {
    //   copy your firebase config informations
    apiKey: "AIzaSyBIYvM5ZsdY5C-xH8qIIRDruHs3aPGMW54",
        authDomain: "the-cloud-mart.firebaseapp.com",
        databaseURL: "https://the-cloud-mart-default-rtdb.asia-southeast1.firebasedatabase.app",
        projectId: "the-cloud-mart",
        storageBucket: "the-cloud-mart.appspot.com",
        messagingSenderId: "558540507839",
        appId: "1:558540507839:web:bf367f1785365b7178540a",
        measurementId: "G-F2EEES7N52"
};

// initialize firebase
firebase.initializeApp(firebaseConfig);

// reference your database
var contactFormDB = firebase.database().ref("contactForm");

document.getElementById("contactForm").addEventListener("submit", submitForm);

function submitForm(e) {
    e.preventDefault();

    var name = getElementVal("name");
    var emailid = getElementVal("emailid");
    var msgContent = getElementVal("msgContent");

    saveMessages(name, emailid, msgContent);

    //   enable alert
    document.querySelector(".alert").style.display = "block";

    //   remove the alert
    setTimeout(() => {
        document.querySelector(".alert").style.display = "none";
    }, 3000);

    //   reset the form
    //document.getElementById("contactForm").reset();
}

const saveMessages = (name, emailid, msgContent) => {
    var newContactForm = contactFormDB.push();

    newContactForm.set({
        name: name,
        emailid: emailid,
        msgContent: msgContent,
    });
};

const getElementVal = (id) => {
    return document.getElementById(id).value;
};
