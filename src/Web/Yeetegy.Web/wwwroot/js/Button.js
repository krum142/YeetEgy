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

function likeButton() {
    var id = event.target.getAttribute("id");
    var urll = "/Posts/Like?id=" + id.split("_").pop();

    request = new XMLHttpRequest();
    request.open('GET', urll, /* async = */ false);
    request.send();
    var errorCode = request.responseURL.split('=').pop();
    var code = request.status;
    if (errorCode == 401) {
        alert(errorCode);
    } else if (code == 200) {
        event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
    } else if(code == 204){
        event.target.innerHTML = parseInt(event.target.innerHTML) - 1;
    }

}
