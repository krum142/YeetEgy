function addComment() {
    var description = document.getElementById("commentDescription").value;
    var token = $("#voteform input[name=__RequestVerificationToken]").val();
    var postId = $("#postId").attr("value");
    var voteUrl = "/api/Comments";
    var file = document.getElementById("commentFileInput").files[0];

    obj = new FormData();
    obj.set('description', description);
    obj.set('postId',postId);
    obj.append('file', file);

    var request = new XMLHttpRequest();

    request.open('POST', voteUrl, /* async = */ false);
    request.setRequestHeader("X-CSRF-TOKEN", token);

    request.send(obj);

    alert(request.status);
}
