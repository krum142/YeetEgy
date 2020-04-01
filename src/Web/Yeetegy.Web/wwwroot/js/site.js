// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//Get the button


//var myPageIndex = 0;
//var noMoredata = false;
//var inProgress = false;
//var jsonn;
//var posthtml;

//function DomOperation() {
//    if (noMoredata === false && inProgress === false) {
//        inProgress = true;
//        fetch('/Posts/GetPosts?page=' + myPageIndex + "&category=" + location.pathname.split("/").pop(),
//            {
//                method: "GET",
//            }).then(response => {
//                if (response.status === 200) {
//                    response.text().then(html => {
//                        document.getElementById("PostContainer").innerHTML += html;
//                    });
//                    inProgress = false;
//                    myPageIndex += 5;
//                }
//            });
//    }
//}

//function getPosts() {
//    var docHeight = $(document).height();
//    var winScrolled = $(window).height() + $(window).scrollTop();
//    if ((docHeight - winScrolled) < 200) { // scroll time 
//        DomOperation.call();
//    }
//}

//$(document).ready(function () {
//    $(window).on("scroll", getPosts);
//});

