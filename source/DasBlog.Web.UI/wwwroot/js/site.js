// Write your JavaScript code.
function deleteEntry(entryId, entryTitle) {
    if (confirm("Are you sure you want to delete this item? \n\n" + entryTitle)) {
        location.href = "post/" + entryId + "/delete"
    }
}