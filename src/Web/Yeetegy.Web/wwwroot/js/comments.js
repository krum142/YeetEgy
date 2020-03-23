function addComment() {
    var description = $("#commentDescription").val();
    var token = $("#voteform input[name=__RequestVerificationToken]").val();
    var postId = $("#postId").attr("value");
    var voteUrl = "/api/Comments";
    var file = document.getElementById("commentFileInput").files[0];


    if (isStringNullOrWhiteSpace(description)) {
        $("#commentError").addClass("badge-danger");
        $("#commentError").text('Comment is required.');
        $("#commentError").removeAttr("hidden");
    } else {
        $("#commentError").text("");
        var obj = new FormData();
        obj.set('description', description);
        obj.set('postId', postId);
        obj.append('file', file);

        var request = new XMLHttpRequest();
        request.open('POST', voteUrl, /* async = */ false);
        request.setRequestHeader("X-CSRF-TOKEN", token);
        $("#commentSpinner").attr("hidden", false);
        request.send(obj);
        $("#commentSpinner").attr("hidden", true);


    }if (request.status === 200) {
        $("#commentError").text("");
        $("#commentError").removeClass("badge-danger");
        $("#commentError").addClass("badge-success");
        $("#commentError").text("Created");
        $("#commentError").removeAttr("hidden");
    } else if (request.status === 400) {
        $("#commentError").addClass("badge-danger");
        $("#commentError").text("");
        var filError = JSON.parse(request.response).errors.File[0];
        $("#commentError").text(filError);
        $("#commentError").removeAttr("hidden");
    }

    $("#commentDescription").val('');
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