// Write your JavaScript code.
function deleteEntry(entryId, entryTitle) {
    if (confirm("Are you sure you want to delete this item? \n\n" + entryTitle)) {
        location.href = "post/" + entryId + "/delete"
    }
}    
function linkToUser(emailAddress, linkAbility) {
    if (linkAbility === "disabled") {
        alert("You must complete the current operation (create, edit or delete user), before you can select another");
        return;
    }
    location.href = emailAddress;
}
