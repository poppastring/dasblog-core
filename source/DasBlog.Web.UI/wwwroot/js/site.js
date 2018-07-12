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
// showError - true = a business rule preventing the user from shooting themself in the foot
// prohibits the edit or delete progressing
// the id of the Save or Delete input element
//
// if proceding with the operation kicked off by the Save or Delete button would
// violate a business rule then a message is displayed and no further processing takes place.
// If, on the other hand, it is ok to proceed then we need to copy the behaviour of a normal
// form submit where a key-value pair is passed as part of the form being the name of the
// clicked button and the associated value - e.g. submitAction=Save or submitAction=Delete.
// is added to the form as a hidden control
// UsersController.Maintenance post handler needs needs to see this .
function showLastUserError(showError, btnId) {
    if (showError) {
        alert("You can't delete your own user record or reduce its privileges");
        return;     // the edit or delete mode will remain active
    }
    var btn = document.getElementById(btnId);
    var frm = btn.form;
    setSubmitActionOnForm(frm, btn.name, btn.value);
    frm.submit();
}
// actionName == "submitAction" (as required by the UsersController parameter binding) actionValue = "Save" or "Delete"
function setSubmitActionOnForm(frm, actionName, actionValue) {
    var input = document.createElement("input");
    input.type = "hidden";
    input.name = actionName;
    input.value = actionValue;
    frm.appendChild(input);
}