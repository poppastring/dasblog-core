
function commentManagement(postid, commentid, commentText, httpVerb) {
    // Store comment details for modal
    window.pendingCommentAction = {
        postid: postid,
        commentid: commentid,
        httpVerb: httpVerb
    };

    // Set modal content based on action type
    const modalTitle = httpVerb === 'DELETE' ? 'Confirm Delete' : 'Confirm Approve';
    const modalBody = commentText;
    const modalButton = httpVerb === 'DELETE' ? 'Delete' : 'Approve';
    const modalButtonClass = httpVerb === 'DELETE' ? 'btn-danger' : 'btn-success';

    // Get or create modal
    let modal = document.getElementById('commentActionModal');
    if (!modal) {
        // Create modal if it doesn't exist
        const modalHtml = `
            <div class="modal fade" id="commentActionModal" tabindex="-1" aria-labelledby="commentActionModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="commentActionModalLabel"></h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body" id="commentActionModalBody"></div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <button type="button" class="btn" id="confirmCommentActionButton"></button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        document.body.insertAdjacentHTML('beforeend', modalHtml);
        modal = document.getElementById('commentActionModal');
    }

    // Update modal content
    document.getElementById('commentActionModalLabel').textContent = modalTitle;
    document.getElementById('commentActionModalBody').textContent = modalBody;
    const confirmButton = document.getElementById('confirmCommentActionButton');
    confirmButton.textContent = modalButton;
    confirmButton.className = `btn ${modalButtonClass}`;

    // Remove existing event listener and add new one
    const newConfirmButton = confirmButton.cloneNode(true);
    confirmButton.parentNode.replaceChild(newConfirmButton, confirmButton);

    newConfirmButton.addEventListener('click', function() {
        executeCommentAction();
    });

    // Show modal
    const bsModal = new bootstrap.Modal(modal);
    bsModal.show();
}

function executeCommentAction() {
    const action = window.pendingCommentAction;
    if (!action) return;

    const oReq = new XMLHttpRequest();
    const url = '/admin/post/' + action.postid + '/comments/' + action.commentid;

    oReq.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            console.log('Comment action completed successfully');

            // Close modal
            const modal = document.getElementById('commentActionModal');
            const bsModal = bootstrap.Modal.getInstance(modal);
            if (bsModal) {
                bsModal.hide();
            }

            // Set success message in session storage
            const successMessage = action.httpVerb === 'DELETE' 
                ? 'Comment deleted successfully!' 
                : 'Comment approved successfully!';

            console.log('Setting sessionStorage with message:', successMessage);
            sessionStorage.setItem('commentActionSuccess', successMessage);

            // Verify it was set
            console.log('Verification - sessionStorage value:', sessionStorage.getItem('commentActionSuccess'));

            // Small delay to ensure sessionStorage is committed before reload
            setTimeout(function() {
                console.log('Reloading page...');
                window.location.reload();
            }, 100);
        } else if (this.readyState == 4) {
            console.error('Comment action failed with status:', this.status);
        }
    };

    oReq.open(action.httpVerb, url);
    oReq.send();

    // Clear pending action
    window.pendingCommentAction = null;
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