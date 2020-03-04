// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//Get the button
var mybutton = document.getElementById("myBtn");

// When the user scrolls down 20px from the top of the document, show the button
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

var myh2 = document.getElementById("test123");


$(document).ready(function () {
    var pageindex = 2;
    var NoMoredata = false;
    var inProgress = false;
    var jsonn;
    var posthtml;

    var template =
        '<div class="col-lg-8 col-md-8 mb-8" style="margin-bottom: 40px">\n' +
            '<div class="card h-100">\n' +
            '<div class="card-header font-italic">\n' +
            '<h5>TittleToChange</h5>\n' +
            '</div>\n' +
            '<img id="1" src="ImgToChange" class="card-img-top">\n' +
            '<div class="card-footer">\n' +
            '<button type="button" class="btn btn-dark far fa-thumbs-up" style="width: 100px">Likes</button>\n' +
            '<button type="button" class="btn btn-dark far fa-thumbs-down" style="width: 100px">Dislikes</button>\n' +
            '<button type="button" class="btn btn-dark far fa-comment" style="width: 100px"></button>\n' +
        '</div></div></div>\n';

    alert(location.pathname);
    $(window).on("scroll",
        function () {
            var docHeight = $(document).height();
            var winScrolled = $(window).height() + $(window).scrollTop();
            if ((docHeight - winScrolled) < 200) { // scroll time 
                if (NoMoredata === false && inProgress === false) {
                    inProgress = true;
                    fetch('/Posts/GetPost',
                        {
                            method: "GET",
                            headers: {
                                "X-Category": location.pathname
                            }
                        }).then(response => {
                            if (response.status === 200) {
                                response.json().then(j => {
                                    jsonn = JSON.parse(j);
                                    if (jsonn.length === 0) {
                                        NoMoredata = true;
                                    } else {
                                        for (let i = 0; i < 5; i++) {
                                            posthtml = template,
                                                posthtml = posthtml.replace("Dislikes", jsonn[i].Dislikes),
                                                posthtml = posthtml.replace("Likes", jsonn[i].Likes),
                                                posthtml = posthtml.replace("ImgToChange", jsonn[i].ImgUrl),
                                                posthtml = posthtml.replace("TittleToChange", jsonn[i].Tittle),
                                                document.getElementById("PostContainer").innerHTML += posthtml;
                                        }
                                    }

                                });
                                inProgress = false;
                            }
                        });
                }

            }

        });
});


