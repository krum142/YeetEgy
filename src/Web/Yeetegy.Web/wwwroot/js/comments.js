var myPageIndex = 0;
var noMoredata = false;
var inProgress = false;
var jsonn;
var posthtml;
var postid = $("#postId").attr('value');
var deleteInProgress = false;

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
                                var deleteHtml = "";
                                if (jsonn[i].replaysCount > 0) {
                                    replayHtml = '<a onclick="loadReplays()" id="LoadReplays_' + jsonn[i].id + '" style="font-weight: bold;">View ' + jsonn[i].replaysCount + ' Replays</a>\n';
                                }
                                if ($("#userId").attr('value') === jsonn[i].applicationUserId) {
                                    deleteHtml = '<a class="dropdown-item" onclick="deleteComment()" id="' + jsonn[i].id + '">Delete</a>\n';
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
                                    deleteHtml +
                                    '</div></div></div><div style="margin-left: -15px">\n' +
                                    '<a style="font-weight: bold;" onclick="loadAddReplayForm()" id="LoadForm_' + jsonn[i].id + '">Replay</a>\n' +
                                    '<span id="createReplayContainer_' + jsonn[i].id + '"></span>\n' +
                                    '<span style="margin-left: 7px"></span>\n' +
                                    '<a onclick="voteButton()" id="Comment_Like_' + jsonn[i].id + '" class="fa fa-thumbs-up">' + jsonn[i].likes + '</a>\n' +
                                    '<a onclick="voteButton()" id="Comment_Dislike_' + jsonn[i].id + '" class="fa fa-thumbs-down">' + jsonn[i].dislikes + '</a><br>\n' +
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



function loadComments() {
    var docHeight = $(document).height();
    var winScrolled = $(window).height() + $(window).scrollTop();
    if ((docHeight - winScrolled) < 200) { // scroll time 
        domOperationComments.call();
    }
}

function loadReplays() {
    var commentId = event.target.getAttribute("id").split('_')[1];
    $("#LoadReplays_" + commentId).remove();
    fetch("/api/Replays?commentId=" + commentId,
        {
            method: "GET"
        }).then(response => {
            if (response.status === 200) {
                response.json().then(j => {
                    jsonn = j;
                    $("#Comment_" + commentId).remove();
                    for (var i = 0; i < jsonn.length; i++) {
                        var deleteHtml = "";

                        if ($("#userId").attr('value') === jsonn[i].applicationUserId) {
                            deleteHtml = '<a class="dropdown-item" onclick="deleteComment()" id="' + jsonn[i].id + '">Delete</a>\n';
                        }

                        $("#Replay-Container_" + commentId).append(
                            '<hr><div class="row"><div style="width: 14%; margin-right: 7px">\n' +
                            '<img style="display: block; margin: 0 auto; border-radius: 50%;" width="40" height="40" src="' + jsonn[i].applicationUserAvatarUrl + '" alt=" "/>\n' +
                            '</div><div style="width: 83%; float: right;">\n' +
                            '<div style="width: 100%; word-wrap: break-word;" class="row">\n' +
                            '<div style="width: 97%; float: left">\n' +
                            '<a style="font-weight: bold;">' + jsonn[i].applicationUserUsername + '</a>\n' +
                            '<h9><span class="badge badge-light">' + jsonn[i].time + '</span></h9><br>\n' +
                            '<a>' + jsonn[i].description + '</a><span>\n' +
                            '<img style="width: 100%" src="' + jsonn[i].imgUrl + '" alt=" ">\n' +
                            '</span></div><div class="dropdown" style="width: 3%;">\n' +
                            '<a class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">\n' +
                            '</a><div class="dropdown-menu" aria-labelledby="dropdownMenuButton">\n' +
                            '<a class="dropdown-item" href="#">Action</a>\n' +
                            deleteHtml +
                            '</div></div></div>\n' +
                            '<div style="margin-left: -15px">\n' +
                            '<a onclick="voteButton()" id="Comment_Like_' + jsonn[i].id + '" class="fa fa-thumbs-up">' + jsonn[i].likes + '</a>\n' +
                            '<a onclick="voteButton()" id="Comment_Dislike_' + jsonn[i].id + '" class="fa fa-thumbs-down">' + jsonn[i].dislikes + '</a>\n' +
                            '</div></div></div>\n'
                        );
                    }
                });
            }
        });
}

function loadAddReplayForm() {
    var commentId = event.target.getAttribute("id").split('_')[1];
    $("#LoadForm_" + commentId).remove();
    var avatarUrl = $("#avatarUrl").attr("src");
    $("#createReplayContainer_" + commentId).append(
        '<div class="row">\n' +
        '<div style="width: 14%;">\n' +
        '<img style="display: block; margin: 0 auto; border-radius: 50%;" width="50" height="50" src="' + avatarUrl + '" />\n' +
        '</div><div style="width: 83%; float: right;">\n' +
        '<div class="spinner-border text-success" id="commentSpinner_' + commentId + '" hidden role="status">\n' +
        '<span class="sr-only"></span></div>\n' +
        '<span class="badge badge-pill" id="commentError_' + commentId + '"></span>\n' +
        '<textarea placeholder="Enter your replay here..." id="replayDescription_' + commentId + '" class="form-control" style="resize: none; width: 100%"></textarea>\n' +
        '<div style="width: 100%; height: 41px; margin-top: 1px">\n' +
        '<label><span class="btn btn-warning">\n' +
        'Add IMG… <input type="file" class="custom-file-input" style="display: none;" id="replayFileInput_' + commentId + '">\n' +
        '</span></label>\n' +
        '<button class="btn btn-warning" onclick="addReplay()" id="' + commentId + '" style="float: right">Post</button>\n' +
        '<button class="btn btn-light" onclick="clearReplay()" id="' + commentId + '" style="float: right">Cancel</button>\n' +
        '</div></div></div>\n'
    );
}

function addReplay() {
    var commentId = event.target.getAttribute("id");
    var replayDescription = $("#replayDescription_" + commentId).val();
    var token = $("#voteform input[name=__RequestVerificationToken]").val();
    var postId = $("#postId").attr("value");
    var myUrl = "/api/Replays";
    var file = document.getElementById("replayFileInput_" + commentId).files[0];

    var commentEr = $("#commentError_" + commentId);
    var spinner = $("#commentSpinner_" + commentId);
    var request;

    if (isStringNullOrWhiteSpace(replayDescription)) {
        commentEr.addClass("badge-danger");
        commentEr.text('Comment is required.');
        commentEr.removeAttr("hidden");
    } else {
        commentEr.text("");
        var obj = new FormData();
        obj.set('description', replayDescription);
        obj.set('postId', postId);
        obj.set('commentId', commentId);
        obj.append('file', file);

        request = new XMLHttpRequest();
        request.open('POST', myUrl, /* async = */ false);
        request.setRequestHeader("X-CSRF-TOKEN", token);
        spinner.attr("hidden", false);
        request.send(obj);
        spinner.attr("hidden", true);

        if (request.responseURL.includes("HttpError")) {
            window.location = request.responseURL;
        } else if (request.status === 200) {
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

        if (request.responseURL.includes("HttpError")) {
            window.location = request.responseURL;
        } else if (request.status === 200) {
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

function clearReplay() {
    var commentId = event.target.getAttribute("id");
    var control = $("#replayFileInput_" + commentId);
    var text = $("#replayDescription_" + commentId);

    text.val('');
    control.replaceWith(control.val('').clone(true));
}

function clearComment() {
    var control = $("#commentFileInput");
    var text = $("#commentDescription");

    text.val('');
    control.replaceWith(control.val('').clone(true));
}

function deleteComment() {
    var id = event.target.getAttribute("id");
    var token = $("#voteform input[name=__RequestVerificationToken]").val();
    var deleteUrl = "/api/Comments";

    var obj = new FormData();
    obj.set("id",id);
    if (deleteInProgress === false) {
        deleteInProgress = true;
        fetch(deleteUrl,
            {
                method: 'DELETE',
                headers: {
                    'X-CSRF-TOKEN': token
                },
                body: obj
            }).then(deleteInProgress = false);
    }
    
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