// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//Get the button
var pageindex = 2;
var noMoredata = false;
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
        '<button type="button"  onClick="likeButton()" data-arg1="PostId" class="btn btn-dark fa fa-thumbs-up" style="width: 100px">Likes</button>\n' +
        '<button type="button"  onClick="likeButton()" data-arg1="PostId" class="btn btn-dark fa fa-thumbs-down" style="width: 100px">Dislikes</button>\n' +
        '<button type="button" onClick="likeButton()" data-arg1="PostId" class="btn btn-dark fa fa-comment" style="width: 100px"></button>\n' +
        '</div></div></div>\n';

function getPosts() {
    var docHeight = $(document).height();
    var winScrolled = $(window).height() + $(window).scrollTop();
    if ((docHeight - winScrolled) < 200) { // scroll time 
        if (noMoredata === false && inProgress === false) {
            inProgress = true;
            fetch('/Posts/GetPost',
                {
                    method: "GET",
                    headers: {
                        "X-Category": location.pathname.split("/").pop()
                    }
                }).then(response => {
                if (response.status === 200) {
                    response.json().then(j => {
                        jsonn = JSON.parse(j);
                        if (jsonn.length === 0) {
                            noMoredata = true;
                        } else {
                            for (let i = 0; i < 5; i++) {
                                posthtml = template,
                                    posthtml = posthtml.replace("PostId", jsonn[i].Id),
                                    posthtml = posthtml.replace("Dislikes", jsonn[i].Dislikes),
                                    posthtml = posthtml.replace("PostId", jsonn[i].Id),
                                    posthtml = posthtml.replace("Likes", jsonn[i].Likes),
                                    posthtml = posthtml.replace("PostId", jsonn[i].Id),
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
}

$(document).ready(function () {
    //alert(location.pathname);
    $(window).on("scroll",getPosts);
});
