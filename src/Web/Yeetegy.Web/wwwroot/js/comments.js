var myPageIndex = 0;
var noMoredata = false;
var inProgress = false;
var jsonn;
var posthtml;
var postid = $("#postId").attr('value');

function domOperationComments() {
    if (noMoredata === false && inProgress === false) {
        inProgress = true;
        fetch('/api/Comments/?postId=' + postid + '&offset=' + myPageIndex,
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
                                var replayHtml = "";
                                if (jsonn[i].replaysCount > 0) {
                                    replayHtml = '<a onclick="loadReplays()" id="Comment_' + jsonn[i].id + '" style="font-weight: bold;">View ' + jsonn[i].replaysCount + ' Replays</a>\n';
                                }

                                document.getElementById("Comment-Container").innerHTML +=
                                    '<div class="row"><div style="width: 14%;margin-right: 7px">\n' +
                                    '<img style="display: block; margin: 0 auto; border-radius: 50%;" width="50" height="50" src="' + jsonn[i].applicationUserAvatarUrl + '" />\n' +
                                    '</div><div style="width: 83%; float: right;">\n' +
                                    '<div style="width: 100%; word-wrap: break-word;" class="row">\n' +
                                    '<div style="width: 97%; float: left">\n' +
                                    '<a style="font-weight: bold;">' + jsonn[i].applicationUserUsername + '</a>\n' +
                                    '<h9><span class="badge badge-light">' + jsonn[i].time + '</span></h9><br>\n' +
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
                                    '<a onclick="commentVoteButton()" id="Like_' + jsonn[i].id + '" class="fa fa-thumbs-up">' + jsonn[i].likes + '</a>\n' +
                                    '<a onclick="commentVoteButton()" id="Dislike_' + jsonn[i].id + '" class="fa fa-thumbs-down">' + jsonn[i].dislikes + '</a><br>\n' +
                                    replayHtml +
                                    '<div id="Replay-Container_' + jsonn[i].id + '"></div>\n' +
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

function loadReplays() {
    var commentId = event.target.getAttribute("id").split("_")[1];
    fetch("/api/Replays?commentId=" + commentId,
        {
            method: "GET"
        }).then(response => {
            if (response.status === 200) {
                response.json().then(j => {
                    jsonn = j;
                    $("#Comment_" + commentId).remove();
                    for (var i = 0; i < jsonn.length; i++) {
                        $("#Replay-Container_" + commentId).append(
                            '<hr><div class="row"><div style="width: 14%; margin-right: 7px">\n' +
                            '<img style="display: block; margin: 0 auto; border-radius: 50%;" width="40" height="40" src="' + jsonn[i].applicationUserAvatarUrl + '" alt=" "/>\n' +
                            '</div><div style="width: 83%; float: right;">\n' +
                            '<div style="width: 100%; word-wrap: break-word;" class="row">\n' +
                            '<div style="width: 97%; float: left">\n' +
                            '<a style="font-weight: bold;">' + jsonn[i].applicationUserUsername + '</a>\n' +
                            '<h9><span class="badge badge-light">' + jsonn[i].time + '</span></h9><br>\n' +
                            '<a>' + jsonn[i].description + '</a><span>\n' +
                            '<img style="width: 100%" src="'+ jsonn[i].imgUrl+'" alt=" ">\n' +
                            '</span></div><div class="dropdown" style="width: 3%;">\n' +
                            '<a class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">\n' +
                            '</a><div class="dropdown-menu" aria-labelledby="dropdownMenuButton">\n' +
                            '<a class="dropdown-item" href="#">Action</a>\n' +
                            '<a class="dropdown-item" href="#">Another action</a>\n' +
                            '<a class="dropdown-item" href="#">Something else here</a>\n' +
                            '</div></div></div>\n' +
                            '<div style="margin-left: -15px">\n' +
                            '<a onclick="testfun()" class="fa fa-thumbs-up">' + jsonn[i].likes + '</a>\n' +
                            '<span style="margin-left: 7px"></span>\n' +
                            '<a onclick="testfun()" class="fa fa-thumbs-down">' + jsonn[i].dislikes + '</a>\n' +
                            '</div></div></div>\n'
                        );
                    }
                });
            }
        });
}

function commentVoteButton() {
    var myEvent = event.target.getAttribute("id").split("_");
    var vote = myEvent[0] === "Like" ? true : false;
    var id = myEvent[1];
    var token = $("#voteform input[name=__RequestVerificationToken]").val();

    var voteUrl = "/api/Votes/Comment";
    var json = { commentId: id, isUpVote: vote }
    var request = new XMLHttpRequest();
    request.open('POST', voteUrl, /* async = */ false);
    request.setRequestHeader("X-CSRF-TOKEN", token);
    request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    request.send(JSON.stringify(json));

    var code = request.status;
    var myResponse = JSON.parse(request.response);

    if (code != 200) {
        if (code === 401) {
            window.location = "/Identity/Account/Login";
        } else {
            window.location = "/Home/HttpError?statusCode=" + code;
        }
    }
    /// UnLike,UnDislike,
    /// LikeToDislike,DislikeToLike
    switch (myResponse.status) {
        case "Like": case "Dislike":
            event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
            break;
        case "UnLike": case "UnDislike":
            event.target.innerHTML = parseInt(event.target.innerHTML) - 1;
            break;
        case "LikeToDislike":
            event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
            event.target.previousSibling.previousSibling.innerHTML =
                parseInt(event.target.previousSibling.previousSibling.innerHTML) - 1;
            break;
        case "DislikeToLike":
            event.target.innerHTML = parseInt(event.target.innerHTML) + 1;
            event.target.nextSibling.nextSibling.innerHTML =
                parseInt(event.target.nextSibling.nextSibling.innerHTML) - 1;
            break;
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