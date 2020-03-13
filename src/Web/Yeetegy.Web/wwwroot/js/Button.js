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
    var nameOfFunction = this[event.target.name];
    var id = event.target.getAttribute("id");
    //alert(id);

    fetch("/Posts/Like?id=" + id.split("_").pop(),
        {
            method: "POST",
        }).then(response => {
            if (response.status === 200) {
                isClicked = true;
            } else if(response.status === 204){
                window.location = "/Identity/Account/Login";
            }
    });


    if (isClicked === true) {
        var newLikeCount = parseInt(event.target.innerHTML) - 1;
        event.target.innerHTML = newLikeCount;
        isClicked = false;
    } else {
        var newLikeCount = parseInt(event.target.innerHTML) + 1;
        event.target.innerHTML = newLikeCount;
    }
   
}
