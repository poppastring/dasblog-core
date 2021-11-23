
function commentManagement(postid, commentid, commentText, httpVerb) {
    if (confirm(commentText)) {
        var oReq = new XMLHttpRequest();

        url = 'post/' + postid + '/comments/' + commentid;
        
        oReq.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                location.href = window.location.href;
            }
        };

        oReq.open(httpVerb, url);
        oReq.send();
    }
}

function deleteEntry(entryUrl, entryTitle) {
    if (confirm("Are you sure you want to delete this item? \n\n" + entryTitle)) {
        location.href = entryUrl;
    }
}

function linkToUser(emailAddress, linkAbility) {
    if (linkAbility === "disabled") {
        alert("You must complete the current operation (create, edit or delete user), before you can select another");
        return;
    }
    location.href = emailAddress;
}

function showLastUserError(showError) {
    if (showError) {
        alert("You can't delete your own user record or reduce its privileges");
        return;
    }
    window.maintenanceForm.submit();
}