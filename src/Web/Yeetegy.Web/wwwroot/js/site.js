// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//Get the button
var mybutton = document.getElementById("myBtn");

// When the user scrolls down 20px from the top of the document, show the button
window.onscroll = function () { scrollFunction(), MyTest() };

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

var myh2 = document.getElementById("test123");

$(document).ready(function () {
    var pageindex = 2;
    var NoMoredata = false;
    var inProgress = false;
    var jsonn;
    var newContent;
    $(window).on("scroll", function () {
        var docHeight = $(document).height();
        var winScrolled = $(window).height() + $(window).scrollTop();
        if ((docHeight - winScrolled) < 500) {
            //console.log("module scrolled to bottom");
            inProgress = true;
            alert("new funct");
            fetch('/Posts/GetPost',
                {
                    method: "GET"
                }).then(response => {
                    if (response.status === 200) {
                        response.json().then(j => {
                            jsonn = JSON.parse(j),
                                document.getElementById("srcP").src = jsonn.ImgUrl;
                        });
                    }
                });
        }


    });
})