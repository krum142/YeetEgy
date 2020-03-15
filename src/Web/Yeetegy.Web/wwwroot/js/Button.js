// When the user scrolls down 20px from the top of the document, show the button
var mybutton = document.getElementById("myBtn");
var isClicked = false;
var notLoggedIn = false;
window.onscroll = function () { scrollFunction() };

function scrollFunction() {
    if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
        mybutton.style.display = "block";
    } else {
        mybutton.style.display = "none";
    }
}

// When the user clicks on the button, scroll to the top of the document
function topFunction() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}

function voteButton() {
    var id = event.target.getAttribute("id");
    var vote = id.split("_")[0];
    var urll = "/Posts/Vote?" + "vote=" + vote + "&id=" + id.split("_")[1];

    request = new XMLHttpRequest();
    request.open('GET', urll, /* async = */ false);
    request.send();

    var myRequestUrl = request.responseURL;
    var errorCode = myRequestUrl.split('=').pop();
    var code = request.status;

    if (errorCode == 404) {
        window.location = myRequestUrl;
    }
    else if (errorCode == 401) {
        window.location = "/Identity/Account/Login";
    }
    else if (vote === "Like") {
        if (code == 202) {
            event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
            event.target.nextSibling.nextSibling.innerHTML =
                parseInt(event.target.nextSibling.nextSibling.innerHTML) - 1;
        }
        else if (code == 200) {
            event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
        }
        else if (code == 204) {
            event.target.innerHTML = parseInt(event.target.innerHTML) - 1;
        }
    }
    else if (vote === "Dislike") {
        if (code == 202) {
            event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
            event.target.previousSibling.previousSibling.innerHTML =
                parseInt(event.target.previousSibling.previousSibling.innerHTML) - 1;
        }
        else if (code == 200) {
            event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
        }
        else if (code == 204) {
            event.target.innerHTML = parseInt(event.target.innerHTML) - 1;
        }
    }

}

//function disLikeButton() {
//    var id = event.target.getAttribute("id");
//    var urll = "/Posts/Dislike?id=" + id.split("_").pop();

//    request = new XMLHttpRequest();
//    request.open('GET', urll, /* async = */ false);
//    request.send();
//    var myRequestUrl = request.responseURL;
//    var errorCode = myRequestUrl.split('=').pop();
//    var code = request.status;

//    if (errorCode == 404) {
//        window.location = myRequestUrl;
//    } else if (errorCode == 401) {
//        window.location = "/Identity/Account/Login";
//    } else if (code == 202) {
//        event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
//        event.target.previousSibling.previousSibling.innerHTML = parseInt(event.target.previousSibling.previousSibling.innerHTML) - 1;
//    } else if (code == 200) {
//        event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
//    } else if (code == 204) {
//        event.target.innerHTML = parseInt(event.target.innerHTML) - 1;
//    }
//}