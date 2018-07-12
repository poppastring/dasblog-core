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
function showLastUserError(showError, formId) {
    if (showError) {
        alert("You can't delete your own user record or reduce its privileges");
    }
    var frm = document.getElementById(formId);
    setSubmitActionOnForm(frm, formId === 'editForm' ? 'Save' : 'Delete');
    frm.submit();
}
function setSubmitActionOnForm(frm, actionName) {
    var input = document.createElement("input");
    input.type = "hidden";
    input.name = "submitAction";
    input.value = actionName;
    frm.appendChild(input);
}