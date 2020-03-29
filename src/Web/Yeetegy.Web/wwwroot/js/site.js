// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//Get the button


var myPageIndex = 0;
var noMoredata = false;
var inProgress = false;
var jsonn;
var posthtml;

function DomOperation() {
    if (noMoredata === false && inProgress === false) {
        inProgress = true;
        fetch('/Posts/GetPosts?page=' + myPageIndex,
            {
                method: "GET",
                headers: {
                    "X-Category": location.pathname.split("/").pop()
                }
            }).then(response => {
                if (response.status === 200) {
                    response.json().then(j => {
                        jsonn = j;
                        if (jsonn.length === 0) {
                            noMoredata = true;
                        } else {
                            for (let i = 0; i < 5; i++) {
                                document.getElementById("PostContainer").innerHTML +=
                                    '<div class="col-lg-8 col-md-12" style="margin-bottom: 30px">\n' +
                                    '<div>\n' +
                                    '<hr style="border-top: 2px solid #043927;">\n' +
                                    '<h5><span class="badge badge-light">' + jsonn[i].time + '</span></h5>\n' +
                                    '<div class="card h-100">\n' +
                                    '<div class="card-header font-italic">\n' +
                                    '<h5><b>' + jsonn[i].tittle + '</b></h5>\n' +
                                    '</div>\n' +
                                    '<img id="1" src="' + jsonn[i].imgUrl + '" class="card-img-top">\n' +
                                    '<div class="card-footer">\n' +
                                    '<button type="button"  onClick="voteButton()" id="Post_Like_' + jsonn[i].id + '" class="btn btn-dark fa fa-thumbs-up" style="width: 90px">' + jsonn[i].likes + '</button>\n' +
                                    '<button type="button"  onClick="voteButton()" id="Post_Dislike_' + jsonn[i].id + '" class="btn btn-dark fa fa-thumbs-down" style="width: 90px">' + jsonn[i].dislikes + '</button>\n' +
                                    '<button type="button" onClick="redirect()" id="' + jsonn[i].id + '" class="btn btn-dark fa fa-comment" style="width: 90px">' + jsonn[i].commentsCount + '</button>\n' +
                                    '</div></div></div></div>\n';
                            }
                        }
                    });
                    inProgress = false;
                    myPageIndex += 5;
                }
            });
    }
}

function getPosts() {
    var docHeight = $(document).height();
    var winScrolled = $(window).height() + $(window).scrollTop();
    if ((docHeight - winScrolled) < 200) { // scroll time 
        DomOperation.call();
    }
}

////document.onload = getPosts();
//$(document).ready(function () {
//    //alert(location.pathname);
//    $(window).on("scroll", getPosts);
//});

