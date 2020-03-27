var myPageIndex = 0;
var noMoredata = false;
var inProgress = false;
var jsonn;
var posthtml;

function domOperationComments() {
    if (noMoredata === false && inProgress === false) {
        inProgress = true;
        fetch('/api/Comments/?postid=5d26fa31-a661-4655-bd21-49d1ff60ff9b&offset=' + myPageIndex,
            {
                method: "GET",
            }).then(response => {
                if (response.status === 200) {
                    response.json().then(j => {
                        jsonn = j;
                        if (jsonn.length === 0) {
                            noMoredata = true;
                        } else {
                            for (let i = 0; i < 10; i++) {
                                document.getElementById("Comment-Container").innerHTML +=
                                    '<div class="row"><div style="width: 14%;margin-right: 7px">\n' +
                                    '<img style="display: block; margin: 0 auto; border-radius: 50%;" width="50" height="50" src="https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fc2.staticflickr.com%2F2%2F1386%2F791791681_ecc5efac79_b.jpg&f=1&nofb=1" />\n' +
                                    '</div><div style="width: 83%; float: right;">\n' +
                                    '<div style="width: 100%; word-wrap: break-word;" class="row">\n' +
                                    '<div style="width: 97%; float: left">\n' +
                                    '<a style="font-weight: bold;">Username</a>\n' +
                                    '<h9><span class="badge badge-light">5h</span></h9><br>\n'+
                                    '<a>' + jsonn[i].description + '</a>\n' +
                                    '<span><img style="width: 100%;" src="' + jsonn[i].imgUrl + '" alt=" "></span>\n' +
                                    '</div><div class="dropdown" style="width: 3%;">\n' +
                                    '<a class="dropdown-toggle" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"></a>\n' +
                                    '<div class="dropdown-menu" aria-labelledby="dropdownMenuButton">\n' +
                                    '<a class="dropdown-item" href="#">Action</a>\n' +
                                    '<a class="dropdown-item" href="#">Another action</a>\n' +
                                    '<a class="dropdown-item" href="#">Something else here</a>\n' +
                                    '</div></div></div><div style="margin-left: -15px">\n' +
                                    '<a onclick="testfun()"><b>Replay</b></a>\n' +
                                    '<span style="margin-left: 7px"></span>\n' +
                                    '<a onclick="testfun()" class="fa fa-thumbs-up">' + jsonn[i].likes + '</a>\n' +
                                    '<span style="margin-left: 7px"></span>\n' +
                                    '<a onclick="testfun()" class="fa fa-thumbs-down">' + jsonn[i].dislikes + '</a>\n' +
                                    '<div id="Replay-Container"></div>\n' +
                                    '</div></div></div>\n' +
                                    '<hr>\n';
                            }
                        }
                    });
                    inProgress = false;
                    myPageIndex += 10;
                }
            });
    }
}

function getComments() {
    var docHeight = $(document).height();
    var winScrolled = $(window).height() + $(window).scrollTop();
    if ((docHeight - winScrolled) < 200) { // scroll time 
        domOperationComments.call();
    }
}

function addComment() {
    var description = $("#commentDescription").val();
    var token = $("#voteform input[name=__RequestVerificationToken]").val();
    var postId = $("#postId").attr("value");
    var voteUrl = "/api/Comments";
    var file = document.getElementById("commentFileInput").files[0];

    var commentEr = $("#commentError");
    var spinner = $("#commentSpinner");
    var request;

    if (isStringNullOrWhiteSpace(description)) {
        commentEr.addClass("badge-danger");
        commentEr.text('Comment is required.');
        commentEr.removeAttr("hidden");
    } else {
        commentEr.text("");
        var obj = new FormData();
        obj.set('description', description);
        obj.set('postId', postId);
        obj.append('file', file);

        request = new XMLHttpRequest();
        request.open('POST', voteUrl, /* async = */ false);
        request.setRequestHeader("X-CSRF-TOKEN", token);
        spinner.attr("hidden", false);
        request.send(obj);
        spinner.attr("hidden", true);

        if (request.status === 200) {
            commentEr.text("");
            commentEr.removeClass("badge-danger");
            commentEr.addClass("badge-success");
            commentEr.text("Created");
        } else if (request.status === 400) {
            commentEr.addClass("badge-danger");
            commentEr.text("");
            var filError = JSON.parse(request.response).errors.File[0];
            commentEr.text(filError);
        } else if (request.status === 401) {
            window.location = "/Identity/Account/Login";
        } else {
            window.location = "/Home/HttpError?statusCode=" + request.status;
        }
    }


    clearComment.call();
}



function clearComment() {
    var control = $("#commentFileInput");
    var text = $("#commentDescription");

    text.val('');
    control.replaceWith(control.val('').clone(true));
}

/**
  * Checks the string if undefined, null, not typeof string, empty or space(s)
  * @param {any} str string to be evaluated
  * @returns {boolean} the evaluated result
*/

function isStringNullOrWhiteSpace(str) {
    return str === undefined || str === null
        || typeof str !== 'string'
        || str.match(/^ *$/) !== null;
}